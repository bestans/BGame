using System;
using System.Collections.Generic;
using System.Text;
using Bestan.Common;
using BGame.Common;

/*=============================================================================================== 
 Author   : yeyouhuan
 Created  : 2019/9/30 11:40:41
 Summary  : 属性测试
 ===============================================================================================*/
namespace TestNetCore
{
    public enum ATTR_ID
    {
        STRENGTH = 10,       //力量
        STRENGTH_ABL = 11,
        AGILITY = 20,       //敏捷
        AGILITY_ABL = 21,
        AGILITY_PCT = 22,
        PHYSICAL = 30,       //体质
        PHYSICAL_ABL = 31,  

        ATTACK_SPEED = 60,          // 攻击速度
        ATTACK_SPEED_ABL = 61,
        MAGIC_PROFICIENT = 70,      // 魔法精通
        MAGIC_PROFICIENT_ABL = 71,
        PHY_DEFEND = 80,            // 物理护甲
        PHY_DEFEND_ABL = 81,
        MAGIC_DEFEND = 90,          // 魔法防御
        MAGIC_DEFEND_ABL = 91,
        DODGE = 100,                // 闪避
        DODGE_ABL = 101,
        CRIT = 110,                 // 暴击
        CRIT_ABL = 111,
        CRIT_DEGREE = 120,          // 暴击伤害
        CRIT_DEGREE_ABL = 121,
        HP_MAX = 130,               // 生命上限
        HP = 140,                   // 当前生命值
        HP_RECOVER = 150,           // 生命回复
        KILL_HP_RECOVER = 160,      // 击杀回复生命
        ENERGY = 170,               // 能量值
        ENERGY_MAX = 180,           // 能量上限
        ENERGY_RECOVER = 190,       // 能量回复
        KILL_ENERGY_REVOVER = 200,  // 击杀能量回复
        DEFEND_TYPE = 210,          // 护甲类型
        MOVE_SPEED = 220,           // 移动速度
        CD_REDUCE = 230,            // CD减免
    }

    public class TestAttrBase : AttrBase<ATTR_ID>
    {
        public TestAttrBase(TestAttrConfig config) : base(config)
        {

        }

        protected override void OnRebuildFinish(Dictionary<ATTR_ID, AttrChangeUnit> changeMap, bool isInit)
        {
            base.OnRebuildFinish(changeMap, isInit);
            if (isInit)
            {
                return;
            }
            foreach (var it in changeMap)
            {
                Console.WriteLine("change=>{0}:old={1},new={2}", it.Key, it.Value.oldValue, it.Value.newValue);
            }
        }
    }
    public class TestAttrConfig : AttrConfig<ATTR_ID>
    {

    }

    public class InitAttr : BaseLuaConfig
    {
        public Dictionary<ATTR_ID, double> init_attr;
    }

    class TestAttribute
    {
        public static void test()
        {
            test1();
        }

        static void print(TestAttrBase tb, string desc)
        {
            Console.WriteLine();
            foreach (var it in tb.GetBaseAttribute().Data)
            {
                Console.WriteLine(desc + ":base:key={0},value={1}", it.Key, it.Value);
            }
            Console.WriteLine();
            foreach (var it in tb.GetFinalAttribute().Data)
            {
                Console.WriteLine(desc + ":final:key={0},value={1}", it.Key, it.Value);
            }
        }
        static void test1()
        {
            var config = LuaConfigs.LoadSingleConfig<TestAttrConfig>("test_bak.lua");
            config.WriteToFile("test.lua");
            Console.WriteLine(config.ToLuaString());
            var initattr = LuaConfigs.LoadSingleConfig<InitAttr>("init.lua");
            Console.WriteLine(config.ToString());
            Console.WriteLine(initattr.ToString());
            TestAttrBase tb = new TestAttrBase(config);
            tb.AddAttributes(initattr.init_attr);
            print(tb, "BEF");
            tb.Rebuild(true);
            print(tb, "AFT");

            Console.WriteLine("adddddddddddddddddddd");
            tb.AddAttribute(ATTR_ID.AGILITY_ABL, 1);
            tb.Rebuild();
            print(tb, "ADD1");
            tb.AddAttribute(ATTR_ID.STRENGTH_ABL, 1);
            tb.Rebuild();
            print(tb, "ADD2");

            Console.WriteLine("removeeeeeeeeeeeeeeeee");
            tb.RemoveAttribute(ATTR_ID.AGILITY_ABL, 1);
            tb.Rebuild();
            print(tb, "REM1");
            tb.RemoveAttribute(ATTR_ID.STRENGTH_ABL, 1);
            tb.Rebuild();
            print(tb, "REM2");

            Console.WriteLine("addlistttttttttttttttttt");
            var attList = new Dictionary<ATTR_ID, double> {
                { ATTR_ID.AGILITY_ABL, 1 },
                { ATTR_ID.STRENGTH_ABL, 1 },
            };
            tb.AddAttributes(attList);
            tb.Rebuild();
            print(tb, "addlist");

            Console.WriteLine("removelistttttttttttttttttt");
            tb.RemoveAttributes(attList);
            tb.Rebuild();
            print(tb, "removelist");
        }
    }
}
