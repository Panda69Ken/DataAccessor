using Microsoft.Extensions.Logging;
using System.Text;

namespace My.Framework.RedisAccessor
{
    public class CacheRedisLog : TextWriter
    {
        public CacheRedisLog(ILogger<CacheRedisLog> logger)
        {
            Logger = logger;
        }

        public ILogger<CacheRedisLog> Logger { get; }

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            // NotImplementedException();
        }

        public override void Write(string format, params object[] arg)
        {
            Logger.LogDebug(format, arg);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            Logger.LogDebug(format, arg0, arg1, arg2);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            Logger.LogDebug(format, arg0, arg1);
        }

        public override void Write(string format, object arg0)
        {
            Logger.LogDebug(format, arg0);
        }

        public override void Write(string value)
        {
            Logger.LogDebug(value);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            Logger.LogDebug(format, arg);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            Logger.LogDebug(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            Logger.LogDebug(format, arg0, arg1);
        }

        public override void WriteLine(string format, object arg0)
        {
            Logger.LogDebug(format, arg0);
        }

        public override void WriteLine(string value)
        {
            Logger.LogDebug(value);
        }
    }
}
