using System;

namespace MainCore.Services.Interface
{
    public interface ITimerManager : IDisposable
    {
        public void Start(int index);
    }
}