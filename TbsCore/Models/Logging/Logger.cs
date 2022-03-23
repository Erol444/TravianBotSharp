using System;
using Serilog;
using TbsCore.Extensions;
using TbsCore.Tasks;

namespace TbsCore.Models.Logging

{
    public class Logger
    {
        private readonly ILogger _logger;

        public Logger(string username)
        {
            _logger = Log.ForContext("Username", username);
        }

        public void Information(string message, BotTask obj = null)
        {
            if (obj != null)
            {
                _logger.Information($"[{obj.GetName()}] {message}");
            }
            else
            {
                _logger.Information(message);
            }
        }

        public void Warning(string message, BotTask obj = null)
        {
            if (obj != null)
            {
                _logger.Warning($"[{obj.GetName()}] {message}");
            }
            else
            {
                _logger.Warning(message);
            }
        }

        public void Error(Exception error, string message, BotTask obj = null)
        {
            if (obj != null)
            {
                _logger.Error(error, $"[{obj.GetName()}] {message}");
            }
            else
            {
                _logger.Error(error, message);
            }
        }
    }
}