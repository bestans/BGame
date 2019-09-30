using System;
using System.Collections.Generic;
using System.Text;
using Bestan.Common;

/*=============================================================================================== 
 Author   : yeyouhuan
 Created  : 2019/9/25 16:18:11
 Summary  : 
 ===============================================================================================*/
namespace BGame.Common
{
    public class AttrConfig<T> : BaseLuaConfig where T : Enum
    {
        /// <summary>
        /// 一级属性影响因子
        /// [一级属性类型 : [二级属性类型 : 影响比例]]
        /// </summary>
        public Dictionary<T, Dictionary<T, double>> oneLevelRelationMap;

        /// <summary>
        /// 二级属性依赖属性（由oneLevelRelationMap计算而得）
        /// [二级属性类型 : [一级属性类型 : 影响比例]]
        /// </summary>
        public Dictionary<T, Dictionary<T, double>> TwoLevelRelyOnEffectMap { get; set; }
        /// <summary>
        /// 一级属性列表，由配置计算而得
        /// </summary>
        public List<T> OneLevelAttrs { get; set; }
        /// <summary>
        /// 是否一级属性
        /// </summary>
        /// <param name="attrId"></param>
        /// <returns></returns>
        public bool IsLevelOneAttr(T attrId)
        {
            return OneLevelAttrs.IndexOf(attrId) >= 0;
        }

        protected override void AfterLoad()
        {
            TwoLevelRelyOnEffectMap = new Dictionary<T, Dictionary<T, double>>();
            OneLevelAttrs = new List<T>();
            foreach (var one in oneLevelRelationMap)
            {
                foreach (var two in one.Value)
                {
                    var twoAttrId = two.Key;
                    if (!TwoLevelRelyOnEffectMap.TryGetValue(twoAttrId, out Dictionary<T, double> relyMap))
                    {
                        relyMap = new Dictionary<T, double>();
                        TwoLevelRelyOnEffectMap[twoAttrId] = relyMap;
                    }
                    relyMap[one.Key] = two.Value;
                }
                OneLevelAttrs.Add(one.Key);
            }
        }
    }
}
