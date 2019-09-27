using System;
using System.Collections.Generic;
using System.Text;
using Bestan.Common;

/*=============================================================================================== 
 Author   : yeyouhuan
 Created  : 2019/9/25 16:03:34
 Summary  : 属性数据
 ===============================================================================================*/
namespace BGame.Common
{
    public class AttrBaseUnit<T> : DoubleDataMap<T> where T : Enum
    {
    }

    public class AttrBase<T> where T : Enum
    {
        public static readonly Type ATTR_TYPE = typeof(T);
        protected AttrBaseUnit<T> baseAttr = new AttrBaseUnit<T>();
        protected AttrBaseUnit<T> finalAttr = new AttrBaseUnit<T>();

        protected AttrConfig<T> Config { set; get; }
        protected HashSet<T> changeAttrList = new HashSet<T>();

        public void Rebuild()
        {
            finalAttr.Clear();
            //先计算一级属性
            foreach (var entry in baseAttr.Data)
            {
                var attrId = entry.Key;
                if (!Config.IsLevelOneAttr(attrId) || !GAttr.IsAttrType(attrId))
                {
                    continue;
                }
                var value = CalcLevelOneAttr(attrId);
                finalAttr.Set(attrId, value);
            }
            foreach (var entry in baseAttr.Data)
            {
                var attrId = entry.Key;
                if (Config.IsLevelOneAttr(attrId) || !GAttr.IsAttrType(attrId))
                {
                    continue;
                }
                var value = CalcLevelTwoAttr(attrId);
                finalAttr.Set(attrId, value);
            }
        }

        protected double CalcLevelOneAttr(T attrId)
        {
            return CalcTotalAttribute(attrId);
        }
        public double Get(T attrId)
        {
            return finalAttr[attrId];
        }

        protected double CalcLevelTwoAttr(T attrId)
        {
            var value = CalcTotalAttribute(attrId);
            if (!Config.twoLevelRelyOnEffectMap.TryGetValue(attrId, out Dictionary<T, double> relyEffect))
            {
                return value;
            }
            //加上基础属性的影响值
            foreach (var entry in relyEffect)
            {
                value += Get(entry.Key) * entry.Value;
            }
            return value;
        }

        /// <summary>
        /// 计算指定属性的值
        /// </summary>
        /// <param name="attrId"></param>
        /// <returns></returns>
        protected double CalcTotalAttribute(T attrId)
        {
            if (!GAttr.IsAttrType(attrId)) return 0;

            var valueAttr = (T)Enum.ToObject(ATTR_TYPE, Convert.ToInt32(attrId) + GAttr.ATTR_ID_ABL);
            var percentAttr = (T)Enum.ToObject(ATTR_TYPE, Convert.ToInt32(attrId) + GAttr.ATTR_ID_PCT);

            return baseAttr[valueAttr] * (1.0f + baseAttr[percentAttr] / 100.0f);
        }
    }
}
