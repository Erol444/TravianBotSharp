using FluentResults;

namespace MainCore.Errors
{
    public class NeedLogin : Error
    {
        public NeedLogin() : base("Account is logged out.")
        {
        }
    }
}