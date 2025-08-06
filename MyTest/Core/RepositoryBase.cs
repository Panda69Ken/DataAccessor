using My.Framework.MySQLAccessor;
using System.Linq.Expressions;

namespace MyTest.Core
{
    public class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly IContextContainer _contextContainer;
        protected readonly IMySqlContext _master;
        protected readonly IMySqlContext _slave;

        public RepositoryBase(IContextContainer contextContainer)
        {
            _contextContainer = contextContainer;
            _master = _contextContainer.GetMasterContext("chat");
            _slave = _contextContainer.GetSalveContextRandom("chat");
        }

        public virtual string GetTableName()
        {
            return _master.Query<T>().GetTableName();
        }

        public virtual async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate, bool master = false)
        {
            return await (master ? _master : _slave).Query<T>().Where(predicate).ToListAsync();
        }

        public virtual async Task<T> FirstAsync(Expression<Func<T, bool>> predicate, bool master = false)
        {
            var items = await (master ? _master : _slave).Query<T>().Find(predicate).ToListAsync();
            return items.FirstOrDefault();
        }

        public virtual async Task<long> CountAsync(Expression<Func<T, bool>> predicate, bool master = false)
        {
            var items = await (master ? _master : _slave).Query<T>().Find(predicate).CountAsync();
            return items;
        }

        public virtual async Task<T> GetAsync(dynamic key, bool master = false)
        {
            return await (master ? _master : _slave).GetAsync<T>(key);
        }

        public virtual async Task<int> UpdateAsync(Expression<Func<T, bool>> where, Expression<Func<T, T>> pars)
        {
            return await _master.Query<T>().UpdateAsync(where, pars);
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            return await _master.UpdateAsync(entity);
        }

        public virtual Task<bool> DeleteAsync(T entity)
        {
            return _master.DeleteAsync<T>(entity);
        }

        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate, bool master = false)
        {
            var items = await (master ? _master : _slave).Query<T>().Find(predicate).ToListAsync();
            if (!items.Any()) return false;
            return await _master.DeleteAsync<T>(items.FirstOrDefault());
        }

        public virtual async Task<bool> InsertAsync(T entity)
        {
            return await _master.InsertAsync(entity);
        }

        public virtual async Task<int> ExecuteAsync(string sql, object param)
        {
            return await _master.ExecuteAsync(sql, param);
        }

        public virtual async Task<IEnumerable<T>> QueryAsync(string sql, object param = null, bool master = false)
        {
            return await (master ? _master : _slave).QueryAsync<T>(sql, param ?? new { });
        }

        public virtual async Task<string> ExecuteScalarAsync(string sql, object param = null, bool master = false)
        {
            return await (master ? _master : _slave).ExecuteScalarAsync<string>(sql, param ?? new { });
        }

        public virtual async Task<T> QuerySingleAsync<T>(string sql, object param, bool master = false)
        {
            return await (master ? _master : _slave).QuerySingleAsync<T>(sql, param ?? new { });
        }

        public virtual async Task<Tuple<long, IEnumerable<T>>> QueryMultipleAsync(string sql, object param = null, bool master = false)
        {
            var result = await (master ? _master : _slave).QueryMultipleAsync<long, T>(sql, param ?? new { });
            return new Tuple<long, IEnumerable<T>>(result.Item1.SingleOrDefault(), result.Item2);
        }

        public virtual bool UnitOfWork(Action action, bool isTransactionScopeAsync = true)
        {
            return _master.UnitOfWork(action, isTransactionScopeAsync);
        }

        public virtual Task<int> BulkInsert(List<T> items)
        {
            return _master.Query<T>().BulkInsertAsync(items);
        }

        public virtual Task<int> BulkUpdateAsync(List<T> items)
        {
            return _master.Query<T>().BulkUpdateAsync(items);
        }

    }
}
