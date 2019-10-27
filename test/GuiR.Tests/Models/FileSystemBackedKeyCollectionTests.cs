using GuiR.Models;
using GuiR.Redis;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GuiR.Tests.Models
{
    public class FileSystemBackedKeyCollectionTests
    {
        private class FakeCacheFile : FileSystemBackedKeyCollection.ICacheFile
        {
            public IEnumerable<string> ReadAllLines()
            {
                yield break;
            }

            public void WriteLine(string line)
            {
            }

            public void Flush()
            {
            }

            public void Dispose()
            {
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

                var collection = new FileSystemBackedKeyCollection(fakeKeyInfo);

                collection.BackgroundLoadStarted += (object sender, System.EventArgs e) => eventFired = true;

                await collection.PopulateFileSystemAsync();

                Assert.True(eventFired);
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

                var collection = new FileSystemBackedKeyCollection(fakeKeyInfo);

                collection.BackgroundLoadProgress += (object sender, System.EventArgs e) => eventFired = true;

                await collection.PopulateFileSystemAsync();

                await Task.Delay(10);

                Assert.True(eventFired);
            }

            private IEnumerable<string> FakeEnumerable()
            {
                while(true)
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

                var collection = new FileSystemBackedKeyCollection(fakeKeyInfo);

                collection.BackgroundLoadComplete += (object sender, System.EventArgs e) => eventFired = true;

                await collection.PopulateFileSystemAsync();

                Assert.True(eventFired);
            }

            private IEnumerable<string> FakeEnumerable()
            {
                yield break;
            }
        }
    }
}
