using Avalonia.Controls;
using System.Threading.Tasks;

namespace TbsCrossPlatform.Services
{
    public interface IWaitingService
    {
        public Task Show();

        public void Close();
    }
}