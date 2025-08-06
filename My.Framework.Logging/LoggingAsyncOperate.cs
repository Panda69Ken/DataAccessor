using Microsoft.Extensions.Logging;

namespace My.Framework.Logging
{
    public static class LoggingAsyncOperate
    {
        /// <summary>异步写入日志条目.</summary>
        /// <param name="logger"></param>
        /// <param name="logLevel">条目将写在这个级别上.</param>
        /// <param name="eventId">时间Id.</param>
        /// <param name="state">要写的条目。也可以是对象.</param>
        /// <param name="exception">与此条目相关的异常.</param>
        /// <param name="formatter">函数创建 <c>string</c> 消息 <paramref name="state" /> 和 <paramref name="exception" />.</param>
        public static Task LogAsync<TState>(this ILogger logger, LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            return Task.Run(() => { logger.Log<TState>(logLevel, eventId, state, exception, formatter); });
        }

        /// <summary>异步格式化并写入错误日志消息.</summary>
        /// <param name="logger">这 <see cref="T:Microsoft.Extensions.Logging.ILogger" /> 要写入.</param>
        /// <param name="eventId">与日志关联的时间Id.</param>
        /// <param name="exception">异常日志.</param>
        /// <param name="message">日志消息的格式化字符串.</param>
        /// <param name="args">包含0个或更多个对象格式的对象数组.</param>
        public static Task LogErrorAsync(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            return Task.Run(() =>
            {
                logger.LogError(eventId, exception, message, args);
            });
        }

        /// <summary>异步格式化并写入错误日志消息.</summary>
        /// <param name="logger">这 <see cref="T:Microsoft.Extensions.Logging.ILogger" /> 要写入.</param>
        /// <param name="eventId">与日志关联的事件ID.</param>
        /// <param name="message">日志消息的格式字符串.</param>
        /// <param name="args">包含0个或更多个对象格式的对象数组.</param>
        public static Task LogErrorAsync(this ILogger logger, EventId eventId, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            return Task.Run(() => { logger.LogError(eventId, message, args); });
        }

        /// <summary>异步格式化并写入错误日志消息.</summary>
        /// <param name="logger">这 <see cref="T:Microsoft.Extensions.Logging.ILogger" /> 要写入.</param>
        /// <param name="exception">日志异常.</param>
        /// <param name="message">日志消息的格式字符串.</param>
        /// <param name="args">包含0个或更多个对象格式的对象数组.</param>
        public static Task LogErrorAsync(this ILogger logger, Exception exception, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            return Task.Run(() =>
            {
                logger.LogError(exception, message, args);
            });
        }

        /// <summary>异步格式化并写入错误日志消息.</summary>
        /// <param name="logger">这 <see cref="T:Microsoft.Extensions.Logging.ILogger" /> 要写入.</param>
        /// <param name="message">日志消息格式化字符串.</param>
        /// <param name="args">包含0个或更多个对象格式的对象数组.</param>
        public static Task LogErrorAsync(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            return Task.Run(() => { logger.LogError(message, args); });
        }

        /// <summary>异步格式化写入信息日志信息.</summary>
        /// <param name="logger">这 <see cref="T:Microsoft.Extensions.Logging.ILogger" /> 要写入.</param>
        /// <param name="eventId">与日志关联的事件ID.</param>
        /// <param name="exception">异常日志.</param>
        /// <param name="message">日志消息的格式字符串.</param>
        /// <param name="args">包含0个或更多个对象格式的对象数组.</param>
        public static Task LogInfoAsync(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            return Task.Run(() => { logger.LogInformation(eventId, exception, message, args); });
        }

        /// <summary>异步格式化写入信息日志信息.</summary>
        /// <param name="logger">这 <see cref="T:Microsoft.Extensions.Logging.ILogger" /> 要写入.</param>
        /// <param name="eventId">与日志关联的事件ID.</param>
        /// <param name="message">日志消息的格式字符串.</param>
        /// <param name="args">包含0个或更多个对象格式的对象数组.</param>
        public static Task LogInfoAsync(this ILogger logger, EventId eventId, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            return Task.Run(() => { logger.LogInformation(eventId, message, args); });
        }

        /// <summary>异步格式化写入信息日志信息.</summary>
        /// <param name="logger">这 <see cref="T:Microsoft.Extensions.Logging.ILogger" /> 要写入.</param>
        /// <param name="exception">异常日志.</param>
        /// <param name="message">日志消息的格式字符串.</param>
        /// <param name="args">包含0个或更多个对象格式的对象数组.</param>
        public static Task LogInfoAsync(this ILogger logger, Exception exception, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            return Task.Run(() => { logger.LogInformation(exception, message, args); });
        }

        /// <summary>异步格式化写入信息日志信息.</summary>
        /// <param name="logger">这 <see cref="T:Microsoft.Extensions.Logging.ILogger" /> 要写入.</param>
        /// <param name="message">日志消息的格式字符串.</param>
        /// <param name="args">包含0个或更多个对象格式的对象数组.</param>
        public static Task LogInfoAsync(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            return Task.Run(() => { logger.LogInformation(message, args); });
        }

        /// <summary>异步格式化并写入警告日志消息.</summary>
        /// <param name="logger">这 <see cref="T:Microsoft.Extensions.Logging.ILogger" /> 要写入.</param>
        /// <param name="eventId">与日志关联的事件ID.</param>
        /// <param name="exception">日志异常.</param>
        /// <param name="message">日志消息的格式字符串.</param>
        /// <param name="args">包含0个或更多个对象格式的对象数组.</param>
        public static Task LogWarningAsync(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            return Task.Run(() => { logger.LogWarning(eventId, exception, message, args); });
        }

        /// <summary>异步格式化并写入警告日志消息.</summary>
        /// <param name="logger">这 <see cref="T:Microsoft.Extensions.Logging.ILogger" /> 要写入.</param>
        /// <param name="eventId">与日志关联的事件ID.</param>
        /// <param name="message">日志消息的格式字符串.</param>
        /// <param name="args">包含0个或更多个对象格式的对象数组.</param>
        public static Task LogWarningAsync(this ILogger logger, EventId eventId, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            return Task.Run(() => { logger.LogWarning(eventId, message, args); });
        }


        /// <summary>异步格式化并写入警告日志消息.</summary>
        /// <param name="logger">这 <see cref="T:Microsoft.Extensions.Logging.ILogger" /> 要写入.</param>
        /// <param name="exception">日志异常</param>
        /// <param name="message">日志消息的格式字符串.</param>
        /// <param name="args">包含0个或更多个对象格式的对象数组.</param>
        public static Task LogWarningAsync(this ILogger logger, Exception exception, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            return Task.Run(() =>
            {
                logger.LogWarning(exception, message, args);
            });
        }


        /// <summary>异步格式化并写入警告日志消息.</summary>
        /// <param name="logger">这 <see cref="T:Microsoft.Extensions.Logging.ILogger" /> 要写入.</param>
        /// <param name="message">日志消息的格式字符串.</param>
        /// <param name="args">包含0个或更多个对象格式的对象数组.</param>
        public static Task LogWarningAsync(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            return Task.Run(() => { logger.LogWarning(message, args); });
        }

    }
}
