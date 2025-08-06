using Newtonsoft.Json;

namespace My.Framework.Foundation.Json.Converter
{
    /// <summary>
    /// json中bool值转换成int
    /// </summary>
    public class BooleanConverterToInt : JsonConverter
    {
        /// <summary>
        /// Item1=1,Item2=0
        /// </summary>
        private readonly Tuple<string, string> _trueOrFalse = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BooleanConverterToInt()
        {
            _trueOrFalse = new Tuple<string, string>("1", "0");
        }

        /// <summary>
        /// 写入json
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            bool f = bool.Parse(value.ToString());
            writer.WriteValue(f ? Convert.ToInt32(_trueOrFalse.Item1) : Convert.ToInt32(_trueOrFalse.Item2));
        }

        /// <summary>
        /// 读取json
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool isNullable = IsNullableType(objectType);
            Type t = isNullable ? Nullable.GetUnderlyingType(objectType) : objectType;

            if (reader.TokenType == JsonToken.Null)
            {
                if (!IsNullableType(objectType))
                {
                    throw new Exception(string.Format("不能转换空值为{0}.", objectType));
                }
                return null;
            }
            try
            {
                if (reader.TokenType == JsonToken.String)
                {
                    string boolText = reader.Value.ToString();
                    if (boolText.Equals("1", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    else if (boolText.Equals(_trueOrFalse.Item2, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }

                if (reader.TokenType == JsonToken.Integer)
                {
                    //数值
                    return Convert.ToInt32(reader.Value) == 1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("无法将值'{0}'转换成'{1}'", reader.Value, objectType));
            }
            throw new Exception(string.Format("解析时出现意外的标记'{0}'", reader.TokenType));
        }

        /// <summary>
        /// 是否可以转换
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }

        /// <summary>
        /// 是否为空类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsNullableType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return (type.BaseType.FullName == "System.ValueType" && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
