using System.Threading.Tasks;

namespace MainCore.Services
{
    public interface IUseragentManager
    {
        public string Get();

        public Task Load();
    }
}