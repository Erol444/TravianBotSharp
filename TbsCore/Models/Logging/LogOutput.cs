using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TbsCore.Models.Logging
{
    public class LogOutput
    {
        public event EventHandler<UpdateLogEventArgs> LogUpdated;

        private IDictionary<string, LinkedList<string>> _logs = new Dictionary<string, LinkedList<string>>();

        public string GetLog(string username)
        {
            return string.Join("\n", _logs[username]);
        }

        public string GetLastLog(string username)
        {
            return _logs[username].First.Value;
        }

        public void Add(string username, string message)
        {
            _logs[username].AddFirst(message);
            // keeps 200 message
            while (_logs[username].Count > 200)
            {
                _logs[username].RemoveLast();
            }
            OnUpdateLog(username);
        }

        protected void OnUpdateLog(string username)
        {
            LogUpdated?.Invoke(this, new UpdateLogEventArgs(username));
        }
    }

    public class UpdateLogEventArgs : EventArgs
    {
        public string Username { get; set; }

        public UpdateLogEventArgs(string username)
        {
            Username = username;
        }
    }
}