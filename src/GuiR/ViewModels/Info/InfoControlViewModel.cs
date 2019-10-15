using GuiR.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuiR.ViewModels.Info
{
    public class InfoControlViewModel : ViewModelBase
    {
        private readonly RedisProxy _redis;

        public InfoControlViewModel(RedisProxy redis) => _redis = redis;

        private IGrouping<string, KeyValuePair<string, string>>[] _info;
        public IGrouping<string, KeyValuePair<string, string>>[] Info
        {
            get => _info;

            set
            {
                _info = value;

                RaisePropertyChangedEvent(nameof(Info));
            }
        }

        public async ValueTask<IGrouping<string, KeyValuePair<string, string>>[]> LoadAsync() =>
            Info = await _redis.GetInfoAsync();
    }
}
