using My.Framework.Foundation.Redis;
using My.Framework.Foundation.Serializer;

namespace My.Framework.RedisAccessor.Configuration
{
    public interface IRedisCachingConfiguration
    {
        /// <summary>
        /// Redis的基本配置
        /// </summary>
        RedisConfig RedisConfig { get; }

        /// <summary>
        /// 选择要序列化的对象
        /// </summary>
        ISerializer Serializer { get; }

        /// <summary>
        /// 记录Redis的日志
        /// </summary>
        TextWriter LogWriter { get; }
    }
}
