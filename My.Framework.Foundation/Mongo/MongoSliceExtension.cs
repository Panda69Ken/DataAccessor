namespace My.Framework.Foundation.Mongo
{
    /// <summary>
    /// Mongodb片键扩展类
    /// </summary>
    public static class MongoSliceExtension
    {
        /// <summary>
        /// 生成MongoDB片键
        /// </summary>
        /// <returns></returns>
        public static string SliceValue(this string sliceKey)
        {
            var iCode = sliceKey.GetHashCode();
            return Math.Abs(iCode % 65536).ToString().PadLeft(5, '0');
        }
    }
}
