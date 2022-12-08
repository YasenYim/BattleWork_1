using System;
using System.Collections.Generic;
using System.Text;

namespace BattleWork_1
{
    // 技能类型
    enum SkillType
    {
        HpDamage,
        HpHeal,
        MpDamage,
        MpHeal,
        HpRecover,
        MpRocover,
    }

    // 技能对象
    enum SkillAim
    {
        EnemyBody,
        MyBody,
    }

    // 技能类
    class Skill
    { 
        public string name { get; private set; }           // 技能名称
        public int mpcost { get; private set; }            // 魔力值
        public int base_value { get; private set; }        // 基础数值
        public int multi_value { get; private set; }       // 额外倍率，万分比
        public SkillType type { get; private set; }        // 技能类型
        public SkillAim aim { get; private set; }          // 技能对象
        public bool IsCrit { get; private set; }           // 判断技能是否是暴击伤害

        public Skill(string name, int mpcost, int multi_value, int base_value = 0, SkillType type = SkillType.HpDamage, SkillAim aim = SkillAim.EnemyBody, bool IsCrit = true)
        {
            this.name = name;
            this.mpcost = mpcost;
            this.multi_value = multi_value;
            this.base_value = base_value;
            this.type = type;
            this.aim = aim;
            this.IsCrit = IsCrit;
        }
        // 技能生效函数
        public void SkillEffect(Character cha, Character other)
        {
            // 玩家扣魔力
            cha.MpCost(mpcost);
            Console.WriteLine($"{cha.name}对{other.name}使用了{name}！");
            // 技能伤害计算
            float skillnumber = base_value + (float)(cha.attack * multi_value) / 10000;

            // 暴击判断（这段程序不能实现）
            if (IsCrit)
            {
                // 如果角色的crit大于等于100,产生暴击伤害
                if (Program.random.Next(1, 101) <= cha.crit)
                   {Console.WriteLine("暴击！");skillnumber = skillnumber * cha.critDamage / 10000; } 
            }

            // 技能分类
            switch (type)
            {
                // 血量伤害
                case SkillType.HpDamage:
                    {
                        int damage = (int)(skillnumber * 300 / (300 + other.def));
                        Console.WriteLine($"{cha.name}对{other.name}造成了{damage}点伤害！");
                        other.HpCost(damage);
                    }
                    break;
                // 血量恢复
                case SkillType.HpHeal:
                    {
                        int damage = (int)(skillnumber * 300 / (300 + other.def));
                        other.HpCost(damage);
                        cha.HpHeal(damage);
                        Console.WriteLine($"{cha.name}吸收了{other.name}{damage}点生命值！");
                    }
                    break;
                // 魔力伤害
                case SkillType.MpDamage:
                    {
                        other.MpCost((int)skillnumber);
                        Console.WriteLine($"{cha.name}对{other.name}造成了{(int)skillnumber}点魔力损失！");
                    }
                    break;
                // 魔力恢复
                case SkillType.MpHeal:
                    {
                        other.MpCost((int)skillnumber);
                        cha.MpHeal((int)skillnumber);
                        Console.WriteLine($"{cha.name}吸收了{other.name}{(int)skillnumber}点魔力！");
                    }
                    break;
                // 血量恢复
                case SkillType.HpRecover:
                    {
                        other.HpHeal((int)skillnumber);
                        Console.WriteLine($"{cha.name}恢复了{other.name}{(int)skillnumber}点生命！");
                    }
                    break;
                // 魔力值恢复
                case SkillType.MpRocover:
                    {
                        other.MpHeal((int)skillnumber);
                        Console.WriteLine($"{cha.name}恢复了{other.name}{(int)skillnumber}点魔力！");
                    }
                    break;
            }
        }


       
    }

    // 角色类
    class Character
    {
        public Skill NormalAttack = new Skill("普通攻击", 0, 10000);
        public string name { get; private set; }             // 角色名称
        public int mhp { get; private set; }
        public int hp { get; private set; }
        public int mmp { get; private set; }
        public int mp { get; private set; }
        public int attack { get; private set; }
        public int def { get; private set; }
        public int crit { get; private set; }                // 暴击率
        public int critDamage { get; private set; }          // 暴击伤害，万分比
        public List<Skill> skills { get; private set; }      // 把技能加入列表中

        public bool alive = true;                            // 默认是活着的

        // 角色构造函数
        public Character(string name, int hp, int mp, int attack, int def, int crit, int critDamage)
        {
            this.name = name;
            this.mhp = hp;
            this.hp = hp;
            this.mmp = mp;
            this.mp = mp;
            this.attack = attack;
            this.def = def;
            this.crit = crit;
            this.critDamage = critDamage;
            skills = new List<Skill>();
            skills.Add(NormalAttack);
            this.alive = true;
        }
        public void HpCost(int damage)
        {
            hp -= damage;
            if (hp <= 0)
            {
                alive = false;
            }
        }
        public void HpHeal(int heal)
        {
            hp += heal;
            if (hp > mhp)
            {
                hp = mhp;
            }
        }

        // 扣魔力值函数
        public void MpCost(int cost)
        {
            mp -= cost;
            if (mp < 0) { mp = 0; }
        }
        public void MpHeal(int heal)
        {
            mp += heal;
            if (mp > mmp)
            {
                mp = mmp;
            }
        }
       
        // 把技能添加到列表
        public void AddSkill(Skill skill)
        {
            skills.Add(skill);
        }

        // 打印角色信息
        public void PrintChar()
        {Console.WriteLine($"{name}：生命值{hp}/{mhp}，魔力{mp}/{mmp}");}

        // 打印角色技能函数
        public void PrintSkill()
        {
            for (int i = 0; i < skills.Count; i++)
            {Console.WriteLine($"技能{i + 1}：{skills[i].name}，魔力消耗：{skills[i].mpcost}");}
        }

        // 选择技能函数（选择技能并返回技能序号）
        public int SkillChoose()
        {
            int i = 0;
            while (true)
            {
                while (true)
                {
                    if (int.TryParse(Console.ReadLine(), out i)) { break; }
                    else { Console.WriteLine("请重新输入："); }
                }

                // 输入的数字从1开始同时不能大于技能的长度
                if (i > skills.Count || i <= 0) { Console.WriteLine("不存在这个序号的技能！请从1开始输入："); }

                // 如果技能需要花费的魔力值大于角色的魔力值，则不能使用消耗魔力的技能
                else if (skills[i - 1].mpcost > mp)
                {Console.WriteLine("魔力值不足以释放！请重新输入：");}

                else { break; }

            }
            // 返回技能的序号
            return i - 1;
        }
    }
}
