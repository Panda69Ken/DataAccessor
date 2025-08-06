namespace My.Framework.Foundation.Mongo
{
    /// <summary>
    /// Mongodb分片基类
    /// </summary>
    public class SliceBaseEntity : BaseEntity
    {
        /// <summary>
        /// 片键
        /// </summary>
        public string SliceValue { get; set; } = "";
    }
}
