﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GuiR.Models
{
    public class FileSystemBackKeyCollection : IList<string>, IDisposable
    {
        private readonly IEnumerable<string> _baseKeyEnumeration;
        private readonly string _filePath;
        private CancellationTokenSource _backgroundCancellationTokenSource;

        private Stream _fileStream;
        private StreamReader _reader;
        private StreamWriter _writer;

        public FileSystemBackKeyCollection(IEnumerable<string> baseKeyEnumeration)
        {
            _baseKeyEnumeration = baseKeyEnumeration;
            _filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _fileStream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _reader = new StreamReader(_fileStream);
            _writer = new StreamWriter(_fileStream);

            PopulateFileSystem();
        }

        private void PopulateFileSystem()
        {
            var cancelToken = _backgroundCancellationTokenSource.Token;

            Task.Run(() =>
            {
                foreach (var key in _baseKeyEnumeration)
                {
                    _writer.WriteLine(key);

                    Count++;
                }
            }, cancelToken);
        }

        private void CancelBackgroundLoading() => _backgroundCancellationTokenSource.Cancel();

        private IEnumerable<string> InternalEnumerable()
        {
            string line;

            while ((line = _reader.ReadLine()) != null)
            {
                yield return line;
            }

            yield break;
        }

        #region IList<string>

        public string this[int index]
        {
            get => throw new NotImplementedException(); //File.ReadLines(_filePath).Skip(index - 1).Take(1).First();

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
            _fileStream.Dispose();

            // If we're done with the collection lets clean up the file system too.
            File.Delete(_filePath);
        }

        #endregion
    }
}