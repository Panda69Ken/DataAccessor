using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using My.Framework.Foundation;
using My.Framework.Logging;
using My.Framework.MongoDbAccessor.Interface;
using My.Framework.RedisAccessor;
using MyTest.Core;
using MyTest.Model;

namespace MyTest.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly UserRepository _userRepository;
        private readonly IEasyRedisClient _redis;
        private readonly IMongoService _mongo;
        public GreeterService(ILogger<GreeterService> logger, UserRepository userRepository,
            IEasyRedisClient redis, IMongoServiceContainer container)
        {
            _logger = logger;
            _userRepository = userRepository;
            _redis = redis;

            _mongo = container.GetService("user");
        }

        public override async Task<HelloReply> DataOperation(Empty request, ServerCallContext context)
        {
            var aff = await _userRepository.InsertAsync(new UserEntity
            {
                UserName = $"user:{DateTime.UtcNow.D2L()}",
                Password = "123123"
            });

            aff = await _userRepository.UpdateAsync(new UserEntity
            {
                Id = 1,
                UserName = $"user:{DateTime.UtcNow.D2L()}",
                Password = "123123"
            });

            var userName = $"user:{DateTime.UtcNow.D2L()}";
            aff = await _userRepository.UpdateAsync(a => a.Id == 1, b => new UserEntity
            {
                UserName = userName,    //注意这里的UserName是一个表达式，不能直接赋值
                Password = "123456"
            }) > 0;

            var member = await _userRepository.GetAsync(1);

            var member1 = await _userRepository.FirstAsync(a => a.Id == 1);

            var members = await _userRepository.WhereAsync(a => a.Id == 1);

            var count = await _userRepository.CountAsync(a => a.Id > 0);

            aff = await _userRepository.DeleteAsync(member);

            aff = await _userRepository.DeleteAsync(a => a.Id == 1);

            //执行sql语句，更新数据
            aff = await _userRepository.ExecuteAsync("UPDATE user SET user_name=@userName WHERE id=@Id;",
                new { id = 1, userName = $"user:{DateTime.UtcNow.D2L()}" }) > 0;

            //执行sql语句，返回聚合
            members = await _userRepository.QueryAsync("SELECT * FROM user WHERE id>@Id;", new { id = 0 });

            //执行sql语句，返回单个值
            var userName1 = await _userRepository.ExecuteScalarAsync("SELECT user_name FROM user WHERE id=@Id;", new { id = 1 });

            //执行sql语句，返回实体
            member = await _userRepository.QuerySingleAsync<UserEntity>("SELECT * FROM user WHERE id>@Id ORDER BY id DESC LIMIT 0, 1;",
                new { id = 0 });

            //执行多个sql语句
            var sql = @"SELECT COUNT(*) FROM user WHERE id>@Id; 
                                SELECT * FROM user WHERE id>@Id ORDER BY id DESC LIMIT 0, 10;";
            var result = await _userRepository.QueryMultipleAsync(sql, new { id = 0 });

            //批量添加数据
            var items = new List<UserEntity>();
            for (int i = 0; i < 100; i++)
            {
                items.Add(new UserEntity
                {
                    UserName = $"user:{DateTime.UtcNow.D2L()}_{i}",
                    Password = $"123123_{i}"
                });
            }
            aff = await _userRepository.BulkInsert(items) > 0;

            //批量更新数据
            for (int i = 41; i <= 50; i++)
            {
                items.Add(new UserEntity
                {
                    Id = i,
                    UserName = $"user:{DateTime.UtcNow.D2L()}_{i}",
                    Password = $"123123_{i}"
                });
            }
            aff = await _userRepository.BulkUpdateAsync(items) > 0;

            //事务操作
            aff = _userRepository.UnitOfWork(() =>
            {
                var aff11 = _userRepository.InsertAsync(new UserEntity
                {
                    UserName = $"user:{DateTime.UtcNow.D2L()}",
                    Password = "123123"
                }).Result;

                if (aff11 == false)
                    throw new ArgumentException("添加数据失败");

                aff11 = _userRepository.UpdateAsync(new UserEntity
                {
                    Id = 100,
                    UserName = $"user:{DateTime.UtcNow.D2L()}",
                    Password = "123123"
                }).Result;

                if (aff11 == false)
                    throw new ArgumentException("更新数据失败");
            });

            return new HelloReply
            {
                Message = "Hello "
            };
        }

        public override async Task<HelloReply> RedisOperation(Empty request, ServerCallContext context)
        {
            var key = "myTest:redisOperation";

            var time = 60;

            await _redis.AddAsync(key, DateTime.UtcNow.D2L(), DateTime.Now.AddSeconds(time));

            var exist = await _redis.ExistsAsync(key);

            var result = await _redis.ReplaceAsync(key, DateTime.UtcNow.D2L());

            result = await _redis.ReplaceWithExpiryAsync(key, DateTime.UtcNow.D2L());

            var entity = await _redis.GetAsync<long>(key);

            result = await _redis.RemoveAsync(key);

            var key1 = $"{key}_All";
            var result1 = await _redis.GetOrAddAsync(key1, () =>
            {
                return new { time = DateTime.UtcNow.D2L() };
            }, DateTime.Now.AddSeconds(time));


            var list = await _redis.GetAllAsync<Dictionary<string, object>>([key1]);

            var db = _redis.CacheRedisClient.Database;

            //自增加1
            var key12 = $"{key}:StringIncrement";
            var lastNumber = db.StringIncrement(key12);
            await db.KeyExpireAsync(key12, DateTime.Now.AddSeconds(2));

            //有序集合
            var key123 = $"{key}:Sorted";
            await db.SortedSetAddAsync(key123, 1, DateTime.UtcNow.D2L());
            await db.SortedSetAddAsync(key123, 2, DateTime.UtcNow.D2L());
            await db.SortedSetAddAsync(key123, 3, DateTime.UtcNow.D2L());

            var stores = await db.SortedSetRangeByScoreAsync(key123, 0, DateTime.UtcNow.D2L());
            //更多方法查看IDatabase接口

            await _redis.RemoveAllAsync([key1, key12, key123]);

            return new HelloReply
            {
                Message = "Hello "
            };
        }

        public override async Task<HelloReply> MongoOperation(Empty request, ServerCallContext context)
        {
            await _mongo.AddAsync(new DeviceLogDTO
            {
                ServiceId = 1000,
                SourceType = DeviceSourceEnum.Register,
                Value = "userId=1;userName=abcdefg",
                CreateTime = DateTime.UtcNow.D2L(),
                Uid = SGUID.GenerateComb().ToString(),
            });

            var items = new List<DeviceLogDTO>() {
                new() { ServiceId = 1000, SourceType = DeviceSourceEnum.Login, Value = "userId=2;userName=abcdefg2222", CreateTime = DateTime.UtcNow.D2L(), Uid = SGUID.GenerateComb().ToString() },
                new() { ServiceId = 1000, SourceType = DeviceSourceEnum.Password, Value = "userId=3;userName=abcdefg3333", CreateTime = DateTime.UtcNow.D2L(), Uid = SGUID.GenerateComb().ToString() }
            };
            await _mongo.AddRangeAsync(items);

            await _mongo.UpdateAsync(() => new DeviceLogDTO()
            {
                Value = "userId=1;userName=abcdefg",
                CreateTime = DateTime.UtcNow.D2L()
            }, x => x.ServiceId == 1000 && x.SourceType == DeviceSourceEnum.Register);

            var uid = SGUID.GenerateComb().ToString();
            var entity = new DeviceLogDTO
            {
                ServiceId = 1000,
                SourceType = DeviceSourceEnum.None,
                Value = "userId=4;userName=abcdefg444",
                CreateTime = DateTime.UtcNow.D2L(),
                Uid = uid,
            };
            await _mongo.AddAsync(entity);
            var deleteCount = _mongo.Delete<DeviceLogDTO>(x => x.Uid == uid);

            uid = SGUID.GenerateComb().ToString();
            entity = new DeviceLogDTO
            {
                ServiceId = 1000,
                SourceType = DeviceSourceEnum.None,
                Value = "userId=5;userName=abcdefg555",
                CreateTime = DateTime.UtcNow.D2L(),
                Uid = uid,
            };
            await _mongo.AddAsync(entity);
            //自增步长
            var incCount = _mongo.UpdateInc(() => new DeviceLogDTO()
            {
                ServiceId = 1
            }, x => x.Uid == uid);

            var newEntity = _mongo.FindOneAndUpdate(() => new DeviceLogDTO()
            {
                SourceType = DeviceSourceEnum.Password
            }, x => x.Uid == uid, () => new DeviceLogDTO
            {
                ServiceId = 2
            });

            var query = _mongo.GetQueryable<DeviceLogDTO>().Where(x => x.SourceType == DeviceSourceEnum.Password);
            query = query.Where(x => x.Uid == uid);
            var list = query.ToList();

            //指定文档操作
            var collection = _mongo.MongoDb.GetCollection<DeviceLogDTO>("device_log");

            uid = SGUID.GenerateComb().ToString();
            entity = new DeviceLogDTO
            {
                ServiceId = 1000,
                SourceType = DeviceSourceEnum.None,
                Value = "userId=6;userName=abcdefg666",
                CreateTime = DateTime.UtcNow.D2L(),
                Uid = uid,
            };
            await collection.InsertOneAsync(entity);
            //await collection.InsertManyAsync(new List<DeviceLogDTO> { entity });


            var builderlist = new List<FilterDefinition<DeviceLogDTO>>
            {
                Builders<DeviceLogDTO>.Filter.Eq("Uid", uid),
                Builders<DeviceLogDTO>.Filter.Eq("SourceType", (int)DeviceSourceEnum.None),
                //Builders<DeviceLogDTO>.Filter.Gte("CreateTime", startTime),
                //Builders<DeviceLogDTO>.Filter.Lte("CreateTime", endTime)
            };
            entity = await collection.Find(Builders<DeviceLogDTO>.Filter.And(builderlist)).SortByDescending(a => a.CreateTime).FirstOrDefaultAsync();

            var count = await collection.Find(Builders<DeviceLogDTO>.Filter.And(builderlist)).CountDocumentsAsync();

            var entitys = await collection.Find(Builders<DeviceLogDTO>.Filter.And(builderlist)).SortByDescending(a => a.CreateTime).ToListAsync();

            var time = DateTime.UtcNow.AddSeconds(-5).D2L();
            var filer = Builders<DeviceLogDTO>.Filter.Where(a => a.CreateTime <= time);
            await collection.DeleteManyAsync(filer);
            //更多方法查看IMongoCollection接口

            //使用管道运行聚合脚本,更多脚本参考Mongo官方文档
            string @match = "{$match:{'SourceType' : " + (int)DeviceSourceEnum.None + ", 'CreateTime' : { $gte :" + 0 + ", $lte : " + 0 + "} }}";
            string @group = "{$group:{_id: null, Count: {$sum: 1 } }}";
            string @project = "{$project:{ _id: 0,'Count':1 }}";
            var @stages = new List<IPipelineStageDefinition>
            {
                new JsonPipelineStageDefinition<BsonDocument, BsonDocument>(@match),
                new JsonPipelineStageDefinition<BsonDocument, BsonDocument>(@group),
                new JsonPipelineStageDefinition<BsonDocument, BsonDocument>(@project),
            };
            var collection1 = _mongo.MongoDb.GetCollection<BsonDocument>("device_log");
            var @pipeline = new PipelineStagePipelineDefinition<BsonDocument, BsonDocument>(@stages);
            var @data = await collection1.AggregateAsync(@pipeline);
            var result1 = await @data.FirstOrDefaultAsync();

            return new HelloReply
            {
                Message = "Hello "
            };
        }

        public override Task<HelloReply> LogOperation(Empty request, ServerCallContext context)
        {
            _logger.LogInformation("Info");
            _logger.LogInformationEx("Info", "Info");

            _logger.LogDebug("Debug");
            _logger.LogDebugEx("Debug", "Debug");

            _logger.LogWarning("Warning");
            _logger.LogWarningEx("Warning", "Warning");

            _logger.LogError("Error");
            _logger.LogErrorEx("Error", "Error");

            return Task.FromResult(new HelloReply
            {
                Message = "Hello "
            });
        }
    }
}
