using GuiR.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

        public Task<IEnumerable<RedisServerInformation>> GetServerSettingsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SaveServerSettingsAsync(IEnumerable<RedisServerInformation> serverSettings)
        {
            using (var file = File.Create(FileLocation))
            using (var writer = new StreamWriter(file))
            {
                foreach(var serverSetting in serverSettings)
                {
                    await writer.WriteAsync($"{serverSetting.ServerName}|{serverSetting.ServerAddress}|{serverSetting.ServerPort}\n");
                }
            }
        }
    }
}
