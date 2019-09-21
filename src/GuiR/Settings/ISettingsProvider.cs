using GuiR.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuiR.Settings
{
    public interface ISettingsProvider
    {
        Task<IEnumerable<RedisServerInformation>> GetServerSettingsAsync();

        Task SaveServerSettingsAsync(IEnumerable<RedisServerInformation> serverSettings);
    }
}