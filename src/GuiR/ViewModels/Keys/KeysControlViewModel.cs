using GuiR.Models;
using GuiR.Models.Virtualization;
using GuiR.Redis;
using System;
using System.Collections.ObjectModel;
using System.Timers;
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
        private int _progressPercent;

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

        public ReadOnlyCollection<DatabaseInfo> Databases
        {
            get => _databases;

            set
            {
                _databases = value;

                RaisePropertyChangedEvent(nameof(Databases));
            }
        }

        public int ProgressPercent
        {
            get => _progressPercent;

            set
            {
                _progressPercent = value;

                RaisePropertyChangedEvent(nameof(ProgressPercent));
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
            new DelegateCommand(async () =>
            {
                KeysList = new VirtualizingCollection<string>(await _keyCollection.FilterKeysAsync(KeyFilter));
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

            StartBackgroundProgressMonitor();
        }

        private void OnBackgroundKeyRefreshProgress(object sender, EventArgs e) => UpdateKeysList();

        private void OnBackgroundKeyRefreshCompleted(object sender, EventArgs e)
        {
            CancelButtonVisibility = "Hidden";
            RefreshButtonVisibility = "Visible";

            StopBackgroundProgressMonitor();
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

        private Timer progressMonitorTimer;

        private void StartBackgroundProgressMonitor()
        {
            progressMonitorTimer = new Timer();
            progressMonitorTimer.Interval = 1_000;
            progressMonitorTimer.Elapsed += ProgressMonitorTimer_Elapsed;
            progressMonitorTimer.Start();
        }

        private void ProgressMonitorTimer_Elapsed(object sender, ElapsedEventArgs e) =>
            ProgressPercent = (int)(((double)_keyCollection.Count / _keyCollection.KeyInfo.KeyCount) * 100);

        private void StopBackgroundProgressMonitor()
        {
            progressMonitorTimer.Stop();
            progressMonitorTimer.Dispose();

            ProgressPercent = 0;
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