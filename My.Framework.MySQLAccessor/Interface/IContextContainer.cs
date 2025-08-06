namespace My.Framework.MySQLAccessor
{
    public interface IContextContainer
    {
        /// <summary>
        /// 得到一个Master MySql操作上下文
        /// </summary>
        /// <param name="contextName">配置名</param>
        /// <returns></returns>
        IMySqlContext GetMasterContext(string contextName);

        /// <summary>
        /// 随机获取一个Slave MySql操作上下文
        /// </summary>
        /// <param name="contextName">配置名</param>
        /// <returns></returns>
        IMySqlContext GetSalveContextRandom(string contextName);

    }
}
