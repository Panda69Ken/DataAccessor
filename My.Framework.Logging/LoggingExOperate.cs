using Microsoft.Extensions.Logging;

namespace My.Framework.Logging
{
    public static class LoggingExOperate
    {

        //${event-properties:item=Tags}

        /// <summary>
        /// 记录Error日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">错误描述</param>
        /// <param name="args">参数（生成ExceptionlessTags）</param>
        public static void LogErrorEx(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (args == null || args.Length == 0)
            {
                logger.LogError(message);
            }
            else
            {
                logger.Log(LogLevel.Error,
                    default(EventId),
                    new NlogEvent(message)
                        .AddProp("Tags", new HashSet<string>(args.Where(m => m != null).Select(m => m.ToString()))),
                    //.AddProp("MemberId", 1234567890),
                    null,
                    NlogEvent.Formatter);
            }
        }

        /// <summary>
        /// 记录Debug日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">错误描述</param>
        /// <param name="args">参数（生成ExceptionlessTags）</param>
        public static void LogDebugEx(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (args == null || args.Length == 0)
            {
                logger.LogDebug(message);
            }
            else
            {
                logger.Log(Microsoft.Extensions.Logging.LogLevel.Debug,
                    default(EventId),
                    new NlogEvent(message).AddProp("Tags",
                        new HashSet<string>(args.Where(m => m != null).Select(m => m.ToString()))),
                    null,
                    NlogEvent.Formatter);
            }
        }

        /// <summary>
        /// 记录Info日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">错误描述</param>
        /// <param name="args">参数（生成ExceptionlessTags）</param>
        public static void LogInformationEx(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (args == null || args.Length == 0)
            {
                logger.LogInformation(message);
            }
            else
            {
                logger.Log(Microsoft.Extensions.Logging.LogLevel.Information,
                    default(EventId),
                    new NlogEvent(message).AddProp("Tags",
                        new HashSet<string>(args.Where(m => m != null).Select(m => m.ToString()))),
                    null,
                    NlogEvent.Formatter);
            }
        }


        /// <summary>
        /// 记录Warn日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">错误描述</param>
        /// <param name="args">参数（生成ExceptionlessTags）</param>
        public static void LogWarningEx(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (args == null || args.Length == 0)
            {
                logger.LogWarning(message);
            }
            else
            {
                logger.Log(Microsoft.Extensions.Logging.LogLevel.Warning,
                    default(EventId),
                    new NlogEvent(message).AddProp("Tags",
                        new HashSet<string>(args.Where(m => m != null).Select(m => m.ToString()))),
                    null,
                    NlogEvent.Formatter);
            }
        }


        /// <summary>
        /// 记录Error日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">异常</param>
        /// <param name="message">错误描述</param>
        /// <param name="args">参数（生成ExceptionlessTags）</param>
        public static void LogErrorEx(this ILogger logger, Exception exception, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (args == null || args.Length == 0)
            {
                logger.LogError(exception, message);
            }
            else
            {
                logger.Log(Microsoft.Extensions.Logging.LogLevel.Error,
                    default(EventId),
                    new NlogEvent(message).AddProp("Tags",
                        new HashSet<string>(args.Where(m => m != null).Select(m => m.ToString()))),
                    exception,
                    NlogEvent.Formatter);
            }
        }

        /// <summary>
        /// 记录Debug日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">异常</param>
        /// <param name="message">错误描述</param>
        /// <param name="args">参数（生成ExceptionlessTags）</param>
        public static void LogDebugEx(this ILogger logger, Exception exception, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (args == null || args.Length == 0)
            {
                logger.LogDebug(exception, message);
            }
            else
            {
                logger.Log(Microsoft.Extensions.Logging.LogLevel.Debug,
                    default(EventId),
                    new NlogEvent(message).AddProp("Tags",
                        new HashSet<string>(args.Where(m => m != null).Select(m => m.ToString()))),
                    exception,
                    NlogEvent.Formatter);
            }
        }

        /// <summary>
        /// 记录Info日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">异常</param>
        /// <param name="message">错误描述</param>
        /// <param name="args">参数（生成ExceptionlessTags）</param>
        public static void LogInformationEx(this ILogger logger, Exception exception, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (args == null || args.Length == 0)
            {
                logger.LogInformation(exception, message);
            }
            else
            {
                logger.Log(Microsoft.Extensions.Logging.LogLevel.Information,
                    default(EventId),
                    new NlogEvent(message).AddProp("Tags",
                        new HashSet<string>(args.Where(m => m != null).Select(m => m.ToString()))),
                    exception,
                    NlogEvent.Formatter);
            }
        }


        /// <summary>
        /// 记录Warn日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">异常</param>
        /// <param name="message">错误描述</param>
        /// <param name="args">参数（生成ExceptionlessTags）</param>
        public static void LogWarningEx(this ILogger logger, Exception exception, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (args == null || args.Length == 0)
            {
                logger.LogWarning(exception, message);
            }
            else
            {
                logger.Log(Microsoft.Extensions.Logging.LogLevel.Warning,
                    default(EventId),
                    new NlogEvent(message).AddProp("Tags",
                        new HashSet<string>(args.Where(m => m != null).Select(m => m.ToString()))),
                    exception,
                    NlogEvent.Formatter);
            }
        }



    }
}
