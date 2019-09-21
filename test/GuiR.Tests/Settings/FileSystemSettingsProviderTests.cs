using GuiR.Settings;
using Xunit;

namespace GuiR.Tests.Settings
{
    public class FileSystemSettingsProviderTests
    {
        public class ParseConfigurationLineCan
        {
            [Fact]
            public void ParseConfigurationLine()
            {
                var exampleConfigurationLine = "name|host|1234\n";

                var parsedConfigurationLine = FileSystemSettingsProvider.ParseConfigurationLine(exampleConfigurationLine);

                Assert.Equal("name", parsedConfigurationLine.ServerName);
                Assert.Equal("host", parsedConfigurationLine.ServerAddress);
                Assert.Equal(1234, parsedConfigurationLine.ServerPort);
            }
        }
    }
}
