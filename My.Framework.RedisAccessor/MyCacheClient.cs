using My.Framework.Foundation.Redis;
using My.Framework.Foundation.Serializer;
using My.Framework.RedisAccessor.Configuration;
using My.Framework.RedisAccessor.ServerIteration;
using StackExchange.Redis;

namespace My.Framework.RedisAccessor
{
    public class MyCacheClient : ICacheRedisClient
    {
        private static Lazy<ConnectionMultiplexer> _lazyConnection;
        private readonly ServerEnumerationStrategy _serverEnumerationStrategy;

        public MyCacheClient(IRedisCachingConfiguration redisCachingConfiguration) :
            this(redisCachingConfiguration.Serializer,
                redisCachingConfiguration.RedisConfig,
                redisCachingConfiguration.LogWriter)
        {
        }

        public MyCacheClient(ISerializer serializer, RedisConfig redisConfig, TextWriter logWriter)
        {
            RedisConfig = redisConfig ?? throw new ArgumentNullException(nameof(redisConfig));

            _serverEnumerationStrategy = redisConfig.ServerEnumerationStrategy ?? new ServerEnumerationStrategy();

            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            _lazyConnection =
                new Lazy<ConnectionMultiplexer>(() =>
                {
                    var connection = ConnectionMultiplexer.Connect(RedisConfig.RedisConnect, logWriter);
                    connection.PreserveAsyncOrder = false;
                    return connection;
                });
        }

        private static ConnectionMultiplexer ConnectionMultiplexer => _lazyConnection.Value;

        public RedisConfig RedisConfig { get; }

        public IConnectionMultiplexer Connection => ConnectionMultiplexer;

        public ISerializer Serializer { get; }

        public IDatabase Database => ConnectionMultiplexer.GetDatabase(RedisConfig.Database);

        public bool Exists(string key)
        {
            return Database.KeyExists(FormatKey(key));
        }

        public Task<bool> ExistsAsync(string key)
        {
            if (RedisConfig.CloseRedis)
                return Task.FromResult(false);

            return Database.KeyExistsAsync(FormatKey(key));
        }

        public bool Remove(string key)
        {
            if (RedisConfig.CloseRedis)
                return false;

            return Database.KeyDelete(FormatKey(key));
        }

        public Task<bool> RemoveAsync(string key)
        {
            if (RedisConfig.CloseRedis)
                return Task.FromResult(false);

            return Database.KeyDeleteAsync(FormatKey(key));
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            if (RedisConfig.CloseRedis)
                return;
            var redisKeys = keys.Select(x => (RedisKey)FormatKey(x)).ToArray();
            Database.KeyDelete(redisKeys);
        }

        public Task RemoveAllAsync(IEnumerable<string> keys)
        {
            if (RedisConfig.CloseRedis)
                return Task.FromResult(0);

            var redisKeys = keys.Select(x => (RedisKey)FormatKey(x)).ToArray();
            return Database.KeyDeleteAsync(redisKeys);
        }

        public T Get<T>(string key)
        {
            if (RedisConfig.CloseRedis)
                return default(T);

            var valueBytes = Database.StringGet(FormatKey(key));

            if (!valueBytes.HasValue)
                return default(T);

            return Serializer.Deserialize<T>(valueBytes);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            if (RedisConfig.CloseRedis)
                return default(T);

            var valueBytes = await Database.StringGetAsync(FormatKey(key));

            if (!valueBytes.HasValue)
                return default(T);

            return await Serializer.DeserializeAsync<T>(valueBytes);
        }

        public bool Add<T>(string key, T value)
        {
            if (RedisConfig.CloseRedis)
                return false;

            var entryBytes = Serializer.Serialize(value);

            return Database.StringSet(FormatKey(key), entryBytes);
        }

        public async Task<bool> AddAsync<T>(string key, T value)
        {
            if (RedisConfig.CloseRedis)
                return false;

            var entryBytes = await Serializer.SerializeAsync(value);

            return await Database.StringSetAsync(FormatKey(key), entryBytes);
        }

        public bool Replace<T>(string key, T value)
        {
            if (RedisConfig.CloseRedis)
                return false;

            return Add(FormatKey(key), value);
        }

        public Task<bool> ReplaceAsync<T>(string key, T value)
        {
            if (RedisConfig.CloseRedis)
                return Task.FromResult(false);

            return AddAsync(FormatKey(key), value);
        }

        public bool Add<T>(string key, T value, DateTimeOffset expiresAt)
        {
            if (RedisConfig.CloseRedis)
                return false;

            var entryBytes = Serializer.Serialize(value);
            var expiration = expiresAt.Subtract(DateTimeOffset.Now);

            return Database.StringSet(FormatKey(key), entryBytes, expiration);
        }

        public async Task<bool> AddAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            if (RedisConfig.CloseRedis)
                return false;

            var entryBytes = await Serializer.SerializeAsync(value);
            var expiration = expiresAt.Subtract(DateTimeOffset.Now);

            return await Database.StringSetAsync(FormatKey(key), entryBytes, expiration);
        }

        public bool Replace<T>(string key, T value, DateTimeOffset expiresAt)
        {
            if (RedisConfig.CloseRedis)
                return false;

            return Add(FormatKey(key), value, expiresAt);
        }

        public Task<bool> ReplaceAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            if (RedisConfig.CloseRedis)
                return Task.FromResult(false);

            return AddAsync(FormatKey(key), value, expiresAt);
        }

        public bool Add<T>(string key, T value, TimeSpan expiresIn)
        {
            if (RedisConfig.CloseRedis)
                return false;

            var entryBytes = Serializer.Serialize(value);

            return Database.StringSet(FormatKey(key), entryBytes, expiresIn);
        }

        public async Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            if (RedisConfig.CloseRedis)
                return false;

            var entryBytes = await Serializer.SerializeAsync(value);

            return await Database.StringSetAsync(FormatKey(key), entryBytes, expiresIn);
        }

        public bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            if (RedisConfig.CloseRedis)
                return false;

            return Add(FormatKey(key), value, expiresIn);
        }

        public Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            if (RedisConfig.CloseRedis)
                return Task.FromResult(false);

            return AddAsync(FormatKey(key), value, expiresIn);
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            if (RedisConfig.CloseRedis)
                return default(IDictionary<string, T>);

            var redisKeys = keys.Select(x => (RedisKey)FormatKey(x)).ToArray();
            var result = Database.StringGet(redisKeys);

            var dict = new Dictionary<string, T>(StringComparer.Ordinal);
            for (var index = 0; index < redisKeys.Length; index++)
            {
                var value = result[index];
                dict.Add(ReFormatKey(redisKeys[index]), value == RedisValue.Null ? default(T) : Serializer.Deserialize<T>(value));
            }

            return dict;
        }

        public async Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            if (RedisConfig.CloseRedis)
                return default(IDictionary<string, T>);

            var redisKeys = keys.Select(x => (RedisKey)FormatKey(x)).ToArray();
            var result = await Database.StringGetAsync(redisKeys);
            var dict = new Dictionary<string, T>(StringComparer.Ordinal);
            for (var index = 0; index < redisKeys.Length; index++)
            {
                var value = result[index];
                dict.Add(ReFormatKey(redisKeys[index]), value == RedisValue.Null ? default(T) : Serializer.Deserialize<T>(value));
            }
            return dict;
        }

        public bool AddAll<T>(IList<Tuple<string, T>> items)
        {
            if (RedisConfig.CloseRedis)
                return false;

            var values = items
                .Select(item => new KeyValuePair<RedisKey, RedisValue>(item.Item1, Serializer.Serialize(item.Item2)))
                .ToArray();

            return Database.StringSet(values);
        }

        public async Task<bool> AddAllAsync<T>(IList<Tuple<string, T>> items)
        {
            if (RedisConfig.CloseRedis)
                return false;

            var values = items
                .Select(item => new KeyValuePair<RedisKey, RedisValue>(item.Item1, Serializer.Serialize(item.Item2)))
                .ToArray();

            return await Database.StringSetAsync(values);
        }

        public bool SetAdd<T>(string key, T item) where T : class
        {
            if (RedisConfig.CloseRedis)
                return false;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedObject = Serializer.Serialize(item);

            return Database.SetAdd(FormatKey(key), serializedObject);
        }

        public async Task<bool> SetAddAsync<T>(string key, T item) where T : class
        {
            if (RedisConfig.CloseRedis)
                return false;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedObject = await Serializer.SerializeAsync(item);

            return await Database.SetAddAsync(FormatKey(key), serializedObject);
        }

        public long SetAddAll<T>(string key, params T[] items) where T : class
        {
            if (RedisConfig.CloseRedis)
                return 0;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (items == null)
                throw new ArgumentNullException(nameof(items), "items cannot be null.");

            if (items.Any(item => item == null))
                throw new ArgumentException("items cannot contains any null item.", nameof(items));

            return Database.SetAdd(FormatKey(key),
                items.Select(item => Serializer.Serialize(item)).Select(x => (RedisValue)x).ToArray());
        }

        public async Task<long> SetAddAllAsync<T>(string key, params T[] items) where T : class
        {
            if (RedisConfig.CloseRedis)
                return 0;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (items == null)
                throw new ArgumentNullException(nameof(items), "items cannot be null.");

            if (items.Any(item => item == null))
                throw new ArgumentException("items cannot contains any null item.", nameof(items));

            return await Database.SetAddAsync(FormatKey(key),
                items.Select(item => Serializer.Serialize(item)).Select(x => (RedisValue)x).ToArray());
        }

        public bool SetRemove<T>(string key, T item) where T : class
        {
            if (RedisConfig.CloseRedis)
                return false;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedObject = Serializer.Serialize(item);

            return Database.SetRemove(FormatKey(key), serializedObject);
        }

        public async Task<bool> SetRemoveAsync<T>(string key, T item) where T : class
        {
            if (RedisConfig.CloseRedis)
                return false;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedObject = await Serializer.SerializeAsync(item);

            return await Database.SetRemoveAsync(FormatKey(key), serializedObject);
        }

        public long SetRemoveAll<T>(string key, params T[] items) where T : class
        {
            if (RedisConfig.CloseRedis)
                return 0;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (items == null)
                throw new ArgumentNullException(nameof(items), "items cannot be null.");

            if (items.Any(item => item == null))
                throw new ArgumentException("items cannot contains any null item.", nameof(items));

            return Database.SetRemove(FormatKey(key),
                items.Select(item => Serializer.Serialize(item)).Select(x => (RedisValue)x).ToArray());
        }

        public async Task<long> SetRemoveAllAsync<T>(string key, params T[] items) where T : class
        {
            if (RedisConfig.CloseRedis)
                return 0;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (items == null)
                throw new ArgumentNullException(nameof(items), "items cannot be null.");

            if (items.Any(item => item == null))
                throw new ArgumentException("items cannot contains any null item.", nameof(items));

            return await Database.SetRemoveAsync(FormatKey(key),
                items.Select(item => Serializer.Serialize(item)).Select(x => (RedisValue)x).ToArray());
        }

        public string[] SetMember(string memberName)
        {
            if (RedisConfig.CloseRedis)
                return default(string[]);

            return Database.SetMembers(memberName).Select(x => x.ToString()).ToArray();
        }

        public async Task<string[]> SetMemberAsync(string memberName)
        {
            if (RedisConfig.CloseRedis)
                return default(string[]);

            return (await Database.SetMembersAsync(memberName)).Select(x => x.ToString()).ToArray();
        }

        public IEnumerable<T> SetMembers<T>(string key)
        {
            if (RedisConfig.CloseRedis)
                return default(IEnumerable<T>);

            var members = Database.SetMembers(key);
            return members.Select(m => m == RedisValue.Null ? default(T) : Serializer.Deserialize<T>(m));
        }

        public async Task<IEnumerable<T>> SetMembersAsync<T>(string key)
        {
            if (RedisConfig.CloseRedis)
                return default(IEnumerable<T>);

            var members = await Database.SetMembersAsync(key);

            return members.Select(m => m == RedisValue.Null ? default(T) : Serializer.Deserialize<T>(m));
        }

        public IEnumerable<string> SearchKeys(string pattern)
        {
            var keys = new HashSet<RedisKey>();

            var multiplexer = Database.Multiplexer;
            var servers = ServerIteratorFactory.GetServers((ConnectionMultiplexer)multiplexer, _serverEnumerationStrategy).ToArray();
            if (!servers.Any())
                throw new Exception("No server found to serve the KEYS command.");

            foreach (var server in servers)
            {
                var dbKeys = server.Keys(Database.Database, pattern);
                foreach (var dbKey in dbKeys)
                    if (!keys.Contains(dbKey))
                        keys.Add(dbKey);
            }

            return keys.Select(x => (string)x);
        }

        public Task<IEnumerable<string>> SearchKeysAsync(string pattern)
        {
            return Task.Factory.StartNew(() => SearchKeys(pattern));
        }

        public void FlushDb()
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            foreach (var endpoint in endPoints)
                Database.Multiplexer.GetServer(endpoint).FlushDatabase(Database.Database);
        }

        public async Task FlushDbAsync()
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            foreach (var endpoint in endPoints)
                await Database.Multiplexer.GetServer(endpoint).FlushDatabaseAsync(Database.Database);
        }

        public void Save(SaveType saveType)
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            foreach (var endpoint in endPoints)
                Database.Multiplexer.GetServer(endpoint).Save(saveType);
        }

        public async Task SaveAsync(SaveType saveType)
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            foreach (var endpoint in endPoints)
                await Database.Multiplexer.GetServer(endpoint).SaveAsync(saveType);
        }

        public Dictionary<string, string> GetInfo()
        {
            var info = Database.ScriptEvaluate("return redis.call('INFO')").ToString();

            return ParseInfo(info);
        }

        public async Task<Dictionary<string, string>> GetInfoAsync()
        {
            var info = (await Database.ScriptEvaluateAsync("return redis.call('INFO')")).ToString();

            return ParseInfo(info);
        }

        public long Publish<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
        {
            var sub = ConnectionMultiplexer.GetSubscriber();
            return sub.Publish(channel, Serializer.Serialize(message), flags);
        }

        public async Task<long> PublishAsync<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
        {
            var sub = ConnectionMultiplexer.GetSubscriber();
            return await sub.PublishAsync(channel, await Serializer.SerializeAsync(message), flags);
        }

        public void Subscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = ConnectionMultiplexer.GetSubscriber();
            sub.Subscribe(channel, (redisChannel, value) => handler(Serializer.Deserialize<T>(value)), flags);
        }

        public async Task SubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler,
            CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = ConnectionMultiplexer.GetSubscriber();
            await sub.SubscribeAsync(channel,
                async (redisChannel, value) => await handler(Serializer.Deserialize<T>(value)), flags);
        }

        public void Unsubscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = ConnectionMultiplexer.GetSubscriber();
            sub.Unsubscribe(channel, (redisChannel, value) => handler(Serializer.Deserialize<T>(value)), flags);
        }

        public async Task UnsubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler,
            CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = ConnectionMultiplexer.GetSubscriber();
            await sub.UnsubscribeAsync(channel, (redisChannel, value) => handler(Serializer.Deserialize<T>(value)),
                flags);
        }

        public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
        {
            var sub = ConnectionMultiplexer.GetSubscriber();
            sub.UnsubscribeAll(flags);
        }

        public async Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
        {
            var sub = ConnectionMultiplexer.GetSubscriber();
            await sub.UnsubscribeAllAsync(flags);
        }

        public long ListLeftPush<T>(string key, T item) where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedItem = Serializer.Serialize(item);

            return Database.ListLeftPush(FormatKey(key), serializedItem);
        }

        public async Task<long> ListLeftPushAsync<T>(string key, T item) where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedItem = await Serializer.SerializeAsync(item);

            return await Database.ListLeftPushAsync(FormatKey(key), serializedItem);
        }

        public T ListRightPop<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            var item = Database.ListRightPop(FormatKey(key));

            return item == RedisValue.Null ? null : Serializer.Deserialize<T>(item);
        }

        public async Task<T> ListRightPopAsync<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            var item = await Database.ListRightPopAsync(FormatKey(key));

            if (item == RedisValue.Null) return null;

            return item == RedisValue.Null ? null : await Serializer.DeserializeAsync<T>(item);
        }

        public bool HashDelete(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashDelete(FormatKey(hashKey), key, commandFlags);
        }

        public long HashDelete(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashDelete(FormatKey(hashKey), keys.Select(x => (RedisValue)x).ToArray(), commandFlags);
        }

        public bool HashExists(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashExists(FormatKey(hashKey), key, commandFlags);
        }

        public T HashGet<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisValue = Database.HashGet(FormatKey(hashKey), key, commandFlags);
            return redisValue.HasValue ? Serializer.Deserialize<T>(redisValue) : default(T);
        }

        public Dictionary<string, T> HashGet<T>(string hashKey, IEnumerable<string> keys,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return keys.Select(x => new { key = x, value = HashGet<T>(FormatKey(hashKey), x, commandFlags) })
                .ToDictionary(kv => kv.key, kv => kv.value, StringComparer.Ordinal);
        }

        public Dictionary<string, T> HashGetAll<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database
                .HashGetAll(hashKey, commandFlags)
                .ToDictionary(
                    x => x.Name.ToString(),
                    x => Serializer.Deserialize<T>(x.Value),
                    StringComparer.Ordinal);
        }

        public long HashIncerementBy(string hashKey, string key, long value,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashIncrement(FormatKey(hashKey), key, value, commandFlags);
        }

        public double HashIncerementBy(string hashKey, string key, double value,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashIncrement(FormatKey(hashKey), key, value, commandFlags);
        }

        public IEnumerable<string> HashKeys(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashKeys(FormatKey(hashKey), commandFlags).Select(x => x.ToString());
        }

        public long HashLength(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashLength(FormatKey(hashKey), commandFlags);
        }

        public bool HashSet<T>(string hashKey, string key, T value, bool nx = false,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashSet(FormatKey(hashKey), key, Serializer.Serialize(value), nx ? When.NotExists : When.Always,
                commandFlags);
        }

        public void HashSet<T>(string hashKey, Dictionary<string, T> values,
            CommandFlags commandFlags = CommandFlags.None)
        {
            var entries = values.Select(kv => new HashEntry(kv.Key, Serializer.Serialize(kv.Value)));
            Database.HashSet(FormatKey(hashKey), entries.ToArray(), commandFlags);
        }

        public IEnumerable<T> HashValues<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashValues(FormatKey(hashKey), commandFlags).Select(x => Serializer.Deserialize<T>(x));
        }

        public Dictionary<string, T> HashScan<T>(string hashKey, string pattern, int pageSize = 10,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return Database
                .HashScan(FormatKey(hashKey), pattern, pageSize, commandFlags)
                .ToDictionary(x => x.Name.ToString(),
                    x => Serializer.Deserialize<T>(x.Value),
                    StringComparer.Ordinal);
        }

        public async Task<bool> HashDeleteAsync(string hashKey, string key,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashDeleteAsync(FormatKey(hashKey), key, commandFlags);
        }

        public async Task<long> HashDeleteAsync(string hashKey, IEnumerable<string> keys,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashDeleteAsync(FormatKey(hashKey), keys.Select(x => (RedisValue)x).ToArray(), commandFlags);
        }

        public async Task<bool> HashExistsAsync(string hashKey, string key,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashExistsAsync(FormatKey(hashKey), key, commandFlags);
        }

        public async Task<T> HashGetAsync<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisValue = await Database.HashGetAsync(FormatKey(hashKey), key, commandFlags);
            return redisValue.HasValue ? Serializer.Deserialize<T>(redisValue) : default(T);
        }

        public async Task<Dictionary<string, T>> HashGetAsync<T>(string hashKey, IEnumerable<string> keys,
            CommandFlags commandFlags = CommandFlags.None)
        {
            var result = new Dictionary<string, T>();
            foreach (var key in keys)
            {
                var value = await HashGetAsync<T>(FormatKey(hashKey), key, commandFlags);

                result.Add(key, value);
            }

            return result;
        }

        public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string hashKey,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return (await Database
                    .HashGetAllAsync(FormatKey(hashKey), commandFlags))
                .ToDictionary(
                    x => x.Name.ToString(),
                    x => Serializer.Deserialize<T>(x.Value),
                    StringComparer.Ordinal);
        }

        public async Task<long> HashIncerementByAsync(string hashKey, string key, long value,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashIncrementAsync(FormatKey(hashKey), key, value, commandFlags);
        }

        public async Task<double> HashIncerementByAsync(string hashKey, string key, double value,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashIncrementAsync(FormatKey(hashKey), key, value, commandFlags);
        }

        public async Task<IEnumerable<string>> HashKeysAsync(string hashKey,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return (await Database.HashKeysAsync(FormatKey(hashKey), commandFlags)).Select(x => x.ToString());
        }

        public async Task<long> HashLengthAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashLengthAsync(FormatKey(hashKey), commandFlags);
        }

        public async Task<bool> HashSetAsync<T>(string hashKey, string key, T value, bool nx = false,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashSetAsync(FormatKey(hashKey), key, Serializer.Serialize(value),
                nx ? When.NotExists : When.Always, commandFlags);
        }

        public async Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values,
            CommandFlags commandFlags = CommandFlags.None)
        {
            var entries = values.Select(kv => new HashEntry(kv.Key, Serializer.Serialize(kv.Value)));
            await Database.HashSetAsync(FormatKey(hashKey), entries.ToArray(), commandFlags);
        }

        public async Task<IEnumerable<T>> HashValuesAsync<T>(string hashKey,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return (await Database.HashValuesAsync(FormatKey(hashKey), commandFlags)).Select(x => Serializer.Deserialize<T>(x));
        }

        public async Task<Dictionary<string, T>> HashScanAsync<T>(string hashKey, string pattern, int pageSize = 10,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return (await Task.Run(() => Database.HashScan(FormatKey(hashKey), pattern, pageSize, commandFlags)))
                .ToDictionary(x => x.Name.ToString(), x => Serializer.Deserialize<T>(x.Value), StringComparer.Ordinal);
        }

        public void Dispose()
        {
            ConnectionMultiplexer.Dispose();
        }

        #region 私有方法

        private Dictionary<string, string> ParseInfo(string info)
        {
            var lines = info.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var data = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line) || line[0] == '#')
                    continue;

                var idx = line.IndexOf(':');
                if (idx > 0) // double check this line looks about right
                {
                    var key = line.Substring(0, idx);
                    var infoValue = line.Substring(idx + 1).Trim();

                    data.Add(key, infoValue);
                }
            }

            return data;
        }

        /// <summary>
        /// 标准化字符串
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <returns>通过前缀标注好缓存Key</returns>
        private string FormatKey(string key)
        {
            return IsPrefixEmpty() ? key : $"{RedisConfig.RedisKeyPrefix}*{key}";
        }

        private bool IsPrefixEmpty() => string.IsNullOrWhiteSpace(RedisConfig.RedisKeyPrefix);


        /// <summary>
        /// 标准化字符串逆向操作
        /// </summary>
        /// <param name="prefixKey">标准化后的缓存key</param>
        /// <returns>标准化字符串逆向操作后的结果</returns>
        private string ReFormatKey(string prefixKey)
        {
            if (IsPrefixEmpty()) return prefixKey;
            var formatPre = RedisConfig.RedisKeyPrefix + "*";
            return prefixKey.StartsWith(formatPre) ? prefixKey.Substring(formatPre.Length) : prefixKey;
        }

        #endregion
    }
}
