using MainCore.Models.Runtime;
using MainCore.Tasks.Base;
using System;
using System.Collections.Generic;

namespace MainCore.Helper.Interface
{
    public interface ILogHelper
    {
        void Error(int accountId, string message);

        void Error(int accountId, string message, BotTask task);

        void Error(int accountId, string message, Exception error);

        LinkedList<LogMessage> GetLog(int accountId);

        void Information(int accountId, string message);

        void Information(int accountId, string message, BotTask task);

        void Init();

        void Shutdown();

        void Warning(int accountId, string message);

        void Warning(int accountId, string message, BotTask task);
    }
}