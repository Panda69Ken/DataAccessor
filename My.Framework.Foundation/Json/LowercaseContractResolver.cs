using Newtonsoft.Json.Serialization;

namespace My.Framework.Foundation.Json
{
    /// <summary>
    /// json小写
    /// </summary>
    public class LowercaseContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// 解决属性名（json属性名转成小写）
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}
