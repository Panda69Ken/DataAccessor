using MongoDB.Driver;
using System.Linq.Expressions;

namespace My.Framework.MongoDbAccessor.Interface
{
    public interface IMongoService
    {
        /// <summary>
        /// MongoDB数据库
        /// </summary>
        IMongoDatabase MongoDb { get; set; }
        /// <summary>
        /// 找到一个单一的文件，并原子化更新
        /// </summary>
        /// <typeparam name="T">要更新的实体类型。</typeparam>
        /// <param name="update">要更改的字段表达式。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="updateInc">要自增操作的表达式</param>
        /// <param name="returnDocumentAfter">是否返回操作后版本</param>
        /// <returns>returnDocumentAfter 为true返回更新后文档,returnDocumentAfter为false返回更新前文档</returns>
        T FindOneAndUpdate<T>(Expression<Func<T>> update, Expression<Func<T, bool>> where,
            Expression<Func<T>> updateInc = null, bool returnDocumentAfter = true) where T : class, new();

        /// <summary>
        /// 异步找到一个单一的文件，并原子化更新
        /// </summary>
        /// <typeparam name="T">要更新的实体类型。</typeparam>
        /// <param name="update">要更改的字段表达式。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="updateInc">要自增操作的表达式</param>
        /// <param name="returnDocumentAfter">是否返回操作后版本</param>
        /// <returns>returnDocumentAfter 为true返回更新后文档,returnDocumentAfter为false返回更新前文档</returns>
        Task<T> FindOneAndUpdateAsync<T>(Expression<Func<T>> update, Expression<Func<T, bool>> where,
            Expression<Func<T>> updateInc = null, bool returnDocumentAfter = true) where T : class, new();


        /// <summary>
        /// 按条件将指定对象做自增操作。
        /// </summary>
        /// <typeparam name="T">要更新的实体类型。</typeparam>
        /// <param name="updateInc">要自增操作的表达式</param>
        /// <param name="where">条件表达式。</param>
        /// <returns>受影响的记录数。</returns>
        long UpdateInc<T>(Expression<Func<T>> updateInc, Expression<Func<T, bool>> where) where T : class, new();

        /// <summary>
        /// 异步按条件将指定对象做自增操作。
        /// </summary>
        /// <typeparam name="T">要更新的实体类型。</typeparam>
        /// <param name="updateInc">要自增操作的表达式</param>
        /// <param name="where">条件表达式。</param>
        /// <returns>受影响的记录数。</returns>
        Task<long> UpdateIncAsync<T>(Expression<Func<T>> updateInc, Expression<Func<T, bool>> where)
            where T : class, new();


        /// <summary>
        /// 合并插入和更新，当存在相同键值的实体则执行更新，否则执行插入。
        /// </summary>
        /// <typeparam name="T">类型参数。</typeparam>
        /// <param name="entity">实体参数。</param>
        /// <param name="keyStatement">键声明。注意：因为一些原因（比如声明的键并不具有唯一性）可能在声明的键下存在多个实体，
        /// 则本操作会将多余实体全部删除。只更新和保留其中一个。</param>
        /// <returns>受影响的实体数。</returns>
        void Set<T>(T entity, Expression<Func<T, object>> keyStatement) where T : class, new();

        /// <summary>
        /// 按条件修改实体对象的部分字段。
        /// </summary>
        /// <typeparam name="T">要更新的实体类型。</typeparam>
        /// <param name="update">要更改的字段表达式。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="updateInc">要自增操作的表达式</param>
        /// <returns>受影响的记录数。</returns>
        long Update<T>(Expression<Func<T>> update, Expression<Func<T, bool>> where, Expression<Func<T>> updateInc = null)
            where T : class, new();

        /// <summary>
        /// 异步按条件修改实体对象的部分字段。
        /// </summary>
        /// <typeparam name="T">要更新的实体类型。</typeparam>
        /// <param name="update">要更改的字段表达式。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="updateInc">要自增操作的表达式</param>
        /// <returns>受影响的记录数。</returns>
        Task<long> UpdateAsync<T>(Expression<Func<T>> update, Expression<Func<T, bool>> where,
            Expression<Func<T>> updateInc = null) where T : class, new();

        /// <summary>
        /// 将存在相同键值的实体更新为指定实体。
        /// </summary>
        /// <typeparam name="T">类型参数。</typeparam>
        /// <param name="entity">实体参数。</param>
        /// <param name="keyStatement">键声明。注意：因为一些原因（比如声明的键并不具有唯一性）可能在声明的键下存在多个实体，
        /// 则本操作会将多余实体全部删除。只更新和保留其中一个。</param>
        /// <returns>受影响的实体数。</returns>
        long Update<T>(T entity, Expression<Func<T, object>> keyStatement) where T : class, new();

        /// <summary>
        /// 异步将存在相同键值的实体更新为指定实体。
        /// </summary>
        /// <typeparam name="T">类型参数。</typeparam>
        /// <param name="entity">实体参数。</param>
        /// <param name="keyStatement">键声明。注意：因为一些原因（比如声明的键并不具有唯一性）可能在声明的键下存在多个实体，
        /// 则本操作会将多余实体全部删除。只更新和保留其中一个。</param>
        /// <returns>受影响的实体数。</returns>
        Task<long> UpdateAsync<T>(T entity, Expression<Func<T, object>> keyStatement) where T : class, new();

        /// <summary>
        /// 按条件删除，通常用于删除多条实体。
        /// </summary>
        /// <typeparam name="T">要删除的实体类型。</typeparam>
        /// <param name="where">执行删除的条件。</param>
        /// <returns>删除的实体数。</returns>
        long Delete<T>(Expression<Func<T, bool>> where) where T : class, new();

        /// <summary>
        /// 异步按条件删除，通常用于删除多条实体。
        /// </summary>
        /// <typeparam name="T">要删除的实体类型。</typeparam>
        /// <param name="where">执行删除的条件。</param>
        /// <returns>删除的实体数。</returns>
        Task<long> DeleteAsync<T>(Expression<Func<T, bool>> where) where T : class, new();

        /// <summary>
        /// 删除与指定参数有相同键值的同类型实体，通常用于删除一条实体。
        /// </summary>
        /// <typeparam name="T">类型参数。</typeparam>
        /// <param name="entity">实体参数。</param>
        /// <param name="keyStatement">键声明。注意：因为一些原因（比如声明的键并不具有唯一性）可能在声明的键下存在多个实体，
        /// 则本操作会将这些实体全部删除。</param>
        /// <returns>删除的实体数。</returns>
        long Delete<T>(T entity, Expression<Func<T, object>> keyStatement) where T : class, new();

        /// <summary>
        /// 异步删除与指定参数有相同键值的同类型实体，通常用于删除一条实体。
        /// </summary>
        /// <typeparam name="T">类型参数。</typeparam>
        /// <param name="entity">实体参数。</param>
        /// <param name="keyStatement">键声明。注意：因为一些原因（比如声明的键并不具有唯一性）可能在声明的键下存在多个实体，
        /// 则本操作会将这些实体全部删除。</param>
        /// <returns>删除的实体数。</returns>
        Task<long> DeleteAsync<T>(T entity, Expression<Func<T, object>> keyStatement) where T : class, new();


        /// <summary>
        /// 返回可查询泛型对象。
        /// </summary>
        /// <typeparam name="T">返回类型。</typeparam>
        /// <param name="isForcingPrimary">是否必须查分片主节点。</param>
        /// <returns>可查接口。</returns>
        IQueryable<T> GetQueryable<T>(bool isForcingPrimary = true) where T : class, new();

        /// <summary>
        /// 添加一个实体。
        /// </summary>
        /// <typeparam name="T">要添加的实体类型。</typeparam>
        /// <param name="entity">要添加的实体。</param>
        void Add<T>(T entity);

        /// <summary>
        /// 异步添加一个实体。
        /// </summary>
        /// <typeparam name="T">要添加的实体类型。</typeparam>
        /// <param name="entity">要添加的实体。</param>
        Task AddAsync<T>(T entity);

        /// <summary>
        /// 添加多个同类型的实体。
        /// </summary>
        /// <typeparam name="T">要添加的实体类型。</typeparam>
        /// <param name="entities">要添加的实体列表。</param>
        void AddRange<T>(IEnumerable<T> entities) where T : class, new();

        /// <summary>
        /// 异步添加多个同类型的实体。
        /// </summary>
        /// <typeparam name="T">要添加的实体类型。</typeparam>
        /// <param name="entities">要添加的实体列表。</param>
        Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, new();
    }
}
