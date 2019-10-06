using GuiR.Configuration;
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

                switch (keyType)
                {
                    case RedisTypes.StringType:
                        KeyContent.Content = new StringKey(CurrentKey);
                        break;
                    case RedisTypes.ListType:
                        KeyContent.Content = new ListKey(CurrentKey);
                        break;
                    case RedisTypes.HashType:
                        KeyContent.Content = new HashKey(CurrentKey);
                        break;
                    case RedisTypes.SetType:
                        KeyContent.Content = new SetKey(CurrentKey);
                        break;
                    case RedisTypes.SortedSetType:
                        KeyContent.Content = new SortedSetKey(CurrentKey);
                        break;
                    case RedisTypes.HyperLogLog:
                        KeyContent.Content = new HyperLogLogKey(CurrentKey);
                        break;
                    default:
                        KeyContent.Content = null;
                        break;
                }
            }
        }
    }
}
