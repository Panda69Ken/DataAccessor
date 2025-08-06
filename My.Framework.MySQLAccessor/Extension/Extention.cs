using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace My.Framework.MySQLAccessor
{
    public static class Extention
    {
        public static List<T> Filter<T, K>(this IEnumerable<T> source, Func<T, K> keySelector)
        {
            List<T> list = [];
            List<object> list2 = [];
            foreach (T current in source)
            {
                object item = keySelector(current);
                if (list2.IndexOf(item) <= -1)
                {
                    list.Add(current);
                    list2.Add(item);
                }
            }
            return list;
        }

        public static List<T> ToList<T>(this IDataReader reader)
        {
            List<T> list = [];
            PropertyInfo[] properties = typeof(T).GetProperties();
            while (reader.Read())
            {
                T t = Activator.CreateInstance<T>();
                PropertyInfo[] array = properties;
                for (int i = 0; i < array.Length; i++)
                {
                    PropertyInfo propertyInfo = array[i];
                    try
                    {
                        if (reader.GetOrdinal(propertyInfo.Name) >= 0 && reader[propertyInfo.Name] != DBNull.Value && propertyInfo.CanWrite)
                        {
                            propertyInfo.SetValue(t, (propertyInfo.PropertyType.BaseType == typeof(Enum)) ? Enum.ToObject(propertyInfo.PropertyType, reader[propertyInfo.Name]) : Convert.ChangeType(reader[propertyInfo.Name], propertyInfo.PropertyType), null);
                        }
                    }
                    catch
                    {
                    }
                }
                list.Add(t);
            }
            reader.Close();
            reader.Dispose();
            return list;
        }

        public static List<T> ToList<T>(this DataTable table)
        {
            List<T> list = [];
            if (table == null || table.Rows.Count == 0)
            {
                return list;
            }
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (DataRow dataRow in table.Rows)
            {
                T t = Activator.CreateInstance<T>();
                PropertyInfo[] array = properties;
                for (int i = 0; i < array.Length; i++)
                {
                    PropertyInfo propertyInfo = array[i];
                    if (table.Columns.Contains(propertyInfo.Name) && dataRow[propertyInfo.Name] != DBNull.Value && propertyInfo.CanWrite)
                    {
                        propertyInfo.SetValue(t, (propertyInfo.PropertyType.BaseType == typeof(Enum)) ? Enum.ToObject(propertyInfo.PropertyType, dataRow[propertyInfo.Name]) : Convert.ChangeType(dataRow[propertyInfo.Name], propertyInfo.PropertyType), null);
                    }
                }
                list.Add(t);
            }
            return list;
        }

        public static T ToEntity<T>(this DataRow dr)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            T t = Activator.CreateInstance<T>();
            PropertyInfo[] array = properties;
            for (int i = 0; i < array.Length; i++)
            {
                PropertyInfo propertyInfo = array[i];
                if (dr.Table.Columns.Contains(propertyInfo.Name) && dr[propertyInfo.Name] != DBNull.Value && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(t, (propertyInfo.PropertyType.BaseType == typeof(Enum)) ? Enum.ToObject(propertyInfo.PropertyType, dr[propertyInfo.Name]) : Convert.ChangeType(dr[propertyInfo.Name], propertyInfo.PropertyType), null);
                }
            }
            return t;
        }

        public static object Compile(this Expression exp)
        {
            if (exp is ConstantExpression constantExpression)
            {
                return constantExpression.Value;
            }

            switch (exp.Type.Name)
            {
                case "Int32":
                    return Expression.Lambda<Func<int>>(exp).Compile()();
                case "Int16":
                    return Expression.Lambda<Func<short>>(exp).Compile()();
                case "Int64":
                    return Expression.Lambda<Func<long>>(exp).Compile()();
                case "Byte":
                    return Expression.Lambda<Func<byte>>(exp).Compile()();
                case "Double":
                    return Expression.Lambda<Func<double>>(exp).Compile()();
                case "Single":
                    return Expression.Lambda<Func<float>>(exp).Compile()();
                case "Decimal":
                    return Expression.Lambda<Func<decimal>>(exp).Compile()();
                case "Boolean":
                    return Expression.Lambda<Func<bool>>(exp).Compile()();
                case "Guid":
                    return Expression.Lambda<Func<Guid>>(exp).Compile()();
                case "DateTime":
                    return Expression.Lambda<Func<DateTime>>(exp).Compile()();
                case "List`1":
                    if (exp.Type.FullName == "System.Collections.Generic.List`1[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]" ||
                        exp.Type.FullName == "System.Collections.Generic.List`1[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]")
                    {
                        return Expression.Lambda<Func<List<int>>>(exp).Compile()();
                    }
                    return Expression.Lambda<Func<List<string>>>(exp).Compile()();
                case "Nullable`1":
                    var type = Nullable.GetUnderlyingType(exp.Type);
                    return NullableCompile(exp, type);
            }
            return Expression.Lambda<Func<string>>(exp).Compile()();
        }


        public static object NullableCompile(Expression exp, Type type)
        {
            return type.Name switch
            {
                "Int32" => Expression.Lambda<Func<int?>>(exp).Compile()(),
                "Int16" => Expression.Lambda<Func<short?>>(exp).Compile()(),
                "Int64" => Expression.Lambda<Func<long?>>(exp).Compile()(),
                "Byte" => Expression.Lambda<Func<byte?>>(exp).Compile()(),
                "Double" => Expression.Lambda<Func<double?>>(exp).Compile()(),
                "Single" => Expression.Lambda<Func<float?>>(exp).Compile()(),
                "Decimal" => Expression.Lambda<Func<decimal?>>(exp).Compile()(),
                "Boolean" => Expression.Lambda<Func<bool?>>(exp).Compile()(),
                "Guid" => Expression.Lambda<Func<Guid?>>(exp).Compile()(),
                "DateTime" => Expression.Lambda<Func<DateTime?>>(exp).Compile()(),
                _ => Expression.Lambda<Func<string>>(exp).Compile()(),
            };
        }
    }
}
