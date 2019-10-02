using GuiR.Configuration;
using GuiR.Models;
using GuiR.Redis;
using System.Windows.Controls;

namespace GuiR.Controls.KeyDisplay
{
    public delegate void KeyChanged();

    public partial class KeyDisplayContainer : UserControl
    {
        public event KeyChanged KeyChanged;

        private string _currentKey;
        private RedisProxy _redis;
        private IServerContext _serverContext;

        private RedisServerInformation _serverInfo;
        private int _databaseId;

        public string CurrentKey
        {
            get => _currentKey;

            set
            {
                _currentKey = value;

                KeyChanged();
            }
        }

        public KeyDisplayContainer()
        {
            InitializeComponent();

            _redis = ServiceLocator.GetService<RedisProxy>();

            KeyChanged += HandleKeyChange;
        }

        private async void HandleKeyChange()
        {
            if (CurrentKey == default)
            {
                KeyContent = null;
            }
            else
            {
                // TODO: Remove ServerInfo and DatabaseId requirement from the redis proxy methods, we'll get those from the server context.
                var keyType = await _redis.GetKeyTypeAsync(ServerInfo, DatabaseId, CurrentKey);

                switch(keyType)
                {
                    // TODO: Simplify the required constructor parameters for stringkey, listkey, and hashkey by removing serverinfo and databaseid...

                    case RedisTypes.StringType:
                        KeyContent.Content = new StringKey(ServerInfo, DatabaseId, CurrentKey);
                        break;
                    case RedisTypes.ListType:
                        KeyContent.Content = new ListKey(ServerInfo, DatabaseId, CurrentKey);
                        break;
                    case RedisTypes.HashType:
                        KeyContent.Content = new HashKey(ServerInfo, DatabaseId, CurrentKey);
                        break;
                    default:
                        KeyContent.Content = null;
                        break;
                }
            }
        }
    }
}
