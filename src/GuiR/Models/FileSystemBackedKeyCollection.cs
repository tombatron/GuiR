using GuiR.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace GuiR.Models
{
    public class FileSystemBackedKeyCollection : IList<string>, IDisposable
    {
        public interface ICacheFile : IDisposable
        {
            IEnumerable<string> ReadAllLines();
            Task WriteLineAsync(string line);
            Task FlushAsync();
        }

        public class DefaultCacheFile : ICacheFile
        {
            public IEnumerable<string> ReadAllLines()
            {
                throw new NotImplementedException();
            }

            public Task WriteLineAsync(string line)
            {
                throw new NotImplementedException();
            }

            public Task FlushAsync()
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        private readonly KeyInfo _baseKeyEnumeration;
        private readonly string _filePath;
        private readonly string _cacheFile;

        private CancellationTokenSource _backgroundCancellationTokenSource;

        private Stream _writeStream;
        private StreamWriter _writer;

        public event EventHandler BackgroundLoadStarted;
        public event EventHandler BackgroundLoadProgress;
        public event EventHandler BackgroundLoadComplete;

        private bool _backgroundLoadComplete = false;

        public KeyInfo KeyInfo => _baseKeyEnumeration;

        public FileSystemBackedKeyCollection(KeyInfo baseKeyEnumeration)
        {
            _backgroundCancellationTokenSource = new CancellationTokenSource();
            _baseKeyEnumeration = baseKeyEnumeration;

            _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GuiR");
            _cacheFile = Path.Combine(_filePath, $"{Guid.NewGuid().ToString("N")}.key_data");
            _writeStream = new FileStream(_cacheFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            _writer = new StreamWriter(_writeStream);
        }

        public IList<string> FetchRange(int startIndex, int count) =>
            InternalEnumerable().Skip(startIndex).Take(count).ToList();

        public List<string> FilterKeys(string keyFilter) =>
            InternalEnumerable().Where(x => x.StartsWith(keyFilter)).ToList();

        protected virtual void OnBackgroundLoadStarted(EventArgs e = null)
        {
            _backgroundLoadComplete = false;

            EventHandler handler = BackgroundLoadStarted;

            handler?.Invoke(this, e);
        }

        protected virtual void OnBackgroundLoadProgress(EventArgs e = null)
        {
            EventHandler handler = BackgroundLoadProgress;

            handler?.Invoke(this, e);
        }

        protected virtual void OnBackgroundLoadComplete(EventArgs e = null)
        {
            _backgroundLoadComplete = true;

            EventHandler handler = BackgroundLoadComplete;

            handler?.Invoke(this, e);
        }

        public async ValueTask PopulateFileSystemAsync()
        {
            var cancelToken = _backgroundCancellationTokenSource.Token;
            cancelToken.Register(() => OnBackgroundLoadComplete());

            OnBackgroundLoadStarted();

#pragma warning disable CS4014 // We actually don't want to await this.
            Task.Run(() =>
            {
                void ProgressTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) =>
                    OnBackgroundLoadProgress();

                var progressTimer = new Timer();
                progressTimer.Interval = 10_000;
                progressTimer.Elapsed += ProgressTimer_Elapsed;

                progressTimer.Start();

                using (_baseKeyEnumeration)
                {
                    foreach (var key in _baseKeyEnumeration.Keys)
                    {
                        if (cancelToken.IsCancellationRequested)
                        {
                            progressTimer.Stop();
                            return;
                        }

                        _writer.WriteLine(key);

                        Count++;
                    }
                }

                _writer.Flush();

                progressTimer.Stop();

                OnBackgroundLoadProgress();
                OnBackgroundLoadComplete();
            }, cancelToken);
#pragma warning restore CS4014

            while (!_backgroundLoadComplete)
            {
                if (Count >= 500)
                {
                    break;
                }

                await Task.Delay(50);
            }
        }

        public void CancelBackgroundLoading() => _backgroundCancellationTokenSource.Cancel();

        private IEnumerable<string> InternalEnumerable()
        {
            using (var readStream = new FileStream(_cacheFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(readStream))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        #region IList<string>

        public string this[int index]
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public int Count { get; private set; }

        public bool IsReadOnly => true;

        public IEnumerator<string> GetEnumerator() => InternalEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region Not Implemented

        public void Add(string item) => throw new NotImplementedException();

        public void Clear() => throw new NotImplementedException();

        public bool Contains(string item) => throw new NotImplementedException();

        public void CopyTo(string[] array, int arrayIndex) => throw new NotImplementedException();

        public int IndexOf(string item) => throw new NotImplementedException();

        public void Insert(int index, string item) => throw new NotImplementedException();

        public bool Remove(string item) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();

        #endregion

        #endregion

        #region IDisposable

        public void Dispose()
        {
            // If we're done with this collection before we're done loading the backing file
            // we need to cancel the loading.
            CancelBackgroundLoading();

            _writer.Dispose();
            _writeStream.Dispose();

            // If we're done with the collection lets clean up the file system too.
            File.Delete(_cacheFile);
        }

        #endregion
    }
}