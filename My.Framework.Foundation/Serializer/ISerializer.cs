namespace My.Framework.Foundation.Serializer
{
    /// <summary>
    ///     对要缓存的值进行序列化
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        ///     序列化成字节
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        byte[] Serialize(object item);

        /// <summary>
        ///     异步序列化成字节
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<byte[]> SerializeAsync(object item);

        /// <summary>
        ///     反序列化成一个对象
        /// </summary>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        object Deserialize(byte[] serializedObject);

        /// <summary>
        ///     异步反序列化成一个对象
        /// </summary>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        Task<object> DeserializeAsync(byte[] serializedObject);

        /// <summary>
        ///     反序列化成一个泛型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        T Deserialize<T>(byte[] serializedObject);

        /// <summary>
        ///     异步反序列化成一个泛型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        Task<T> DeserializeAsync<T>(byte[] serializedObject);
    }
}
