using System;
using System.Collections.Generic;
using System.Text;

/*=============================================================================================== 
 Author   : yeyouhuan
 Created  : 2019/9/25 16:03:34
 Summary  : 属性的定义
 ===============================================================================================*/
namespace BGame.Common
{
    public class GAttr
    {
        /// <summary>
        /// 属性ID表示类型
        /// </summary>
        public const int ATTR_ID_TYPE = 0;
        /// <summary>
        /// 属性ID表示绝对值
        /// </summary>
        public const int ATTR_ID_ABL = 1;
        /// <summary>
        /// 属性ID表示百分比
        /// </summary>
        public const int ATTR_ID_PCT = 2;
        /// <summary>
        /// 需要根据精度来计算
        /// </summary>
        public const double PRECISION = 0.000001;

        /// <summary>
        /// 指定属性是否类型属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attrId"></param>
        /// <returns></returns>
        public static bool IsAttrType<T>(T attrId) where T : Enum
        {
            return Convert.ToInt32(attrId) % 10 == ATTR_ID_TYPE;
        }

        public static bool Equal(double src, double dst)
        {
            return Math.Abs(src - dst) <= PRECISION;
        }

        public static T GetAttrType<T>(T attrId) where T : Enum
        {
            var value = Convert.ToInt32(attrId);
            value = value - value % 10;
            return (T)Enum.ToObject(AttrManager<T>.ATTR_TYPE, value);
        }
    }
}
