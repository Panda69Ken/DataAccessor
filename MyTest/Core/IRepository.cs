using System.Linq.Expressions;

namespace MyTest.Core
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate, bool master = false);

        Task<T> FirstAsync(Expression<Func<T, bool>> predicate, bool master = false);

        Task<long> CountAsync(Expression<Func<T, bool>> predicate, bool master = false);

        Task<int> UpdateAsync(Expression<Func<T, bool>> where, Expression<Func<T, T>> pars);

        Task<bool> UpdateAsync(T entity);

        Task<bool> DeleteAsync(T entity);

        Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate, bool master = false);

        Task<bool> InsertAsync(T entity);

        Task<int> ExecuteAsync(string sql, object param);

        Task<IEnumerable<T>> QueryAsync(string sql, object param = null, bool master = false);

        Task<string> ExecuteScalarAsync(string sql, object param = null, bool master = false);

        Task<T> QuerySingleAsync<T>(string sql, object param, bool master = false);

        Task<Tuple<long, IEnumerable<T>>> QueryMultipleAsync(string sql, object param = null, bool master = false);

        Task<int> BulkInsert(List<T> items);

        Task<int> BulkUpdateAsync(List<T> items);

        bool UnitOfWork(Action action, bool isTransactionScopeAsync = true);
    }
}
