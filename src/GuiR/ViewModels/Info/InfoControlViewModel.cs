using GuiR.Models;
using GuiR.Redis;
using System.Threading.Tasks;

namespace GuiR.ViewModels.Info
{
    public class InfoControlViewModel : ViewModelBase
    {
        private readonly RedisProxy _redis;

        public InfoControlViewModel(RedisProxy redis) => _redis = redis;

        private string _info;
        public string Info
        {
            get => _info;

            set
            {
                _info = value;

                RaisePropertyChangedEvent(nameof(Info));
            }
        }

        public async ValueTask<string> LoadAsync(RedisServerInformation serverInfo) =>
            Info = await _redis.GetInfoAsync(serverInfo);
    }
}
