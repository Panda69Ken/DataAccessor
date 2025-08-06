using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace My.Framework.RedisAccessor
{
    public class EasyRedisClient : IEasyRedisClient
    {
        public ICacheRedisClient CacheRedisClient { get; }

        private readonly ILogger<EasyRedisClient> _logger;

        public EasyRedisClient(ICacheRedisClient cacheRedisClient, ILogger<EasyRedisClient> logger)
        {
            CacheRedisClient = cacheRedisClient;
            _logger = logger;
        }

        public async Task<bool> AddAsync<T>(string key, T value)
        {
            try
            {
                return await CacheRedisClient.AddAsync(key, value);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return false;
            }
        }

        public async Task<bool> AddAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            try
            {
                return await CacheRedisClient.AddAsync(key, value, expiresAt);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return false;
            }
        }

        public async Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            try
            {
                return await CacheRedisClient.AddAsync(key, value, expiresIn);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return false;
            }
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> func, TimeSpan expiresIn)
        {
            try
            {
                if (CacheRedisClient.RedisConfig.CloseRedis)
                    return await func.Invoke();

                var valueBytes = await CacheRedisClient.Database.StringGetAsync(FormatKey(key));

                if (valueBytes.HasValue)
                    return await CacheRedisClient.Serializer.DeserializeAsync<T>(valueBytes);

                var value = await func.Invoke();
                if (value != null)
                {
                    var entryBytes = await CacheRedisClient.Serializer.SerializeAsync(value);

                    await CacheRedisClient.Database.StringSetAsync(FormatKey(key), entryBytes, expiresIn);
                }
                return value;
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return await func.Invoke();
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return await func.Invoke();
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await CacheRedisClient.ExistsAsync(key);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "判断缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return false;
            }
        }

        public async Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            try
            {
                return await CacheRedisClient.GetAllAsync<T>(keys);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "获取缓存值集合异常");

                return default(IDictionary<string, T>);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return default(IDictionary<string, T>);
            }
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> func)
        {
            try
            {
                if (CacheRedisClient.RedisConfig.CloseRedis)
                    return await func.Invoke();

                var valueBytes = await CacheRedisClient.Database.StringGetAsync(FormatKey(key));

                if (valueBytes.HasValue)
                    return await CacheRedisClient.Serializer.DeserializeAsync<T>(valueBytes);

                var value = await func.Invoke();
                if (value != null)
                {
                    var entryBytes = await CacheRedisClient.Serializer.SerializeAsync(value);

                    await CacheRedisClient.Database.StringSetAsync(FormatKey(key), entryBytes);
                }
                return value;
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return await func.Invoke();
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return await func.Invoke();
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                return await CacheRedisClient.GetAsync<T>(key);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "获取缓存值异常");

                return default(T);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return default(T);
            }
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<T> func)
        {
            try
            {
                if (CacheRedisClient.RedisConfig.CloseRedis)
                    return func.Invoke();

                var valueBytes = await CacheRedisClient.Database.StringGetAsync(FormatKey(key));

                if (valueBytes.HasValue)
                    return await CacheRedisClient.Serializer.DeserializeAsync<T>(valueBytes);

                var value = func.Invoke();
                if (value != null)
                {
                    var entryBytes = await CacheRedisClient.Serializer.SerializeAsync(value);

                    await CacheRedisClient.Database.StringSetAsync(FormatKey(key), entryBytes);
                }
                return value;
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return func.Invoke();
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return func.Invoke();
            }
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> func, DateTimeOffset expiresAt)
        {
            try
            {
                if (CacheRedisClient.RedisConfig.CloseRedis)
                    return await func.Invoke();

                var valueBytes = await CacheRedisClient.Database.StringGetAsync(FormatKey(key));

                if (valueBytes.HasValue)
                    return await CacheRedisClient.Serializer.DeserializeAsync<T>(valueBytes);

                var value = await func.Invoke();
                if (value != null)
                {
                    var entryBytes = await CacheRedisClient.Serializer.SerializeAsync(value);
                    var expiration = expiresAt.Subtract(DateTimeOffset.Now);

                    if (expiration.TotalMilliseconds > 0)
                    {
                        await CacheRedisClient.Database.StringSetAsync(FormatKey(key), entryBytes, expiration);
                    }
                }
                return value;

            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return await func.Invoke();
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return await func.Invoke();
            }
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<T> func, DateTimeOffset expiresAt)
        {
            try
            {
                if (CacheRedisClient.RedisConfig.CloseRedis)
                    return func.Invoke();

                var valueBytes = await CacheRedisClient.Database.StringGetAsync(FormatKey(key));

                if (valueBytes.HasValue)
                    return await CacheRedisClient.Serializer.DeserializeAsync<T>(valueBytes);

                var value = func.Invoke();
                if (value != null)
                {
                    var entryBytes = await CacheRedisClient.Serializer.SerializeAsync(value);
                    var expiration = expiresAt.Subtract(DateTimeOffset.Now);

                    if (expiration.TotalMilliseconds > 0)
                    {
                        await CacheRedisClient.Database.StringSetAsync(FormatKey(key), entryBytes, expiration);
                    }
                }
                return value;

            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return func.Invoke();
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return func.Invoke();
            }
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<T> func, TimeSpan expiresIn)
        {
            try
            {
                if (CacheRedisClient.RedisConfig.CloseRedis)
                    return func.Invoke();

                var valueBytes = await CacheRedisClient.Database.StringGetAsync(FormatKey(key));

                if (valueBytes.HasValue)
                    return await CacheRedisClient.Serializer.DeserializeAsync<T>(valueBytes);

                var value = func.Invoke();
                if (value != null)
                {
                    var entryBytes = await CacheRedisClient.Serializer.SerializeAsync(value);

                    await CacheRedisClient.Database.StringSetAsync(FormatKey(key), entryBytes, expiresIn);
                }
                return value;
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return func.Invoke();
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return func.Invoke();
            }
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<T> func, int expirySencod)
        {
            return await GetOrAddAsync<T>(key, func, TimeSpan.FromSeconds(expirySencod));
        }

        public async Task RemoveAllAsync(IEnumerable<string> keys)
        {
            try
            {
                await CacheRedisClient.RemoveAllAsync(keys);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "移除缓存值异常");
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "移除缓存值异常");
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            try
            {
                return await CacheRedisClient.RemoveAsync(key);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "移除缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "移除缓存值异常");

                return false;
            }
        }

        public async Task<bool> ReplaceAsync<T>(string key, T value)
        {
            try
            {
                return await CacheRedisClient.ReplaceAsync(key, value);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "替换缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "替换缓存值异常");

                return false;
            }
        }

        public async Task<bool> ReplaceAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            try
            {
                return await CacheRedisClient.ReplaceAsync(key, value, expiresAt);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "替换缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "替换缓存值异常");

                return false;
            }
        }

        public async Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            try
            {
                return await CacheRedisClient.ReplaceAsync(key, value, expiresIn);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "替换缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "替换缓存值异常");

                return false;
            }
        }

        public async Task<bool> ReplaceWithExpiryAsync<T>(string key, T value)
        {
            try
            {
                var expiry = await CacheRedisClient.Database.StringGetWithExpiryAsync(key);

                if (expiry.Expiry != null)
                    await CacheRedisClient.ReplaceAsync(key, value, expiry.Expiry.Value);

                return true;
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "替换缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "替换缓存值异常");

                return false;
            }
        }

        /// <summary>
        /// 标准化字符串
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <returns>通过前缀标注好缓存Key</returns>
        private string FormatKey(string key)
        {
            return IsPrefixEmpty() ? key : $"{CacheRedisClient.RedisConfig.RedisKeyPrefix}*{key}";
        }

        private bool IsPrefixEmpty() => string.IsNullOrWhiteSpace(CacheRedisClient.RedisConfig.RedisKeyPrefix);


    }
}
