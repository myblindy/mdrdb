using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace mdrdb.Infrastructure
{
    public class EFLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new EFLogger();

        public void Dispose()
        {
        }

        private class EFLogger : ILogger
        {
            public IDisposable BeginScope<TState>(TState state) => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                Debug.WriteLine(formatter(state, exception));
            }
        }
    }
}
