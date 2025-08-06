using MongoDB.Bson.Serialization.Attributes;

namespace MyTest.Model
{
    [BsonIgnoreExtraElements]
    public class DeviceLogDTO
    {
        //public ObjectId _id { get; set; }
        public int ServiceId { get; set; }
        public DeviceSourceEnum SourceType { get; set; }
        public string Value { get; set; }
        //[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public long CreateTime { get; set; }
        public string Uid { get; set; } = string.Empty;
    }

    public enum DeviceSourceEnum
    {
        None = 0,
        Register = 1,
        Login = 2,
        Password = 3
    }

}
