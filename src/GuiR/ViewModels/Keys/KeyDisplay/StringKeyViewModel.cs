using GuiR.Models;
using GuiR.Redis;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuiR.ViewModels.Keys.KeyDisplay
{
    public class StringKeyViewModel : ViewModelBase
    {
        private readonly RedisProxy _redis;

        public StringKeyViewModel(RedisProxy redis) => _redis = redis;

        public string Key { get; set; }

        private string _keyValue;
        public string KeyValue
        {
            get => _keyValue;

            set
            {
                _keyValue = value;

                RaisePropertyChangedEvent(nameof(KeyValue));
            }
        }

        public RedisServerInformation ServerInfo { get; set; }

        public int DatabaseId { get; set; }

        // TODO: Figure out how to invoke this via a behavior...
        public ICommand LoadKeyValue =>
            new DelegateCommand(async () =>
            {
                KeyValue = await _redis.GetStringValueAsync(ServerInfo, DatabaseId, Key);
            });

        public async ValueTask LoadValue()
        {
            KeyValue = await _redis.GetStringValueAsync(ServerInfo, DatabaseId, Key);
        }
    }
}
