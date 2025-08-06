namespace My.Framework.MongoDbAccessor.Interface
{
    /// <summary>
    /// Mongo服务容器
    /// </summary>
    public interface IMongoServiceContainer
    {
        /// <summary>
        /// 得到Mongodb操作服务
        /// </summary>
        /// <param name="configName">配置名</param>
        /// <returns></returns>
        IMongoService GetService(string configName);

    }
}
