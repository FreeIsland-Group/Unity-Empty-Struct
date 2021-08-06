using UnityEngine;

namespace DefaultNamespace.Data.Struct
{
    public class SaveHero
    {
        public string nickname;
        public int id; // table id
        public int hero_id; // hero id
        public int slot = 0; // hero slot
        public int exp;
        public int hp_increase = 0;
        public int attack_increase = 0;
        public int defence_increase = 0;
        public int precise_increase = 0;
        public int evasion_increase = 0;

        public SaveHero SetData(int id, int exp, string nickname)
        {
            hero_id = id;
            this.exp = exp;
            this.nickname = nickname;

            return this;
        }
    }
}
