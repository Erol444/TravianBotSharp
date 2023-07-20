using Microsoft.Extensions.DependencyInjection;

namespace MainCore.DependencyInjector
{
    public interface IInjector
    {
        IServiceCollection Configure(IServiceCollection services);
    }
}