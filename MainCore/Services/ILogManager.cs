using MainCore.Models.Runtime;
using System;
using System.Collections.Generic;

namespace MainCore.Services
{
    public interface ILogManager : IDisposable
    {
        public void Init();

        public void AddAccount(int accountId);

        public LinkedList<LogMessage> GetLog(int accountId);

        public void Information(int accountId, string message);

        public void Warning(int accountId, string message);

        public void Error(int accountId, string message, Exception error);
    }
}