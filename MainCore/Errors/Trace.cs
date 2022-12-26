using FluentResults;

namespace MainCore.Errors
{
    public class Trace : Error
    {
        public Trace(string message) : base(message)
        {
        }

        public static string TraceMessage(
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            return $"from file: {sourceFilePath} [{sourceLineNumber - 1}]";
        }
    }
}