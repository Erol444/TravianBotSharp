using FluentResults;
using System;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface ISleepHelper
    {
        Result Execute();

        TimeSpan GetWorkTime();

        void Load(int accountId, CancellationToken cancellationToken);
    }
}