using Microsoft.Extensions.DependencyInjection;
using My.Framework.Foundation.Redis;
using My.Framework.Foundation.Serializer;
using My.Framework.RedisAccessor.Configuration;
using My.Framework.RedisAccessor.NumberCreater;
using My.Framework.RedisAccessor.Serializer;

namespace My.Framework.RedisAccessor
{
    public static class CacheRedisServiceCollectionExtensions
    {
        public static IServiceCollection AddSingletonCacheRedis(this IServiceCollection services,
            Action<RedisConfig> redisConfigOptions, ISerializer serializer = null)
        {
            AddCacheRedis(services, redisConfigOptions, serializer);

            services.AddSingleton<ICacheRedisClient, MyCacheClient>();
            services.AddSingleton<IEasyRedisClient, EasyRedisClient>();

            return services;
        }

        private static void AddCacheRedis(IServiceCollection services,
            Action<RedisConfig> redisConfigOptions, ISerializer serializer)
        {
            if (redisConfigOptions == null)
            {
                throw new ArgumentNullException(nameof(redisConfigOptions));
            }

            services.Configure(redisConfigOptions);

            if (serializer == null)
            {
                services.AddSingleton<ISerializer, NewtonsoftSerializer>();
            }
            else
            {
                services.AddSingleton(s => serializer);
            }

            services.AddSingleton<IRedisCachingConfiguration, RedisCachingConfig>();

            services.AddSingleton<INumberCreater>(f => new NumberRedisCreater(f.GetService<ICacheRedisClient>()));
        }
    }
}
