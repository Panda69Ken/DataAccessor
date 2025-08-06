namespace My.Framework.Foundation
{
    /// <summary>
    /// Enum扩展类
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获得Enum的描述信息
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string Description(this Enum enumValue)
        {
            string str = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (objs == null || objs.Length == 0) return str;
            System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
            return da.Description;
        }

        /// <summary>
        /// 生成Enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumc">Enum字符串</param>
        /// <returns></returns>
        public static T GetEnum<T>(this string enumc) where T : struct
        {
            Enum.TryParse<T>(enumc, out var val);

            return val;
        }
    }
}
