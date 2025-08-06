using System.ComponentModel;
using System.Data;

namespace My.Framework.MySQLAccessor
{
    public interface IMySqlContext
    {

        /// <summary>
        /// 创建一个数据库连接
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IDbConnection CreateConnection();
        /// <summary>
        /// 执行参数化SQL
        /// </summary>
        /// <param name="sql">所要执行的SQL</param>
        /// <param name="param">查询所需参数</param>
        /// <returns>受影响的行数</returns>
        int Execute(string sql, object param);

        /// <summary>
        /// 执行参数化SQL
        /// </summary>
        /// <param name="sql">所要执行的SQL</param>
        /// <param name="param">查询所需参数</param>
        /// <returns>受影响的行数</returns>
        Task<int> ExecuteAsync(string sql, object param);

        /// <summary>
        /// 执行参数化SQL，选择单个值 
        /// </summary>
        /// <typeparam name="TK">返回的类型</typeparam>
        /// <param name="sql">执行的SQL</param>
        /// <param name="param">用于此命令的参数</param>
        /// <returns>返回第一个单元格，为 <typeparamref name="TK"/>.</returns>
        TK ExecuteScalar<TK>(string sql, object param);


        /// <summary>
        /// 执行参数化SQL，选择单个值 异步方法执行
        /// </summary>
        /// <typeparam name="TK">返回的类型</typeparam>
        /// <param name="sql">执行的SQL</param>
        /// <param name="param">用于此命令的参数</param>
        /// <returns>返回第一个单元格，为 <typeparamref name="TK"/>.</returns>
        Task<TK> ExecuteScalarAsync<TK>(string sql, object param);

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
        IEnumerable<T> Query<T>(string sql, object param);

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
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param);

        /// <summary>
        /// 执行一个返回多个结果集的命令，并依次返回每个结果集。
        /// </summary>
        /// <typeparam name="TFirst">第一个结果集中的对象类型</typeparam>
        /// <typeparam name="TSecond">第二个结果集中的对象类型</typeparam>
        /// <param name="sql">所要查询的SQL</param>
        /// <param name="param">用于查询的参数</param>
        /// <returns></returns>
        Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>> QueryMultiple<TFirst, TSecond>(string sql, object param);


        /// <summary>
        /// 执行一个返回多个结果集的命令，并依次返回每个结果集。
        /// </summary>
        /// <typeparam name="TFirst">第一个结果集中的对象类型</typeparam>
        /// <typeparam name="TSecond">第二个结果集中的对象类型</typeparam>
        /// <param name="sql">所要查询的SQL</param>
        /// <param name="param">用于查询的参数</param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>>> QueryMultipleAsync<TFirst, TSecond>(string sql,
            object param);


        /// <summary>
        /// 执行一个返回多个结果集的命令，并依次返回每个结果集。
        /// </summary>
        /// <typeparam name="TFirst">第一个结果集中的对象类型</typeparam>
        /// <typeparam name="TSecond">第二个结果集中的对象类型</typeparam>
        /// <typeparam name="TThird">第三个结果集中的对象类型</typeparam>
        /// <param name="sql">所要查询的SQL</param>
        /// <param name="param">用于查询的参数</param>
        /// <returns></returns>
        Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>> QueryMultiple<TFirst, TSecond,
            TThird>(string sql, object param);

        /// <summary>
        /// 执行一个返回多个结果集的命令，并依次返回每个结果集。
        /// </summary>
        /// <typeparam name="TFirst">第一个结果集中的对象类型</typeparam>
        /// <typeparam name="TSecond">第二个结果集中的对象类型</typeparam>
        /// <typeparam name="TThird">第三个结果集中的对象类型</typeparam>
        /// <param name="sql">所要查询的SQL</param>
        /// <param name="param">用于查询的参数</param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>> QueryMultipleAsync<TFirst, TSecond,
            TThird>(string sql,
            object param);

        /// <summary>
        /// 执行一个返回多个结果集的命令，并依次返回每个结果集中单独实体
        /// </summary>
        /// <typeparam name="TFirst">第一个结果集中的对象类型</typeparam>
        /// <typeparam name="TSecond">第二个结果集中的对象类型</typeparam>
        /// <param name="sql">所要查询的SQL</param>
        /// <param name="param">用于查询的参数</param>
        /// <returns>返回每个结果集中单独实体</returns>
        Tuple<TFirst, TSecond> QueryMultipleSingle<TFirst, TSecond>(string sql, object param);

        /// <summary>
        /// 执行一个返回多个结果集的命令，并依次返回每个结果集中单独实体
        /// </summary>
        /// <typeparam name="TFirst">第一个结果集中的对象类型</typeparam>
        /// <typeparam name="TSecond">第二个结果集中的对象类型</typeparam>
        /// <param name="sql">所要查询的SQL</param>
        /// <param name="param">用于查询的参数</param>
        /// <returns>返回每个结果集中单独实体</returns>
        Task<Tuple<TFirst, TSecond>> QueryMultipleSingleAsync<TFirst, TSecond>(string sql, object param);

        /// <summary>
        /// 执行一个返回多个结果集的命令，并依次返回每个结果集中单独实体
        /// </summary>
        /// <typeparam name="TFirst">第一个结果集中的对象类型</typeparam>
        /// <typeparam name="TSecond">第二个结果集中的对象类型</typeparam>
        /// <typeparam name="TThird">第三个结果集中的对象类型</typeparam>
        /// <param name="sql">所要查询的SQL</param>
        /// <param name="param">用于查询的参数</param>
        /// <returns>返回每个结果集中单独实体</returns>
        Tuple<TFirst, TSecond, TThird> QueryMultipleSingle<TFirst, TSecond, TThird>(string sql, object param);


        /// <summary>
        /// 执行一个返回多个结果集的命令，并依次返回每个结果集中单独实体
        /// </summary>
        /// <typeparam name="TFirst">第一个结果集中的对象类型</typeparam>
        /// <typeparam name="TSecond">第二个结果集中的对象类型</typeparam>
        /// <typeparam name="TThird">第三个结果集中的对象类型</typeparam>
        /// <param name="sql">所要查询的SQL</param>
        /// <param name="param">用于查询的参数</param>
        /// <returns>返回每个结果集中单独实体</returns>
        Task<Tuple<TFirst, TSecond, TThird>>
            QueryMultipleSingleAsync<TFirst, TSecond, TThird>(string sql, object param);

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
        IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn = "Id");


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
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn = "Id");

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
        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param, string splitOn = "Id");


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
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param, string splitOn = "Id");


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
        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param, string splitOn = "Id");


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
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param, string splitOn = "Id");


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
        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param, string splitOn = "Id");

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
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param, string splitOn = "Id");


        /// <summary>
        /// 执行单行查询，将输入的数据返回为<typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">要返回的结果的类型。</typeparam>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="param"></param>
        /// <returns>所提供类型的数据序列;如果查询基本类型（int，string等），则假定第一列中的数据，
        /// 否则将为每行创建一个实例，并假定直接列名===成员名称映射（不区分大小写） 。</returns>
        Task<T> QuerySingleAsync<T>(string sql, object param);

        /// <summary>
        /// 执行单行查询，将输入的数据返回为<typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">要返回的结果的类型。</typeparam>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="param"></param>
        /// <returns>所提供类型的数据序列;如果查询基本类型（int，string等），则假定第一列中的数据，
        /// 否则将为每行创建一个实例，并假定直接列名===成员名称映射（不区分大小写） 。</returns>
        T QuerySingle<T>(string sql, object param);

        /// <summary>
        /// 原子性操作
        /// </summary>
        /// <param name="action"></param>
        /// <param name="isTransactionScopeAsync"></param>
        bool UnitOfWork(Action action, bool isTransactionScopeAsync = true);

        /// <summary>
        /// 插入表“Ts”中的实体并返回标识ID。
        /// </summary>
        /// <typeparam name="T">插入的类型自增Id需标识[Key]</typeparam>
        /// <param name="entity">插入的实体</param>
        /// <returns>操作结果True or False</returns>
        bool Insert<T>(T entity) where T : class;

        /// <summary>
        /// 异步方式插入表“Ts”中的实体并返回标识ID。
        /// </summary>
        /// <typeparam name="T">插入的类型自增Id需标识[Key]</typeparam>
        /// <param name="entity">插入的实体</param>
        /// <returns>操作结果True or False</returns>
        Task<bool> InsertAsync<T>(T entity) where T : class;

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="T">T实体中必须包含标有[Key] [ExplicitKey]的属性。</typeparam>
        /// <param name="entity"></param>
        /// <returns>操作结果True or False</returns>

        bool Update<T>(T entity) where T : class;

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="T">T实体中必须包含标有[Key] [ExplicitKey]的属性。</typeparam>
        /// <param name="entity"></param>
        /// <returns>操作结果True or False</returns>
        Task<bool> UpdateAsync<T>(T entity) where T : class;


        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T">T实体中必须包含标有[Key] [ExplicitKey]的属性。</typeparam>
        /// <param name="entity"></param>
        /// <returns>操作结果True or False</returns>
        bool Delete<T>(T entity) where T : class;

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T">T实体中必须包含标有[Key] [ExplicitKey]的属性。</typeparam>
        /// <param name="entity"></param>
        /// <returns>操作结果True or False</returns>
        Task<bool> DeleteAsync<T>(T entity) where T : class;

        /// <summary>
        /// 从表“Ts”中返回单个实体的单个实体。 T必须是接口类型。
        ///实体中必须包标有[Key] [ExplicitKey]属性。
        ///创建的实体被跟踪/拦截以进行更改并由Update（）扩展使用。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">要获取的实体的ID必须标有[Key]或[ExplicitKey]属性</param>
        /// <returns>T的实体</returns>
        T Get<T>(dynamic id) where T : class;


        /// <summary>
        /// 使用.NET 4.5 Task以异步方式从表“Ts”中返回单个实体的单个实体。 T必须是接口类型。
        ///实体中必须标有[Key] [ExplicitKey]属性。
        ///创建的实体被跟踪/拦截以进行更改并由Update（）扩展使用。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">要获取的实体的ID必须标有[Key]或[ExplicitKey]属性</param>
        /// <returns>T的实体</returns>
        Task<T> GetAsync<T>(dynamic id) where T : class;



        Query<T> Query<T>();

    }
}
