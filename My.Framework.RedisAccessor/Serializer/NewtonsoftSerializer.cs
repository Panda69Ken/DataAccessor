using My.Framework.Foundation.Serializer;
using Newtonsoft.Json;
using System.Text;

namespace My.Framework.RedisAccessor.Serializer
{
    public class NewtonsoftSerializer : ISerializer
    {
        private static readonly Encoding Encoding = Encoding.UTF8;

        private readonly JsonSerializerSettings _settings;

        public NewtonsoftSerializer(JsonSerializerSettings settings = null)
        {
            _settings = settings ?? new JsonSerializerSettings();
        }

        public byte[] Serialize(object item)
        {
            var type = item.GetType();
            var jsonString = JsonConvert.SerializeObject(item, type, _settings);
            return Encoding.GetBytes(jsonString);
        }

        public async Task<byte[]> SerializeAsync(object item)
        {
            var type = item.GetType();
            var jsonString = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(item, type, _settings));
            return Encoding.GetBytes(jsonString);
        }

        public object Deserialize(byte[] serializedObject)
        {
            var jsonString = Encoding.GetString(serializedObject);
            return JsonConvert.DeserializeObject(jsonString, typeof(object));
        }

        public Task<object> DeserializeAsync(byte[] serializedObject)
        {
            return Task.Factory.StartNew(() => Deserialize(serializedObject));
        }

        public T Deserialize<T>(byte[] serializedObject)
        {
            var jsonString = Encoding.GetString(serializedObject);
            return JsonConvert.DeserializeObject<T>(jsonString, _settings);
        }

        public Task<T> DeserializeAsync<T>(byte[] serializedObject)
        {
            return Task.Factory.StartNew(() => Deserialize<T>(serializedObject));
        }
    }
}
