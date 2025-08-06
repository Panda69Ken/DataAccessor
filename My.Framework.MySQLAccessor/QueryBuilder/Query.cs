using Dapper;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace My.Framework.MySQLAccessor
{
    public class Query<T>
    {

        private IMySqlContext Context { get; }
        private static readonly ConcurrentDictionary<Type, Dictionary<string, bool>> BulkColumnMapperDic
            = new ConcurrentDictionary<Type, Dictionary<string, bool>>();

        internal Query(IMySqlContext context)
        {
            SqlBuilder = new MySqlBuilder<T> { Type = SqlType.Select };

            Context = context;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected IBuilder<T> SqlBuilder { get; set; }


        public Query<T> SetTable(string tableName)
        {
            SqlBuilder = SqlBuilder.SetTable(tableName);
            return this;
        }

        public Query<T> SetTable(int tableIndex)
        {
            SqlBuilder = SqlBuilder.SetTable(tableIndex);
            return this;
        }

        public string GetTableName()
        {
            return SqlBuilder.TableName;
        }

        #region 执行操作的方法

        /// <summary>
        /// 根据查询条件删除实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>删除的记录数</returns>
        public int Delete(Expression<Func<T, bool>> predicate)
        {
            SqlBuilder.Type = SqlType.Delete;
            SqlBuilder = SqlBuilder.BuildWhere(predicate);
            return Execute();
        }

        /// <summary>
        /// 根据查询条件删除实体[异步执行]
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>删除的记录数</returns>
        public async Task<int> DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            SqlBuilder.Type = SqlType.Delete;
            SqlBuilder = SqlBuilder.BuildWhere(predicate);
            return await ExecuteAsync();
        }

        /// <summary>
        /// 根据查询条件更新实体指定属性
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="pars">要更新的实体属性</param>
        /// <returns>受影响的记录数</returns>
        public int Update(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> pars)
        {
            SqlBuilder.Type = SqlType.Update;
            SqlBuilder = SqlBuilder.BuildWhere(predicate);
            SqlBuilder = SqlBuilder.BuildUpdate(pars);
            return Execute();
        }

        /// <summary>
        /// 根据查询条件更新实体指定属性[异步执行]
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="pars">要更新的实体属性</param>
        /// <returns>受影响的记录数</returns>
        public async Task<int> UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> pars)
        {
            SqlBuilder.Type = SqlType.Update;
            SqlBuilder = SqlBuilder.BuildWhere(predicate);
            SqlBuilder = SqlBuilder.BuildUpdate(pars);
            return await ExecuteAsync();
        }

        /// <summary>
        /// 更新条件更新实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="item">要更新的实体</param>
        /// <returns>受影响的记录数</returns>
        public int Update(Expression<Func<T, bool>> predicate, T item)
        {
            SqlBuilder.Type = SqlType.Update;
            SqlBuilder = SqlBuilder.BuildWhere(predicate);
            SqlBuilder = SqlBuilder.BuildUpdate(item);
            return Execute();
        }

        /// <summary>
        /// 更新条件更新实体[异步执行]
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="item">要更新的实体</param>
        /// <returns>受影响的记录数</returns>
        public async Task<int> UpdateAsync(Expression<Func<T, bool>> predicate, T item)
        {
            SqlBuilder.Type = SqlType.Update;
            SqlBuilder = SqlBuilder.BuildWhere(predicate);
            SqlBuilder = SqlBuilder.BuildUpdate(item);
            return await ExecuteAsync();
        }

        /// <summary>
        /// 执行一个插入操作，插入类型为<typeparamref name="T"/>的实体
        /// </summary>
        /// <param name="item">需要插入的实体</param>
        /// <returns>是否插入成功 True or False</returns>
        public bool Insert(T item)
        {
            SqlBuilder.Type = SqlType.Insert;
            SqlBuilder = SqlBuilder.BuildInsert(item);
            if (SqlBuilder.IdentityProperty != null)
            {
                using (IDbConnection conn = Context.CreateConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    var row = conn.Execute(SqlBuilder.Sql, SqlBuilder.DbParameters);
                    if (row == 1)
                    {
                        var r = conn.Query("Select LAST_INSERT_ID() id");

                        var id = r.First().id;
                        if (id == null) return false;
                        var idp = SqlBuilder.IdentityProperty;
                        idp.SetValue(item, Convert.ChangeType(id, idp.PropertyType), null);

                        return true;
                    }

                    return false;
                }
            }
            return Execute() > 0;
        }

        private Dictionary<string, bool> GetColumnMapperList()
        {
            return BulkColumnMapperDic.GetOrAdd(typeof(T), type =>
            {
                var columnList = new Dictionary<string, bool>();
                foreach (var property in typeof(T).GetProperties())
                {
                    var hasKey = property.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0;
                    if (hasKey)
                        columnList.Add(property.Name, true);

                    if (property.CanWrite && property.DeclaringType == typeof(T) && !hasKey &&
                        property.GetCustomAttributes(typeof(ExtentionAttribute), false).Length <= 0)
                    {
                        columnList.Add(property.Name, false);
                    }
                }
                return columnList;
            });
        }

        public async Task<int> BulkUpdateAsync(List<T> items)
        {
            if (items == null || items.Any() == false) return 0;

            var columnStr = new StringBuilder();
            var sql = "UPDATE {0} T1 JOIN ( {1} ) T2 ON {2} SET {3} ";
            var columnMapperList = GetColumnMapperList();

            foreach (var item in items)
            {
                columnStr.Append("SELECT ");
                foreach (var column in columnMapperList)
                {
                    var property = typeof(T).GetProperty(column.Key);
                    var pType = property.PropertyType;
                    var value = property.GetValue(item);

                    var nullable = value == null;
                    if (nullable) columnStr.Append(" null");
                    else
                    {
                        if (pType.IsEnum)
                        {
                            value = (int)Enum.Parse(pType, value.ToString());
                            columnStr.Append($" {value} ");
                        }
                        else
                        {
                            switch (pType.FullName)
                            {
                                case "System.Boolean":
                                    columnStr.Append($" {((bool)value ? 1 : 0)} ");
                                    break;
                                default:
                                    columnStr.Append($" '{value}' ");
                                    break;
                            }
                        }
                    }

                    if (items.IndexOf(item) == 0)
                        columnStr.Append($" AS {column.Key.ToColName()}");

                    if (column.Key != columnMapperList.Keys.Last())
                        columnStr.Append(",");
                }

                if (items.IndexOf(item) != items.Count - 1)
                    columnStr.Append(" UNION ALL ");
            }

            var key = columnMapperList.First(m => m.Value).Key.ToColName();

            sql = string.Format(sql, SqlBuilder.TableName, columnStr, $"T1.{key} = T2.{key}",
                string.Join(',',
                    columnMapperList.Where(m => !m.Value)
                        .Select(m => $"T1.{m.Key.ToColName()}=T2.{m.Key.ToColName()}")));

            using (var conn = Context.CreateConnection())
            {
                return await conn.ExecuteAsync(sql, new { });
            }

        }

        public async Task<int> BulkInsertAsync(List<T> items)
        {
            if (items == null || items.Any() == false) return 0;

            var columnStr = new StringBuilder();
            var sql = "INSERT INTO {0} ( {1} ) VALUES {2} ";
            var columnMapperList = GetColumnMapperList();

            foreach (var item in items)
            {
                columnStr.Append("( ");

                foreach (var column in columnMapperList.Where(m => !m.Value))
                {
                    var property = typeof(T).GetProperty(column.Key);
                    var pType = property.PropertyType;
                    var value = property.GetValue(item);

                    var nullable = value == null;
                    if (nullable) columnStr.Append(" null");
                    else
                    {
                        if (pType.IsEnum)
                        {
                            value = (int)Enum.Parse(pType, value.ToString());
                            columnStr.Append($" {value} ");
                        }
                        else
                        {
                            switch (pType.FullName)
                            {
                                case "System.Boolean":
                                    columnStr.Append($" {((bool)value ? 1 : 0)} ");
                                    break;
                                default:
                                    columnStr.Append($" '{value}' ");
                                    break;
                            }
                        }
                    }

                    if (column.Key != columnMapperList.Keys.Last())
                        columnStr.Append(",");
                }

                columnStr.Append(" )");
                if (items.IndexOf(item) != items.Count - 1)
                    columnStr.Append(",");
            }

            sql = string.Format(sql, SqlBuilder.TableName,
                string.Join(',', columnMapperList.Where(m => !m.Value).Select(m => $" `{m.Key.ToColName()}`")),
                columnStr);

            using (var conn = Context.CreateConnection())
            {
                return await conn.ExecuteAsync(sql, new { });
            }
        }


        /// <summary>
        /// 执行一个插入操作，插入类型为<typeparamref name="T"/>的实体 [异步操作]
        /// </summary>
        /// <param name="item">需要插入的实体</param>
        /// <returns>是否插入成功 True or False</returns>
        public async Task<bool> InsertAsync(T item)
        {
            SqlBuilder.Type = SqlType.Insert;
            SqlBuilder = SqlBuilder.BuildInsert(item);
            if (SqlBuilder.IdentityProperty != null)
            {
                using (IDbConnection conn = Context.CreateConnection())
                {

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    var row = await conn.ExecuteAsync(SqlBuilder.Sql, SqlBuilder.DbParameters);
                    if (row == 1)
                    {
                        var r = await conn.QueryAsync("Select LAST_INSERT_ID() id");
                        var id = r.First().id;
                        if (id == null) return false;
                        var idp = SqlBuilder.IdentityProperty;
                        idp.SetValue(item, Convert.ChangeType(id, idp.PropertyType), null);

                        return true;
                    }

                    return false;
                }
            }
            return await ExecuteAsync() > 0;
        }

        /// <summary>
        /// 按指定属性执行一个插入操作
        /// </summary>
        /// <param name="pars">需要插入的属性</param>
        /// <returns>是否插入成功 True or False</returns>
        public bool Insert(Expression<Func<T, T>> pars)
        {
            SqlBuilder.Type = SqlType.Insert;
            SqlBuilder = SqlBuilder.BuildInsert(pars);
            return Execute() > 0;
        }

        /// <summary>
        /// 按指定属性执行一个插入操作[异步操作]
        /// </summary>
        /// <param name="pars">需要插入的属性</param>
        /// <returns>是否插入成功 True or False</returns>
        public async Task<bool> InsertAsync(Expression<Func<T, T>> pars)
        {
            SqlBuilder.Type = SqlType.Insert;
            SqlBuilder = SqlBuilder.BuildInsert(pars);
            return await ExecuteAsync() > 0;
        }

        /// <summary>
        /// 执行一个查询返回类型为<typeparamref name="TK"/>的最大
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public TK Max<TK>(Expression<Func<T, TK>> field)
        {
            SqlBuilder.Type = SqlType.Select;
            SqlBuilder = SqlBuilder.Max<TK>(field);
            return ExcuteScalar<TK>();
        }

        /// <summary>
        /// 执行一个查询返回类型为<typeparamref name="TK"/>的最大值[异步操作]
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task<TK> MaxAsync<TK>(Expression<Func<T, TK>> field)
        {
            SqlBuilder.Type = SqlType.Select;
            SqlBuilder = SqlBuilder.Max<TK>(field);
            return await ExcuteScalarAsync<TK>();
        }

        /// <summary>
        /// 执行一个查询返回类型为<typeparamref name="TK"/>的和
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public TK Sum<TK>(Expression<Func<T, TK>> field)
        {
            SqlBuilder.Type = SqlType.Select;
            SqlBuilder = SqlBuilder.Sum<TK>(field);
            return ExcuteScalar<TK>();
        }

        /// <summary>
        /// 执行一个查询返回类型为<typeparamref name="TK"/>的和[异步操作]
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task<TK> SumAsync<TK>(Expression<Func<T, TK>> field)
        {
            SqlBuilder.Type = SqlType.Select;
            SqlBuilder = SqlBuilder.Sum<TK>(field);
            return await ExcuteScalarAsync<TK>();
        }

        /// <summary>
        /// 执行一个查询返回记录数
        /// </summary>
        /// <returns></returns>
        public long Count()
        {
            SqlBuilder.Type = SqlType.Count;
            return ExcuteScalar<long>();
        }

        /// <summary>
        /// 执行一个查询返回记录数[异步方法]
        /// </summary>
        /// <returns></returns>
        public async Task<long> CountAsync()
        {
            SqlBuilder.Type = SqlType.Count;
            return await ExcuteScalarAsync<long>();
        }

        /// <summary>
        /// 执行一个查询返回类型为 <typeparamref name="T"/>的集合
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> ToList()
        {
            return Context.Query<T>(SqlBuilder.Sql, SqlBuilder.DbParameters);
        }

        /// <summary>
        /// 执行一个查询返回类型为 <typeparamref name="T"/>的集合[异步操作]
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> ToListAsync()
        {
            return await Context.QueryAsync<T>(SqlBuilder.Sql, SqlBuilder.DbParameters);

        }

        /// <summary>
        /// 执行SQL返回一个为<typeparamref name="T"/>的实体
        /// </summary>
        /// <returns></returns>
        public T ToEntity()
        {
            return Context.QuerySingle<T>(SqlBuilder.Sql, SqlBuilder.DbParameters);
        }

        /// <summary>
        /// 执行SQL返回一个为<typeparamref name="T"/>的实体[异步操作]
        /// </summary>
        /// <returns></returns>
        public async Task<T> ToEntityAsync()
        {
            return await Context.QuerySingleAsync<T>(SqlBuilder.Sql, SqlBuilder.DbParameters);
        }

        #endregion
        #region SQL拼接操作，仅拼接SQL不执行

        /// <summary>
        ///查询条件
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>

        public Query<T> Where(Expression<Func<T, bool>> predicate)
        {
            SqlBuilder = SqlBuilder.BuildWhere(predicate);
            return this;
        }

        /// <summary>
        /// 根据查询条件TOP 1
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Query<T> Find(Expression<Func<T, bool>> predicate)
        {
            SqlBuilder = SqlBuilder.BuildWhere(predicate);
            SqlBuilder = SqlBuilder.Top(1);
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="sort"></param>
        /// <returns></returns>
        public Query<T> OrderBy<K>(Expression<Func<T, K>> sort)
        {
            SqlBuilder = SqlBuilder.OrderBy<K>(sort);
            return this;
        }


        /// <summary>
        /// 逆向排序
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="sort"></param>
        /// <returns></returns>
        public Query<T> OrderByDescending<K>(Expression<Func<T, K>> sort)
        {
            SqlBuilder = SqlBuilder.OrderByDescending<K>(sort);
            return this;
        }

        /// <summary>
        /// 选择性查询实体属性
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public Query<T> Select(Expression<Func<T, object>> selector)
        {
            SqlBuilder = SqlBuilder.Select(selector);
            return this;
        }


        /// <summary>
        /// 查询top n
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public Query<T> Top(int n)
        {
            SqlBuilder.Type = SqlType.Select;
            SqlBuilder = SqlBuilder.Top(n);
            return this;
        }

        /// <summary>
        /// Select distinct
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="dis"></param>
        /// <returns></returns>
        public Query<T> Distinct<K>(Expression<Func<T, K>> dis)
        {
            SqlBuilder.Type = SqlType.Select;
            SqlBuilder = SqlBuilder.Distinct<K>(dis);
            return this;
        }


        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex">页索引，从1开始</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>
        public Query<T> GetRange(int pageIndex, int pageSize)
        {
            SqlBuilder.Type = SqlType.Select;
            SqlBuilder = SqlBuilder.GetRange(pageIndex, pageSize);
            return this;
        }


        #endregion

        #region 私有方法
        /// <summary>
        /// 执行SQL语句执行SQL语句返回受影响的行数
        /// </summary>
        /// <returns>受影响的行数</returns>
        private int Execute()
        {
            return Context.Execute(SqlBuilder.Sql, SqlBuilder.DbParameters);
        }

        /// <summary>
        /// 异步执行SQL语句返回受影响的行数
        /// </summary>
        /// <returns>受影响的行数</returns>
        private async Task<int> ExecuteAsync()
        {
            return await Context.ExecuteAsync(SqlBuilder.Sql, SqlBuilder.DbParameters);
        }

        /// <summary>
        /// 执行参数化SQL，返回第一个值
        /// </summary>
        /// <typeparam name="TK">返回值类型</typeparam>
        /// <returns>第一个值</returns>
        private TK ExcuteScalar<TK>()
        {
            return Context.ExecuteScalar<TK>(SqlBuilder.Sql, SqlBuilder.DbParameters);
        }

        /// <summary>
        /// 异步执行参数化SQL，返回第一个值
        /// </summary>
        /// <typeparam name="TK">返回值类型</typeparam>
        /// <returns>第一个值</returns>
        private async Task<TK> ExcuteScalarAsync<TK>()
        {
            return await Context.ExecuteScalarAsync<TK>(SqlBuilder.Sql, SqlBuilder.DbParameters);

        }

        #endregion

    }
}