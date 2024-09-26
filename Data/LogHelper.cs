using System;
using Microsoft.Extensions.Configuration;
using Serilog;

public sealed class LogHelper
{
    private readonly ILogger _logger;

    public LogHelper(IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        _logger = Log.Logger;
    }

    public void LogInfo(string message)
    {
        _logger.Information(message);
    }

    public void LogError(string message, Exception ex)
    {
        _logger.Error(ex, message);
    }
}
