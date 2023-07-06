using FluentResults;

namespace MainCore.Helper.Interface
{
    public interface ILoginHelper
    {
        Result Execute(int accountId);
    }
}