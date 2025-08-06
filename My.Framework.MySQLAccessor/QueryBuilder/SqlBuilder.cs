using Dapper;
using My.Framework.Foundation;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace My.Framework.MySQLAccessor
{
    public class SqlBuilder<T>
    {
        protected string SqlField { get; set; } = "*";
        protected string SqlWhere { get; set; }
        protected string SqlOrderBy { get; set; }
        protected string SqlUpdate { get; set; }
        protected string SqlInsert { get; set; }
        protected string SqlTop { get; set; }
        protected string SqlPage { get; set; }

        protected int I { get; set; }
        protected string TableName { get; set; }
        protected string[] TableArr { get; }

        protected string ParameterFormat { get; set; }

        public PropertyInfo IdentityProperty { get; set; }

        public SqlBuilder()
        {
            //DbParameters = new DynamicParameters();
            //System.Type type = typeof(T);
            //ParameterFormat = "@";
            //object[] customAttributes = type.GetCustomAttributes(typeof(TableAttribute), false);
            //TableName = ((IEnumerable<object>)customAttributes).Any() ? ((dynamic)customAttributes[0]).Name : type.Name;

            DbParameters = new DynamicParameters();
            System.Type type = typeof(T);
            ParameterFormat = "@";
            object[] customAttributes = type.GetCustomAttributes(typeof(TableAttribute), false);

            if (((IEnumerable<object>)customAttributes).Any())
            {
                TableName = ((dynamic)customAttributes[0]).Name;
            }
            else
            {
                var tableAttributes = type.GetCustomAttribute<TableMultipleAttribute>();

                if (tableAttributes != null)
                {
                    TableArr = tableAttributes.TableNames;
                    TableName = TableArr[0];
                }
            }
        }

        public IBuilder<T> SetTable(string tableName)
        {
            if (tableName.IsEmpty())
            {
                throw new Exception("tableName不能为空");
            }

            TableName = tableName;
            return (IBuilder<T>)this;
        }

        public IBuilder<T> SetTable(int tableIndex = 0)
        {
            if (TableArr == null || tableIndex + 1 > TableArr.Length)
            {
                throw new Exception("tableIndex设置错误");
            }

            TableName = TableArr[tableIndex];
            return (IBuilder<T>)this;
        }

        protected string Sql
        {
            get
            {
                string str = string.Empty;
                switch (Type)
                {
                    case SqlType.Insert:
                        str = $"INSERT INTO `{TableName}` ({SqlInsert}) VALUES ({ParameterFormat}{SqlInsert.Replace("`", "").Replace(",", "," + ParameterFormat)})";
                        break;
                    case SqlType.Delete:
                        str = $"DELETE FROM `{TableName}` {SqlWhere}";
                        break;
                    case SqlType.Select:
                        string newValue = $"SELECT {SqlTop}{SqlField} FROM `{TableName}`{SqlWhere}";
                        str = string.IsNullOrEmpty(SqlPage) ? newValue + " " + SqlOrderBy : SqlPage.Replace("$_SELECT", newValue.Replace("SELECT ", "")).Replace("$SELECT", newValue).Replace("$_OrderBy", SqlOrderBy.Contains("ASC") ? SqlOrderBy.Replace("ASC", "DESC") : SqlOrderBy.Replace("DESC", "ASC")).Replace("$OrderBy", SqlOrderBy);
                        break;
                    case SqlType.Update:
                        str = $"UPDATE `{TableName}`  SET {SqlUpdate} {SqlWhere}";
                        break;
                    case SqlType.Count:
                        str = $"SELECT COUNT(*) FROM `{TableName}` {SqlWhere}";
                        break;
                }
                return str;
            }
        }

        //public List<DbParameter> DbParameters { get; set; }
        public DynamicParameters DbParameters { get; set; }

        public SqlType Type { get; set; }

        public IBuilder<T> BuildWhere(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                return (IBuilder<T>)this;
            SqlWhere = !string.IsNullOrEmpty(SqlWhere) ? SqlWhere + " AND " + BuildWhere(predicate.Body) : string.Format(" WHERE {0}", BuildWhere(predicate.Body));
            if (SqlWhere == " WHERE ")
                SqlWhere = "";
            return (IBuilder<T>)this;
        }

        public IBuilder<T> BuildUpdate(Expression<Func<T, T>> pars)
        {
            string name = pars.Parameters[0].Name;
            foreach (MemberBinding binding in ((MemberInitExpression)pars.Body).Bindings)
            {
                Expression expression = ((MemberAssignment)binding).Expression;
                SqlUpdate += string.Format("`{0}`={1},", binding.Member.Name.ToColName(), GetRight(expression, name));
            }
            SqlUpdate = SqlUpdate.Trim(',');
            return (IBuilder<T>)this;
        }

        public IBuilder<T> BuildUpdate(T item)
        {
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (property.CanWrite && property.DeclaringType == typeof(T) && (property.GetCustomAttributes(typeof(KeyAttribute), false).Length <= 0 && property.GetCustomAttributes(typeof(ExtentionAttribute), false).Length <= 0))
                {
                    var colName = property.Name.ToColName();
                    SqlUpdate += $"`{colName}`={ParameterFormat}{colName},";
                    DbParameters.Add(colName, Convert.ChangeType(property.GetValue(item), property.PropertyType));
                }
            }
            SqlUpdate = SqlUpdate.Trim(',');
            return (IBuilder<T>)this;
        }

        public IBuilder<T> BuildInsert(Expression<Func<T, T>> pars)
        {
            foreach (MemberBinding binding in ((MemberInitExpression)pars.Body).Bindings)
            {
                var colName = binding.Member.Name.ToColName();
                SqlInsert += $"`{colName}`,";
                object obj = ((MemberAssignment)binding).Expression.Compile();
                DbParameters.Add(colName, obj);
            }
            SqlInsert = SqlInsert.Trim(',');
            return (IBuilder<T>)this;
        }

        public IBuilder<T> BuildInsert(T item)
        {
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                var hasKey = property.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0;

                if (IdentityProperty == null)
                {
                    if (hasKey && property.CanWrite)
                    {
                        IdentityProperty = property;
                    }
                }
                if (property.CanWrite && property.DeclaringType == typeof(T) && !hasKey && property.GetCustomAttributes(typeof(ExtentionAttribute), false).Length <= 0)
                {
                    var colName = property.Name.ToColName();
                    DbParameters.Add(colName, Convert.ChangeType(property.GetValue(item), property.PropertyType));
                    SqlInsert += $"`{colName}`,";
                }
            }
            SqlInsert = SqlInsert.Trim(',');
            return (IBuilder<T>)this;
        }

        public IBuilder<T> OrderBy<K>(Expression<Func<T, K>> sort)
        {
            SqlOrderBy = !string.IsNullOrEmpty(SqlOrderBy) ? SqlOrderBy + string.Format(", `{0}` ASC", ((MemberExpression)sort.Body).Member.Name.ToColName()) : string.Format("ORDER BY `{0}` ASC", ((MemberExpression)sort.Body).Member.Name.ToColName());
            return (IBuilder<T>)this;
        }

        public IBuilder<T> OrderByDescending<K>(Expression<Func<T, K>> sort)
        {
            SqlOrderBy = !string.IsNullOrEmpty(SqlOrderBy) ? SqlOrderBy + string.Format(" , `{0}` DESC", ((MemberExpression)sort.Body).Member.Name.ToColName()) : string.Format("ORDER BY `{0}` DESC", ((MemberExpression)sort.Body).Member.Name.ToColName());
            return (IBuilder<T>)this;
        }

        public IBuilder<T> Select(Expression<Func<T, object>> selector)
        {
            ReadOnlyCollection<MemberInfo> members = ((NewExpression)selector.Body).Members;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (MemberInfo memberInfo in members)
            {
                stringBuilder.AppendFormat("`{0}`{1}", memberInfo.Name.ToColName(), memberInfo.Equals(members.Last<MemberInfo>()) ? "" : ",");
            }
            SqlField = stringBuilder.ToString();
            return (IBuilder<T>)this;
        }

        public IBuilder<T> Top(int n)
        {
            SqlTop = string.Format("LIMIT {0} ", n);
            return (IBuilder<T>)this;
        }

        public IBuilder<T> Distinct<K>(Expression<Func<T, K>> dis)
        {
            SqlField = string.Format("DISTINCT(`{0}`)", ((MemberExpression)dis.Body).Member.Name.ToColName());
            return (IBuilder<T>)this;
        }

        public IBuilder<T> Max<K>(Expression<Func<T, K>> field)
        {
            SqlField = string.Format("MAX(`{0}`)", ((MemberExpression)field.Body).Member.Name.ToColName());
            return (IBuilder<T>)this;
        }

        public IBuilder<T> Sum<K>(Expression<Func<T, K>> field)
        {
            SqlField = string.Format("SUM(`{0}`)", ((MemberExpression)field.Body).Member.Name.ToColName());
            return (IBuilder<T>)this;
        }

        private string BuildIn<V>(List<V> list, string field)
        {
            string empty = string.Empty;
            if (list.Count == 0)
                return empty;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("`{0}` IN (", field);
            for (int index = 0; index < list.Count; ++index)
            {
                stringBuilder.AppendFormat("{0}{1},", ParameterFormat, (field + I));
                DbParameters.Add(field + I, list[index]);
                ++I;
            }
            return stringBuilder.Append(")").ToString().Replace(",)", ")");
        }

        private string BuildWhere(Expression exp)
        {
            string str1 = string.Empty;
            string str2 = string.Empty;
            BinaryExpression binaryExpression = null;
            switch (exp.NodeType)
            {
                case ExpressionType.AndAlso:
                    binaryExpression = (BinaryExpression)exp;
                    str2 = BuildWhere(binaryExpression.Left) + " AND " + BuildWhere(binaryExpression.Right);
                    break;
                case ExpressionType.Call:
                    MethodCallExpression methodCallExpression = (MethodCallExpression)exp;
                    string methodName = methodCallExpression.Method.Name;
                    if (methodName != null)
                    {
                        MemberExpression exp1 = (MemberExpression)methodCallExpression.Object;
                        var typeName = exp1.Type.Name;
                        if (typeName == "List`1" && methodName == "Contains")
                        {
                            string name3 = ((MemberExpression)methodCallExpression.Arguments[0]).Member.Name.ToColName();
                            str2 = exp1.Type.FullName != "System.Collections.Generic.List`1[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]" &&
                                   exp1.Type.FullName != "System.Collections.Generic.List`1[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]"
                                ? BuildIn<string>((List<string>)exp1.Compile(), name3) : BuildIn<int>((List<int>)exp1.Compile(), name3);
                            break;
                        }

                        switch (typeName)
                        {
                            case "String":
                                if (((IEnumerable<string>)new string[3]
                                {
                                    "Contains",
                                    "StartsWith",
                                    "EndsWith"
                                }).Contains<string>(methodName))
                                {
                                    var colName = exp1.Member.Name.ToColName();
                                    var paraName = colName + I;
                                    var like = "";
                                    switch (methodName)
                                    {
                                        case "Contains":
                                            like = $"%{methodCallExpression.Arguments[0].Compile()}%";
                                            break;
                                        case "StartsWith":
                                            like = $"{methodCallExpression.Arguments[0].Compile()}%";
                                            break;
                                        case "EndsWith":
                                            like = $"%{methodCallExpression.Arguments[0].Compile()}";
                                            break;
                                    }

                                    DbParameters.Add(paraName, like);
                                    str2 = $"`{colName}` LIKE @{paraName}";
                                    ++I;
                                    break;
                                }

                                if (methodName == "Equals")
                                {
                                    var colName = exp1.Member.Name.ToColName();
                                    var paraName = colName + I;
                                    DbParameters.Add(paraName, methodCallExpression.Arguments[0].Compile());
                                    ++I;
                                    return $"`{colName}` = @{paraName}";
                                }

                                break;
                            case "Int16":
                            case "Int32":
                            case "Int64":
                                if (methodName == "Equals")
                                {
                                    var colName = exp1.Member.Name.ToColName();
                                    var paraName = colName + I;
                                    DbParameters.Add(paraName, methodCallExpression.Arguments[0].Compile());
                                    ++I;
                                    return $"`{colName}` = @{paraName}";
                                }
                                break;
                        }
                    }
                    break;
                case ExpressionType.Equal:
                    binaryExpression = (BinaryExpression)exp;
                    str1 = "=";
                    break;
                case ExpressionType.GreaterThan:
                    binaryExpression = (BinaryExpression)exp;
                    str1 = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    binaryExpression = (BinaryExpression)exp;
                    str1 = ">=";
                    break;
                case ExpressionType.LessThan:
                    binaryExpression = (BinaryExpression)exp;
                    str1 = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    binaryExpression = (BinaryExpression)exp;
                    str1 = "<=";
                    break;
                case ExpressionType.NotEqual:
                    binaryExpression = (BinaryExpression)exp;
                    str1 = "<>";
                    break;
                case ExpressionType.OrElse:
                    binaryExpression = (BinaryExpression)exp;
                    str2 = "(" + BuildWhere(binaryExpression.Left) + " OR " + BuildWhere(binaryExpression.Right) + ")";
                    break;
            }
            if (!string.IsNullOrEmpty(str1) && binaryExpression.Left is MemberExpression)
            {
                object obj = binaryExpression.Right.Compile();
                if (obj == null)
                {
                    str1 = str1 == "=" ? " is " : " is not ";
                }
                string name = ((MemberExpression)binaryExpression.Left).Member.Name.ToColName();
                DbParameters.Add(name + I, obj);
                str2 = $"`{name}` {str1} {ParameterFormat}{name + I}";
                ++I;
                return str2;
            }

            if (!string.IsNullOrEmpty(str1) && binaryExpression.Left is UnaryExpression unaryExpression)
            {
                object obj = binaryExpression.Right.Compile();
                if (obj == null)
                {
                    str1 = str1 == "=" ? " is " : " is not ";
                }
                string name = ((MemberExpression)unaryExpression.Operand).Member.Name.ToColName();
                DbParameters.Add(name + I, obj);
                str2 = $"`{name}` {str1} {ParameterFormat}{name + I}";
                ++I;
            }
            return str2;

        }

        private string GetRight(Expression exp, string p)
        {
            var rightStr = "";

            if (exp is UnaryExpression unaryExpression)
            {
                string name = "Field" + I;
                ++I;
                DbParameters.Add(name, unaryExpression.Operand.Compile());
                rightStr = ParameterFormat + name;
            }

            if (exp is BinaryExpression binaryExpression)
            {
                rightStr = GetRight(binaryExpression.Left, p) + SqlBuilder<T>.GetOperator(binaryExpression.NodeType) + GetRight(binaryExpression.Right, p);
            }
            if (exp is MemberExpression memberExpression)
            {
                if (memberExpression.Expression == null || memberExpression.Expression.NodeType == ExpressionType.Constant || memberExpression.Expression.NodeType == ExpressionType.MemberAccess && ((MemberExpression)memberExpression.Expression).Member.Name != p)
                {
                    string name = "Field" + I;
                    ++I;
                    DbParameters.Add(name, memberExpression.Compile());
                    rightStr = ParameterFormat + name;
                }
                else
                {
                    rightStr = $"`{memberExpression.Member.Name.ToColName()}`";
                }
            }
            if (exp is ConstantExpression constantExpression)
            {
                string name = "Field" + I;
                ++I;
                DbParameters.Add(name, constantExpression.Value);
                rightStr = ParameterFormat + name;
            }

            return rightStr;
        }

        private static string GetOperator(ExpressionType type)
        {
            return type switch
            {
                ExpressionType.Add => " + ",
                ExpressionType.Divide => " / ",
                ExpressionType.Multiply => " * ",
                ExpressionType.Subtract => " - ",
                _ => string.Empty,
            };
        }
    }
}