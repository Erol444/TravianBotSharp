using FluentResults;

namespace MainCore.Errors
{
    public class NeedLogin : Error
    {
        public NeedLogin() : base("Login page is detected.")
        {
        }
    }
}