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

        private List<StreamCollectionEntry> _keyValue;
        public List<StreamCollectionEntry> KeyValue
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

        public ICommand Next =>
            new DelegateCommand(async () => 
            {
                KeyValue = await GetDataAsync(Key);
            });

        public ICommand Previous =>
            new DelegateCommand(async () =>
            {
                KeyValue = await GetDataAsync(Key);
            });

        protected virtual async ValueTask<List<StreamCollectionEntry>> GetDataAsync(string key, string minId = null)
        {
            var page = await _redis.GetStreamDataAsync(key);

            return page.ToList();
        }
    }
}