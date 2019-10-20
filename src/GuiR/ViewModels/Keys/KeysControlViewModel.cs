using GuiR.Models;
using GuiR.Models.Virtualization;
using GuiR.Redis;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuiR.ViewModels.Keys
{
    public class KeysControlViewModel : ViewModelBase, IDisposable
    {
        private readonly RedisProxy _redis;
        private readonly IServerContext _serverContext;

        private FileSystemBackedKeyCollection _keyCollection;
        private VirtualizingCollection<string> _keysList;
        private string _keyFilter = "";
        private string _refreshButtonVisibility = "Visible";
        private string _cancelButtonVisibility = "Hidden";

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

        public string KeyFilter
        {
            get => _keyFilter;

            set
            {
                _keyFilter = value;

                RaisePropertyChangedEvent(nameof(KeyFilter));
            }
        }

        public VirtualizingCollection<string> KeysList
        {
            get => _keysList;

            set
            {
                _keysList = value;

                RaisePropertyChangedEvent(nameof(KeysList));
            }
        }

        public string RefreshButtonVisibility
        {
            get => _refreshButtonVisibility;

            set
            {
                _refreshButtonVisibility = value;

                RaisePropertyChangedEvent(nameof(RefreshButtonVisibility));
            }
        }

        public string CancelButtonVisibility
        {
            get => _cancelButtonVisibility;

            set
            {
                _cancelButtonVisibility = value;

                RaisePropertyChangedEvent(nameof(CancelButtonVisibility));
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
                _keyCollection = new FileSystemBackedKeyCollection(await _redis.GetKeysAsync(KeyFilter));
                _keyCollection.BackgroundLoadStarted += OnBackgroundKeyRefreshStarted;
                _keyCollection.BackgroundLoadComplete += OnBackgroundKeyRefreshCompleted;

                _keyCollection.PopulateFileSystem();

                await Task.Delay(250);

                var keysSource = new KeyItemsProvider(_keyCollection);

                KeysList = new VirtualizingCollection<string>(keysSource);
            });
                          
        public ICommand FilterKeys =>
            new DelegateCommand(() =>
            {
                var keysSource = new FilteredKeyItemsProvider(_keyCollection.FilterKeys(KeyFilter));

                KeysList = new VirtualizingCollection<string>(keysSource);
            });

        private void OnBackgroundKeyRefreshStarted(object sender, System.EventArgs e)
        {
            CancelButtonVisibility = "Visible";
            RefreshButtonVisibility = "Hidden";
        }

        private void OnBackgroundKeyRefreshCompleted(object sender, System.EventArgs e)
        {
            CancelButtonVisibility = "Hidden";
            RefreshButtonVisibility = "Visible";
        }

        public void Dispose()
        {
            if (_keyCollection != default)
            {
                _keyCollection.Dispose();
            }
        }
    }
}