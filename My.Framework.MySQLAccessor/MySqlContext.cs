using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Data;
using System.Transactions;

namespace My.Framework.MySQLAccessor
{
    public class MySqlContext : IMySqlContext
    {

        private readonly ILogger<MySqlContext> _log;

        //private readonly ConnectionsManager _connectionsManager;

        private readonly string _connectionString;

        public MySqlContext(string connectionString)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            _connectionString = connectionString;
        }

        public MySqlContext(string connectionString, ILogger<MySqlContext> log) : this(connectionString)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }


        /// <summary>
        /// 创建MySql连接
        /// </summary>
        /// <returns></returns>
        public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);

        /// <summary>
        /// 执行参数话SQL
        /// </summary>
        /// <param name="sql">所要执行的SQL</param>
        /// <param name="param">查询所需参数</param>
        /// <returns>受影响的行数</returns>
        public int Execute(string sql, object param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "参数不能为空！");
            }

            using (var conn = CreateConnection())
            {
                return conn.Execute(sql, param);
            }

        }

        /// <summary>
        /// 执行参数话SQL
        /// </summary>
        /// <param name="sql">所要执行的SQL</param>
        /// <param name="param">查询所需参数</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> ExecuteAsync(string sql, object param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "参数不能为空！");
            }

            using (var conn = CreateConnection())
            {
                return await conn.ExecuteAsync(sql, param);
            }

        }

        /// <summary>
        /// 执行参数化SQL，选择单个值 
        /// </summary>
        /// <typeparam name="TK">返回的类型</typeparam>
        /// <param name="sql">执行的SQL</param>
        /// <param name="param">用于此命令的参数</param>
        /// <returns>返回第一个单元格，为 <typeparamref name="TK"/>.</returns>
        public TK ExecuteScalar<TK>(string sql, object param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "参数不能为空！");
            }
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<TK>(sql, param);
            }

        }

        /// <summary>
        /// 执行参数化SQL，选择单个值 异步方法执行
        /// </summary>
        /// <typeparam name="TK">返回的类型</typeparam>
        /// <param name="sql">执行的SQL</param>
        /// <param name="param">用于此命令的参数</param>
        /// <returns>返回第一个单元格，为 <typeparamref name="TK"/>.</returns>
        public async Task<TK> ExecuteScalarAsync<TK>(string sql, object param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "参数不能为空！");
            }

            using (var conn = CreateConnection())
            {
                return await conn.ExecuteScalarAsync<TK>(sql, param);
            }

        }



        /// <summary>
        /// 执行一个查询返回类型为 <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">返回的结果类型。</typeparam>
        /// <param name="sql">查询执行的SQL。</param>
        /// <param name="param">要传递的参数</param>
        /// <returns>
        /// 所提供类型的数据序列;如果查询基本类型（int，string等），
        /// 则假定第一列中的数据，否则将为每行创建一个实例，并假定直接列名===成员名称映射（不区分大小写） 。
        /// </returns>
        public IEnumerable<T> Query<T>(string sql, object param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "参数不能为空！");
            }
            using (var conn = CreateConnection())
            {
                return conn.Query<T>(sql, param);
            }
        }

        /// <summary>
        /// 执行一个查询，返回类型为 <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">返回的结果类型。</typeparam>
        /// <param name="sql">查询执行的SQL。</param>
        /// <param name="param">要传递的参数</param>
        /// <returns>
        /// 所提供类型的数据序列;如果查询基本类型（int，string等），
        /// 则假定第一列中的数据，否则将为每行创建一个实例，并假定直接列名===成员名称映射（不区分大小写） 。
        /// </returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "参数不能为空！");
            }

            using (var conn = CreateConnection())
            {
                return await conn.QueryAsync<T>(sql, param);
            }

        }

        /// <summary>
        /// 执行一个返回多个结果集的命令，并依次返回每个结果集。
        /// </summary>
        /// <typeparam name="TFirst">第一个结果集中的对象类型</typeparam>
        /// <typeparam name="TSecond">第二个结果集中的对象类型</typeparam>
        /// <param name="sql">所要查询的SQL</param>
        /// <param name="param">用于查询的参数</param>
        /// <returns></returns>
        public Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>> QueryMultiple<TFirst, TSecond>(string sql, object param)
        {
            using (var conn = CreateConnection())
            {
                using (var gridReader = conn.QueryMultiple(sql, param))
                {
                    var resultFirst = gridReader.Read<TFirst>();
                    var resultSecond = gridReader.Read<TSecond>();
                    return new Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>>(resultFirst, resultSecond);
                }
            }
        }

        /// <summary>
        /// 执行一个返回多个结果集的命令，并依次返回每个结果集。
        /// </summary>
        /// <typeparam name="TFirst">第一个结果集中的对象类型</typeparam>
        /// <typeparam name="TSecond">第二个结果集中的对象类型</typeparam>
        /// <param name="sql">所要查询的SQL</param>
        /// <param name="param">用于查询的参数</param>
        /// <returns></returns>
        public async Task<Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>>> QueryMultipleAsync<TFirst, TSecond>(string sql,
            object param)
        {
            using (var conn = CreateConnection())
            {
                using (var gridReader = await conn.QueryMultipleAsync(sql, param))
                {
                    var resultFirst = gridReader.ReadAsync<TFirst>().Result;
                    var resultSecond = gridReader.ReadAsync<TSecond>().Result;
                    return new Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>>(resultFirst, resultSecond);
                }
            }
        }

        /// <summary>
        /// 执行一个返回多个结果集的命令，并依次返回每个结果集。
        /// </summary>
        /// <typeparam name="TFirst">第一个结果集中的对象类型</typeparam>
        /// <typeparam name="TSecond">第二个结果集中的对象类型</typeparam>
        /// <typeparam name="TThird">第三个结果集中的对象类型</typeparam>
        /// <param name="sql">所要查询的SQL</param>
        /// <param name="param">用于查询的参数</param>
        /// <returns></returns>
        public Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>> QueryMultiple<TFirst, TSecond, TThird>(string sql, object param)
        {
            using (var conn = CreateConnection())
            {
                using (var gridReader = conn.QueryMultiple(sql, param))
                {
                    var resultFirst = gridReader.Read<TFirst>();
                    var resultSecond = gridReader.Read<TSecond>();
                    var resultThird = gridReader.Read<TThird>();
                    return new Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>(resultFirst, resultSecond, resultThird);
                }
            }
        }

        /// <summary>
        /// 执行一个返回多个结果集的命令，并依次返回每个结果集。
        /// </summary>
        /// <typeparam name="TFirst">第一个结果集中的对象类型</typeparam>
        /// <typeparam name="TSecond">第二个结果集中的对象类型</typeparam>
        /// <typeparam name="TThird">第三个结果集中的对象类型</typeparam>
        /// <param name="sql">所要查询的SQL</param>
        /// <param name="param">用于查询的参数</param>
        /// <returns></returns>
        public async Task<Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>> QueryMultipleAsync<TFirst, TSecond, TThird>(string sql,
            object param)
        {
            using (var conn = CreateConnection())
            {
                using (var gridReader = await conn.QueryMultipleAsync(sql, param))
                {
                    var resultFirst = gridReader.ReadAsync<TFirst>().Result;
                    var resultSecond = gridReader.ReadAsync<TSecond>().Result;
                    var resultThird = gridReader.ReadAsync<TThird>().Result;
                    return new Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>(resultFirst, resultSecond, resultThird);
                }
            }
        }


        /// <summary>
        /// 执行一个返回多个结果集的命令，并依次返回每个结果集中单独实体
        /// </summary>
        /// <typeparam name="TFirst">第一个结果集中的对象类型</typeparam>
        /// <typeparam name="TSecond">第二个结果集中的对象类型</typeparam>
        /// <param name="sql">所要查询的SQL</param>
        /// <param name="param">用于查询的参数</param>
        /// <returns>返回每个结果集中单独实体</returns>
        public Tuple<TFirst, TSecond> QueryMultipleSingle<TFirst, TSecond>(string sql, object param)
        {
            using (var conn = CreateConnection())
            {
                using (var gridReader = conn.QueryMultiple(sql, param))
                {
                    var resultFirst = gridReader.ReadSingle<TFirst>();
                    var resultSecond = gridReader.ReadSingle<TSecond>();
                    return new Tuple<TFirst, TSecond>(resultFirst, resultSecond);
                }
            }
        }

        /// <summary>
        /// 执行一个返回多个结果集的命令，并依次返回每个结果集中单独实体
        /// </summary>
        /// <typeparam name="TFirst">第一个结果集中的对象类型</typeparam>
        /// <typeparam name="TSecond">第二个结果集中的对象类型</typeparam>
        /// <param name="sql">所要查询的SQL</param>
        /// <param name="param">用于查询的参数</param>
        /// <returns>返回每个结果集中单独实体</returns>
        public async Task<Tuple<TFirst, TSecond>> QueryMultipleSingleAsync<TFirst, TSecond>(string sql, object param)
        {
            using (var conn = CreateConnection())
            {
                using (var gridReader = await conn.QueryMultipleAsync(sql, param))
                {
                    var resultFirst = gridReader.ReadSingleAsync<TFirst>().Result;
                    var resultSecond = gridReader.ReadSingleAsync<TSecond>().Result;
                    return new Tuple<TFirst, TSecond>(resultFirst, resultSecond);
                }
            }
        }

        /// <summary>
        /// 执行一个返回多个结果集的命令，并依次返回每个结果集中单独实体
        /// </summary>
        /// <typeparam name="TFirst">第一个结果集中的对象类型</typeparam>
        /// <typeparam name="TSecond">第二个结果集中的对象类型</typeparam>
        /// <typeparam name="TThird">第三个结果集中的对象类型</typeparam>
        /// <param name="sql">所要查询的SQL</param>
        /// <param name="param">用于查询的参数</param>
        /// <returns>返回每个结果集中单独实体</returns>
        public Tuple<TFirst, TSecond, TThird> QueryMultipleSingle<TFirst, TSecond, TThird>(string sql, object param)
        {
            using (var conn = CreateConnection())
            {
                using (var gridReader = conn.QueryMultiple(sql, param))
                {
                    var resultFirst = gridReader.ReadSingle<TFirst>();
                    var resultSecond = gridReader.ReadSingle<TSecond>();
                    var resultThird = gridReader.ReadSingle<TThird>();
                    return new Tuple<TFirst, TSecond, TThird>(resultFirst, resultSecond, resultThird);
                }
            }
        }

        /// <summary>
        /// 执行一个返回多个结果集的命令，并依次返回每个结果集中单独实体
        /// </summary>
        /// <typeparam name="TFirst">第一个结果集中的对象类型</typeparam>
        /// <typeparam name="TSecond">第二个结果集中的对象类型</typeparam>
        /// <typeparam name="TThird">第三个结果集中的对象类型</typeparam>
        /// <param name="sql">所要查询的SQL</param>
        /// <param name="param">用于查询的参数</param>
        /// <returns>返回每个结果集中单独实体</returns>
        public async Task<Tuple<TFirst, TSecond, TThird>> QueryMultipleSingleAsync<TFirst, TSecond, TThird>(string sql, object param)
        {
            using (var conn = CreateConnection())
            {
                using (var gridReader = await conn.QueryMultipleAsync(sql, param))
                {
                    var resultFirst = gridReader.ReadSingleAsync<TFirst>().Result;
                    var resultSecond = gridReader.ReadSingleAsync<TSecond>().Result;
                    var resultThird = gridReader.ReadSingleAsync<TThird>().Result;
                    return new Tuple<TFirst, TSecond, TThird>(resultFirst, resultSecond, resultThird);
                }
            }
        }



        /// <summary>
        /// 使用2种输入类型执行多重映射查询。 
        /// 返回一个单一的类型，从原始类型合并而来 <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">记录集中的第一种类型。</typeparam>
        /// <typeparam name="TSecond">记录集中的第二种类型。</typeparam>
        /// <typeparam name="TReturn">合成的返回类型</typeparam>
        /// <param name="sql">查询的SQL</param>
        /// <param name="map">将行类型映射到返回类型的函数。</param>
        /// <param name="param">查询所需参数</param>
        /// <param name="splitOn">我们应该拆分并读取第二个对象的字段 (默认值: "Id")</param>
        /// <returns>可枚举的<typeparamref name="TReturn"/>.</returns>
        public IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn = "Id")
        {
            using (var conn = CreateConnection())
            {
                return conn.Query(sql, map, param, splitOn: splitOn);
            }
        }

        /// <summary>
        /// 使用2种输入类型执行多重映射查询。 
        /// 返回一个单一的类型，从原始类型合并而来 <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">记录集中的第一种类型。</typeparam>
        /// <typeparam name="TSecond">记录集中的第二种类型。</typeparam>
        /// <typeparam name="TReturn">合成的返回类型</typeparam>
        /// <param name="sql">查询的SQL</param>
        /// <param name="map">将行类型映射到返回类型的函数。</param>
        /// <param name="param">查询所需参数</param>
        /// <param name="splitOn">我们应该拆分并读取第二个对象的字段 (默认值: "Id")</param>
        /// <returns>可枚举的<typeparamref name="TReturn"/>.</returns>
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn = "Id")
        {
            using (var conn = CreateConnection())
            {
                return await conn.QueryAsync(sql, map, param, splitOn: splitOn);
            }
        }

        /// <summary>
        /// 使用3种输入类型执行多重映射查询。 
        /// 返回一个单一的类型，从原始类型合并而来 <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">记录集中的第一种类型。</typeparam>
        /// <typeparam name="TSecond">记录集中的第二种类型。</typeparam>
        /// <typeparam name="TThird">记录集中的第三种类型。</typeparam>
        /// <typeparam name="TReturn">合成的返回类型</typeparam>
        /// <param name="sql">查询的SQL</param>
        /// <param name="map">将行类型映射到返回类型的函数。</param>
        /// <param name="param">查询所需参数</param>
        /// <param name="splitOn">我们应该拆分并读取第二个对象的字段 (默认值: "Id")</param>
        /// <returns>可枚举的<typeparamref name="TReturn"/>.</returns>
        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param, string splitOn = "Id")
        {
            using (var conn = CreateConnection())
            {
                return conn.Query(sql, map, param, splitOn: splitOn);
            }
        }

        /// <summary>
        /// 使用3种输入类型执行多重映射查询。 
        /// 返回一个单一的类型，从原始类型合并而来 <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">记录集中的第一种类型。</typeparam>
        /// <typeparam name="TSecond">记录集中的第二种类型。</typeparam>
        /// <typeparam name="TThird">记录集中的第三种类型。</typeparam>
        /// <typeparam name="TReturn">合成的返回类型</typeparam>
        /// <param name="sql">查询的SQL</param>
        /// <param name="map">将行类型映射到返回类型的函数。</param>
        /// <param name="param">查询所需参数</param>
        /// <param name="splitOn">我们应该拆分并读取第二个对象的字段 (默认值: "Id")</param>
        /// <returns>可枚举的<typeparamref name="TReturn"/>.</returns>
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param, string splitOn = "Id")
        {
            using (var conn = CreateConnection())
            {
                return await conn.QueryAsync(sql, map, param, splitOn: splitOn);
            }
        }

        /// <summary>
        /// 使用4种输入类型执行多重映射查询。 
        /// 返回一个单一的类型，从原始类型合并而来 <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">记录集中的第一种类型。</typeparam>
        /// <typeparam name="TSecond">记录集中的第二种类型。</typeparam>
        /// <typeparam name="TThird">记录集中的第三种类型。</typeparam>
        /// <typeparam name="TFourth">记录集中的第四种类型。</typeparam>
        /// <typeparam name="TReturn">合成的返回类型</typeparam>
        /// <param name="sql">查询的SQL</param>
        /// <param name="map">将行类型映射到返回类型的函数。</param>
        /// <param name="param">查询所需参数</param>
        /// <param name="splitOn">我们应该拆分并读取第二个对象的字段 (默认值: "Id")</param>
        /// <returns>可枚举的<typeparamref name="TReturn"/>.</returns>
        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param, string splitOn = "Id")
        {
            using (var conn = CreateConnection())
            {
                return conn.Query(sql, map, param, splitOn: splitOn);
            }
        }

        /// <summary>
        /// 使用4种输入类型执行多重映射查询。 
        /// 返回一个单一的类型，从原始类型合并而来 <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">记录集中的第一种类型。</typeparam>
        /// <typeparam name="TSecond">记录集中的第二种类型。</typeparam>
        /// <typeparam name="TThird">记录集中的第三种类型。</typeparam>
        /// <typeparam name="TFourth">记录集中的第四种类型。</typeparam>
        /// <typeparam name="TReturn">合成的返回类型</typeparam>
        /// <param name="sql">查询的SQL</param>
        /// <param name="map">将行类型映射到返回类型的函数。</param>
        /// <param name="param">查询所需参数</param>
        /// <param name="splitOn">我们应该拆分并读取第二个对象的字段 (默认值: "Id")</param>
        /// <returns>可枚举的<typeparamref name="TReturn"/>.</returns>
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param, string splitOn = "Id")
        {
            using (var conn = CreateConnection())
            {
                return await conn.QueryAsync(sql, map, param, splitOn: splitOn);
            }
        }

        /// <summary>
        /// 使用5种输入类型执行多重映射查询。 
        /// 返回一个单一的类型，从原始类型合并而来 <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">记录集中的第一种类型。</typeparam>
        /// <typeparam name="TSecond">记录集中的第二种类型。</typeparam>
        /// <typeparam name="TThird">记录集中的第三种类型。</typeparam>
        /// <typeparam name="TFourth">记录集中的第四种类型。</typeparam>
        /// <typeparam name="TFifth">记录集中的第五种类型。</typeparam>
        /// <typeparam name="TReturn">合成的返回类型</typeparam>
        /// <param name="sql">查询的SQL</param>
        /// <param name="map">将行类型映射到返回类型的函数。</param>
        /// <param name="param">查询所需参数</param>
        /// <param name="splitOn">我们应该拆分并读取第二个对象的字段 (默认值: "Id")</param>
        /// <returns>可枚举的<typeparamref name="TReturn"/>.</returns>
        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param, string splitOn = "Id")
        {
            using (var conn = CreateConnection())
            {
                return conn.Query(sql, map, param, splitOn: splitOn);
            }
        }

        /// <summary>
        /// 使用5种输入类型执行多重映射查询。 
        /// 返回一个单一的类型，从原始类型合并而来 <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">记录集中的第一种类型。</typeparam>
        /// <typeparam name="TSecond">记录集中的第二种类型。</typeparam>
        /// <typeparam name="TThird">记录集中的第三种类型。</typeparam>
        /// <typeparam name="TFourth">记录集中的第四种类型。</typeparam>
        /// <typeparam name="TFifth">记录集中的第五种类型。</typeparam>
        /// <typeparam name="TReturn">合成的返回类型</typeparam>
        /// <param name="sql">查询的SQL</param>
        /// <param name="map">将行类型映射到返回类型的函数。</param>
        /// <param name="param">查询所需参数</param>
        /// <param name="splitOn">我们应该拆分并读取第二个对象的字段 (默认值: "Id")</param>
        /// <returns>可枚举的<typeparamref name="TReturn"/>.</returns>
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param, string splitOn = "Id")
        {
            using (var conn = CreateConnection())
            {
                return await conn.QueryAsync(sql, map, param, splitOn: splitOn);
            }
        }

        /// <summary>
        /// 原子性操作
        /// </summary>
        /// <param name="action"></param>
        /// <param name="isTransactionScopeAsync"></param>
        public bool UnitOfWork(Action action, bool isTransactionScopeAsync = true)
        {
            var t = false;
            using (var transaction = new TransactionScope(isTransactionScopeAsync ? TransactionScopeAsyncFlowOption.Enabled : TransactionScopeAsyncFlowOption.Suppress))
            {
                try
                {
                    action.Invoke();
                    transaction.Complete();
                    t = true;
                }
                catch (TransactionAbortedException ex)
                {
                    _log.LogError(ex, "TransactionAbortedException");
                }
                catch (ApplicationException ex)
                {
                    _log.LogError(ex, "ApplicationException");
                }
                catch (Exception e)
                {
                    _log.LogError(e, "TransactionException");
                }
            }
            return t;
        }

        /// <summary>
        /// 执行单行查询，将输入的数据返回为<typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">要返回的结果的类型。</typeparam>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="param"></param>
        /// <returns>所提供类型的数据序列;如果查询基本类型（int，string等），则假定第一列中的数据，
        /// 否则将为每行创建一个实例，并假定直接列名===成员名称映射（不区分大小写） 。</returns>
        public T QuerySingle<T>(string sql, object param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "参数不能为空！");
            }

            using (var conn = CreateConnection())
            {
                return conn.QuerySingleOrDefault<T>(sql, param);
            }
        }

        /// <summary>
        /// 执行单行查询，将输入的数据返回为<typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">要返回的结果的类型。</typeparam>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="param"></param>
        /// <returns>所提供类型的数据序列;如果查询基本类型（int，string等），则假定第一列中的数据，
        /// 否则将为每行创建一个实例，并假定直接列名===成员名称映射（不区分大小写） 。</returns>
        public async Task<T> QuerySingleAsync<T>(string sql, object param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "参数不能为空！");
            }


            using (var conn = CreateConnection())
            {
                return await conn.QuerySingleOrDefaultAsync<T>(sql, param);
            }

        }

        /// <summary>
        ///插入实体返回自增id
        /// </summary>
        /// <typeparam name="T">插入的类型自增Id需标识[Key]</typeparam>
        /// <param name="entity">插入实体</param>
        /// <returns>操作结果True or False</returns>
        public bool Insert<T>(T entity) where T : class
        {
            using (var conn = CreateConnection())
            {
                return conn.Insert(entity);
            }
        }

        /// <summary>
        ///插入实体返回自增id
        /// </summary>
        /// <typeparam name="T">插入的类型自增Id需标识[Key]</typeparam>
        /// <param name="entity">插入实体</param>
        /// <returns>操作结果True or False</returns>
        public async Task<bool> InsertAsync<T>(T entity) where T : class
        {
            using (var conn = CreateConnection())
            {
                return await conn.InsertAsync(entity);
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="T">T实体中必须包含标有[Key] [ExplicitKey]属性。</typeparam>
        /// <param name="entity"></param>
        /// <returns>操作结果True or False</returns>
        public bool Update<T>(T entity) where T : class
        {
            using (var conn = CreateConnection())
            {
                return conn.Update(entity);
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="T">T实体中必须包含标有[Key] [ExplicitKey]属性。</typeparam>
        /// <param name="entity"></param>
        /// <returns>操作结果True or False</returns>
        public async Task<bool> UpdateAsync<T>(T entity) where T : class
        {
            using (var conn = CreateConnection())
            {
                return await conn.UpdateAsync(entity);
            }
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T">T实体中必须包含标有[Key] [ExplicitKey]属性。</typeparam>
        /// <param name="entity"></param>
        /// <returns>操作结果True or False</returns>
        public bool Delete<T>(T entity) where T : class
        {
            using (var conn = CreateConnection())
            {
                return conn.Delete(entity);
            }
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T">T实体中必须包含标有[Key] [ExplicitKey]属性。</typeparam>
        /// <param name="entity"></param>
        /// <returns>操作结果True or False</returns>
        public async Task<bool> DeleteAsync<T>(T entity) where T : class
        {
            using (var conn = CreateConnection())
            {
                return await conn.DeleteAsync(entity);
            }
        }

        /// <summary>
        /// 从表“Ts”中返回单个实体的单个实体。
        ///T实体中必须包含Id或标有[Key] [ExplicitKey]属性。
        ///创建的实体被跟踪/拦截以进行更改并由Update（）扩展使用。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">要获取的实体的ID必须标有[Key]或[ExplicitKey]属性</param>
        /// <returns>T的实体</returns>
        public T Get<T>(dynamic id) where T : class
        {
            using (var conn = CreateConnection())
            {
                return SqlMapperExtensions.Get<T>(conn, id);
            }
        }

        /// <summary>
        /// 使用.NET 4.5 Task以异步方式从表“Ts”中返回单个实体的单个实体。
        ///T实体中必须包含Id或标有[Key] [ExplicitKey]属性。
        ///创建的实体被跟踪/拦截以进行更改并由Update（）扩展使用。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">要获取的实体的ID必须标有[Key]或[ExplicitKey]属性</param>
        /// <returns>T的实体</returns>
        public async Task<T> GetAsync<T>(dynamic id) where T : class
        {
            using (var conn = CreateConnection())
            {
                return await SqlMapperExtensions.GetAsync<T>(conn, id);
            }
        }

        public Query<T> Query<T>()
        {
            return new Query<T>(this);
        }
    }
}
