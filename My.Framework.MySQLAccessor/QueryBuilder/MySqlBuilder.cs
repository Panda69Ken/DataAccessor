namespace My.Framework.MySQLAccessor
{
    public class MySqlBuilder<T> : SqlBuilder<T>, IBuilder<T>
    {
        //public new string Sql => base.SqlField;

        public new string TableName => base.TableName;

        public new string Sql
        {
            get
            {
                SqlType type = base.Type;
                if (type == SqlType.Select)
                {
                    return $"SELECT {SqlField} FROM `{TableName}` {SqlWhere} {SqlOrderBy} {SqlTop}";
                }
                return base.Sql;
            }
        }

        public IBuilder<T> GetRange(int pageIndex, int pageSize)
        {
            SqlTop = $"LIMIT {(pageIndex - 1) * pageSize},{pageSize}";
            return this;
        }


        public new IBuilder<T> Top(int n)
        {
            SqlTop = $"Limit 0,{n}";
            return this;
        }

    }
}