using System;
using System.Collections.Generic;

namespace BattleWork_1
{
    class Program
    {
        public static Random random = new Random();

        // 玩家攻击敌人函数
        static void PlayerAttack(Character cha, Character other)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            // 打印角色技能
            cha.PrintSkill();

            // 请玩家输入数字
            Console.WriteLine("输入对应的技能序号释放技能：");

            // 技能选择，并返回技能序号
            int choose = cha.SkillChoose();
            Skill skill = cha.skills[choose];

            // 技能对象是敌人的时候
            if (skill.aim == SkillAim.EnemyBody)
               { skill.SkillEffect(cha, other); }  // 技能生效
            else { skill.SkillEffect(cha, cha); }

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        // 敌人攻击玩家函数
        static void EnemyAttack(Character cha, Character other)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            int a = 0;
            while (true)
            {
                // a是敌人的技能序号
                a = random.Next(0, cha.skills.Count);
                // 如果敌人使用技能需要扣除的魔力值小于玩家的魔力值
                if (cha.skills[a].mpcost < cha.mp) { break; }
            }
            // 敌人选择技能
            Skill skill = cha.skills[a];
            if (skill.aim == SkillAim.EnemyBody)
               { skill.SkillEffect(cha, other); }
            else { skill.SkillEffect(cha, cha); }

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        // 战斗过程
        static void Battle(Character player, Character other)
        {
            int round = 1;
            while (true)
            {
                Console.WriteLine($"-----第{round}回合-----");

                // 打印玩家和敌人的信息
                player.PrintChar();  
                other.PrintChar();

                // 玩家攻击敌人
                PlayerAttack(player, other);
                if (!other.alive)
                {Console.WriteLine($"{other.name}死亡！{player.name}胜利！");break;}

                // 敌人攻击玩家
                EnemyAttack(other, player);
                if (!player.alive)
                {Console.WriteLine($"{player.name}死亡！{other.name}胜利！");break;}

                round++;
            }
        }
        static void Main(string[] args)
        {
            // 玩家
            Character player = new Character("勇者", 1000, 100, 100, 100, 10, 20000);
            Skill skill1 = new Skill("强击术", 15, 30000, 100);
            Skill skill2 = new Skill("治疗术", 20, 20000, 200, SkillType.HpRecover, SkillAim.MyBody);
            Skill skill3 = new Skill("魔力之泉", 0, 0, 30, SkillType.MpRocover, SkillAim.MyBody, false);
            player.AddSkill(skill1);
            player.AddSkill(skill2);
            player.AddSkill(skill3);

            // 敌人
            Character enemy = new Character("魔王", 3000, 100, 100, 50, 5, 15000);
            Skill ekiill1 = new Skill("黑暗之拥", 10, 20000, 20);
            Skill eskill2 = new Skill("高王之渴", 10, 15000, 0, SkillType.HpHeal);
            Skill eskill3 = new Skill("恶魔灵气", 0, 0, 20, SkillType.MpHeal);
            enemy.AddSkill(ekiill1);
            enemy.AddSkill(eskill2);
            enemy.AddSkill(eskill3);

            // 战斗过程
            Battle(player, enemy);

            Console.ReadKey();
        }
    }
}
