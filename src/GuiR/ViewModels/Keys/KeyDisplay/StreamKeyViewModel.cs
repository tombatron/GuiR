using GuiR.Models;
using GuiR.Redis;
using System.Collections.Generic;
using System.Diagnostics;
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

        public ICommand LoadKeyValue =>
            new DelegateCommand(async () =>
            {
                KeyValue = await GetDataAsync(Key);
            });

        public ICommand NextPage =>
            new DelegateCommand(() => 
            {
                Debug.WriteLine("NextPage");
            });

        public ICommand PreviousPage =>
            new DelegateCommand(() => 
            {
                Debug.WriteLine("PreviousPage");
            });

        protected virtual ValueTask<IEnumerable<StreamCollectionEntry>> GetDataAsync(string key) =>
            _redis.GetStreamDataAsync(key);
    }
}
