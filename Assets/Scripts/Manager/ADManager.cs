using System;
using System.Collections.Generic;
using UnityEngine;
using ByteDance.Union;
using UnityEngine.Analytics;

public class ADManager : MonoBehaviour
{
    public static ADManager instance = null;
    public string category = "";
    private RewardVideoAd rewardAdMoney;
    private RewardVideoAd rewardAdHero;
    private RewardVideoAd rewardAdFightWin;
    private AdNative adNative;
    private Action callback = null;
#if UNITY_IOS
    private ExpressRewardVideoAd expressRewardAdMoney; // for iOS
    private ExpressRewardVideoAd expressRewardAdHero; // for iOS
    private ExpressRewardVideoAd expressRewardAdFightWin; // for iOS
#else

#endif
    
    private AdNative AdNative
    {
        get
        {
            if (this.adNative == null)
            {
                this.adNative = SDK.CreateAdNative();
            }
#if UNITY_ANDROID
            SDK.RequestPermissionIfNecessary();
#endif
            return this.adNative;
        }
    }

    /// <summary>
    /// Load the reward Ad.
    /// </summary>
    public void LoadRewardAdMoney()
    {
        Debug.LogError("尝试加载广告");
        if (rewardAdMoney != null)
        {
            rewardAdMoney.Dispose();
            rewardAdMoney = null;
        }

        string iosSlotID = "900546826";
        string AndroidSlotID = "946062458";
        var adSlot = new AdSlot.Builder()
#if UNITY_IOS
            .SetCodeId(iosSlotID)
#else
            .SetCodeId(AndroidSlotID)
#endif
            .SetSupportDeepLink(true)
            .SetImageAcceptedSize(1080, 1920)
            .SetUserID(DataManager.instance.configData.userID.ToString()) // 用户id,必传参数
            .SetMediaExtra("media_extra") // 附加参数，可选
            .SetOrientation(AdOrientation.Horizontal) // 必填参数，期望视频的播放方向
            .Build();

        AdNative.LoadRewardVideoAd(
            adSlot, new RewardVideoAdListener(this, "money"));
    }
    public void LoadRewardAdHero()
    {
        Debug.LogError("尝试加载广告");
        if (rewardAdHero != null)
        {
            rewardAdHero.Dispose();
            rewardAdHero = null;
        }

        string iosSlotID = "900546826";
        string AndroidSlotID = "946102299";
        var adSlot = new AdSlot.Builder()
#if UNITY_IOS
            .SetCodeId(iosSlotID)
#else
            .SetCodeId(AndroidSlotID)
#endif
            .SetSupportDeepLink(true)
            .SetImageAcceptedSize(1080, 1920)
            .SetUserID(DataManager.instance.configData.userID.ToString()) // 用户id,必传参数
            .SetMediaExtra("media_extra") // 附加参数，可选
            .SetOrientation(AdOrientation.Horizontal) // 必填参数，期望视频的播放方向
            .Build();

        AdNative.LoadRewardVideoAd(
            adSlot, new RewardVideoAdListener(this, "hero"));
    }
    public void LoadRewardAdFightWin()
    {
        Debug.LogError("尝试加载广告");
        if (rewardAdFightWin != null)
        {
            rewardAdFightWin.Dispose();
            rewardAdFightWin = null;
        }

        string iosSlotID = "900546826";
        string AndroidSlotID = "946102302";
        var adSlot = new AdSlot.Builder()
#if UNITY_IOS
            .SetCodeId(iosSlotID)
#else
            .SetCodeId(AndroidSlotID)
#endif
            .SetSupportDeepLink(true)
            .SetImageAcceptedSize(1080, 1920)
            .SetUserID(DataManager.instance.configData.userID.ToString()) // 用户id,必传参数
            .SetMediaExtra("media_extra") // 附加参数，可选
            .SetOrientation(AdOrientation.Horizontal) // 必填参数，期望视频的播放方向
            .Build();

        AdNative.LoadRewardVideoAd(
            adSlot, new RewardVideoAdListener(this, "fightWin"));
    }

    public void SetRewardVideoAd(string type, RewardVideoAd item)
    {
        switch (type)
        {
            case "money":
#if UNITY_IOS
                expressRewardAdMoney = item;
#else
                rewardAdMoney = item;
#endif
                break;
            case "hero":
#if UNITY_IOS
                expressRewardAdHero = item;
#else
                rewardAdHero = item;
#endif
                break;
            case "fightWin":
#if UNITY_IOS
                expressRewardAdFightWin = item;
#else
                rewardAdFightWin = item;
#endif
                break;
        }
    }

    /// <summary>
    /// Show the reward Ad.
    /// </summary>
    public void ShowRewardAdMoney(Action callback, string type)
    {
        if (rewardAdMoney == null)
        {
            DataManager.instance.SetAlert("尚未获取广告", "点“确定”将尝试加载广告，请您稍候再试。\n（若稍后该问题重复出现，请帮忙反馈）");
            instance.LoadRewardAdMoney();
            SetLog(0, type);
            return;
        }
        SetLog(1, type);

        this.callback = callback;
        rewardAdMoney.ShowRewardVideoAd();
    }
    public void ShowRewardAdHero(Action callback, string type)
    {
        if (rewardAdHero == null)
        {
            DataManager.instance.SetAlert("尚未获取广告", "点“确定”将尝试加载广告，请您稍候再试。\n（若稍后该问题重复出现，请帮忙反馈）");
            instance.LoadRewardAdHero();
            SetLog(0, type);
            return;
        }
        SetLog(1, type);

        this.callback = callback;
        rewardAdHero.ShowRewardVideoAd();
    }
    public void ShowRewardAdFightWin(Action callback, string type)
    {
        if (rewardAdFightWin == null)
        {
            DataManager.instance.SetAlert("尚未获取广告", "点“确定”将尝试加载广告，请您稍候再试。\n（若稍后该问题重复出现，请帮忙反馈）");
            instance.LoadRewardAdFightWin();
            SetLog(0, type);
            return;
        }
        SetLog(1, type);

        this.callback = callback;
        rewardAdFightWin.ShowRewardVideoAd();
    }

    private void SetLog(int step, string type = "")
    {
        if (type != "") category = type;
        
        switch (category)
        {
            case "money":
                Analytics.CustomEvent("ADShow", new Dictionary<string, object>
                {
                    {"tag", "money"},
                    {"step", step},
                });
                break;
            case "hero":
                Analytics.CustomEvent("ADShow", new Dictionary<string, object>
                {
                    {"tag", "hero"},
                    {"step", step},
                });
                break;
            case "fightWin":
                Analytics.CustomEvent("ADShow", new Dictionary<string, object>
                {
                    {"tag", "fightWin"},
                    {"step", step},
                });
                break;
            default:
                Analytics.CustomEvent("ADShow", new Dictionary<string, object>
                {
                    {"tag", "unknowAD"},
                    {"step", step},
                });
                break;
        }
    }

    private sealed class RewardVideoAdListener : IRewardVideoAdListener
    {
        private ADManager adManager;
        private string adType;

        public RewardVideoAdListener(ADManager adManager, string type)
        {
            this.adManager = adManager;
            adType = type;
        }

        public void OnError(int code, string message)
        {
            Debug.LogError("OnRewardError: " + message);
        }

        public void OnRewardVideoAdLoad(RewardVideoAd ad)
        {
            Debug.Log("OnRewardVideoAdLoad");

            ad.SetRewardAdInteractionListener(
                new RewardAdInteractionListener(adManager, adType));
            ad.SetDownloadListener(
                new AppDownloadListener(adManager));

            adManager.SetRewardVideoAd(adType, ad);
        }

        public void OnExpressRewardVideoAdLoad(ExpressRewardVideoAd ad)
        {
        }

        public void OnRewardVideoCached()
        {
            Debug.Log("OnRewardVideoCached");
        }
    }

    private sealed class ExpressRewardVideoAdListener : IRewardVideoAdListener
    {
        private ADManager adManager;
        private string adType;

        public ExpressRewardVideoAdListener(ADManager adManager, string type)
        {
            this.adManager = adManager;
            adType = type;
        }

        public void OnError(int code, string message)
        {
            Debug.LogError("OnRewardError: " + message);
        }

        public void OnRewardVideoAdLoad(RewardVideoAd ad)
        {
            Debug.Log("OnRewardVideoAdLoad");

            ad.SetRewardAdInteractionListener(
                new RewardAdInteractionListener(adManager, adType));
            ad.SetDownloadListener(
                new AppDownloadListener(adManager));

            adManager.SetRewardVideoAd(adType, ad);
        }

        // iOS
        public void OnExpressRewardVideoAdLoad(ExpressRewardVideoAd ad)
        {
#if UNITY_IOS
            Debug.Log("OnRewardExpressVideoAdLoad");

            ad.SetRewardAdInteractionListener(
                new RewardAdInteractionListener(this.adManager));
            ad.SetDownloadListener(
                new AppDownloadListener(this.adManager));
            adManager.SetRewardVideoAd(adType, ad);
#else
#endif
        }

        public void OnRewardVideoCached()
        {
            Debug.Log("OnExpressRewardVideoCached");
        }
    }

    private sealed class RewardAdInteractionListener : IRewardAdInteractionListener
    {
        private ADManager adManager;
        private string adType;

        public RewardAdInteractionListener(ADManager adManager, string type)
        {
            this.adManager = adManager;
            adType = type;
        }

        public void OnAdShow()
        {
            Debug.Log("rewardVideoAd show");
        }

        public void OnAdVideoBarClick()
        {
            Debug.Log("rewardVideoAd bar click");
        }

        public void OnAdClose()
        {
            Debug.Log("rewardVideoAd close");
            adManager.SetRewardVideoAd(adType, null);
        }

        // 激励视频播放完毕
        public void OnVideoComplete()
        {
            Debug.Log("rewardVideoAd complete");
            adManager.SetLog(2);
            adManager.LoadRewardAdMoney();
            adManager.callback?.Invoke();
        }

        public void OnVideoError()
        {
            Debug.LogError("rewardVideoAd error");
        }

        public void OnRewardVerify(
            bool rewardVerify, int rewardAmount, string rewardName)
        {
            Debug.Log("verify:" + rewardVerify + " amount:" + rewardAmount +
                      " name:" + rewardName);
        }
    }

    private sealed class AppDownloadListener : IAppDownloadListener
    {
        private ADManager adManager;

        public AppDownloadListener(ADManager adManager)
        {
            this.adManager = adManager;
        }

        public void OnIdle()
        {
        }

        public void OnDownloadActive(
            long totalBytes, long currBytes, string fileName, string appName)
        {
            Debug.Log("下载中，点击下载区域暂停");
        }

        public void OnDownloadPaused(
            long totalBytes, long currBytes, string fileName, string appName)
        {
            Debug.Log("下载暂停，点击下载区域继续");
        }

        public void OnDownloadFailed(
            long totalBytes, long currBytes, string fileName, string appName)
        {
            Debug.LogError("下载失败，点击下载区域重新下载");
        }

        public void OnDownloadFinished(
            long totalBytes, string fileName, string appName)
        {
            Debug.Log("下载完成，点击下载区域重新下载");
        }

        public void OnInstalled(string fileName, string appName)
        {
            Debug.Log("安装完成，点击下载区域打开");
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        instance.LoadRewardAdMoney();
        instance.LoadRewardAdHero();
        instance.LoadRewardAdFightWin();
    }
}
