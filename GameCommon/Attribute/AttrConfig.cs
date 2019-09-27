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
        /// 一级属性列表
        /// </summary>
        public List<T> oneLevelAttrs;
        /// <summary>
        /// 一级属性影响因子，百分比值类型
        /// </summary>
        public Dictionary<T, Dictionary<T, long>> oneLevelRelationMap;

        /// <summary>
        /// 二级属性依赖属性
        /// </summary>
        public Dictionary<T, Dictionary<T, double>> twoLevelRelyOnEffectMap { get; set; }

        /// <summary>
        /// 是否一级属性
        /// </summary>
        /// <param name="attrId"></param>
        /// <returns></returns>
        public bool IsLevelOneAttr(T attrId)
        {
            return oneLevelAttrs.IndexOf(attrId) >= 0;
        }
    }
}
