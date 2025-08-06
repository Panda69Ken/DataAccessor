using Microsoft.Extensions.Options;
using My.Framework.Foundation.MySql;

namespace My.Framework.MySQLAccessor
{
    public class MySqlAccessorConfigOptions
    {
        public MySqlAccessorConfigOptions(IOptions<List<MySqlConnectionConfig>> options)
        {
            ConnectionConfigs = options.Value;
        }

        /// <summary>
        /// 数据库连接配置集
        /// </summary>
        public List<MySqlConnectionConfig> ConnectionConfigs { get; }
    }
}
