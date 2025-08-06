using System.Data;
using System.Text;

namespace My.Framework.Foundation
{
    /// <summary>
    /// 与System.Data.DataTable相关扩展方法
    /// </summary>
    public static class DataTableExtension
    {
        /// <summary>
        /// 将某个DataTable数据转换为CSV文本字符数据
        /// </summary>
        /// <param name="table">包含有数据的DataTable对象</param>
        /// <returns>csv格式的文本字符串</returns>
        public static string ToCSVText(this DataTable table)
        {
            StringBuilder buffer = new StringBuilder(1024);

            //列头
            foreach (DataColumn c in table.Columns)
            {
                if (buffer.Length != 0) buffer.Append(",");
                buffer.AppendFormat("\"{0}\"", c.ColumnName.Replace("\"", "\"\""));
            }

            //数据
            foreach (DataRow row in table.Rows)
            {
                buffer.AppendLine();
                for (var i = 0; i < table.Columns.Count; i++)
                {
                    var c = table.Columns[i];
                    if (i != 0) buffer.Append(",");
                    buffer.AppendFormat("\"{0}\"", row[c].ToString("").Replace("\"", "\"\""));
                }
            }

            return buffer.ToString();
        }

    }
}
