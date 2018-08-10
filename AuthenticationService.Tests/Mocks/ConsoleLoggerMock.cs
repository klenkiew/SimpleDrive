using System;
using Microsoft.Extensions.Logging;
using Moq;

namespace AuthenticationService.Tests.Mocks
{
    public class ConsoleLoggerMock<T> : Mock<ILogger<T>>
    {
        public ConsoleLoggerMock()
        {
            this
                .Setup(log => log.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(),
                    It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()))
                .Callback<LogLevel, EventId, object, Exception, Func<object, Exception, string>>(
                    (level, id, obj, exc, f) => Console.WriteLine(obj));
        }
    }
}