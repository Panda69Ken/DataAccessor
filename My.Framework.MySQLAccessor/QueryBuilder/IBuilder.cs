using Dapper;
using System.Linq.Expressions;
using System.Reflection;

namespace My.Framework.MySQLAccessor
{
    public interface IBuilder<T>
    {
        string TableName
        {
            get;
        }
        string Sql
        {
            get;
        }

        DynamicParameters DbParameters
        {
            get;
            set;
        }

        SqlType Type
        {
            get;
            set;
        }

        IBuilder<T> SetTable(string tableName);
        IBuilder<T> SetTable(int tableIndex = 0);

        /// <summary>
        /// 自增属性
        /// </summary>
        PropertyInfo IdentityProperty { get; set; }

        IBuilder<T> BuildWhere(Expression<Func<T, bool>> predicate);

        IBuilder<T> BuildUpdate(Expression<Func<T, T>> pars);

        IBuilder<T> BuildUpdate(T item);

        IBuilder<T> BuildInsert(Expression<Func<T, T>> pars);

        IBuilder<T> BuildInsert(T item);

        IBuilder<T> OrderBy<K>(Expression<Func<T, K>> sort);

        IBuilder<T> OrderByDescending<K>(Expression<Func<T, K>> sort);

        IBuilder<T> Select(Expression<Func<T, object>> selector);

        IBuilder<T> Top(int n);

        IBuilder<T> Distinct<K>(Expression<Func<T, K>> dis);

        IBuilder<T> GetRange(int pageIndex, int pageSize);

        IBuilder<T> Max<K>(Expression<Func<T, K>> field);

        IBuilder<T> Sum<K>(Expression<Func<T, K>> field);
    }
}