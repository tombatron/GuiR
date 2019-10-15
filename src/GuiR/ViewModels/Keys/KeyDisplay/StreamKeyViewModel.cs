using GuiR.Models;
using GuiR.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace GuiR.ViewModels.Keys.KeyDisplay
{
    public class StreamKeyViewModel : ViewModelBase
    {
        private const int ItemsPerPage = 25;
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

        private bool _previousEnabled;
        public bool PreviousEnabled
        {
            get => _previousEnabled;

            set
            {
                _previousEnabled = value;

                RaisePropertyChangedEvent(nameof(PreviousEnabled));
            }
        }

        private string _streamMinKeyId;
        private string _currentMinKeyId;
        private string _currentMaxKeyId;

        public ICommand LoadKeyValue =>
            new DelegateCommand(async () =>
            {
                KeyValue = await _redis.GetStreamDataAsync(Key, count: ItemsPerPage);

                _streamMinKeyId = KeyValue.Min(x => x.Id);

                SetMinMaxKeyId();
            });

        public ICommand Next =>
            new DelegateCommand(async () =>
            {
                KeyValue = await _redis.GetNextStreamDataAsync(Key, _currentMaxKeyId, count: ItemsPerPage);

                SetMinMaxKeyId();
            });

        public ICommand Previous =>
            new DelegateCommand(async () =>
            {
                if (!PreviousEnabled)
                {
                    return;
                }

                KeyValue = await _redis.GetPreviousStreamDataAsync(Key, _currentMinKeyId, count: ItemsPerPage);

                SetMinMaxKeyId();
            });

        private void SetMinMaxKeyId()
        {
            _currentMinKeyId = KeyValue.Min(x => x.Id);
            _currentMaxKeyId = KeyValue.Max(x => x.Id);

            PreviousEnabled = _currentMinKeyId != _streamMinKeyId;
        }
    }
}