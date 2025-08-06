namespace My.Framework.RedisAccessor.NumberCreater
{
    public class NumberRandomGreater : INumberCreater
    {
        /// <summary>
        /// 生成订单号订单格式为 前缀+日期格式+后缀长度，后缀长度是一个随机数，
        /// </summary>
        /// <param name="prefix">订单前缀</param>
        /// <param name="totalWidth">后缀的位数,默认8位</param>
        /// <param name="timeFormat">日期的格式,默认"yyMMddHHmm"</param>
        /// /// <exception cref="FormatException">日期格式异常</exception>
        /// <exception cref="ArgumentOutOfRangeException">日期格式异常或者totalWidth小于0的异常</exception>
        /// <returns>一个前缀+日期格式+后缀增长的订单号</returns>
        public string Generate(string prefix, int totalWidth = 8, string timeFormat = "yyMMddHHmm")
        {
            if (totalWidth < 0)
            {
                throw new ArgumentOutOfRangeException($"totalWidth = {totalWidth},totalWidth can not less than 0");
            }

            if (totalWidth == 0)
            {
                return $"{prefix}{DateTime.Now.ToString(timeFormat)}";
            }

            var fixWidth = totalWidth;
            var minValue = (int)Math.Pow(10, fixWidth - 1);
            var maxValue = (int)Math.Pow(10, fixWidth) - 1;

            string grouporderid = $"{prefix}{DateTime.Now.ToString(timeFormat)}{new Random().Next(minValue, maxValue)}";

            return grouporderid;
        }
    }
}
