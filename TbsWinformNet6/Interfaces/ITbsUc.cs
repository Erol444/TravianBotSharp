namespace TbsWinformNet6.Interfaces
{
    /// <summary>
    /// Interface for TBS UserControls
    /// </summary>
    public interface ITbsUc
    {
        /// <summary>
        /// Initialize the view
        /// </summary>
        /// <param name="obj"></param>
        void Init(object obj);
        /// <summary>
        /// Update the view
        /// </summary>
        void UpdateUc();

    }
}
