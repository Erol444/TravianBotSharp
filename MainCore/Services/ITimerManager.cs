namespace MainCore.Services
{
    public interface ITimerManager : IDisposable
    {
        public void Start();

        public void Stop();

        public event Action TaskExecute;
    }
}