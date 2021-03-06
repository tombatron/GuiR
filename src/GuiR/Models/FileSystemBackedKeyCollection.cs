﻿using GuiR.Redis;
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
            void WriteLine(string line);
            void Flush();
        }

        public class DefaultCacheFile : ICacheFile
        {
            private readonly string _filePath;
            private readonly string _filePathWithFileName;
            private Stream _writeStream;
            private StreamWriter _writer;

            public DefaultCacheFile()
            {
                _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GuiR");
                _filePathWithFileName = Path.Combine(_filePath, $"{Guid.NewGuid().ToString("N")}.key_data");
                _writeStream = new FileStream(_filePathWithFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                _writer = new StreamWriter(_writeStream);
            }

            public IEnumerable<string> ReadAllLines()
            {
                using (var readStream = new FileStream(_filePathWithFileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(readStream))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        yield return line;
                    }
                }
            }

            public void WriteLine(string line) => _writer.WriteLine(line);

            public void Flush() => _writer.Flush();

            public void Dispose()
            {
                Dispose(true);

                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool what)
            {
                _writer.Dispose();
                _writeStream.Dispose();

                // If we're done with the collection lets clean up the file system too.
                File.Delete(_filePathWithFileName);
            }
        }

        private readonly KeyInfo _baseKeyEnumeration;

        private CancellationTokenSource _backgroundCancellationTokenSource;

        private ICacheFile _cacheFile;

        public event EventHandler BackgroundLoadStarted;
        public event EventHandler BackgroundLoadProgress;
        public event EventHandler BackgroundLoadComplete;

        private bool _backgroundLoadComplete = false;

        public KeyInfo KeyInfo => _baseKeyEnumeration;

        public FileSystemBackedKeyCollection(KeyInfo baseKeyEnumeration) :
            this(baseKeyEnumeration, new DefaultCacheFile())
        { }

        private FileSystemBackedKeyCollection(IEnumerable<string> baseKeyEnumeration) :
            this(new KeyInfo(baseKeyEnumeration, null, 0))
        { }

        public FileSystemBackedKeyCollection(KeyInfo baseKeyEnumeration, ICacheFile cacheFile)
        {
            _backgroundCancellationTokenSource = new CancellationTokenSource();
            _baseKeyEnumeration = baseKeyEnumeration;
            _cacheFile = cacheFile;
        }

        public IList<string> FetchRange(int startIndex, int count) =>
            InternalEnumerable().Skip(startIndex).Take(count).ToList();

        public FileSystemBackedKeyCollection FilterKeys(string keyFilter) =>
            new FileSystemBackedKeyCollection(InternalEnumerable().Where(x => x.StartsWith(keyFilter)));

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
                progressTimer.Interval = 500;
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

                        _cacheFile.WriteLine(key);

                        Count++;
                    }
                }

                _cacheFile.Flush();

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

        private IEnumerable<string> InternalEnumerable() => _cacheFile.ReadAllLines();

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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // If we're done with this collection before we're done loading the backing file
                // we need to cancel the loading.
                CancelBackgroundLoading();

                _cacheFile.Dispose();
            }
        }

        #endregion
    }
}