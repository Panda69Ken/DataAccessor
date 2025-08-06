namespace My.Framework.Foundation
{
    /// <summary>
    /// 有序唯一ID生成器
    /// </summary>
    public sealed class SGUID
    {
        /// <summary>
        /// 随机数生成器。
        /// </summary>
        static Random rd = new Random();

        /// <summary>
        /// 获取有序的唯一ID。
        /// 理论依据参见 http://zh.wikipedia.org/wiki/GUID
        /// 相似实见参见NHibernate的同名方法。
        /// </summary>
        /// <returns></returns>
        public static Guid GenerateComb()
        {
            //取原始Guid。
            string strGuid = Guid.NewGuid().ToString("N");

            //随机数换掉版本号，增加随机性。
            int intRd = rd.Next(0, 16);
            string strRd = intRd.ToString("X");
            strGuid = strGuid.Remove(12, 1);
            strGuid = strGuid.Insert(12, strRd);

            //取12位16进制的时间元(毫秒数)。
            DateTime now = DateTime.UtcNow;
            DateTime baseTime = DateTime.MinValue.AddYears(1100);
            TimeSpan diff = now - baseTime;
            long timestamp = (long)diff.TotalMilliseconds;
            string strTimestamp = timestamp.ToString("X").PadLeft(12, '0');

            //删除原始Guid串中最后的12位时间相关的数据
            string strPatrGUID = strGuid.Substring(0, 20);
            //将生成的12位时间元，加到Guid串的头部使其有序。
            string strSGuid = strTimestamp + strPatrGUID;

            //生成新的有序的Guid。
            return new Guid(strSGuid);
        }
    }
}
