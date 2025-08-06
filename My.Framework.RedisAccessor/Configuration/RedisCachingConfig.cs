using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using My.Framework.Foundation.Redis;
using My.Framework.Foundation.Serializer;

namespace My.Framework.RedisAccessor.Configuration
{
    public class RedisCachingConfig : IRedisCachingConfiguration
    {

        public RedisCachingConfig(IOptions<RedisConfig> options,
            ISerializer serializer,
            ILogger<CacheRedisLog> logger)
        {
            RedisConfig = options.Value;
            Serializer = serializer;
            LogWriter = RedisConfig.HaveLog ? new CacheRedisLog(logger) : null;
        }

        public RedisConfig RedisConfig { get; }

        public ISerializer Serializer { get; }

        public TextWriter LogWriter { get; }
    }
}