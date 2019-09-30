using System;
using System.Collections.Generic;
using System.Text;
using Bestan.Common;

/*=============================================================================================== 
 Author   : yeyouhuan
 Created  : 2019/9/25 16:03:05
 Summary  : 属性公共操作
 ===============================================================================================*/
namespace BGame.Common
{
    public class AttrManager<T> : SingletonProvider<AttrManager<T>> where T :Enum
    {
        public static readonly Type ATTR_TYPE = typeof(T);

        public delegate void OnAttrChange(AttrBase<T> attrData, T attrid, AttrChangeUnit changeInfo, bool isInit);
        protected Dictionary<T, OnAttrChange> rebuildFinishHandleMap = new Dictionary<T, OnAttrChange>();

        public void RegisterRebuildFinish(T attrId, OnAttrChange handle)
        {
            rebuildFinishHandleMap[attrId] = handle;
        }

        public void OnRebuildFinish(AttrBase<T> attrData, Dictionary<T, AttrChangeUnit> changeMap, bool isInit)
        {
            foreach (var entry in rebuildFinishHandleMap)
            {
                if (changeMap.TryGetValue(entry.Key, out AttrChangeUnit changeInfo))
                {
                    entry.Value(attrData, entry.Key, changeInfo, isInit);
                }
            }
        }
    }
}
