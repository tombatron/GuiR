using GuiR.Models;
using GuiR.Models.Virtualization;
using GuiR.Redis;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GuiR.ViewModels.Keys
{
    public class KeysControlViewModel : ViewModelBase, IDisposable
    {
        private readonly RedisProxy _redis;
        private readonly IServerContext _serverContext;

        private FileSystemBackedKeyCollection _keyCollection;
        private VirtualizingCollection<string> _keysList;
        private ReadOnlyCollection<DatabaseInfo> _databases;
        private string _keyFilter = "";
        private string _refreshButtonVisibility = "Visible";
        private string _cancelButtonVisibility = "Hidden";
        private bool _isBackgroundRefreshActive = false;

        private int _databaseId = 0;
        public int DatabaseId
        {
            get => _databaseId;

            set
            {
                var selectedDatabase = Databases[value];
                _databaseId = selectedDatabase.Id;
                _serverContext.DatabaseId = selectedDatabase.Id;

                ClearKeysList();

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

        public bool IsBackgroundRefreshActive
        {
            get => _isBackgroundRefreshActive;

            set
            {
                _isBackgroundRefreshActive = value;

                RaisePropertyChangedEvent(nameof(IsBackgroundRefreshActive));
            }
        }

        public ReadOnlyCollection<DatabaseInfo> Databases
        {
            get => _databases;

            set
            {
                _databases = value;

                RaisePropertyChangedEvent(nameof(Databases));
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
                if (_keyCollection != default)
                {
                    _keyCollection.Dispose();
                }

                _keyCollection = new FileSystemBackedKeyCollection(await _redis.GetKeysAsync());
                _keyCollection.BackgroundLoadStarted += OnBackgroundKeyRefreshStarted;
                _keyCollection.BackgroundLoadProgress += OnBackgroundKeyRefreshProgress;
                _keyCollection.BackgroundLoadComplete += OnBackgroundKeyRefreshCompleted;

                await _keyCollection.PopulateFileSystemAsync();

                UpdateKeysList();
            });

        public ICommand FilterKeys =>
            new DelegateCommand(() =>
            {
                var keysSource = new FilteredKeyItemsProvider(_keyCollection.FilterKeys(KeyFilter));

                KeysList = new VirtualizingCollection<string>(keysSource);
            });

        public ICommand CancelRefreshKeys =>
            new DelegateCommand(() =>
            {
                if (_keyCollection != default)
                {
                    _keyCollection.CancelBackgroundLoading();
                }
            });

        public ICommand LoadDatabases =>
            new DelegateCommand(async () => 
            {
                Databases = await _redis.GetDatabaseInfoAsync();
            });

        private void OnBackgroundKeyRefreshStarted(object sender, EventArgs e)
        {
            CancelButtonVisibility = "Visible";
            RefreshButtonVisibility = "Hidden";
            IsBackgroundRefreshActive = true;
        }

        private void OnBackgroundKeyRefreshProgress(object sender, EventArgs e) => UpdateKeysList();

        private void OnBackgroundKeyRefreshCompleted(object sender, EventArgs e)
        {
            CancelButtonVisibility = "Hidden";
            RefreshButtonVisibility = "Visible";
            IsBackgroundRefreshActive = false;
        }

        private void UpdateKeysList() =>
            KeysList = new VirtualizingCollection<string>(new KeyItemsProvider(_keyCollection));

        private void ClearKeysList()
        {
            KeysList = new VirtualizingCollection<string>(new EmptyKeyItemsProvider());

            if (_keyCollection != default)
            {
                _keyCollection.Dispose();
                _keyCollection = default;
            }
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