using GuiR.Models;
using GuiR.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuiR.ViewModels.Keys.KeyDisplay
{
    public class StreamKeyViewModel : ViewModelBase
    {
        protected readonly RedisProxy _redis;

        public StreamKeyViewModel(RedisProxy redis) => _redis = redis;

        public string Key { get; set; }

        private IEnumerable<StreamCollectionEntry> _keyValue;
        public IEnumerable<StreamCollectionEntry> KeyValue
        {
            get => _keyValue;

            set
            {
                _keyValue = value;

                RaisePropertyChangedEvent(nameof(KeyValue));
            }
        }

        private string _minKeyId;
        private string _maxKeyId;

        public ICommand LoadKeyValue =>
            new DelegateCommand(async () =>
            {
                KeyValue = await GetDataAsync(Key);
            });

        public ICommand NextPage =>
            new DelegateCommand(async () => 
            {
                KeyValue = await GetDataAsync(Key, minId: _maxKeyId);
            });

        public ICommand PreviousPage =>
            new DelegateCommand(async () => 
            {
                KeyValue = await GetDataAsync(Key, maxId: _minKeyId);
            });

        protected virtual async ValueTask<IEnumerable<StreamCollectionEntry>> GetDataAsync(string key, string minId = null, string maxId = null)
        {
            var page = await _redis.GetStreamDataAsync(key, minId: minId, maxId: maxId);

            _minKeyId = page.Min(x => x.Id);
            _maxKeyId = page.Max(x => x.Id);

            return page;
        }
            
    }
}
