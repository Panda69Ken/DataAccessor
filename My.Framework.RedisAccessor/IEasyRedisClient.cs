namespace My.Framework.RedisAccessor
{
    public interface IEasyRedisClient
    {
        /// <summary>
        /// 返回一个CacheRedisClient
        /// </summary>
        ICacheRedisClient CacheRedisClient { get; }

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
        /// 获取一个值，如果值不存在即设置一个缓存并返回该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<T> GetOrAddAsync<T>(string key, Func<T> func);

        /// <summary>
        /// 获取一个值，如果值不存在即设置一个缓存并返回该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> func);

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
        /// 获取一个值，如果值不存在即设置一个缓存并返回该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiresAt">失效期</param>
        /// <returns></returns>
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> func, DateTimeOffset expiresAt);

        /// <summary>
        /// 获取一个值，如果值不存在即设置一个缓存并返回该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiresAt">失效期</param>
        /// <returns></returns>
        Task<T> GetOrAddAsync<T>(string key, Func<T> func, DateTimeOffset expiresAt);


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
        /// 获取一个值，如果值不存在即设置一个缓存并返回该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> func, TimeSpan expiresIn);

        /// <summary>
        /// 获取一个值，如果值不存在即设置一个缓存并返回该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        Task<T> GetOrAddAsync<T>(string key, Func<T> func, TimeSpan expiresIn);

        /// <summary>
        /// 获取一个值，如果值不存在即设置一个缓存并返回该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存Key</param>
        /// <param name="func"></param>
        /// <param name="expirySencod">缓存时间，单位秒</param>
        /// <returns></returns>
        Task<T> GetOrAddAsync<T>(string key, Func<T> func, int expirySencod);

        /// <summary>
        /// 替换一个键值，并设置有效期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn);

        /// <summary>
        /// 替换一个键值，保持原有的有效期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        Task<bool> ReplaceWithExpiryAsync<T>(string key, T value);

        /// <summary>
        /// 获取一组键的值的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys);
    }
}
