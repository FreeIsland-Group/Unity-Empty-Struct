using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Data.Struct;
using Random = UnityEngine.Random;

public class SaveData : BaseData
{
    /**
     * 市场价格
     */
    public class MarketPrice
    {
        public bool canTrade;
        public float nowPrice;
        public float step;
        public float min;
        public int gap = 0;
        public List<float> historyPrice;

        public MarketPrice(bool canTrade, float nowPrice, float step, float minPrice = 0)
        {
            this.canTrade = canTrade;
            this.nowPrice = nowPrice;
            this.step = step;
            if (minPrice == 0)
            {
                minPrice = step * 12;
            }
            min = minPrice;
            historyPrice = new List<float> {nowPrice};
        }
        
        public void UpgradeSave(bool canTrade, float step, float minPrice = 0)
        {
            this.canTrade = canTrade;
            this.step = step;
            if (minPrice == 0)
            {
                minPrice = step * 12;
            }
            min = minPrice;

            UpgradeTurn();
        }
        
        // 价格波动
        public void NewPrice()
        {
            float priceChange;
            int rate = Random.Range(0, 100);
            if (rate > 90)
            {
                priceChange = 0;
            }
            else if (rate > 65)
            {
                priceChange = -step;
            }
            else if (rate > 40)
            {
                priceChange = step;
            }
            else if (rate > 26)
            {
                priceChange = -step * 2;
            }
            else if (rate > 12)
            {
                priceChange = step * 2;
            }
            else if (rate > 6)
            {
                priceChange = -step * Random.Range(3, 8);
            }
            else
            {
                priceChange = step * Random.Range(3, 8);
            }

            nowPrice += priceChange;
            if (nowPrice < step)
            {
                nowPrice = step * 3f;
            } else if (nowPrice < min)
            {
                nowPrice += step * 1.5f;
            } else if (nowPrice > min * 10)
            {
                nowPrice -= step * 1.5f;
            }
        }
        
        // 加价格变化值
        public void SumPrice(float priceChange)
        {
            nowPrice += priceChange;
        }

        // 更新价格
        public void UpgradeTurn()
        {
            if (canTrade && gap-- <= 0)
            {
                gap = 2;
                NewPrice();
                historyPrice.Add(nowPrice);
                if (historyPrice.Count >= 48) historyPrice.Remove(historyPrice.First());
            }
        }
    }

    // 冒险信息
    [Serializable]
    public class AdventureInfo
    {
        public int lockID = 0; // 锁住的冒险ID
        public int adventureID; // 冒险ID，-1 表示不在冒险
        public int allDistance; // 城市距离战斗的总距离
        
        public NowStatus nowStatus; // 部队的状态
        public int nowDistance; // 部队行进的距离
        
        public SentryAim sentryAim; // 传讯兵的目的
        public int sentryDistance; // 传讯兵行进的距离
        
        public string logs; // 旅行日志
        public enum SentryAim
        {
            Empty, // 无
            Back, // 撤退
            Attack, // 进攻
            Sleep // 原地休整，等候命令
        }
        public enum NowStatus
        {
            Attack, // 进攻
            Back, // 撤退
            Sleep // 原地休整，等候命令
        }

        public AdventureInfo(int distance, string logs, int adventureID)
        {
            nowStatus = NowStatus.Attack;
            sentryAim = SentryAim.Empty;
            sentryDistance = 0;
            nowDistance = 0;
            allDistance = distance;
            this.logs = logs;
            this.adventureID = adventureID;
        }

        public void ResetData()
        {
            sentryDistance = 0;
            nowDistance = 0;
            allDistance = 0;
            logs = "历史等待书写……";
            adventureID = -1;
        }

        public void SetStart(int id)
        {
            Dictionary<string, int> time = DataManager.instance.saveData.time;
            
            nowStatus = NowStatus.Attack;
            sentryAim = SentryAim.Empty;
            sentryDistance = 0;
            nowDistance = 0;
            allDistance = DataManager.instance.globalExcel.adventure[id].distance;
            logs = time["year"] + "年" + time["month"] + "月" + time["day"] + "日，部队出发了。\n";
            adventureID = id;
        }
    }

    /**
     * 战队编组
     */
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

    // 存档统计
    public float lifetime = 0;
    public int heroExtract = 0; // 英雄总抽取次数
    public int heroExtractByAD = 0; // 英雄 AD 抽取数量
    public int fightWin = 0; // 胜利次数
    
    public int ver; // 存档版本，对照 DM 里的 App 版本
    public int tutorial; // 所处教程步骤
    // 英雄
    public int expPool; // 经验池

    // 领地信息
    public float human; // 居民
    public float vagrantBuilding; // 百分小数流浪汉（流民）
    public int shelter; // 居民住所
    public int area; // 总领土
    public int areaIdle; // 空地
    public bool policyVagrant; // 政策：流民准入
    public bool policyPlannedParenthood; // 政策：计划生育

    // 领地比率
    public float childRate; // 自然生育率
    public float childAbortionRate; // 自然夭折率
    public float deathRate; // 自然死亡率
    public float deathAid; // 死亡真实减免
    public float abortionAid; // 婴儿夭折真实减免
    public float rebellion; // 反动情绪
    public float disease; // 环境肮脏程度
    public int isStarve; // 饥荒级别
    public AdventureInfo adventure;

    // 时间与统计
    public Dictionary<string, int> time = new Dictionary<string, int>();
    public float seasonHappen;
    public Dictionary<string, int> chartDataMoney = new Dictionary<string, int>();
    public Dictionary<string, int> chartDataPeople = new Dictionary<string, int>();
    public Dictionary<string, int> chartDataBaby = new Dictionary<string, int>();
    public float monthStatisticBaby; // 月新生儿
    public float newPeople;
    public float foodConsume;
    public float moneyArmyConsume;
    public float breadArmyConsume;
    public float moneyBuildConsume;
    public int coolCollection; // 募捐冷却天数

    // 资源
    public Dictionary<string, float> resource = new Dictionary<string, float>();
    public Dictionary<string, int> resourceDefaultMax = new Dictionary<string, int>();

    public Dictionary<string, MarketPrice> market = new Dictionary<string, MarketPrice>(); // 交易资源

    public int[] building = // 建筑
    {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    };

    public int armyShelter = 0; // 军队床位
    public int[] armyInactive = // 军队 - 空闲
    {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    };
    public int[] armyActive = // 军队-在编
    {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    };
    public Dictionary<string, Troops> armyTeam = new Dictionary<string, Troops>(); // 军队-阵型(在编)
    public int nowTeam = 0; // 当前主战战队
    public int team = 0; // 战队列表

    public enum SEASON_ENUM
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }

    [NonSerialized] public readonly Dictionary<string, string> ENGLISH_TO_TEXT = new Dictionary<string, string>();
    private BuildingItemFormat[] buildData;
    private ArmyItemFormat[] armyData;
    private Dictionary<int, int> effect = new Dictionary<int, int>();

    public SaveData()
    {
        ENGLISH_TO_TEXT.Add("armyShelter", "驻军上限");
        ENGLISH_TO_TEXT.Add("money", "金币");
        ENGLISH_TO_TEXT.Add("bread", "食物");
        ENGLISH_TO_TEXT.Add("wood", "木料");
        ENGLISH_TO_TEXT.Add("stone", "石材");
        ENGLISH_TO_TEXT.Add("skin", "皮革");
        ENGLISH_TO_TEXT.Add("ore", "铁矿");
        ENGLISH_TO_TEXT.Add("iron", "铁锭");
        ENGLISH_TO_TEXT.Add("steel", "钢锭");
        ENGLISH_TO_TEXT.Add("porcelain", "瓷器");
        ENGLISH_TO_TEXT.Add("fund", "发展基金");
        ENGLISH_TO_TEXT.Add("dragon", "龙鳞");
        ENGLISH_TO_TEXT.Add("point", "古钱币");
        ENGLISH_TO_TEXT.Add("area", "领地");
        ENGLISH_TO_TEXT.Add("human", "居民");
        
        buildData = DataManager.instance.globalExcel.building;
        armyData = DataManager.instance.globalExcel.army;
        ResetData();
    }

    public override void ResetData()
    {
        ver = DataManager.instance.nowVer;
        // 定义军队和冒险
        for (int i = 0; i < armyInactive.Length; i++) armyInactive[i] = 0;
        for (int i = 0; i < armyActive.Length; i++) armyActive[i] = 0;
        armyTeam.Clear();
        armyTeam.Add("adventure", new Troops("adventure"));
        adventure = new AdventureInfo(0, "", -1);

        // 定义建筑
        for (int i = 0; i < building.Length; i++) building[i] = 0;

        // 初始送建筑
        building[0] = 2;
        building[8] = 2;
        building[9] = 1;
        human = 5;
        
        // 定义市场
        market.Clear();
        market.Add("bread", new MarketPrice(true, 1f, .01f, .15f));
        market.Add("wood", new MarketPrice(true, 1f, .01f, .2f));
        market.Add("stone", new MarketPrice(true, 2f, .02f, .4f));
        market.Add("ore", new MarketPrice(true, 1.5f, .01f, .3f));
        market.Add("iron", new MarketPrice(true, 10f, .08f, 2f));
        market.Add("skin", new MarketPrice(true, 300f, 1f, 60f));
        market.Add("fund", new MarketPrice(true, 150f, 5f, 30f));
        
        // 定义时间
        time.Clear();
        time.Add("year", Random.Range(13, 40));
        time.Add("month", 7);
        time.Add("day", 22);
        time.Add("season", 1);
        seasonHappen = 0; // 本季已经走过的时间比，最高 100
        UpgradeSeason();

        // 定义资源
        resource.Clear();
        resourceDefaultMax.Clear();
        shelter = 6;
        resourceDefaultMax.Add("shelter", 6);
        resourceDefaultMax.Add("armyShelter", 4);

        resource.Add("money", Random.Range(55, 65));
        resource.Add("moneyMax", 999999);
        resourceDefaultMax.Add("money", 999999);

        resource.Add("wood", 9);
        resource.Add("woodMax", 90);
        resourceDefaultMax.Add("wood", 90);

        resource.Add("stone", 5);
        resource.Add("stoneMax", 90);
        resourceDefaultMax.Add("stone", 90);

        resource.Add("bread", 15);
        resource.Add("breadMax", 90);
        resourceDefaultMax.Add("bread", 90);
        resource.Add("lastBreadCost", 0);

        resource.Add("ore", 0);
        resource.Add("oreMax", 90);
        resourceDefaultMax.Add("ore", 90);

        resource.Add("iron", 0);
        resource.Add("ironMax", 90);
        resourceDefaultMax.Add("iron", 90);

        resource.Add("steel", 0);
        resource.Add("steelMax", 90);
        resourceDefaultMax.Add("steel", 90);

        resource.Add("skin", 1);
        resource.Add("skinMax", 90);
        resourceDefaultMax.Add("skin", 90);

        resource.Add("porcelain", 0);
        resource.Add("porcelainMax", 80);
        resourceDefaultMax.Add("porcelain", 80);

        resource.Add("fund", 0);
        resource.Add("fundMax", 9999999);
        resourceDefaultMax.Add("fund", 9999999);

        resource.Add("point", 5);
        resource.Add("pointMax", 195000);
        resourceDefaultMax.Add("point", 195000);

        // 其他数据的刷新
        tutorial = 0;
        expPool = 0;
        vagrantBuilding = 0;
        childRate = .0288f;
        childAbortionRate = .65f;
        deathRate = .006f;
        deathAid = 0;
        abortionAid = 0;
        rebellion = .7f;
        disease = 1.7f;
        isStarve = 0;
        area = 420;
        areaIdle = 420;
        policyVagrant = false;
        policyPlannedParenthood = false;
        monthStatisticBaby = 0;
        newPeople = 0;
        foodConsume = 0;
        moneyArmyConsume = 0;
        breadArmyConsume = 0;
        moneyBuildConsume = 0;
        coolCollection = 0;
        
        BuildingRefresh();
        DataManager.instance.dayEvent.Invoke();
    }

    // 新一回合
    public void NewDay()
    {
        const float MONTH_DAYS = 30;
        newPeople = 0;
        foodConsume = 0;
        moneyArmyConsume = 0;
        breadArmyConsume = 0;
        moneyBuildConsume = 0;
        
        // 资源安全性校验
        foreach (var item in resource)
        {
            if (Double.IsNaN(item.Value))
            {
                resource[item.Key] = 0;
            }
        }

        // 时间更新
        time["day"]++;
        if (time["day"] % 3 == 1)
        {
            DataManager.instance.StoreSave();
        }
        if (time["day"] > MONTH_DAYS)
        {
            time["day"] = 1;
            time["month"]++;
            if (time["month"] > 12)
            {
                time["month"] = 1;
                time["year"]++;
            }

            // 时间迭代完成后的操作
            UpgradeSeason();

            if (chartDataMoney.Count >= 12) chartDataMoney.Remove(chartDataMoney.First().Key);
            if (chartDataMoney.ContainsKey($"{time["month"]}月"))
            {
                chartDataMoney.Remove($"{time["month"]}月");
            }
            chartDataMoney.Add($"{time["month"]}月", (int)resource["money"]);
            
            if (chartDataPeople.Count >= 12) chartDataPeople.Remove(chartDataPeople.First().Key);
            if (chartDataPeople.ContainsKey($"{time["month"]}月"))
            {
                chartDataPeople.Remove($"{time["month"]}月");
            }
            chartDataPeople.Add($"{time["month"]}月", (int)human);
            
            if (chartDataBaby.Count >= 12) chartDataBaby.Remove(chartDataBaby.First().Key);
            if (chartDataBaby.ContainsKey($"{time["month"]}月"))
            {
                chartDataBaby.Remove($"{time["month"]}月");
            }
            chartDataBaby.Add($"{time["month"]}月", (int)monthStatisticBaby);
            
            if (time["month"] == 8)
            {
                int money = Random.Range(50, 75);
                DataManager.instance.SetRemind(time["year"] + "年的王国农业补贴到账啦！共计" + money + "金币。");
                resource["money"] += money;
            }
        
            DataManager.instance.monthEvent.Invoke();
        }

        // 时间衍生数据的更新
        seasonHappen = ((time["month"] - 1 - time["season"] * 3) * 30 + time["day"]) / (3 * MONTH_DAYS);

        BuildingProduct(); // 建筑生产
        BuildingRefresh(); // 建筑固定
        BuildingConsumeRefresh(); // 建筑维护
        ArmyConsumeRefresh(); // 军队维护
        GoodsRefresh(); // 市场
        AdventureRefresh(); // 冒险
        CityRefresh(); // 城市本身，人口、环境等
        
        DataManager.instance.dayEvent.Invoke();
    }

    private void UpgradeSeason()
    {
        switch (time["month"])
        {
            case 2: // 春
            case 3:
            case 4:
                time["season"] = 0;
                break;
            case 5:
            case 6:
            case 7:
                time["season"] = 1;
                break;
            case 8: // 秋
            case 9:
            case 10:
                time["season"] = 2;
                break;
            case 11:
            case 12:
            case 1:
                time["season"] = 3;
                break;
        }
    }

    // 建筑的固化效益刷新
    public void BuildingRefresh()
    {
        int hasArmy = 0;
        foreach (int i in armyActive) { hasArmy += i; }
        foreach (int i in armyInactive) { hasArmy += i; }
        
        // abortionAid
        float theEffect = buildData[7].effect[0];
        if (building[7] > 10)
        {
            theEffect = buildData[7].effect[0] * (building[7] / (144 + building[7] * 2) + 1);
        }
        abortionAid = building[7] * theEffect;
        
        // shelter
        effect = new Dictionary<int, int>()
        {
            {0, 0},
            {1, 0},
            {2, 0},
            {3, 0},
            {4, 0},
        };
        shelter = resourceDefaultMax["shelter"];
        foreach (var i in effect)
            shelter += (int) (building[i.Key] * buildData[i.Key].effect[i.Value]);
        
        // armyShelter
        effect = new Dictionary<int, int>()
        {
            {38, 0},
            {39, 0},
        };
        armyShelter = resourceDefaultMax["armyShelter"] - hasArmy;
        foreach (var i in effect)
            armyShelter += (int) (building[i.Key] * buildData[i.Key].effect[i.Value]);
        
        // bread
        effect = new Dictionary<int, int>()
        {
            {0, 1},
            {1, 1},
            {2, 1},
            {3, 1},
            {4, 1},
            {11, 0},
            {12, 0},
            {13, 0},
        };
        resource["breadMax"] = resourceDefaultMax["bread"];
        foreach (var i in effect)
            resource["breadMax"] += building[i.Key] * buildData[i.Key].effect[i.Value];

        // money
        effect = new Dictionary<int, int>()
        {
            {42, 0},
        };
        resource["moneyMax"] = resourceDefaultMax["money"];
        foreach (var i in effect)
        {
            resource["moneyMax"] += building[i.Key] * buildData[i.Key].effect[i.Value];
        }

        // wood
        effect = new Dictionary<int, int>()
        {
            {18, 0},
            {19, 0},
            {20, 0},
        };
        resource["woodMax"] = resourceDefaultMax["wood"];
        foreach (var i in effect)
            resource["woodMax"] += building[i.Key] * buildData[i.Key].effect[i.Value];
        
        // stone
        effect = new Dictionary<int, int>()
        {
            {24, 0},
            {25, 0},
            {26, 0},
        };
        resource["stoneMax"] = resourceDefaultMax["stone"];
        foreach (var i in effect)
            resource["stoneMax"] += building[i.Key] * buildData[i.Key].effect[i.Value];
        
        // ore
        effect = new Dictionary<int, int>()
        {
            {30, 0},
            {31, 0},
            {32, 0},
        };
        resource["oreMax"] = resourceDefaultMax["ore"];
        foreach (var i in effect)
            resource["oreMax"] += building[i.Key] * buildData[i.Key].effect[i.Value];
        
        // iron
        effect = new Dictionary<int, int>()
        {
            {36, 0},
        };
        resource["ironMax"] = resourceDefaultMax["iron"];
        foreach (var i in effect)
            resource["ironMax"] += building[i.Key] * buildData[i.Key].effect[i.Value];
        
        // skin
        effect = new Dictionary<int, int>()
        {
            {37, 0},
        };
        resource["skinMax"] = resourceDefaultMax["skin"];
        foreach (var i in effect)
            resource["skinMax"] += building[i.Key] * buildData[i.Key].effect[i.Value];

        vagrantBuilding = (building[5] * buildData[5].effect[1]
                           + building[40] * buildData[40].effect[0]) / 100;
    }

    // 建筑的生产效益更新
    private void BuildingProduct()
    {
        // 季节 - food
        if (time["season"] != 2)
        {
            resource["bread"] += building[8] * buildData[8].effect[0];
        }
        if (time["season"] == 2)
        {
            resource["bread"] += building[9] * buildData[9].effect[0];
            resource["bread"] += building[10] * buildData[10].effect[0];
        }
        if (resource["bread"] > resource["breadMax"]) resource["bread"] = resource["breadMax"];
        
        // iron
        effect = new Dictionary<int, int>()
        {
            {35, 0},
            {34, 0},
            {33, 0},
        };
        foreach (var i in effect)
        {
            if (resource["iron"] >= resource["ironMax"]) break;
            if (building[i.Key] <= 0) continue;
            
            if (resource["ore"] >= building[i.Key] * buildData[i.Key].effect[0])
            {
                resource["ore"] -= building[i.Key] * buildData[i.Key].effect[0];
                resource["iron"] += building[i.Key] * buildData[i.Key].effect[1];
            } else if (resource["ore"] > 0)
            {
                float productRate = resource["ore"] / (building[i.Key] * buildData[i.Key].effect[0]);
                resource["ore"] = 0;
                resource["iron"] += building[i.Key] * buildData[i.Key].effect[1] *　productRate;
            }
        }
        if (resource["iron"] > resource["ironMax"]) resource["iron"] = resource["ironMax"];
        
        // wood
        effect = new Dictionary<int, int>()
        {
            {14, 0},
            {15, 0},
            {16, 0},
            {17, 0},
        };
        foreach (var i in effect)
            resource["wood"] += building[i.Key] * buildData[i.Key].effect[i.Value];
        if (resource["wood"] > resource["woodMax"]) resource["wood"] = resource["woodMax"];
        
        // stone
        effect = new Dictionary<int, int>()
        {
            {21, 0},
            {22, 0},
            {23, 0},
        };
        foreach (var i in effect)
            resource["stone"] += building[i.Key] * buildData[i.Key].effect[i.Value];
        if (resource["stone"] > resource["stoneMax"]) resource["stone"] = resource["stoneMax"];
        
        // skin
        if (resource["skin"] > resource["skinMax"]) resource["skin"] = resource["skinMax"];
        
        // ore
        effect = new Dictionary<int, int>()
        {
            {27, 0},
            {28, 0},
            {29, 0},
        };
        foreach (var i in effect)
            resource["ore"] += building[i.Key] * buildData[i.Key].effect[i.Value];
        if (resource["ore"] > resource["oreMax"]) resource["ore"] = resource["oreMax"];
        
        // money
        effect = new Dictionary<int, int>()
        {
            {5, 0},
        };
        foreach (var i in effect)
            resource["money"] += building[i.Key] * buildData[i.Key].effect[i.Value];
        if (resource["money"] > resource["moneyMax"]) resource["money"] = resource["moneyMax"];
    }
    
    // 建筑的回合消耗刷新
    private void BuildingConsumeRefresh()
    {
        foreach (var i in buildData)
            moneyBuildConsume += building[i.index] * i.moneyConsume;
        
        if (resource["money"] - moneyBuildConsume < 0)
        {
            moneyBuildConsume = resource["money"];
            resource["money"] = 0;
        }
        else
        {
            resource["money"] -= moneyBuildConsume;
        }
    }
    
    // 军队的回合消耗刷新
    private void ArmyConsumeRefresh()
    {
        // 金币 - 维护开支
        foreach (var i in armyData)
        {
            moneyArmyConsume += armyActive[i.index] * i.moneyConsume;
            moneyArmyConsume += armyInactive[i.index] * i.moneyConsume;
        }
        if (resource["money"] - moneyArmyConsume < 0)
        {
            moneyArmyConsume = resource["money"];
            resource["money"] = 0;
        }
        else
        {
            resource["money"] -= moneyArmyConsume;
        }
        // 食物 - 维护开支
        foreach (var i in armyData)
        {
            breadArmyConsume += armyActive[i.index] * i.breadConsume;
            breadArmyConsume += armyInactive[i.index] * i.breadConsume;
        }
        if (resource["bread"] - breadArmyConsume < 0)
        {
            breadArmyConsume = resource["bread"];
            resource["bread"] = 0;
        }
        else
        {
            resource["bread"] -= breadArmyConsume;
        }
    }

    // 市场价格刷新
    private void GoodsRefresh()
    {
        List<string> keyList = new List<string>(market.Keys);
        foreach (string itemKey in keyList)
        {
            market[itemKey].UpgradeTurn();
        }
        if (coolCollection > 0)
        {
            coolCollection -= 1;
        }
    }

    // 城市刷新（环境、税收、人口等）
    private void CityRefresh()
    {
        PeopleChange();
        // 税收
        resource["money"] += human * .12f;
        if (resource["money"] > resource["moneyMax"]) resource["money"] = resource["moneyMax"];
        if (resource["money"] < 0) resource["money"] = 0;
        // 环境变化
    }
    
    // 人口消耗和繁衍
    private void PeopleChange()
    {
        // 居民食物消耗
        float newBabyRate;
        if (resource["bread"] > human * .2f)
        {
            foodConsume = human * .2f;
            resource["bread"] -= foodConsume;
            newBabyRate = 1;
        } 
        else if (resource["bread"] > 0)
        {
            foodConsume = resource["bread"];
            resource["bread"] = 0;
            newBabyRate = resource["bread"] / .2f / human / 2; // 吃不饱，生育率减半
        }
        else
        {
            newBabyRate = 0;
        }
        
        // 军人计入生育率计算
        float people = human;
        foreach (int i in armyInactive) { people += i; }
        foreach (int i in armyActive) { people += i; }

        // 居民人口变动
        if (human > shelter) human = shelter;
        // 流民政策
        float vagrant = 0;
        if (newBabyRate > .92f && (policyVagrant || people < 6))
        {
            vagrant += Random.Range(.08f, .26f) + Random.Range(vagrantBuilding / 4, vagrantBuilding * 2);
        }
        // 新生儿
        float baby = 0;
        baby += people * newBabyRate * childRate;
        if (policyPlannedParenthood)
        {
            baby *= .33f;
        }
        float deadBaby = baby * childAbortionRate - abortionAid;
        if (deadBaby < 0) deadBaby = 0;
        baby -= deadBaby;
        if (baby < 0) baby = 0;
        
        // 加总：流民，自然死亡，新生儿
        monthStatisticBaby += baby;
        newPeople = vagrant - people * deathRate + baby;
        if (human + newPeople < 0)
        {
            newPeople = -human;
            human = 0;
        }
        else
        {
            human += newPeople;            
        }
    }

    // 冒险进度刷新
    private void AdventureRefresh()
    {
        if (adventure.adventureID == -1) return;
        
        // 传令兵是否有任务
        if (adventure.sentryAim != AdventureInfo.SentryAim.Empty)
        {
            adventure.sentryDistance += 2;
            if (adventure.sentryDistance >= adventure.nowDistance)
            {
                switch (adventure.sentryAim)
                {
                    case AdventureInfo.SentryAim.Back:
                        adventure.nowStatus = AdventureInfo.NowStatus.Back;
                        break;
                    case AdventureInfo.SentryAim.Sleep:
                        adventure.nowStatus = AdventureInfo.NowStatus.Sleep;
                        break;
                    case AdventureInfo.SentryAim.Attack:
                        adventure.nowStatus = AdventureInfo.NowStatus.Attack;
                        break;
                    default:
                        Debug.LogError("已无效的传令兵命令");
                        break;
                }
                adventure.sentryAim = AdventureInfo.SentryAim.Empty;
                adventure.sentryDistance = 0;
            }
        }

        switch (adventure.nowStatus)
        {
            case AdventureInfo.NowStatus.Attack:
                adventure.nowDistance += 1;
                AdventureAttackEvent();
                break;
            case AdventureInfo.NowStatus.Back:
                adventure.nowDistance -= 1;
                if (adventure.nowDistance <= 0)
                {
                    adventure.adventureID = -1;
                    // 解散军队
                    adventure.logs = time["year"] + "年" + time["month"] + "月" + time["day"] + "日，部队撤退归乡……\n" + adventure.logs;
                }
                AdventureAttackEvent(); // TODO 改为发生撤退事件
                // AdventureBackEvent();
                break;
            case AdventureInfo.NowStatus.Sleep:
                AdventureAttackEvent(); // TODO 改为发生休整事件
                // AdventureSleepEvent();
                break;
            default:
                Debug.LogError("已无效的部队状态。为保证士兵的生命安全，部队将进入撤退状态");
                adventure.nowStatus = AdventureInfo.NowStatus.Back;
                break;
        }
    }

    // 冒险进攻时的事件
    private void AdventureAttackEvent()
    {
        string info = "";
        int rate = Random.Range(0, 8);
        if (rate < 2)
        {
            info += "很温和的天气，很平静的路途，今日无事发生。";
        } else if (rate < 3)
        {
            info += "百无聊赖的士兵唱起了小调，淡淡的思乡愁绪弥漫在队伍中。";
        } else if (rate < 5)
        {
            info += "天阴沉沉的，雨滴落了下来。";
            int rate2 = Random.Range(0, 3);
            if (rate2 == 0)
            {
                info += "雨越下越大，士兵们被雨水淋得湿透，纷纷寻找避雨之处，队伍弥散而去。";
            }
            else if (rate2 == 1)
            {
                info += "雨越下越大，一位士兵被雷电劈中，军队四处炸散逃窜，混乱不堪。";
            }
            else
            {
                info += "这场雨刚刚揭开序幕，就消散无影了，太阳重新露出来，温暖的光束照射在土地上。";
            }
        }
        else
        {
            info += "阳光明媚，这贼老天晒得人心里发慌。";
        }

        adventure.logs = string.Format("{0}月，{1}\n", time["month"], info) + adventure.logs;
        if (time["day"] == 1 && time["month"] == 1)
        {
            adventure.logs = time["year"] + "年到来了，士兵们在家乡之外度过了新年，心态上有些疲惫。\n" + adventure.logs;
        }

        if (adventure.logs.Length > 650)
        {
            adventure.logs = adventure.logs.Substring(0, 400);
        }
    }
}
