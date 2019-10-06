using GuiR.Models;
using GuiR.Redis;
using System.Collections.Generic;
using System.Windows.Input;

namespace GuiR.ViewModels.Keys.KeyDisplay.SortedSet
{
    public class GeoViewModel : ViewModelBase
    {
        private readonly RedisProxy _redis;

        public GeoViewModel(RedisProxy redis) => _redis = redis;

        public string Key { get; set; }

        private IEnumerable<GeoPositionCollectionEntry> _keyValue;

        public IEnumerable<GeoPositionCollectionEntry> KeyValue
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
                KeyValue = await _redis.GetGeoHashSetAsync(Key);
            });
    }
}
