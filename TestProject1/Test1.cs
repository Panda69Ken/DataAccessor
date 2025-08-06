using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using MyTest;

namespace TestProject1
{
    [TestClass]
    public sealed class Test1
    {
        Greeter.GreeterClient _greeterClient;

        [TestInitialize]
        public void Init()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:7035");
            _greeterClient = new Greeter.GreeterClient(channel);
        }

        [TestMethod]
        public async Task DataOperation()
        {
            var result = await _greeterClient.DataOperationAsync(new Empty());
            Console.WriteLine(result);
        }

        [TestMethod]
        public async Task RedisOperation()
        {
            var result = await _greeterClient.RedisOperationAsync(new Empty());
            Console.WriteLine(result);
        }

        [TestMethod]
        public async Task MongoOperation()
        {
            var result = await _greeterClient.MongoOperationAsync(new Empty());
            Console.WriteLine(result);
        }

        [TestMethod]
        public async Task LogOperation()
        {
            var result = await _greeterClient.LogOperationAsync(new Empty());
            Console.WriteLine(result);
        }
    }
}
