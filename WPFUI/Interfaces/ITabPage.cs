namespace WPFUI.Interfaces
{
    public interface ITabPage
    {
        public bool IsActive { get; set; }

        public void OnActived();

        public void OnDeactived();
    }
}