using System.Collections.Specialized;
using System.Reflection;

namespace My.Framework.Foundation
{
    /// <summary>
    /// 与集合相关的扩展方法
    /// </summary>
    public static class CollectionExtension
    {
        #region NameValueCollection.Get<T>
        /// <summary>
        /// 获取某项值
        /// </summary>
        /// <typeparam name="T">获取的类型</typeparam>
        /// <param name="collection">集合</param>
        /// <param name="name">项名称</param>
        /// <exception cref="System.ArgumentException">集合等于null</exception>
        /// <exception cref="System.ArgumentNullException">项名称为空或null</exception>
        /// <example>
        /// <code>
        /// var id = HttpContext.Current.Request.QueryString.Get&lt;int&gt;("id");
        /// </code>
        /// </example>
        /// <returns>项的值</returns>
        public static T Get<T>(this NameValueCollection collection, string name)
        {
            if (collection == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            string value = collection[name];
            return value.As<T>();
        }
        /// <summary>
        /// 获取某项值
        /// </summary>
        /// <typeparam name="T">获取的类型</typeparam>
        /// <param name="collection">集合</param>
        /// <param name="name">项名称</param>
        /// <exception cref="System.ArgumentException">集合等于null</exception>
        /// <exception cref="System.ArgumentNullException">项名称为空或null</exception>
        /// <example>
        /// <code>
        /// var id = HttpContext.Current.Request.QueryString.GetNullable&lt;int&gt;("id");
        /// </code>
        /// </example>
        /// <returns>项的值</returns>
        public static T? GetNullable<T>(this NameValueCollection collection, string name)
             where T : struct
        {
            if (collection == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            string value = collection[name];
            return value?.AsNullable<T>();
        }
        /// <summary>
        /// 获取某项值，并去除值的前后空白字符
        /// </summary>
        /// <typeparam name="T">获取的类型</typeparam>
        /// <param name="collection">集合</param>
        /// <param name="name">项名称</param>
        /// <exception cref="System.ArgumentException">集合等于null</exception>
        /// <exception cref="System.ArgumentNullException">项名称为空或null</exception>
        /// <example>
        /// <code>
        /// var username = HttpContext.Current.Request.QueryString.TrimGetNullable&lt;string&gt;("username");
        /// </code>
        /// </example>
        /// <returns>已去除前后空白字符的项的值</returns>
        public static T? TrimGetNullable<T>(this NameValueCollection collection, string name)
            where T : struct
        {
            if (collection == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            string value = collection[name];

            return value?.Trim().AsNullable<T>();
        }
        /// <summary>
        /// 获取某项值，并去除值的前后空白字符
        /// </summary>
        /// <typeparam name="T">获取的类型</typeparam>
        /// <param name="collection">集合</param>
        /// <param name="name">项名称</param>
        /// <exception cref="System.ArgumentException">集合等于null</exception>
        /// <exception cref="System.ArgumentNullException">项名称为空或null</exception>
        /// <example>
        /// <code>
        /// var username = HttpContext.Current.Request.QueryString.TrimGet&lt;string&gt;("username");
        /// </code>
        /// </example>
        /// <returns>已去除前后空白字符的项的值</returns>
        public static T TrimGet<T>(this NameValueCollection collection, string name)
        {
            if (collection == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            string value = collection[name];
            value = value?.Trim();
            return value.As<T>();
        }
        /// <summary>
        /// 获取某项值
        /// </summary>
        /// <typeparam name="T">获取的类型</typeparam>
        /// <param name="collection">集合</param>
        /// <param name="name">项名称</param>
        /// <param name="replacement">转换失败后返回的替换值</param>
        /// <exception cref="System.ArgumentException">集合等于null</exception>
        /// <exception cref="System.ArgumentNullException">项名称为空或null</exception>
        /// <example>
        /// <code>
        /// var id = HttpContext.Current.Request.QueryString.Get&lt;int&gt;("id", 1);
        /// </code>
        /// </example>
        /// <returns>已去除前后空白字符的项的值</returns>
        public static T Get<T>(this NameValueCollection collection, string name, T replacement)
        {
            if (collection == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            string value = collection[name];
            return value.As<T>(replacement);
        }
        /// <summary>
        /// 获取某项值，并去除值的前后空白字符
        /// </summary>
        /// <typeparam name="T">获取的类型</typeparam>
        /// <param name="collection">集合</param>
        /// <param name="name">项名称</param>
        /// <param name="replacement">转换失败后返回的替换值</param>
        /// <exception cref="System.ArgumentException">集合等于null</exception>
        /// <exception cref="System.ArgumentNullException">项名称为空或null</exception>
        /// <example>
        /// <code>
        /// var city = HttpContext.Current.Request.QueryString.TrimGet&lt;string&gt;("city", "china");
        /// </code>
        /// </example>
        /// <returns>已去除前后空白字符的项的值</returns>
        public static T TrimGet<T>(this NameValueCollection collection, string name, T replacement)
        {
            if (collection == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            string value = collection[name];
            value = value?.Trim();
            return value.As<T>(replacement);
        }
        /// <summary>
        /// 将集合里的各项数据拷入到某个对象的对应属性或字段里。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="collection">集合</param>
        /// <param name="obj">对象</param>
        /// <exception cref="System.ArgumentNullException">obj等于null</exception>
        /// <example>
        /// <code>
        /// var user = new User();
        /// HttpContext.Current.Request.QueryString.CopyTo&lt;User&gt;(user);
        /// HttpContext.Current.Response.Write(user.Name);
        /// </code>
        /// </example>
        public static void CopyTo<T>(this NameValueCollection collection, T obj)
            where T : class
        {
            CopyTo<T>(collection, obj, "", false);
        }
        /// <summary>
        /// 将集合里的各项数据拷入到某个对象的对应属性或字段里。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="collection">集合</param>
        /// <param name="obj">对象</param>
        /// <param name="prefix">参数前缀</param>
        /// <exception cref="System.ArgumentNullException">obj等于null</exception>
        /// <example>
        /// <code>
        /// var user = new User();
        /// HttpContext.Current.Request.QueryString.CopyTo&lt;User&gt;(user);
        /// HttpContext.Current.Response.Write(user.Name, "user.");
        /// </code>
        /// </example>
        public static void CopyTo<T>(this NameValueCollection collection, T obj, string prefix)
            where T : class
        {
            CopyTo<T>(collection, obj, prefix, false);
        }

        /// <summary>
        /// 将集合里的各项数据拷入到某个对象的对应属性或字段里。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="collection">集合</param>
        /// <param name="obj">对象</param>
        /// <param name="trim">是否获取数据前，先去除值的前后空白字符</param>
        /// <exception cref="System.ArgumentNullException">obj等于null</exception>
        /// <example>
        /// <code>
        /// var user = new User();
        /// HttpContext.Current.Request.QueryString.CopyTo&lt;User&gt;(user, true);
        /// HttpContext.Current.Response.Write(user.Name);
        /// </code>
        /// </example>
        public static void CopyTo<T>(this NameValueCollection collection, T obj, bool trim)
            where T : class
        {
            CopyTo<T>(collection, obj, "", trim);
        }
        /// <summary>
        /// 将集合里的各项数据拷入到某个对象的对应属性或字段里。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="collection">集合</param>
        /// <param name="obj">对象</param>
        /// <param name="prefix">参数前缀</param>
        /// <param name="trim">是否获取数据前，先去除值的前后空白字符</param>
        /// <exception cref="System.ArgumentNullException">obj等于null</exception>
        /// <example>
        /// <code>
        /// var user = new User();
        /// HttpContext.Current.Request.QueryString.CopyTo&lt;User&gt;(user, "user.", true);
        /// HttpContext.Current.Response.Write(user.Name);
        /// </code>
        /// </example>
        public static void CopyTo<T>(this NameValueCollection collection, T obj, string prefix, bool trim)
            where T : class
        {
            if (collection == null) return;
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var t = obj.GetType();
            foreach (string key in collection.AllKeys)
            {
                string name = key;
                if (!string.IsNullOrEmpty(prefix))
                {
                    if (key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        name = name.Substring(prefix.Length);
                    }
                    else
                    {
                        continue;
                    }
                }
                string value = collection[key];
                if (value != null)
                {
                    if (trim) value = value.Trim();
                    SetObjectItem(t, obj, name, value);
                }
            }
        }
        /// <summary>
        /// 设置对象的某项
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private static void SetObjectItem(Type type, object obj, string key, string value)
        {
            //分隔属性字段. 如member.id、user.location.city.name或者单属性名称name
            string[] keys = key.Split('.');

            int last = keys.Length - 1;
            for (var i = 0; i < keys.Length; i++)
            {
                string name = keys[i].Trim();
                if (string.IsNullOrEmpty(name)) continue;

                //处理属性
                var pro = type.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                object o;
                if (pro != null)
                {
                    if (pro.CanWrite)  //可写
                    {
                        if (pro.GetIndexParameters().Length == 0) //非索引属性
                        {
                            var pt = pro.PropertyType;
                            if (i == last)         //是最后一项，所以直接处理值
                            {
                                pro.SetValue(obj, value.ConvertTo(pt, out _), null);
                            }
                            else if (pt.IsClass && pt != typeof(string))  //属性类型是类，而非值类型
                            {
                                if (i < last)               //还有子字段需要处理。则实例化
                                {
                                    o = pro.GetValue(obj, null);
                                    if (o == null)          //未有值，所以实例化一个对象
                                    {
                                        o = Activator.CreateInstance(pt, true);
                                        pro.SetValue(obj, o, null);
                                    }
                                    //更改类型
                                    obj = o;
                                    type = pt;
                                    continue;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //处理字段
                    var field = type.GetField(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                    if (field != null)
                    {
                        if (!field.IsInitOnly && !field.IsLiteral) //可写
                        {
                            var ft = field.FieldType;
                            if (i == last)         //是最后一项，所以直接处理值
                            {
                                field.SetValue(obj, value.ConvertTo(ft, out _));
                            }
                            else if (ft.IsClass && ft != typeof(string)) //字段类型是类，而非值类型
                            {
                                if (i < last)               //还有子字段需要处理。则实例化
                                {
                                    o = field.GetValue(obj);
                                    if (o == null)          //未有值，所以实例化一个对象
                                    {
                                        o = Activator.CreateInstance(ft, true);
                                        field.SetValue(obj, o);
                                    }
                                    //更改类型
                                    obj = o;
                                    type = ft;
                                    continue;
                                }
                            }
                        }
                    }
                }

                break;  //已处理过
            }
        }
        #endregion


    }
}
