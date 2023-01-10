using FluentResults;

namespace MainCore.Errors
{
    public class NetworkProblem : Error
    {
        public NetworkProblem(string message) : base($"{message}.")
        {
        }
    }
}