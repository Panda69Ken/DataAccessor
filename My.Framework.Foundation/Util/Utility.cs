using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace My.Framework.Foundation
{
    /// <summary>
    /// 实用类
    /// </summary>
    public static class Utility
    {
        #region 随机函数块
        /// <summary>
        /// 随机数生成器
        /// </summary>
        private static Random _Random;
        /// <summary>
        /// 随机数生成器
        /// </summary>
        public static Random Random
        {
            get
            {
                if (_Random == null)
                {
                    lock (typeof(Utility))
                    {
                        if (_Random == null)
                            _Random = Utility.CreateRandom();
                    }
                }
                return _Random;
            }
        }
        /// <summary>
        /// 生成一个新的随机数生成器
        /// </summary>
        public static Random CreateRandom()
        {
            return new Random(CreateRndSeed());
        }

        /// <summary>
        /// 获取新的随机种子数
        /// </summary>
        /// <returns></returns>
        public static int CreateRndSeed()
        {
            byte[] rndBytes = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(rndBytes);
            return BitConverter.ToInt32(rndBytes, 0);
        }
        /// <summary>
        /// 获取数字与英文字母(大写)列的随机码
        /// </summary>
        /// <param name="length">要获取的长度</param>
        /// <returns></returns>
        public static string CreateRndCode(int length)
        {
            return CreateRndCode("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ", length);
        }
        /// <summary>
        /// 获取随机码
        /// </summary>
        /// <param name="codeChars">随机码字符字符列表，如“0123456789”</param>
        /// <param name="length">要获取的随机码长度</param>
        /// <returns>随机码字符</returns>
        public static string CreateRndCode(string codeChars, int length)
        {
            //默认的值
            if (string.IsNullOrEmpty(codeChars) || length < 1) return string.Empty; ;

            int formatlength = codeChars.Length - 1;
            StringBuilder codes = new StringBuilder(length);
            int i;
            var rnd = Utility.Random;
            for (i = 1; i <= length; i++)
            {
                codes.Append(codeChars[rnd.Next(0, formatlength)]);
            }
            return codes.ToString();
        }
        #endregion

        #region 路径操作函数块
        /// <summary>
        /// 判断地址是否是绝对路径，如网络地址“http://www.baidu.com”；本地地址：“c:\test.txt”
        /// </summary>
        /// <param name="path">要判断的路径地址，如网络地址“http://www.baidu.com”（绝对）、“/test/file.txt”（相对）</param>
        /// <returns></returns>
        public static bool IsAbsolutePath(string path)
        {
            //绝对地址，则直接返回
            Uri uri;
            return Uri.TryCreate(path, UriKind.Absolute, out uri);
        }
        #endregion

        #region 生成实例对象
        /// <summary>
        /// 生成某个实例对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="instance">实例类型，可以为以下三种
        /// <para>“类型名称” ， 例子“MY.Core.Log.FileLoggerFactory”</para>
        /// <para>“类型名称, 程序集名称” ， 例子“MY.Core.Log.FileLoggerFactory, MY.Core”</para>
        /// <para>“类型名称, 程序集文件” ， 例子“MY.Core.Log.FileLoggerFactory, c:\dll\MY.Core.dll”</para>
        /// </param>
        /// <returns></returns>
        public static T CreateInstance<T>(string instance)
        {
            if (!string.IsNullOrEmpty(instance))
            {
                var p = instance.IndexOf(',');
                object item = null;
                if (p == -1)
                {
                    item = typeof(T).Assembly.CreateInstance(instance);
                }
                else
                {
                    string assembly = instance.Substring(p + 1).Trim();
                    instance = instance.Substring(0, p).Trim();
                    if (assembly.IndexOf(':') != -1)
                    {
                        var type = Assembly.LoadFrom(assembly).GetType(instance);
                        item = Activator.CreateInstance(type);
                    }
                    else
                    {
                        var type = Assembly.Load(new AssemblyName(assembly)).GetType(instance);
                        item = Activator.CreateInstance(type);
                    }
                }
                if (item is T variable)
                {
                    return variable;
                }
                else
                {
                    return default(T);
                }
            }
            return default(T);
        }
        #endregion
    }
}
