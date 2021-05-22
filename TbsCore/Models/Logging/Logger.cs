using System;
using Serilog;

namespace TbsCore.Models.Logging
{
    public class Logger
    {
        private ILogger _logger;

        public void Init(string username)
        {
            _logger = Log.ForContext("Username", username);
        }

        public void Information(string message)
        {
            _logger.Information(message);
        }

        public void Warning(string message)
        {
            _logger.Warning(message);
        }

        public void Error(Exception error, string message)
        {
            _logger.Error(error, message);
        }
    }
}