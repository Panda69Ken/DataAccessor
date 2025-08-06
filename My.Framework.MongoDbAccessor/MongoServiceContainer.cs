using Microsoft.Extensions.Options;
using My.Framework.Foundation.Mongo;
using My.Framework.MongoDbAccessor.Interface;
using System.Collections.Concurrent;

namespace My.Framework.MongoDbAccessor
{
    /// <summary>
    /// Mongo服务容器
    /// </summary>
    public class MongoServiceContainer : IMongoServiceContainer
    {

        private static List<MongoDbConfig> _mongoInfoList = null;

        private static readonly ConcurrentDictionary<string, MongoDbContext> Container = new ConcurrentDictionary<string, MongoDbContext>();

        public MongoServiceContainer(IOptions<List<MongoDbConfig>> option)
        {
            _mongoInfoList = option.Value;
        }

        /// <inheritdoc />
        /// <summary>
        /// 得到Mongodb操作服务
        /// </summary>
        /// <param name="configName">配置名</param>
        /// <returns></returns>
        public IMongoService GetService(string configName)
        {

            return Container.GetOrAdd(configName, x =>
            {
                var mongoInfo = _mongoInfoList.Find(c => c.MongoConfigName == configName);
                if (mongoInfo == null)
                {
                    throw new Exception($"未发现{configName}配置节点!");
                }

                var mongoDbContext = new MongoDbContext(mongoInfo.ConncetionString);
                return mongoDbContext;
            });

        }
    }
}
