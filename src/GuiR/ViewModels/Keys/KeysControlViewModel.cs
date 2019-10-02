using GuiR.Redis;
using System.Collections.Generic;
using System.Windows.Input;

namespace GuiR.ViewModels.Keys
{
    public class KeysControlViewModel : ViewModelBase
    {
        private readonly RedisProxy _redis;
        private readonly IServerContext _serverContext;

        private int _databaseId = 0;
        public int DatabaseId
        {
            get => _databaseId;

            set
            {
                _databaseId = value;
                _serverContext.DatabaseId = value;

                RaisePropertyChangedEvent(nameof(DatabaseId));
            }
        }

        private string _keyFilter = "*";

        public string KeyFilter
        {
            get => _keyFilter;

            set
            {
                _keyFilter = value;

                RaisePropertyChangedEvent(nameof(KeyFilter));
            }
        }

        private List<string> _keysList = new List<string>();

        public List<string> KeysList
        {
            get => _keysList;

            set
            {
                _keysList = value;

                RaisePropertyChangedEvent(nameof(KeysList));
            }
        }

        public KeysControlViewModel(RedisProxy redis, IServerContext serverContext)
        {
            _redis = redis;
            _serverContext = serverContext;
        }

        public ICommand RefreshKeys =>
            new DelegateCommand(async () =>
            {
                KeysList = await _redis.GetKeysAsync(KeyFilter);
            });
    }
}
