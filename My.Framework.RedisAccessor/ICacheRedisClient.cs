using My.Framework.Foundation.Redis;
using My.Framework.Foundation.Serializer;
using StackExchange.Redis;

namespace My.Framework.RedisAccessor
{
    public interface ICacheRedisClient : IDisposable
    {
        /// <summary>
        /// 配置选项
        /// </summary>
        RedisConfig RedisConfig { get; }

        /// <summary>
        /// redis的连接管理实例
        /// </summary>
        IConnectionMultiplexer Connection { get; }

        /// <summary>
        /// 返回一个序列化的实例
        /// </summary>
        ISerializer Serializer { get; }

        /// <summary>
        /// 返回一个缓存数据库的实例
        /// </summary>
        IDatabase Database { get; }

        /// <summary>
        /// 异步检查键值是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// 异步移除一个键值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> RemoveAsync(string key);

        /// <summary>
        /// 异步移除一组键值
        /// </summary>
        /// <param name="keys"></param>
        Task RemoveAllAsync(IEnumerable<string> keys);

        /// <summary>
        /// 根据一个键异步获取一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// 异步添加一对键值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> AddAsync<T>(string key, T value);

        /// <summary>
        /// 异步根据一个键替换一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> ReplaceAsync<T>(string key, T value);

        /// <summary>
        /// 异步添加一对键值，并设置失效期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt">失效期</param>
        /// <returns></returns>
        Task<bool> AddAsync<T>(string key, T value, DateTimeOffset expiresAt);

        /// <summary>
        /// 异步替换一个键值并设置失效期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        Task<bool> ReplaceAsync<T>(string key, T value, DateTimeOffset expiresAt);

        /// <summary>
        /// 添加一对键值，并设置有效期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn);

        /// <summary>
        /// 替换一对键值，并设置有效期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn);

        Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys);

        Task<bool> AddAllAsync<T>(IList<Tuple<string, T>> items);

        Task<bool> SetAddAsync<T>(string key, T item) where T : class;

        Task<long> SetAddAllAsync<T>(string key, params T[] items) where T : class;

        Task<bool> SetRemoveAsync<T>(string key, T item) where T : class;

        Task<long> SetRemoveAllAsync<T>(string key, params T[] items) where T : class;

        Task<string[]> SetMemberAsync(string memberName);

        Task<IEnumerable<T>> SetMembersAsync<T>(string key);

        Task<IEnumerable<string>> SearchKeysAsync(string pattern);

        Task FlushDbAsync();

        Task SaveAsync(SaveType saveType);

        Task<Dictionary<string, string>> GetInfoAsync();

        Task<long> PublishAsync<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None);

        Task SubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None);

        Task UnsubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None);

        Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None);

        Task<long> ListLeftPushAsync<T>(string key, T item) where T : class;

        Task<T> ListRightPopAsync<T>(string key) where T : class;

        Task<bool> HashDeleteAsync(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None);

        Task<long> HashDeleteAsync(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None);

        Task<bool> HashExistsAsync(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None);

        Task<T> HashGetAsync<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None);

        Task<Dictionary<string, T>> HashGetAsync<T>(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None);

        Task<Dictionary<string, T>> HashGetAllAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None);

        Task<long> HashIncerementByAsync(string hashKey, string key, long value, CommandFlags commandFlags = CommandFlags.None);

        Task<double> HashIncerementByAsync(string hashKey, string key, double value, CommandFlags commandFlags = CommandFlags.None);

        Task<IEnumerable<string>> HashKeysAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None);

        Task<long> HashLengthAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None);

        Task<bool> HashSetAsync<T>(string hashKey, string key, T value, bool nx = false, CommandFlags commandFlags = CommandFlags.None);

        Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values, CommandFlags commandFlags = CommandFlags.None);

        Task<IEnumerable<T>> HashValuesAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None);

        Task<Dictionary<string, T>> HashScanAsync<T>(string hashKey, string pattern, int pageSize = 10, CommandFlags commandFlags = CommandFlags.None);
    }
}
