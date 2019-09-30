using System;
using System.Collections.Generic;
using System.Linq;
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
        public AttrBaseUnit()
        {

        }

        public AttrBaseUnit(Dictionary<T, double> data):base(data)
        {
        }

        public AttrBaseUnit<T> Clone()
        {
            return new AttrBaseUnit<T>(new Dictionary<T, double>(Data));
        }
    }

    /// <summary>
    /// 属性值变化单元
    /// </summary>
    public class AttrChangeUnit
    {
        public double oldValue;
        public double newValue;

        public AttrChangeUnit(double oldValue, double newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }

    public class AttrBase<T> where T : Enum
    {
        public static readonly Type ATTR_TYPE = typeof(T);
        protected AttrBaseUnit<T> baseAttr = new AttrBaseUnit<T>();
        protected AttrBaseUnit<T> finalAttr = new AttrBaseUnit<T>();

        protected AttrConfig<T> Config { set; get; }
        protected HashSet<T> changeAttrList = new HashSet<T>();

        public AttrBase (AttrConfig<T> Config)
        {
            this.Config = Config;
        }

        /// <summary>
        /// 重新计算属性
        /// </summary>
        /// <param name="isInit"></param>
        public void Rebuild(bool isInit = false)
        {
            if (isInit)
            {
                finalAttr.Clear();
            }

            Dictionary<T, AttrChangeUnit> changeMap = new Dictionary<T, AttrChangeUnit>();
            //先计算一级属性
            foreach (var attrId in Config.OneLevelAttrs)
            {
                var value = CalcLevelOneAttr(attrId);
                var oldValue = finalAttr[attrId];
                finalAttr.Set(attrId, value);
                if (!GAttr.Equal(value, oldValue)) {
                    //有变化
                    if (Config.oneLevelRelationMap.TryGetValue(attrId, out Dictionary<T, double> relations))
                    {
                        foreach (var entry in relations)
                        {
                            AddChange(entry.Key);
                        }
                    }
                    changeMap.Add(attrId, new AttrChangeUnit(oldValue, value));
                }
            }

            //再计算二级属性
            foreach (var entry in changeAttrList)
            {
                var attrId = GAttr.GetAttrType(entry);
                //不是一级属性，并且不是属性类型，另外非初始化时不在变化列表里
                if (Config.IsLevelOneAttr(attrId)
                    || (!isInit && !changeAttrList.Contains(attrId)))
                {
                    continue;
                }
                var value = CalcLevelTwoAttr(attrId);
                var oldValue = finalAttr[attrId];
                finalAttr.Set(attrId, value);
                changeMap.Add(attrId, new AttrChangeUnit(oldValue, value));
            }
            changeAttrList.Clear();
            OnRebuildFinish(changeMap, isInit);
        }

        /// <summary>
        /// 属性重新计算完毕后
        /// </summary>
        /// <param name="changeMap"></param>
        protected virtual void OnRebuildFinish(Dictionary<T, AttrChangeUnit> changeMap, bool isInit)
        {
            AttrManager<T>.Instance.OnRebuildFinish(this, changeMap, isInit);
        }

        public AttrBaseUnit<T> GetBaseAttribute()
        {
            return baseAttr;
        }

        public AttrBaseUnit<T> GetFinalAttribute()
        {
            return finalAttr;
        }

        protected void AddChange(T attrId)
        {
            changeAttrList.Add(GAttr.GetAttrType(attrId));
        }
        protected void AddChange(List<T> attrIds)
        {
            foreach (var attrId in attrIds)
            {
                AddChange(attrId);
            }
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="attrId"></param>
        /// <param name="offset"></param>
        public void AddAttribute(T attrId, double offset)
        {
            baseAttr.Modify(attrId, offset);
            AddChange(attrId);
        }

        /// <summary>
        /// 添加属性map
        /// </summary>
        /// <param name="dataMap"></param>
        public void AddAttributes(Dictionary<T, double> dataMap)
        {
            baseAttr.MergeData(dataMap);
            AddChange(dataMap.Keys.ToList());
        }

        /// <summary>
        /// 移除属性
        /// </summary>
        /// <param name="attrId"></param>
        /// <param name="offset"></param>
        public void RemoveAttribute(T attrId, double offset)
        {
            baseAttr.Modify(attrId, -offset);
            AddChange(attrId);
        }

        /// <summary>
        /// 移除属性map
        /// </summary>
        /// <param name="dataMap"></param>
        public void RemoveAttributes(Dictionary<T, double> dataMap)
        {
            foreach (var entry in dataMap)
            {
                RemoveAttribute(entry.Key, entry.Value);
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
            if (!Config.TwoLevelRelyOnEffectMap.TryGetValue(attrId, out Dictionary<T, double> relyEffect))
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

            var attrIdIndex = Convert.ToInt32(attrId);
            var valueAttr = (T)Enum.ToObject(ATTR_TYPE, attrIdIndex + GAttr.ATTR_ID_ABL);
            var percentAttr = (T)Enum.ToObject(ATTR_TYPE, attrIdIndex + GAttr.ATTR_ID_PCT);

            var value = baseAttr[valueAttr] * (1.0f + baseAttr[percentAttr] / 100.0f);
            return Math.Max(0, value);
        }
    }
}
