using FluentResults;

namespace MainCore.Helper.Interface
{
    public interface IInvalidPageHelper
    {
        Result CheckPage(int accountId);
    }
}