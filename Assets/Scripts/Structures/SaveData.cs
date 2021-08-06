using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Data.Struct;
using Random = UnityEngine.Random;

public class SaveData : BaseData
{
    // Sub Class Demo
    [Serializable]
    public class Troops
    {
        public class TroopItem
        {
            public int index; // 在中队中的 Index
            public int id = -1; // 军种 ID
            public string name = ""; // 名称
            public int number = 0; // 数量
            public int type = 0; // 类型？
            public string icon = null; // 军种 Sprite，可能为 null，则读默认的

            public TroopItem(int index)
            {
                this.index = index;
            }

            public void ResetValue(int id = -1, string name = "", int number = 0)
            {
                this.id = id;
                this.name = name;
                this.number = number;
                type = 0;
                icon = null;
            }
        }

        public string index; // 索引
        public int heroSlot = -1; // 英雄槽位
        public List<TroopItem> list = new List<TroopItem>();

        public Troops(string index)
        {
            this.index = index;

            for (int i = 0; i < 25; i++)
            {
                list.Add(new TroopItem(i));
            }
        }
    }
    
    public float lifetime = 0;
    public int ver; // 存档版本，对照 DM 里的 App 版本

    public override void ResetData()
    {
        ver = DataManager.instance.nowVer;
        DataManager.instance.dayEvent.Invoke();
    }

    // 新一回合
    public void NewTurn()
    {
        const float MONTH_DAYS = 30;
        
        DataManager.instance.dayEvent.Invoke();
    }
}
