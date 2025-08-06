namespace My.Framework.RedisAccessor
{
    /// <summary>
    ///     订单号等的生成器
    /// </summary>
    public interface INumberCreater
    {
        /// <summary>
        ///     生成订单号订单格式为 前缀+日期格式+后缀长度，
        /// </summary>
        /// <param name="prefix">订单前缀</param>
        /// <param name="totalWidth">后缀的位数,默认8位</param>
        /// <param name="timeFormat">日期的格式,默认"yyMMddHHmm"</param>
        /// <returns>一个前缀+日期格式+后缀增长的订单号</returns>
        string Generate(string prefix = "", int totalWidth = 8, string timeFormat = "yyMMddHHmm");
    }
}
