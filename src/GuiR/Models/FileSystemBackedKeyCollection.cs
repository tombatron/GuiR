using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GuiR.Models
{
    public class FileSystemBackedKeyCollection : IList<string>, IDisposable
    {
        private readonly IEnumerable<string> _baseKeyEnumeration;
        private readonly string _filePath;
        private readonly string _cacheFile;

        private CancellationTokenSource _backgroundCancellationTokenSource;

        private Stream _writeStream;
        private Stream _readStream;
        private StreamReader _reader;
        private StreamWriter _writer;

        public event EventHandler BackgroundLoadStarted;
        public event EventHandler BackgroundLoadComplete;

        public FileSystemBackedKeyCollection(IEnumerable<string> baseKeyEnumeration)
        {
            _backgroundCancellationTokenSource = new CancellationTokenSource();
            _baseKeyEnumeration = baseKeyEnumeration;

            _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GuiR");
            _cacheFile = Path.Combine(_filePath, $"{Guid.NewGuid().ToString("N")}.key_data");
            _writeStream = new FileStream(_cacheFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            _readStream = new FileStream(_cacheFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            _reader = new StreamReader(_readStream);
            _writer = new StreamWriter(_writeStream);
        }

        public IList<string> FetchRange(int startIndex, int count) =>
            InternalEnumerable().Skip(startIndex).Take(count).ToList();

        public List<string> FilterKeys(string keyFilter) =>
            InternalEnumerable().Where(x => x.StartsWith(keyFilter)).ToList();

        protected virtual void OnBackgroundLoadStarted(EventArgs e)
        {
            EventHandler handler = BackgroundLoadStarted;

            handler?.Invoke(this, e);
        }

        protected virtual void OnBackgroundLoadComplete(EventArgs e)
        {
            EventHandler handler = BackgroundLoadComplete;

            handler?.Invoke(this, e);
        }

        public void PopulateFileSystem()
        {
            var cancelToken = _backgroundCancellationTokenSource.Token;
            cancelToken.Register(() => OnBackgroundLoadComplete(null));

            OnBackgroundLoadStarted(null);

            Task.Run(() =>
            {
                foreach (var key in _baseKeyEnumeration)
                {
                    if(cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

                    _writer.WriteLine(key);

                    Count++;
                }

                _writer.Flush();

                OnBackgroundLoadComplete(null);
            }, cancelToken);
        }

        public void CancelBackgroundLoading() => _backgroundCancellationTokenSource.Cancel();

        private IEnumerable<string> InternalEnumerable()
        {
            string line;

            while ((line = _reader.ReadLine()) != null)
            {
                yield return line;
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
            _reader.Dispose();
            _writeStream.Dispose();
            _readStream.Dispose();

            // If we're done with the collection lets clean up the file system too.
            File.Delete(_cacheFile);
        }

        #endregion
    }
}