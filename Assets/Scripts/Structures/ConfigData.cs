public class ConfigData : BaseData
{
    public float music = 0.6f;
    public float sound = 0.6f;

    public string siteURL;
    public string userToken = "";
    public string username = "";

    public int point = 0;
    public int userID = 0;
    public int cloudSaveVer = 0;
    public string cloudSave;
    public long checkUpdate = 0;

    // 统计：玩家的
    public bool firstHeroBar = true; // 玩家的首个英雄酒馆
    public bool firstAdventure = true; // 玩家的首次冒险
    public bool firstHero = true; // 玩家的首次
    public bool firstHeroFiveStar = true; // 玩家的首次
    public float lifetime = 0; // 总游戏时长
    public int heroExtract = 0; // 英雄总抽取次数
    public int heroExtractByAD = 0; // 英雄 AD 抽取数量
    public int fightWin = 0; // 胜利次数

    public override void ResetData()
    {
        music = 0.6f;
        sound = 0.6f;
    }
}
