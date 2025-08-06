using System.Xml;

namespace My.Framework.Foundation
{
    /// <summary>
    /// 针对XML的扩展
    /// </summary>
    public static class XmlExtension
    {
        /// <summary>
        /// 获取某个节点的文本内容
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetText(this XmlNode node)
        {
            return node == null ? string.Empty : node.InnerText;
        }
        /// <summary>
        /// 获取某个节点的值
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        public static string GetValue(this XmlNode attr)
        {
            return attr == null ? string.Empty : attr is XmlAttribute ? attr.Value : attr.InnerText;
        }
    }
}
