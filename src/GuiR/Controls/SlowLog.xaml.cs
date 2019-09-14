using GuiR.Models;
using System.Windows.Controls;

namespace GuiR.Controls
{
    public partial class SlowLog : UserControl
    {
        private readonly RedisServerInformation _serverInfo;

        public SlowLog(RedisServerInformation serverInfo)
        {
            InitializeComponent();

            _serverInfo = serverInfo;
        }
    }
}
