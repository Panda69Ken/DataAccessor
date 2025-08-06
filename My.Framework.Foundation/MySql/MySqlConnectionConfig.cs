namespace My.Framework.Foundation.MySql
{
    /// <summary>
    /// MySql连接配置
    /// </summary>
    public class MySqlConnectionConfig
    {
        /// <summary>
        /// 配置名
        /// </summary>
        public string MySqlConfigName { get; set; } = "";

        /// <summary>
        /// MySql主库连接字符串
        /// </summary>
        public string MasterConncetString { get; set; } = "";

        /// <summary>
        /// 从库连接字符串集合
        /// </summary>

        public List<string> SlaveConnectStrings { get; set; }
    }
}
