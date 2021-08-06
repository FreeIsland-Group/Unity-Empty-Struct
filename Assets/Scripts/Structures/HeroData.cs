using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Data.Struct;
using UnityEngine;
using UnityEngine.Networking;

namespace DefaultNamespace.Data
{
    public class HeroData
    {
        public Hero[] heroes; // 拥有英雄列表
        public int normalColling; // 抽卡剩余 CD
        public int ADColling; // 广告抽卡剩余 CD heroCold

        public HeroData(List<int> colling, List<SaveHero> heroes = null)
        {
            normalColling = colling[0] > 0 ? colling[0] : 0;
            ADColling = colling[1] > 0 ? colling[1] : 0;
            
            this.heroes = new Hero[10];
            if (heroes != null)
            {
                foreach (var hero in heroes)
                {
                    this.heroes[hero.slot] = new Hero(hero);
                }
            }
        }

        public void RefreshHeroJob()
        {
            // 这里每次登录都会调用，在这刷新玩家的士兵状况
            for (int i = DataManager.instance.saveData.armyActive.Length - 1; i > 0; i--)
            {
                DataManager.instance.saveData.armyActive[i] = 0;
            }
            foreach (var item in DataManager.instance.saveData.armyTeam["adventure"].list)
            {
                if (item.id == -1) continue;
                DataManager.instance.saveData.armyActive[item.id] += item.number;
            }

            // 正常逻辑
            var advTeam = DataManager.instance.saveData.armyTeam["adventure"];
            if (advTeam.heroSlot == -1)
            {
                DataManager.instance.saveData.adventure.ResetData();
                return;
            }
            
            var heroI = getHeroBySlot(advTeam.heroSlot);
            if (heroI == null)
            {
                advTeam.heroSlot = -1;
                DataManager.instance.saveData.adventure.ResetData();
            }
            else
            {
                heroI.slot = advTeam.heroSlot;
            }
        }
        
        // 新一回合
        public void NewDay()
        {
            if (normalColling > 0) normalColling--;
            if (ADColling > 0) ADColling--;
        }

        // 添加英雄
        public Hero addHero(SaveHero heroObj)
        {
            for (int i = 0; i < heroes.Length; i++)
            {
                if (heroes[i] == null)
                {
                    heroes[i] = new Hero(heroObj);
                    return heroes[i];
                }
            }
            
            return null;
        }

        // 以槽位获取英雄
        public Hero getHeroBySlot(int slot)
        {
            foreach (var hero in heroes)
            {
                if (hero != null && hero.slot == slot)
                {
                    return hero;
                }
            }
            
            return null;
        }

        // 移除英雄用对象
        public IEnumerator RemoveHero(Hero heroObj)
        {
            UnityWebRequest www = DataManager.instance.HttpRequest(
                $"api/city/hero?slot={heroObj.slot}", UnityWebRequest.kHttpVerbDELETE);
            yield return www.SendWebRequest();

            if (DataManager.instance.UseHttpMiddleware(www))
            {
                for (int i = 0; i < heroes.Length; i++)
                {
                    if (heroes[i] != null && heroes[i].slot == heroObj.slot)
                    {
                        heroes[i] = null;
                    }
                }
            }

            www.Dispose();
        }

        // 移除英雄用槽位
        public IEnumerator RemoveHero(int slot, Action action = null)
        {
            var index = -1;
            for (int i = 0; i < heroes.Length; i++)
            {
                if (heroes[i] != null && heroes[i].slot == slot)
                {
                    index = i;
                }
            }
            if (index == -1) yield break;

            UnityWebRequest www = DataManager.instance.HttpRequest(
                $"api/city/hero?id={heroes[index].tableID}", UnityWebRequest.kHttpVerbDELETE);
            yield return www.SendWebRequest();

            if (DataManager.instance.UseHttpMiddleware(www))
            {
                heroes[index] = null;
            }

            action?.Invoke();

            www.Dispose();
        }

        public class Hero
        {
            public int id; // 英雄配置表的 ID
            public int slot;
            public int job; // 工作，-1 标识待业; 1 标识冒险
            public string name; // 昵称
            public int quality;

            public string qualityText
            {
                get
                {
                    switch (DataManager.instance.globalExcel.hero[id].quality)
                    {
                        case 1:
                            return "<color='#FFEECE'>子爵</color>";
                        case 2:
                            return "<color='#31CCFF'>伯爵</color>";
                        case 3:
                            return "<color='#ABAAFF'>侯爵</color>";
                        case 4:
                            return "<color='#FF9704'>公爵</color>";
                        case 5:
                            return "<color='#FF5C1C'>亲王</color>";
                        default:
                            DataManager.instance.SetAlert("Bug", "这英雄品质没见过啊……");
                            break;
                    }

                    return "Bug";
                }
            }

            public int level;
            public int exp;
            public int expAim;
            public int tableID;

            public float hp;
            public float attack;
            public float defence;
            public float precise;
            public float evasion;

            public readonly float hpIncrease;
            public readonly float attackIncrease;
            public readonly float defenceIncrease;
            public readonly float preciseIncrease;
            public readonly float evasionIncrease;

            public int propsA; // 装备 ID
            public int propsB;

            public Hero(SaveHero hero)
            {
                propsA = -1;
                propsB = -1;
                job = -1;
                level = 0;
                id = hero.hero_id;
                slot = hero.slot;
                tableID = hero.id;
                name = hero.nickname;
                exp = hero.exp;

                var excel = DataManager.instance.globalExcel;
                quality = excel.hero[id].quality;

                if (hero.hp_increase != 0)
                    hpIncrease = hero.hp_increase / 100f;     
                else
                    hpIncrease = excel.hero[id].hpIncrease;

                if (hero.attack_increase != 0)
                    attackIncrease = hero.attack_increase / 100f;     
                else
                    attackIncrease = excel.hero[id].attackIncrease;

                if (hero.defence_increase != 0)
                    defenceIncrease = hero.defence_increase / 100f;     
                else
                    defenceIncrease = excel.hero[id].defenceIncrease;

                if (hero.precise_increase != 0)
                    preciseIncrease = hero.precise_increase / 100f;     
                else
                    preciseIncrease = excel.hero[id].preciseIncrease;

                if (hero.evasion_increase != 0)
                    evasionIncrease = hero.evasion_increase / 100f;     
                else
                    evasionIncrease = excel.hero[id].evasionIncrease;

                expAim = 120;
                while (exp > expAim)
                {
                    level++;
                    expAim += Mathf.RoundToInt(100 * Mathf.Pow(1.2f, level + 1));
                }
                UpgradeLevel();
                if (name == "") name = excel.hero[id].title;
            }

            // 获得新的经验
            public IEnumerator GotExp(int exp, bool formPool = false)
            {
                WWWForm form = new WWWForm();
                form.AddField("exp", exp);
                form.AddField("id", tableID);
                
                UnityWebRequest www = DataManager.instance.HttpRequest(
                    $"api/city/hero/exp", UnityWebRequest.kHttpVerbPOST, form);
                yield return www.SendWebRequest();

                if (DataManager.instance.UseHttpMiddleware(www))
                {
                    this.exp += exp;
                    if (formPool)
                    {
                        DataManager.instance.saveData.expPool -= exp;                        
                    }
                    UpgradeLevel();
                    DataManager.instance.SetRemind("注入完成！");
                }
                else
                {
                    DataManager.instance.SetAlert("经验更新失败", "快去群里反馈给作者（Tap 论坛留言也行——不定期看）：" + www.error);
                }

                www.Dispose();
            }

            // 更新名字
            public IEnumerator Rename(string nickname)
            {
                WWWForm form = new WWWForm();
                form.AddField("name", nickname);
                form.AddField("id", tableID);
                
                UnityWebRequest www = DataManager.instance.HttpRequest(
                    $"api/city/hero", UnityWebRequest.kHttpVerbPUT, form);
                yield return www.SendWebRequest();

                if (DataManager.instance.UseHttpMiddleware(www))
                {
                    name = nickname;
                    DataManager.instance.SetRemind("修改完成！重启游戏生效");
                }

                www.Dispose();
            }

            // 升级校准
            private void UpgradeLevel()
            {
                var excel = DataManager.instance.globalExcel;
                if (exp > expAim)
                {
                    level++;
                    expAim += Mathf.RoundToInt(100 * Mathf.Pow(1.2f, level + 1));
                }

                hp = excel.hero[id].hp + hpIncrease * level;
                attack = excel.hero[id].attack + attackIncrease * level;
                defence = excel.hero[id].defence + defenceIncrease * level;
                precise = excel.hero[id].precise + preciseIncrease * level;
                evasion = excel.hero[id].evasion + evasionIncrease * level;
            }
        }
    }
}
