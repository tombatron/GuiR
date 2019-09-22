using GuiR.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("GuiR.Tests")]

namespace GuiR.Settings
{
    public class FileSystemSettingsProvider : ISettingsProvider
    {
        public FileSystemSettingsProvider()
        {
            var localApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            FileLocation = Path.Combine(localApplicationDataPath, "GuiR", "servers.config");
        }
 
        public string FileLocation { get; }

        public async Task<IEnumerable<RedisServerInformation>> GetServerSettingsAsync()
        {
            var result = new List<RedisServerInformation>();

            using (var file = File.OpenRead(FileLocation))
            using (var reader = new StreamReader(file))
            {
                var configurationLine = await reader.ReadLineAsync();
                var parsedConfiguration = ParseConfigurationLine(configurationLine);

                result.Add(parsedConfiguration);
            }

            return result;
        }

        public async Task SaveServerSettingsAsync(IEnumerable<RedisServerInformation> serverSettings)
        {
            using (var file = File.Create(FileLocation))
            using (var writer = new StreamWriter(file))
            {
                foreach (var serverSetting in serverSettings)
                {
                    await writer.WriteAsync($"{serverSetting.ServerName.Trim()}|{serverSetting.ServerAddress.Trim()}|{serverSetting.ServerPort}\n");
                }
            }
        }

        internal static RedisServerInformation ParseConfigurationLine(string configurationLine)
        {
            string serverName;
            string serverAddress;
            string serverPort;

            var configurationLineSpan = configurationLine.AsSpan();

            var firstDelimiter = configurationLineSpan.IndexOf('|');
            var secondDelimiter = configurationLineSpan.LastIndexOf('|');

            serverName = configurationLineSpan.Slice(0, firstDelimiter).ToString();
            serverAddress = configurationLineSpan.Slice(serverName.Length + 1, (secondDelimiter - serverName.Length) - 1).ToString();
            serverPort = configurationLineSpan.Slice(secondDelimiter + 1, (configurationLine.Length - secondDelimiter) - 2).ToString();

            return new RedisServerInformation
            {
                ServerName = serverName,
                ServerAddress = serverAddress,
                ServerPort = int.Parse(serverPort)
            };
        }
    }
}