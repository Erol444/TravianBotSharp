using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TbsCore.Models.Logging
{
    public class LogOutput
    {
        public event EventHandler<UpdateLogEventArgs> LogUpdated;

        private IDictionary<string, LinkedList<string>> _logs = new Dictionary<string, LinkedList<string>>();
        private readonly object _syncRoot = new object();

        public string GetLog(string username)
        {
            if (_logs.ContainsKey(username))
            {
                lock (_syncRoot)
                {
                    var log = string.Join("", _logs[username]);
                    return log;
                }
            }
            return "";
        }

        public string GetLastLog(string username)
        {
            return _logs.ContainsKey(username) ? _logs[username].First.Value : "";
        }

        /// <summary>
        /// lock before use because as i read Dictionary is not theard safe
        /// </summary>
        /// <param name="username"></param>
        /// <param name="message"></param>
        public void Add(string username, string message)
        {
            lock (_syncRoot)
            {
                _logs[username].AddFirst(message);
                // keeps 200 message
                while (_logs[username].Count > 200)
                {
                    _logs[username].RemoveLast();
                }
            }
            OnUpdateLog(username);
        }

        public void AddUsername(string username)
        {
            if (!_logs.ContainsKey(username))
            {
                _logs.Add(username, new LinkedList<string>());
            }
        }

        private void OnUpdateLog(string username)
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