using System.Threading.Tasks;

namespace MainCore.Services.Interface
{
    public interface IUseragentManager
    {
        public string Get();

        public Task Load();
    }
}