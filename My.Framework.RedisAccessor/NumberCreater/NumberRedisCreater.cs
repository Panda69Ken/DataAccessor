namespace My.Framework.RedisAccessor.NumberCreater
{
    public class NumberRedisCreater : INumberCreater
    {
        /// <summary>
        ///     缓存的客户端
        /// </summary>
        private readonly ICacheRedisClient _cacheRedisClient;

        /// <summary>
        ///     失败重试的号码生成器
        /// </summary>
        private readonly INumberCreater _nextCreater;

        /// <summary>
        ///     默认构造函数是使用两次 NumberRedisCreater 进行重试
        /// </summary>
        /// <param name="cacheRedisClient"></param>
        public NumberRedisCreater(ICacheRedisClient cacheRedisClient) :
            this(new NumberRedisCreater(new NumberRandomGreater(), cacheRedisClient),
                cacheRedisClient)
        {
        }

        public NumberRedisCreater(INumberCreater nextCreater, ICacheRedisClient cacheRedisClient)
        {
            _nextCreater = nextCreater;
            _cacheRedisClient = cacheRedisClient;
        }

        /// <summary>
        ///     生成订单号订单格式为 前缀+日期格式+后缀长度，后缀长度是根据Redis的StringIncrement来自增长的，
        /// </summary>
        /// <param name="prefix">订单前缀</param>
        /// <param name="totalWidth">后缀的位数,默认8位</param>
        /// <param name="timeFormat">日期的格式,默认"yyMMddHHmm"</param>
        /// ///
        /// <exception cref="FormatException">日期格式异常</exception>
        /// <exception cref="ArgumentOutOfRangeException">日期格式异常或者totalWidth小于0的异常</exception>
        /// <returns>一个前缀+日期格式+后缀增长的订单号</returns>
        public string Generate(string prefix, int totalWidth = 8, string timeFormat = "yyMMddHHmm")
        {
            if (totalWidth < 0)
                throw new ArgumentOutOfRangeException($"totalWidth = {totalWidth},totalWidth can not less than 0");

            try
            {
                var key = $"{prefix}{DateTime.Now.ToString(timeFormat)}";

                if (totalWidth == 0)
                {
                    return key;
                }

                var database = _cacheRedisClient.Database;
                var lastNumber = database.StringIncrement(key);

                if (lastNumber == 1)
                    database.KeyExpireAsync(key, new TimeSpan(1, 0, 0, 0));

                return $"{key}{lastNumber.ToString().PadLeft(totalWidth, '0')}";
            }
            catch (FormatException)
            {
                throw;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }
            catch (Exception)
            {
                if (_nextCreater != null)
                    return _nextCreater.Generate(prefix, totalWidth, timeFormat);
                throw;
            }
        }
    }
}
