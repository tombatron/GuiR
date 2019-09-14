using GuiR.Models;
using System;
using System.Windows.Input;

namespace GuiR.ViewModels
{
    public class NewServer : ViewModelBase
    {
        private string _serverName;
        private string _serverAddress;
        private int _serverPort = 6379; // Default Redis Port

        public string ServerName
        {
            get => _serverName;

            set
            {
                _serverName = value;

                RaisePropertyChangedEvent(nameof(ServerName));
            }
        }

        public string ServerAddress
        {
            get => _serverAddress;

            set
            {
                _serverAddress = value;

                RaisePropertyChangedEvent(nameof(ServerAddress));
            }
        }

        public int ServerPort
        {
            get => _serverPort;

            set
            {
                _serverPort = value;

                RaisePropertyChangedEvent(nameof(ServerPort));
            }
        }

        public Action<RedisServerInformation> OnSave { get; set; }

        public ICommand GetServerInformation =>
            new DelegateCommand(() => OnSave(new RedisServerInformation { ServerAddress = ServerAddress, ServerName = ServerName, ServerPort = ServerPort }));
    }
}