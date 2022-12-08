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
        public int multi_value { get; private set; }       // 额外倍率，万分比
        public int base_value { get; private set; }        // 基础数值
        public SkillType type { get; private set; }        // 技能类型
        public SkillAim aim { get; private set; }          // 技能对象
        public bool IsCrit { get; private set; }

        // 技能生效函数
        public void SkillEffect(Character cha, Character other)
        {
            cha.MpCost(mpcost);
            Console.WriteLine($"{cha.name}对{other.name}使用了{name}！");
            float skillnumber = base_value + (float)(cha.attack * multi_value) / 10000;
            if (IsCrit)
            {
                if (Program.random.Next(1, 101) <= cha.critChance)
                {
                    Console.WriteLine("暴击！");
                    skillnumber = skillnumber * cha.critDamage / 10000;
                }
            }
            switch (type)
            {
                case SkillType.HpDamage:
                    {
                        int damage = (int)(skillnumber * 300 / (300 + other.def));
                        Console.WriteLine($"{cha.name}对{other.name}造成了{damage}点伤害！");
                        other.hpcost(damage);
                    }
                    break;
                case SkillType.HpHeal:
                    {
                        int damage = (int)(skillnumber * 300 / (300 + other.def));
                        other.hpcost(damage);
                        cha.hpheal(damage);
                        Console.WriteLine($"{cha.name}吸收了{other.name}{damage}点生命值！");
                    }
                    break;
                case SkillType.MpDamage:
                    {
                        other.MpCost((int)skillnumber);
                        Console.WriteLine($"{cha.name}对{other.name}造成了{(int)skillnumber}点魔力损失！");
                    }
                    break;
                case SkillType.MpHeal:
                    {
                        other.MpCost((int)skillnumber);
                        cha.mpheal((int)skillnumber);
                        Console.WriteLine($"{cha.name}吸收了{other.name}{(int)skillnumber}点魔力！");
                    }
                    break;
                case SkillType.HpRecover:
                    {
                        other.hpheal((int)skillnumber);
                        Console.WriteLine($"{cha.name}恢复了{other.name}{(int)skillnumber}点生命！");
                    }
                    break;
                case SkillType.MpRocover:
                    {
                        other.mpheal((int)skillnumber);
                        Console.WriteLine($"{cha.name}恢复了{other.name}{(int)skillnumber}点魔力！");
                    }
                    break;
            }
        }
        public Skill(string name, int mpcost, int multi_value, int base_value = 0, SkillType type = SkillType.HpDamage, SkillAim aim = SkillAim.EnemyBody, bool ifcrit = true)
        {
            this.name = name;
            this.mpcost = mpcost;
            this.multi_value = multi_value;
            this.base_value = base_value;
            this.type = type;
            this.aim = aim;
            this.IsCrit = ifcrit;
        }
    }
    class Character
    {
        public Skill NormalAttack = new Skill("普通攻击", 0, 10000);
        public string name { get; private set; }          // 角色名称
        public int mhp { get; private set; }
        public int hp { get; private set; }
        public int mmp { get; private set; }
        public int mp { get; private set; }
        public int attack { get; private set; }
        public int def { get; private set; }
        public int critChance { get; private set; }              //暴击率
        public int critDamage { get; private set; }           //暴击伤害，万分比
        public List<Skill> skills { get; private set; }    // 把技能加入列表中

        public bool alive = true;                          // 默认是活着的
        public void hpcost(int damage)
        {
            hp -= damage;
            if (hp <= 0)
            {
                alive = false;
            }
        }
        public void hpheal(int heal)
        {
            hp += heal;
            if (hp > mhp)
            {
                hp = mhp;
            }
        }

        // 扣魔力值函数
        public void MpCost(int damage)
        {
            mp -= damage;
            if (mp < 0) { mp = 0; }
        }
        public void mpheal(int heal)
        {
            mp += heal;
            if (mp > mmp)
            {
                mp = mmp;
            }
        }
        public Character(string name, int hp, int mp, int attack, int def, int critChance, int critDamage)
        {
            this.name = name;
            this.mhp = hp;
            this.hp = hp;
            this.mmp = mp;
            this.mp = mp;
            this.attack = attack;
            this.def = def;
            this.critChance = critChance;
            this.critDamage = critDamage;
            skills = new List<Skill>();
            skills.Add(NormalAttack);
            this.alive = true;
        }
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

        // 选择技能的函数
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
                if (i > skills.Count || i <= 0) { Console.WriteLine("不存在这个序号的技能！请从1开始输入："); }

                else if (skills[i - 1].mpcost > mp)
                {Console.WriteLine("魔力值不足以释放！请重新输入：");}

                else { break; }

            }
            return i - 1;
        }
    }
}
