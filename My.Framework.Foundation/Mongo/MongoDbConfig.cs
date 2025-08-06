namespace My.Framework.Foundation.Mongo
{
    /// <summary>
    /// Mongodb数据库配置信息
    /// </summary>
    public class MongoDbConfig
    {
        /// <summary>
        /// 配置名
        /// </summary>
        public string MongoConfigName { get; set; } = "";

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConncetionString { get; set; } = "";

        /// <summary>
        /// 写关注
        /// </summary>
        public string WriteCountersign { get; set; } = "";
    }
}
