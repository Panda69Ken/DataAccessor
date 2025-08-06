using System.Data;
using System.Reflection;

namespace My.Framework.Foundation
{
    /// <summary>
    /// 与类型相关的扩展方法
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// 获取类型对应的System.Data.DbType值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType GetDbType(this Type type)
        {
            var tc = Type.GetTypeCode(type);
            switch (tc)
            {
                case TypeCode.Char:
                    return DbType.StringFixedLength;
                case TypeCode.DBNull:
                case TypeCode.Empty:
                    return DbType.Object;
                case TypeCode.Object:
                    //处理特殊的类型
                    if (type == typeof(Guid)) return DbType.Guid;
                    if (type.IsArray && type == typeof(byte[])) return DbType.Binary;
                    return DbType.Object;
                default:
                    return tc.ToString().As<DbType>();
            }
        }

        /// <summary>
        /// 创建某个类型
        /// </summary>
        /// <param name="typeInstance"></param>
        /// <returns></returns>
        public static Type CreateType(string typeInstance)
        {
            if (string.IsNullOrEmpty(typeInstance)) return null;
            string typeName = typeInstance.Trim();
            if (string.IsNullOrEmpty(typeName)) return null;

            int p = typeName.IndexOf(',');
            Type type = null;
            if (p == -1)
            {
                //从当前程序域里建立类型

                type = Type.GetType(typeName, false, true);
                if (type == null)
                {
                    //搜索当前程序域里的所有程序集
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly asm in assemblies)
                    {
                        type = asm.GetType(typeName, false, true);
                        if (type != null) break;
                    }
                }
            }
            else
            {
                string assembly = typeName.Substring(p + 1).TrimStart();
                typeName = typeName.Substring(0, p).TrimEnd();

                //从某个程序集里建立类型
                Assembly asm;
                if (assembly.IndexOf(":", StringComparison.Ordinal) != -1)
                {
                    asm = Assembly.LoadFrom(assembly);
                }
                else
                {
                    asm = Assembly.Load(assembly);
                }
                if (asm != null)
                {
                    type = asm.GetType(typeName, false, true);
                }
            }
            return type;
        }

        /// <summary>
        /// 获取某个类型对应的默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetDefaultValue(this Type type)
        {
            if (type == typeof(string))
            {
                return null;
            }
            else if (type == typeof(DateTime))
            {
                return DateTime.MinValue;
            }
            else if (type == typeof(bool))
            {
                return false;
            }
            else if (type == typeof(int) ||
              type == typeof(uint) ||
              type == typeof(long) ||
              type == typeof(ulong) ||
              type == typeof(float) ||
              type == typeof(double) ||
              type == typeof(byte) ||
              type == typeof(sbyte) ||
              type == typeof(short) ||
              type == typeof(ushort) ||
              type == typeof(decimal))
            {
                return 0;
            }
            else if (type == typeof(char))
            {
                return '\0';
            }
            else if (type == typeof(Guid))
            {
                return Guid.Empty;
            }
            else if (type == typeof(TimeSpan))
            {
                return TimeSpan.MinValue;
            }
            else if (type.IsEnum)
            {
                //枚举类型。则获取第一个默认项
                return Enum.GetValues(type).GetValue(0);
            }
            else
            {
                return null;
            }
        }
    }
}
