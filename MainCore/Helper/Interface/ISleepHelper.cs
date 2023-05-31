using FluentResults;
using MainCore.Models.Database;
using System;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface ISleepHelper
    {
        Result Execute(int accountId, CancellationToken token);

        Access GetNextAccess(int accountId);

        TimeSpan GetSleepTime(int accountId);

        TimeSpan GetWorkTime(int accountId);

        bool IsForceSleep(int accountId);

        Result Sleep(int accountId, DateTime sleepEnd, CancellationToken token);

        Result WakeUp(int accountId, Access nextAccess);
    }
}