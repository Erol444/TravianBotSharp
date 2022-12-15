namespace MainCore.Services.Interface
{
    public interface ITimerManager
    {
        public void Start(int index);

        public void Shutdown();
    }
}