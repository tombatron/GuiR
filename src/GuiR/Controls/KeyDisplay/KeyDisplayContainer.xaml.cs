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
                var keyType = await _redis.GetKeyTypeAsync(CurrentKey);

                switch(keyType)
                {
                    // TODO: Simplify the required constructor parameters for stringkey, listkey, and hashkey by removing serverinfo and databaseid...

                    case RedisTypes.StringType:
                        KeyContent.Content = new StringKey(CurrentKey);
                        break;
                    case RedisTypes.ListType:
                        KeyContent.Content = new ListKey(CurrentKey);
                        break;
                    case RedisTypes.HashType:
                        KeyContent.Content = new HashKey(CurrentKey);
                        break;
                    default:
                        KeyContent.Content = null;
                        break;
                }
            }
        }
    }
}
