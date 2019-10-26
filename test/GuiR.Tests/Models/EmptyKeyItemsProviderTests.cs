using GuiR.Models;
using Xunit;

namespace GuiR.Tests.Models
{
    public class EmptyKeyItemsProviderTests
    {
        public class FetchCount
        {
            [Fact]
            public void WillBeZero()
            {
                var provider = new EmptyKeyItemsProvider();

                Assert.Equal(0, provider.FetchCount());
            }
        }

        public class FetchRange
        {
            [Theory]
            [InlineData(0, 0)]
            [InlineData(int.MinValue, int.MinValue)]
            [InlineData(int.MinValue, int.MaxValue)]
            [InlineData(int.MaxValue, int.MinValue)]
            [InlineData(int.MaxValue, int.MaxValue)]
            public void WillReturnAnEmptyList(int startIndex, int count)
            {
                var provider = new EmptyKeyItemsProvider();

                var result = provider.FetchRange(startIndex, count);

                Assert.Equal(0, result.Count);
            }
        }
    }
}
