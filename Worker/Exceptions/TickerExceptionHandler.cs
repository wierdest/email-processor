using System;
using TickerQ.Utilities.Enums;
using TickerQ.Utilities.Interfaces;

namespace Worker.Exceptions;

public class TickerExceptionHandler(
    ILogger<TickerExceptionHandler> logger
    
    ) : ITickerExceptionHandler
{
    public Task HandleCanceledExceptionAsync(Exception exception, Guid tickerId, TickerType tickerType)
    {
        logger.LogInformation(
            exception,
            "⏹️ Ticker job {TickerId} of type {TickerType} was canceled at {Time}.",
            tickerId,
            tickerType,
            DateTime.UtcNow
        );
        
        return Task.CompletedTask;
    }

    public Task HandleExceptionAsync(Exception exception, Guid tickerId, TickerType tickerType)
    {
        logger.LogError(
            exception,
            "❌ Ticker job {TickerId} of type {TickerType} failed at {Time}: {Message}",
            tickerId,
            tickerType,
            DateTime.UtcNow,
            exception.Message
        );

        return Task.CompletedTask;
    }
}
