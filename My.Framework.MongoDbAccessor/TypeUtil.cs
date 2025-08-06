using System.Reflection;

namespace My.Framework.MongoDbAccessor
{
    /// <summary>
    /// 类型操作的帮助类。
    /// </summary>
    public sealed class TypeUtil
    {
        /// <summary>
        /// 判断类型是否是基本类型。
        /// </summary>
        /// <param name="pType"></param>
        /// <returns></returns>
        public static bool IsSimpleType(Type pType)
        {
            return GetSimpleType().Any(ww => ww.FullName == pType.FullName);
        }

        /// <summary>
        /// 获取简单数据类型。
        /// </summary>
        /// <returns>取到的简单数据类型</returns>
        public static List<Type> GetSimpleType()
        {
            List<Type> theTypes = new List<Type>
            {
                typeof(int),
                typeof(short),
                typeof(uint),
                typeof(ushort),
                typeof(long),
                typeof(ulong),
                typeof(byte),
                typeof(sbyte),
                typeof(float),
                typeof(double),
                typeof(decimal),
                typeof(char),
                typeof(string),
                typeof(TimeSpan),
                typeof(DateTime),
                typeof(bool),
                typeof(byte[]),
                typeof(int?),
                typeof(short?),
                typeof(uint?),
                typeof(ushort?),
                typeof(long?),
                typeof(ulong?),
                typeof(byte?),
                typeof(sbyte?),
                typeof(float?),
                typeof(double?),
                typeof(decimal?),
                typeof(char?),
                typeof(DateTime?),
                typeof(TimeSpan?),
                typeof(bool?),
                typeof(DBNull)
            };
            return theTypes;
        }

        /// <summary>
        /// 跟据属性路由将属性值赋给指定对象的属性
        /// 注意：仅仅是属性，字段将被忽略。
        /// </summary>
        /// <param name="Parameter">要赋值的对象</param>
        /// <param name="PropertyRoute">要赋值的属性的路由</param>
        /// <param name="PropertyValue">要赋的值</param>
        /// <returns>赋值后的属性</returns>
        public static object EvaluatePropertyByRoute(Object Parameter, string PropertyRoute, object PropertyValue)
        {
            //属性的路由（可能存在多级 如prop1.prop2.prop3）。
            List<string> PropertyRoutes = PropertyRoute.Split('.')
                .Where(ww => !string.IsNullOrWhiteSpace(ww)).ToList();

            //属性路由所指向的目标属性。
            object objGoal = null;

            //要赋值的属性的类型。
            Type EvaluatePropType = null;

            //属性路由寻址暂存变量（哨兵）
            object objSentinel = Parameter;
            for (int i = 0; i < PropertyRoutes.Count(); i++)
            {
                if (i == (PropertyRoutes.Count() - 1))
                {
                    objGoal = objSentinel;
                    EvaluatePropType = objSentinel.GetType().GetProperty(PropertyRoutes[i]).PropertyType;
                    break;
                }

                PropertyInfo routePropInfo = objSentinel.GetType().GetProperty(PropertyRoutes[i]);
                if (routePropInfo != null)
                {
                    if (routePropInfo.GetValue(objSentinel) == null)
                    {
                        object PropValue = Activator.CreateInstance(routePropInfo.PropertyType);
                        routePropInfo.SetValue(objSentinel, PropValue);
                    }

                    objSentinel = routePropInfo.GetValue(objSentinel);
                }

                if (objSentinel == null)
                {
                    break;
                }
            }

            if (PropertyValue != null)
            {
                //值的类型转换。
                object objValue = SimpleTypeCompatible(PropertyValue, EvaluatePropType);

                //属性的赋值。
                if (objGoal != null)
                {
                    if (objGoal.GetType().GetProperty(PropertyRoutes[PropertyRoutes.Count - 1]) != null)
                    {
                        objGoal.GetType().GetProperty(PropertyRoutes[PropertyRoutes.Count - 1]).SetValue(objGoal, objValue);
                    }
                }
            }

            return Parameter;
        }

        /// <summary>
        /// 将一简单类型的值转为指定类型。
        /// </summary>
        /// <param name="obj">源值</param>
        /// <param name="pType">目标类型</param>
        /// <returns>目标值</returns>
        public static object SimpleTypeCompatible(object obj, Type pType)
        {
            //不是简单类型，直接反回
            if (obj == null || !IsSimpleType(pType))
            {
                return obj;
            }

            //DBNull返回空。
            if (obj is DBNull)
            {
                return null;
            }

            //目标类型（简单类型）。
            Type EvaluatePropType = pType;


            //输入值。
            object inObj = obj;

            //输出值。
            object outObj = null;

            //目标类型是否可空。
            bool isNullable = (pType.IsGenericType && pType.
              GetGenericTypeDefinition().Equals
              (typeof(Nullable<>)));

            if (isNullable)
            {
                EvaluatePropType = Nullable.GetUnderlyingType(EvaluatePropType);
            }

            if (inObj is DBNull)
            {
                outObj = null;
            }
            else if (EvaluatePropType.FullName == typeof(string).FullName)
            {
                outObj = inObj;
            }
            else if (EvaluatePropType.FullName == typeof(bool).FullName)
            {
                outObj = (inObj.ToString() == "1" || inObj.ToString().ToLower() == "true");
            }
            else
            {
                outObj = EvaluatePropType.GetMethod("Parse", new Type[] { typeof(string) })
                   .Invoke(null, new object[] { inObj.ToString() });
            }

            return outObj;
        }

        /// <summary>
        /// 跟据属性路由取对象的属性值或字段值。
        /// </summary>
        /// <param name="Parameter">要取值对象</param>
        /// <param name="StrRoute">属性路由</param>
        /// <returns>取到的结果</returns>
        public static object GetPropertyValueByRoute(object Parameter, string StrRoute)
        {
            object Obj = Parameter;
            List<string> routeList = StrRoute.Split('.').ToList()
                .Where(ww => !string.IsNullOrWhiteSpace(ww)).ToList();

            foreach (string strRout in routeList)
            {
                if (Obj == null)
                    break;

                if (Obj.GetType().GetProperty(strRout) != null)
                {
                    Obj = Obj.GetType().GetProperty(strRout).GetValue(Obj);
                    continue;
                }
                if (Obj.GetType().GetField(strRout) != null)
                {
                    Obj = Obj.GetType().GetField(strRout).GetValue(Obj);
                    continue;
                }

                Obj = null;
                break;
            }

            return Obj;
        }
    }
}
