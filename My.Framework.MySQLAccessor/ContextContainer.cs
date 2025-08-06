using Microsoft.Extensions.Options;
using My.Framework.Foundation.MySql;
using System.Collections.Concurrent;

namespace My.Framework.MySQLAccessor
{
    public class ContextContainer : IContextContainer
    {
        private static List<MySqlConnectionConfig> _connectionConfigs;

        private static readonly ConcurrentDictionary<string, IMySqlContext> ContextDict = new ConcurrentDictionary<string, IMySqlContext>();

        public ContextContainer(IOptions<List<MySqlConnectionConfig>> configs)
        {
            _connectionConfigs = configs.Value;
        }

        /// <summary>
        /// 得到一个Master MySql操作上下文
        /// </summary>
        /// <param name="contextName">配置名</param>
        /// <returns></returns>
        public IMySqlContext GetMasterContext(string contextName)
        {
            return ContextDict.GetOrAdd(contextName, x =>
            {
                var config = GetConfig(contextName);
                var context = new MySqlContext(config.MasterConncetString);
                return context;
            });
        }

        /// <summary>
        /// 随机获取一个Slave MySql操作上下文
        /// </summary>
        /// <param name="contextName">配置名</param>
        /// <returns></returns>
        public IMySqlContext GetSalveContextRandom(string contextName)
        {
            var config = GetConfig(contextName);
            var salveConnectinList = config.SlaveConnectStrings;
            var arrLength = salveConnectinList.Count;
            if (arrLength == 0)
            {
                throw new Exception($"配置名为[{contextName}]的数据库配置,不存在SlaveConnectStrings");
            }

            return new MySqlContext(salveConnectinList[new Random().Next(arrLength)]);
        }


        private MySqlConnectionConfig GetConfig(string contextName)
        {
            var config = _connectionConfigs.Find(x => x.MySqlConfigName == contextName);
            if (config == null)
            {
                throw new Exception($"不存在配置名为[{contextName}]的数据库配置");
            }

            return config;
        }
    }
}
