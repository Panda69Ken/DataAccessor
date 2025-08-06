using System.Collections;

namespace My.Framework.Foundation
{
    /// <summary>
    /// 与字典相关的扩展方法
    /// </summary>
    public static class DictionarieExtension
    {
        /// <summary>
        /// 获取指定键值对应的值，如果不存在对应键数据，则返回默认值
        /// </summary>
        /// <param name="dict">字典对象</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static object GetOrDefault(this IDictionary dict, object key, object defaultValue)
        {
            return (dict.Contains(key) ? dict[key] : defaultValue);
        }
        /// <summary>
        /// 获取指定键值对应的值，如果不存在对应键数据，则返回默认值
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dict">字典对象</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            return (dict.ContainsKey(key) ? dict[key] : defaultValue);
        }
        /// <summary>
        /// 获取指定键值对应的值,如果不存在对应键数据则调用函数委托获取值,并将值存入字典后返回
        /// </summary>
        /// <param name="dict">字典对象</param>
        /// <param name="key">键</param>
        /// <param name="func">获取新值的函数委托</param>
        /// <returns></returns>
        public static object GetOrAdd(this IDictionary dict, object key, Func<object> func)
        {
            if (dict.Contains(key))
            {
                return dict[key];
            }
            else
            {
                var value = func.Invoke();
                dict.Add(key, value);
                return value;
            }
        }
        /// <summary>
        /// 获取指定键值对应的值,如果不存在对应键数据则调用函数委托获取值,并将值存入字典后返回
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dict">字典对象</param>
        /// <param name="key">键</param>
        /// <param name="func">获取新值的函数委托</param>
        /// <returns></returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> func)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            else
            {
                var value = func.Invoke();
                dict.Add(key, value);
                return value;
            }
        }
    }
}
