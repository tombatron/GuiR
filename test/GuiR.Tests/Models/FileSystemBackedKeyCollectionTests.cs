using GuiR.Models;
using GuiR.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GuiR.Tests.Models
{
    public class FileSystemBackedKeyCollectionTests
    {
        private class FakeCacheFile : FileSystemBackedKeyCollection.ICacheFile
        {
            private readonly List<string> _internalBuffer = new List<string>();

            public IEnumerable<string> ReadAllLines() => _internalBuffer;

            public void WriteLine(string line) => _internalBuffer.Add(line);

            public void Dispose() => _internalBuffer.Clear();

            public void Flush()
            {
                // NO OP
            }
        }

        public class BackgroundLoadStarted
        {
            [Fact]
            public async Task WillFireOnPopulateFileSystem()
            {
                var fakeCacheFile = new FakeCacheFile();

                var fakeKeyInfo = new KeyInfo(FakeEnumerable(), null, 1_000_000);

                var eventFired = false;

                using (var collection = new FileSystemBackedKeyCollection(fakeKeyInfo, fakeCacheFile))
                {
                    collection.BackgroundLoadStarted += (object sender, System.EventArgs e) => eventFired = true;

                    await collection.PopulateFileSystemAsync();

                    Assert.True(eventFired);
                }
            }

            private IEnumerable<string> FakeEnumerable()
            {
                yield return string.Empty;
            }
        }

        public class BackgroundLoadProgress
        {
            [Fact]
            public async Task WillFireOnDuringLoad()
            {
                var fakeCacheFile = new FakeCacheFile();

                var fakeKeyInfo = new KeyInfo(FakeEnumerable(), null, 1_000_000);

                var eventFired = false;

                using (var collection = new FileSystemBackedKeyCollection(fakeKeyInfo, fakeCacheFile))
                {
                    collection.BackgroundLoadProgress += (object sender, System.EventArgs e) => eventFired = true;

                    await collection.PopulateFileSystemAsync();

                    await Task.Delay(10);

                    Assert.True(eventFired);
                }
            }

            private IEnumerable<string> FakeEnumerable()
            {
                while (true)
                {
                    Thread.Sleep(1);

                    yield return string.Empty;
                }
            }
        }

        public class BackgroundLoadComplete
        {
            [Fact]
            public async Task WillFireOnceEnumerationIsComplete()
            {
                var fakeCacheFile = new FakeCacheFile();

                var fakeKeyInfo = new KeyInfo(FakeEnumerable(), null, 0);

                var eventFired = false;

                var collection = new FileSystemBackedKeyCollection(fakeKeyInfo, fakeCacheFile);

                collection.BackgroundLoadComplete += (object sender, System.EventArgs e) => eventFired = true;

                await collection.PopulateFileSystemAsync();

                Assert.True(eventFired);
            }

            private IEnumerable<string> FakeEnumerable()
            {
                yield break;
            }
        }

        public class PopulateFileSystemAsyncWithLessThan500Items
        {
            [Fact]
            public async Task WillReturnIfBackgroundLoadIsComplete()
            {
                var fakeCacheFile = new FakeCacheFile();

                var fakeKeyInfo = new KeyInfo(FakeEnumerable(), null, 1);

                using (var collection = new FileSystemBackedKeyCollection(fakeKeyInfo, fakeCacheFile))
                {
                    await collection.PopulateFileSystemAsync();

                    Assert.Single(collection);
                }
            }

            private IEnumerable<string> FakeEnumerable()
            {
                yield return string.Empty;
            }
        }

        public class PopulateFileSystemAsyncWith500Items
        {
            [Fact]
            public async Task WillReturnIfBackgroundLoadIsComplete()
            {
                var fakeCacheFile = new FakeCacheFile();

                var fakeKeyInfo = new KeyInfo(FakeEnumerable(), null, 1);

                using (var collection = new FileSystemBackedKeyCollection(fakeKeyInfo, fakeCacheFile))
                {
                    await collection.PopulateFileSystemAsync();

                    Assert.Equal(500, collection.Count);
                }
            }

            private IEnumerable<string> FakeEnumerable() =>
                Enumerable.Range(0, 500).Select(x => string.Empty);
        }

        public class PopulateFileSystemAsyncWithMoreThan500Items
        {
            [Fact]
            public async Task WillReturnIfAtleast500ItemsHaveLoaded()
            {
                var fakeCacheFile = new FakeCacheFile();

                var fakeKeyInfo = new KeyInfo(FakeEnumerable(), null, 1);

                using (var collection = new FileSystemBackedKeyCollection(fakeKeyInfo, fakeCacheFile))
                {
                    await collection.PopulateFileSystemAsync();

                    Assert.True(collection.Count >= 500);
                }
            }

            private IEnumerable<string> FakeEnumerable()
            {
                for (var i = 0; i < 600; i++)
                {
                    yield return string.Empty;
                }

                // This is a little over 277 hours...
                Thread.Sleep(1_000_000_000);

                for (var i = 0; i < 1_000_000; i++)
                {
                    yield return string.Empty;
                }
            }
        }

        public class CancelBackgroundLoading
        {
            [Fact]
            public async Task WillStopBackgroundLoading()
            {
                var fakeCacheFile = new FakeCacheFile();

                var fakeKeyInfo = new KeyInfo(FakeEnumerable(), null, 1_000_000);

                var backgroundLoadComplete = false;

                using (var collection = new FileSystemBackedKeyCollection(fakeKeyInfo, fakeCacheFile))
                {
                    collection.BackgroundLoadComplete += (object sender, System.EventArgs e) => backgroundLoadComplete = true;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    collection.PopulateFileSystemAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                    Assert.False(backgroundLoadComplete);

                    collection.CancelBackgroundLoading();

                    await Task.Delay(1_000);

                    Assert.True(backgroundLoadComplete);
                }
            }

            private IEnumerable<string> FakeEnumerable()
            {
                // This will take hours to finish. 
                for (var i = 0; i < 1_000_000; i++)
                {
                    yield return string.Empty;

                    Thread.Sleep(500);
                }
            }
        }
    }
}