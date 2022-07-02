using System;

namespace MainCore.Services
{
    public interface ITimerManager : IDisposable
    {
        public void Start();
    }
}