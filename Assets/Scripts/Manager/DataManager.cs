using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DefaultNamespace.Data;
using DefaultNamespace.Data.Struct;
using Newtonsoft.Json;
using Prefab;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager instance = null;
    public Remind remind;
    public Alert scrollAlert;
    public Alert alert;
    public Confirm confirm;
    public InputConfirm inputConfirm;
    [NonSerialized] public HeroData heroData;
    [NonSerialized] public SaveData saveData;
    [NonSerialized] public ConfigData configData;
    [NonSerialized] public GlobalExcel globalExcel;
    public TextAsset jsonFile;
    public GameObject canvas = null;
    [NonSerialized] public int nowVer = 70;
    [NonSerialized] public string nowChannel = "TapTap";
    // [NonSerialized] public string nowChannel = "好游快爆";
    [NonSerialized] public float ONE_DAY = 2.3f;

    // 游戏内的选项
    [NonSerialized] public float turn = 0;
    [NonSerialized] public bool pause = true;
    [NonSerialized] public int armyTeamCount = 8;
    [NonSerialized] public Dictionary<string, bool> dayEventKeyList = new Dictionary<string, bool>();
    public UnityEvent dayEvent;
    [NonSerialized] public Dictionary<string, bool> monthEventKeyList = new Dictionary<string, bool>();
    public UnityEvent monthEvent;

    private float configSavingTime = 0;
    private float wantQuitTime = 0;
    private long focusTime = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            string configPath = Application.persistentDataPath + "/config.data";
            if (File.Exists(configPath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream file = File.Open(configPath, FileMode.Open);
                configData = JsonConvert.DeserializeObject<ConfigData>((string) formatter.Deserialize(file));
                file.Close();
            }
            else
            {
                configData = new ConfigData();
            }
#if UNITY_EDITOR
            configData.siteURL = "http://www.blog.com/";
            // configData.siteURL = "https://api.uiosun.com/";
#else
            configData.siteURL = "https://api.uiosun.com/";
#endif

            excel = JsonUtility.FromJson<Excel>(jsonFile.text);
            excel.RevertDictionary();
            saveData = new SaveData();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void OnApplicationFocus(bool pauseStatus)
    {
        if (pauseStatus)
        {
            focusTime = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() - focusTime;
            if (focusTime > 60 * 15)
            {
                // 超过十五分钟再回来，算作重新加载游戏
                Analytics.CustomEvent("SaveLoad", new Dictionary<string, object>
                {
                    {"userID", configData.userID},
                    {"lifetime", configData.lifetime},
                    {"onceLifetime", saveData.lifetime},
                });
            }
        }
        else
        {
            focusTime = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        }
    }

    // 通用 403 消息体
    class ForbiddenMessage
    {
        public string message;
    }

    // HTTP 请求中间件
    public bool UseHttpMiddleware(UnityWebRequest request)
    {
        Debug.Log(request.error);
        Debug.Log(request.downloadHandler.text);
        if (!request.isDone || request.isNetworkError || request.isHttpError)
        {
            if (request.error.Contains("401"))
            {
                configData.userToken = "";
                configData.username = "";
                configData.point = 0;
                configData.userID = 0;
                SaveConfig();
                if (SceneManager.GetActiveScene().name != "Main")
                {
                    SetAlert("登录状态失效",
                        "请登录一个吧，辛苦啦！\n" +
                        "\n如果你感觉登录状态失效的特别快，请赶紧告诉我们。\n\n备注：“告诉我们”就能解决问题？谁说的，荒谬，我们只是很擅长拿程序猿祭天。:)", () =>
                        {
                            SceneManager.LoadScene("Main");
                        });
                }
                Debug.Log("401 中间件拦截：" + request.error);
            }
            else if (request.error.Contains("403"))
            {
                Debug.Log("403 中间件拦截：" + request.error);
                var i = JsonConvert.DeserializeObject<ForbiddenMessage>(request.downloadHandler.text);
                SetRemind(i.message);
            }
            else if (request.error.Contains("426"))
            {
                Debug.Log("426 中间件拦截：" + request.error);
                SetAlert("该升级啦！", "朋友，快去" + nowChannel + "下载最新版吧，你当前的版本已经不被支持了。");
            }
            else
            {
                SetAlert("服务器炸啦！", "很沉重的说，服务器报错了：" + request.error + request.downloadHandler.text + "\n请截图告知程序猿哦，大概会有补偿吧——我猜的");
            }

            return false;
        }

        return true;
    }

    // HTTP 请求生成器
    public UnityWebRequest HttpRequest(string URI, string method = UnityWebRequest.kHttpVerbGET, WWWForm form = null)
    {
        UnityWebRequest request = new UnityWebRequest();
        request.method = method;
        request.url = configData.siteURL + URI;
        var urlParse = new Uri(request.url);
        if (urlParse.Query == "")
            request.url = request.url + "?v=" + nowVer;
        else
            request.url = request.url + "&v=" + nowVer;
        if (form != null)
        {
            if (method == UnityWebRequest.kHttpVerbPOST || method == UnityWebRequest.kHttpVerbPUT)
            {
                request.uploadHandler = (UploadHandler) new UploadHandlerRaw(form.data);
                request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            }
            else
            {
                Debug.LogError(method + "方法不适合添加 Form 表单");
            }
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        request.useHttpContinue = false;
        request.redirectLimit = 0;
        request.timeout = 30;

        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + configData.userToken);

        // obsolete
        request.chunkedTransfer = false;

        return request;
    }

    void Start()
    {
        Debug.Log(Application.persistentDataPath);
        if (dayEvent == null)
            dayEvent = new UnityEvent();
        if (monthEvent == null)
            monthEvent = new UnityEvent();
    }

    void Update()
    {
        if (wantQuitTime > 0) wantQuitTime -= Time.deltaTime;
        if (Input.GetKey(KeyCode.Escape)) {
            if (wantQuitTime > 0) {
                Application.Quit();
            }
            wantQuitTime = 1;
        }

        if (configSavingTime > 0)
        {
            configSavingTime -= Time.deltaTime;
            if (configSavingTime <= 0) SaveConfig(true);
        }

        if (!pause || SceneManager.GetActiveScene().name == "Fight")
        {
            configData.lifetime += Time.deltaTime;
            saveData.lifetime += Time.deltaTime;
        }

        if (pause) return;

        turn += Time.deltaTime / ONE_DAY;
        if (turn < 1) return;

        turn = 0;
        saveData.NewDay();
        heroData.NewDay();
    }

    // 注册每日触发的观察者
    public void RegisterDayEvent(string key, UnityAction RefreshAction)
    {
        if (!dayEventKeyList.ContainsKey(key))
        {
            dayEventKeyList[key] = true;
            dayEvent.AddListener(RefreshAction);
        }
        else
        {
            Debug.LogError("事件重复注册！");
        }
    }

    // 注册每月触发的观察者
    public void RegisterMonthEvent(string key, UnityAction RefreshAction)
    {
        if (!monthEventKeyList.ContainsKey(key))
        {
            monthEventKeyList[key] = true;
            monthEvent.AddListener(RefreshAction);
        }
        else
        {
            Debug.LogError("事件重复注册！");
        }
    }

    // 新建存档
    public void InitSave()
    {
        saveData = new SaveData();
        saveData.ResetData();

        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }

        FileStream file;
        string jsonData;
        BinaryFormatter formatter = new BinaryFormatter();
        string savePath = Application.persistentDataPath + "/save.data";
        string configPath = Application.persistentDataPath + "/config.data";
        if (!File.Exists(configPath))
        {
            file = File.Create(configPath);
            jsonData = JsonConvert.SerializeObject(configData, Formatting.Indented);
            formatter.Serialize(file, jsonData);

            file.Close();
        }

        saveData.ver = nowVer;
        file = File.Create(savePath);
        jsonData = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        formatter.Serialize(file, jsonData);
        file.Close();
        instance.heroData.RefreshHeroJob();

        Analytics.CustomEvent("SaveInit", new Dictionary<string, object>
        {
            {"userID", configData.userID},
            {"lifetime", configData.lifetime},
        });
    }

    // 加载存档
    public void LoadSave()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string savePath = Application.persistentDataPath + "/save.data";
        if (File.Exists(savePath))
        {
            FileStream file = File.Open(savePath, FileMode.Open);
            saveData = JsonConvert.DeserializeObject<SaveData>((string) formatter.Deserialize(file));
            file.Close();
            saveData.ver = nowVer;
            if (!saveData.market["fund"].canTrade || saveData.market["fund"].min == 600f)
            {
                saveData.market["bread"].UpgradeSave(true, .01f, .15f);
                saveData.market["wood"].UpgradeSave(true, .01f, .2f);
                saveData.market["stone"].UpgradeSave(true, .02f, .4f);
                saveData.market["ore"].UpgradeSave(true, .01f, .3f);
                saveData.market["iron"].UpgradeSave(true,.08f, 2f);
                saveData.market["skin"].UpgradeSave(true, 1f, 60f);
                saveData.market["fund"].UpgradeSave(true, 5f, 30f);
            }
            heroData.RefreshHeroJob();

            Analytics.CustomEvent("SaveLoad", new Dictionary<string, object>
            {
                {"userID", configData.userID},
                {"lifetime", configData.lifetime},
                {"onceLifetime", saveData.lifetime},
            });

            return;
        }

        InitSave();
    }

    // 存储存档
    public void StoreSave()
    {
        Debug.Log("保存存档");
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.data");
        var jsonData = JsonConvert.SerializeObject(saveData, Formatting.Indented)
            .Replace("\n", "")
            .Replace("\r", "")
            .Replace(" ", "");

        formatter.Serialize(file, jsonData);

        file.Close();
    }

    // 手动存储配置文件
    public void SaveConfig(bool isSave = false)
    {
        // 延迟且唯一操作
        if (!isSave)
        {
            configSavingTime = 2;
            return;
        }
        Debug.Log("保存配置");

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/config.data");
        var jsonData = JsonConvert.SerializeObject(configData, Formatting.Indented)
            .Replace("\n", "")
            .Replace("\r", "")
            .Replace(" ", "");
        formatter.Serialize(file, jsonData);

        file.Close();
    }
}
