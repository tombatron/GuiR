using GuiR.Configuration;
using GuiR.Models;
using GuiR.Redis;
using System.Windows;
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

        public static readonly DependencyProperty ServerInfoProperty = DependencyProperty.Register(nameof(ServerInfo), typeof(RedisServerInformation), typeof(KeyDisplayContainer));

        public RedisServerInformation ServerInfo
        {
            get => (RedisServerInformation)GetValue(ServerInfoProperty);

            set => SetValue(ServerInfoProperty, value);
        }

        public static readonly DependencyProperty DatabaseIdProperty = DependencyProperty.Register(nameof(DatabaseId), typeof(int), typeof(KeyDisplayContainer));

        public int DatabaseId
        {
            get => (int)GetValue(DatabaseIdProperty);

            set => SetValue(DatabaseIdProperty, value);
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
                var keyType = await _redis.GetKeyTypeAsync(ServerInfo, DatabaseId, CurrentKey);

                switch(keyType)
                {
                    case RedisTypes.StringType:
                        KeyContent.Content = new StringKey(ServerInfo, DatabaseId, CurrentKey);
                        break;
                    default:
                        KeyContent.Content = null;
                        break;
                }
            }
        }
    }
}
