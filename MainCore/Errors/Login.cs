using FluentResults;

namespace MainCore.Errors
{
    public class Login : Error
    {
        public Login() : base("Account is logged out.")
        {
        }
    }
}