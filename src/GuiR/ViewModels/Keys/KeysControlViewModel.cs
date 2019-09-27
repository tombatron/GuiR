using GuiR.Models;
using GuiR.Redis;
using System.Collections.Generic;
using System.Windows.Input;

namespace GuiR.ViewModels.Keys
{
    public class KeysControlViewModel : ViewModelBase
    {
        private readonly RedisProxy _redis;
        public RedisServerInformation ServerInfo { get; set; }

        private int _databaseId = 0;
        public int DatabaseId
        {
            get => _databaseId;

            set
            {
                _databaseId = value;

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

        public KeysControlViewModel(RedisProxy redis) => _redis = redis;

        public ICommand RefreshKeys =>
            new DelegateCommand(async () =>
            {
                KeysList = await _redis.GetKeysAsync(ServerInfo, DatabaseId, KeyFilter);
            });


        // TODO: Put logic here that determines the key type. 
        // TODO: Create a new control that we can data bind the key and type to.
        //       The new control will be in charge of determining how to display the key.

        private string _selectedKey;
        public string SelectedKey
        {
            get => _selectedKey;

            set => _selectedKey = value;
        }

        public string SelectedKeyType { get; set; }
    }
}
