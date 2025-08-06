namespace My.Framework.Foundation.Redis
{
    /// <summary>
    /// 缓存配置
    /// </summary>
    public class RedisConfig
    {
        /// <summary>
        /// 要选择的Redis的数据库
        /// </summary>
        public int Database { get; set; }

        /// <summary>
        /// 缓存键前缀
        /// </summary>
        public string RedisKeyPrefix { get; set; } = "";

        /// <summary>
        /// Redis的连接串
        /// </summary>
        public string RedisConnect { get; set; } = "";

        /// <summary>
        /// 是否开启日志
        /// </summary>
        public bool HaveLog { get; set; }

        /// <summary>
        /// 是否关闭缓存
        /// </summary>
        public bool CloseRedis { get; set; }

        /// <summary>
        /// 服务器策略
        /// </summary>
        public ServerEnumerationStrategy ServerEnumerationStrategy { get; set; }
    }
}
