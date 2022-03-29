using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TbsCore.Models.Logging
{
    public class LogOutput
    {
        private LogOutput()
        {
        }

        private static LogOutput instance = null;
        private readonly object objLock = new object();

        public static LogOutput Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LogOutput();
                }
                return instance;
            }
        }

        public event EventHandler<UpdateLogEventArgs> LogUpdated;

        private readonly ConcurrentDictionary<string, LinkedList<string>> _logs = new ConcurrentDictionary<string, LinkedList<string>>();

        public string GetLog(string username)
        {
            if (_logs.ContainsKey(username))
            {
                lock (objLock)
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

        public void AddUsername(string username)
        {
            _logs.TryAdd(username, new LinkedList<string>());
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