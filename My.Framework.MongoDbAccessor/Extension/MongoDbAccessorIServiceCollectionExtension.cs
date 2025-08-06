using Microsoft.Extensions.DependencyInjection;
using My.Framework.Foundation.Mongo;
using My.Framework.MongoDbAccessor.Interface;

namespace My.Framework.MongoDbAccessor.Extension
{
    public static class MongoDbAccessorIServiceCollectionExtension
    {
        public static IServiceCollection AddMongoDbAccessor(this IServiceCollection services, Action<List<MongoDbConfig>> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddSingleton<IMongoServiceContainer, MongoServiceContainer>();

            return services;
        }
    }
}
