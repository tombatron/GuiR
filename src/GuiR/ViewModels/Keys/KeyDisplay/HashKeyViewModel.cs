using GuiR.Models;
using GuiR.Redis;
using System.Collections.Generic;
using System.Windows.Input;

namespace GuiR.ViewModels.Keys.KeyDisplay
{
    public class HashKeyViewModel : ViewModelBase
    {
        private readonly RedisProxy _redis;

        public HashKeyViewModel(RedisProxy redis) => _redis = redis;

        public string Key { get; set; }

        public IEnumerable<HashCollectionEntry> _keyValue;
        public IEnumerable<HashCollectionEntry> KeyValue
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

        public ICommand LoadKeyValue =>
            new DelegateCommand(async () => 
            {
                KeyValue = await _redis.GetHashAsync(ServerInfo, DatabaseId, Key);
            });

    }
}
