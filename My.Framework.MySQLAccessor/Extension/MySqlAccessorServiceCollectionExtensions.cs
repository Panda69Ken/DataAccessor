using Microsoft.Extensions.DependencyInjection;
using My.Framework.Foundation.MySql;

namespace My.Framework.MySQLAccessor
{
    public static class MySqlAccessorServiceCollectionExtensions
    {
        public static IServiceCollection AddScopedMySqlDbContext(this IServiceCollection services, Action<List<MySqlConnectionConfig>> config)
        {
            services.Configure(config);

            services.AddSingleton<IContextContainer, ContextContainer>();

            return services;
        }
    }
}
