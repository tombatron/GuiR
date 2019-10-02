using GuiR.Redis;
using System.Collections.Generic;
using System.Windows.Input;

namespace GuiR.ViewModels.Keys.KeyDisplay
{
    public class ListKeyViewModel : ViewModelBase
    {
        private readonly RedisProxy _redis;

        public ListKeyViewModel(RedisProxy redis) => _redis = redis;

        public string Key { get; set; }

        private IEnumerable<string> _keyValue;
        public IEnumerable<string> KeyValue
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
                KeyValue = await _redis.GetListValueAsync(Key);
            });
    }
}