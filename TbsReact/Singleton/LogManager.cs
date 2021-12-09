using TbsCore.Models.Logging;

namespace TbsReact.Singleton
{
    public class LogManager
    {
        private static readonly LogManager instance = new();
        private LogOutput Log;

        private LogManager()
        {
        }

        public static LogManager Instance
        {
            get
            {
                return instance;
            }
        }

        private void _setLogOutput(LogOutput logOutput)
        {
            instance.Log = logOutput;
            instance.Log.LogUpdated += LogUpdate;
        }

        public static void SetLogOutput(LogOutput logOutput)
        {
            instance._setLogOutput(logOutput);
        }

        private string _getLogData(string username)
        {
            return Log.GetLog(username);
        }

        public static string GetLogData(string username)
        {
            return instance._getLogData(username);
        }

        private void LogUpdate(object sender, UpdateLogEventArgs e)
        {
            UpdateLogData(e.Username);
        }

        public void UpdateLogData(string username)
        {
            if (!AccountManager.CheckGroup(username)) return;
            AccountManager.SendMessage(username, "logger", Log.GetLastLog(username));
        }
    }
}