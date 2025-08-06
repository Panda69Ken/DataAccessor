using My.Framework.Foundation;
using My.Framework.Foundation.Mongo;
using My.Framework.Foundation.MySql;
using My.Framework.Foundation.Redis;
using My.Framework.Logging;
using My.Framework.Logging.Extension;
using My.Framework.MongoDbAccessor.Extension;
using My.Framework.MySQLAccessor;
using My.Framework.RedisAccessor;
using My.Framework.RedisAccessor.Serializer;
using MyTest.Core;
using MyTest.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .ConfigureAppConfiguration((context, builder) =>
    {
        if (context.HostingEnvironment.IsDevelopment())
        {
            builder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json");
        }
        else
        {
            builder.AddJsonFile("appsettings.json");
        }
    })
    .ConfigureLogging((context, builder) =>
    {
        var config = context.Configuration.GetSection("NLog").Get<NlogConfig>();
        builder.ConfigNLog(config);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddLogging();

        services.AddScopedMySqlDbContext(config =>
        {
            var mySqlConfig = context.Configuration.GetSection("MySqls").Get<List<MySqlConnectionConfig>>();
            config.AddRange(mySqlConfig);
        });

        services.AddSingletonCacheRedis(config =>
        {
            var redisConfig = context.Configuration.GetSection("RedisConfig").Get<RedisConfig>();
            config.Database = redisConfig.Database;
            config.RedisConnect = redisConfig.RedisConnect;
            config.CloseRedis = redisConfig.CloseRedis;
            config.HaveLog = redisConfig.HaveLog;
        }, new NewtonsoftSerializer(new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        }));

        services.AddMongoDbAccessor(config =>
        {
            var mongoConfigs = context.Configuration.GetSection("Mongos").Get<List<MongoDbConfig>>();
            config.AddRange(mongoConfigs);
        });

    });

builder.Services.AddScoped<UserRepository>();

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
