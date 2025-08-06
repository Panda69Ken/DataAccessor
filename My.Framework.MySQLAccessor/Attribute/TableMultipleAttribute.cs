using My.Framework.Foundation;

namespace My.Framework.MySQLAccessor
{
    public class TableMultipleAttribute : Attribute
    {

        public string[] TableNames { get; set; }
        public TableMultipleAttribute(string[] tableNameList)
        {
            TableNames = tableNameList;
        }

        public TableMultipleAttribute(Type T)
        {
            var enums = new List<string>();

            foreach (Enum item in Enum.GetValues(T))
            {
                enums.Add(item.Description());
            }

            if (enums.Any())
                TableNames = enums.ToArray();

        }
    }
}
