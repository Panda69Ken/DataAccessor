using System.Globalization;

namespace My.Framework.Foundation
{
    /// <summary>
    /// 时间转换辅助类
    /// </summary>
    public static class TimeExtension
    {
        /// <summary>
        /// 用于计算时间戳的时间值
        /// </summary>
        private static DateTime _unixTimestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// UNIX 最小时间
        /// </summary>
        /// <returns>C#格式时间</returns>
        public static DateTime UnixMiniTime()
        {
            return _unixTimestamp.ToLocalTime();
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime L2D(this string timeStamp)
        {
            DateTime dtStart = _unixTimestamp.ToLocalTime();
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime L2D(this long timeStamp)
        {
            DateTime dtStart = _unixTimestamp.ToLocalTime();
            return dtStart.AddMilliseconds(timeStamp);
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static long D2L(this DateTime time)
        {
            DateTime startTime = _unixTimestamp.ToLocalTime();
            return (long)(time - startTime).TotalMilliseconds;
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳(10位)格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static int D2ISecond(this DateTime time)
        {
            DateTime startTime = _unixTimestamp.ToLocalTime();
            return (int)(time - startTime).TotalSeconds;
        }

        /// <summary>
        ///  DateTime时间格式转成短时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ToInt64(DateTime time)
        {
            return (long)(time.ToUniversalTime() - _unixTimestamp).TotalSeconds;
        }

        /// <summary>
        /// 时间转换为简明时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static string ConciseTime(this long timeStamp)
        {
            return ConciseTime(L2D(timeStamp));
        }

        /// <summary>
        /// 根据生日计算年龄
        /// </summary>
        /// <param name="birthDate"></param>
        /// <returns></returns>
        public static int CalculateAgeCorrect(this DateTime birthDate)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - birthDate.Year;
            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day)) age--;
            return age;
        }

        /// <summary>
        /// 使用C#把发表的时间改为几个月,几天前,几小时前,几分钟前,或几秒前
        /// 2008年03月15日 星期六 02:35
        ///  ref： http://www.cnblogs.com/summers/p/3225716.html
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ConciseTime(this DateTime datetime)
        {
            if (datetime.Date == DateTime.Now.Date)
            {
                TimeSpan span = DateTime.Now - datetime;
                if (span.TotalHours > 1)
                {
                    return $"{datetime:HH:mm}";
                }
                else
                {
                    if (span.TotalMinutes <= 1)
                        return "刚刚";
                    else
                        return $"{(int)span.TotalMinutes}分钟前";
                }
            }
            else
            {
                if ((DateTime.Now.Date - datetime.Date).Days == 1)
                {
                    return $"昨天{datetime:HH:mm}";
                }
                else
                {
                    return $"{datetime:yyyy-MM-dd HH:mm}";
                }
            }
        }

        /// <summary>
        /// 获取RFC822文档定义的时间格式字符串,如: Thu, 21 Dec 2000 16:01:07 +0800
        /// </summary>
        /// <param name="time">需要转换的时间</param>
        /// <returns>RFC822格式表示的时间字符串</returns>
        public static string ToRFC822Time(this DateTime time)
        {
            string rfcTime = time.ToString("ddd, dd MMM yyyy HH':'mm':'ss", CultureInfo.InvariantCulture);
            rfcTime += time.ToString(" zzz").Replace(":", "");
            return rfcTime;
        }
    }
}
