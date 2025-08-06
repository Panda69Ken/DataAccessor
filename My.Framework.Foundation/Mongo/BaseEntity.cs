namespace My.Framework.Foundation.Mongo
{
    /// <summary>
    /// Mongodb文档基类
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; } = "";

        /// <summary>
        /// 时间
        /// </summary>
        public long Date { get; set; }
    }
}
