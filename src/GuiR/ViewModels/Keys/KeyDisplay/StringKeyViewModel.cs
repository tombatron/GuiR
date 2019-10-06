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

        public ICommand LoadKeyValue =>
            new DelegateCommand(async () =>
            {
                KeyValue = await GetDataAsync(Key);
            });

        protected virtual ValueTask<string> GetDataAsync(string key) =>
            _redis.GetStringValueAsync(key);
    }
}