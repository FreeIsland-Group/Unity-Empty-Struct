using System;
using System.Collections;
using System.Collections.Generic;
using Prefab;
using UnityEngine;
using Random = UnityEngine.Random;

public class SupportManager : MonoBehaviour
{
    public static SupportManager instance = null;
    public GameObject canvas = null;
    public AlertPanel alert;
    public ConfirmPanel confirm;
    public ConfirmPanel inputConfirm;

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

    // 新增一个提醒框
    public void SetAlert(string titleText, string infoText, Action callback = null)
    {
        AlertPanel newAlert = Instantiate(alert, canvas.transform);
        newAlert.InitText(titleText, infoText, callback);
    }

    // 新增一个选择框
    public void SetConfirm(string titleText, string infoText, Action sureCallback, Action cancelCallback,
        string cancelText = "放 弃", string sureText = "确 定")
    {
        ConfirmPanel newConfirm = Instantiate(confirm, canvas.transform);
        newConfirm.InitText(titleText, infoText, sureCallback, cancelCallback);
    }

    // 新增一个输入选择框
    public void SetInputConfirm(string titleText, string infoText, Action<string> sureCallback, Action cancelCallback,
        string placeholderText = "请输入...", string cancelText = "放 弃", string sureText = "确 定")
    {
        InputConfirm newConfirm = Instantiate(inputConfirm, canvas.transform);
        newConfirm.InitText(titleText, infoText, sureCallback, cancelCallback, placeholderText, cancelText, sureText);
    }

    // 生成姓名
    public Dictionary<string, string> RandomName(int sex = 0, string family = null)
    {
        Debug.Log("Function call: RandomName");
        string[] firstName =
            "赵 钱 孙 李 周 吴 郑 王 冯 陈 褚 卫 蒋 沈 韩 杨 朱 秦 尤 许 何 吕 施 张 孔 曹 严 华 金 魏 陶 姜 戚 谢 邹 喻 柏 水 窦 章 云 苏 潘 葛 奚 范 彭 郎 鲁 韦 昌 马 苗 凤 花 方 俞 任 袁 柳 酆 鲍 史 唐 费 廉 岑 薛 雷 贺 倪 汤 滕 殷 罗 毕 郝 邬 安 常 乐 于 时 傅 皮 卞 齐 康 伍 余 元 卜 顾 孟 平 黄 和 穆 萧 尹 姚 邵 湛 汪 祁 毛 禹 狄 米 贝 明 臧 计 伏 成 戴 谈 宋 茅 庞 熊 纪 舒 屈 项 祝 董 粱 杜 阮 蓝 闵 席 季 麻 强 贾 路 娄 危 江 童 颜 郭 梅 盛 林 刁 钟 徐 邱 骆 高 夏 蔡 田 樊 胡 凌 霍 虞 万 支 柯 昝 管 卢 莫 经 房 裘 缪 干 解 应 宗 丁 宣 贲 邓 郁 单 杭 洪 包 诸 左 石 崔 吉 钮 龚 程 嵇 邢 滑 裴 陆 荣 翁 荀 羊 於 惠 甄 麴 家 封 芮 羿 储 靳 汲 邴 糜 松 井 段 富 巫 乌 焦 巴 弓 牧 隗 山 谷 车 侯 宓 蓬 全 郗 班 仰 秋 仲 伊 宫 宁 仇 栾 暴 甘 钭 厉 戎 祖 武 符 刘 景 詹 束 龙 叶 幸 司 韶 郜 黎 蓟 薄 印 宿 白 怀 蒲 邰 从 鄂 索 咸 籍 赖 卓 蔺 屠 蒙 池 乔 阴 欎 胥 能 苍 双 闻 莘 党 翟 谭 贡 劳 逄 姬 申 扶 堵 冉 宰 郦 雍 舄 璩 桑 桂 濮 牛 寿 通 边 扈 燕 冀 郏 浦 尚 农 温 别 庄 晏 柴 瞿 阎 充 慕 连 茹 习 宦 艾 鱼 容 向 古 易 慎 戈 廖 庾 终 暨 居 衡 步 都 耿 满 弘 匡 国 文 寇 广 禄 阙 东 殴 殳 沃 利 蔚 越 夔 隆 师 巩 厍 聂 晁 勾 敖 融 冷 訾 辛 阚 那 简 饶 空 曾 毋 沙 乜 养 鞠 须 丰 巢 关 蒯 相 查 後 荆 红 游 竺 权 逯 盖 益 桓 公 万俟 司马 上官 欧阳 夏侯 诸葛 闻人 东方 赫连 皇甫 尉迟 公羊 澹台 公冶 宗政 濮阳 淳于 单于 太叔 申屠 公孙 仲孙 轩辕 令狐 钟离 宇文 长孙 慕容 鲜于 闾丘 司徒 司空 亓官 司寇 仉 督 子车 颛孙 端木 巫马 公西 漆雕 乐正 壤驷 公良 拓跋 夹谷 宰父 谷梁 晋 楚 闫 法 汝 鄢 涂 钦 段干 百里 东郭 南门 呼延 归 海 羊舌 微生 岳 帅 缑 亢 况 后 有 琴 梁丘 左丘 东门 西门 商 牟 佘 佴 伯 赏 南宫 墨 哈 谯 笪 年 爱 阳 佟"
                .Split(' ');
        string[] womenName = "賢 淑 德 慧 貞 卿 端 莊 靜 秀 雅 娟 英 华 巧 美 娜 静 惠 珠 翠 芝 玉 萍 红 娥 玲 芬 芳 燕 彩 春 菊 兰 凤 洁 梅 琳 素 云 莲 真 环 雪 荣 爱 妹 霞 香 月 莺 媛 艳 瑞 凡 佳 嘉 琼 勤 珍 贞 莉 桂 娣 叶 璧 璐 娅 琦 晶 妍 茜 秋 珊 莎 锦 黛 青 倩 婷 姣 婉 娴 瑾 颖 露 瑶 怡 婵 雁 蓓 纨 仪 荷 丹 蓉 眉 君 琴 蕊 薇 菁 梦 岚 苑 筠 柔 竹 霭 凝 晓 欢 霄 枫 芸 菲 寒 欣 滢 伊 亚 宜 可 姬 舒 影 荔 枝 思 丽 飘 育 馥 宁 婕 馨 瑗 琰 韵 融 园 艺 咏 聪 澜 纯 毓 悦 昭 冰 爽 琬 茗 羽 希 婀 惜 霜 凌 忆 紫 安 灵 奚 芷 蕾 乐 怜 菡 卉 南 迎 之 依 元 柏 妙 代 萱 尔 谷 傲 槐 夏 涵 易 亦 海 蓝 宛 绮 慕 儿 优 璇 幽 晴 甜 瑜 熙 婀娜 惜霜 凌寒 忆翠 凌香 紫安 灵奚 芷蕾 乐丹 怜菡 雁卉 翠柔 紫南 迎梦 之瑶 依珊 元柏 冰露 妙竹 代萱 尔珍 冰萍 紫真 谷雪 傲蕾 之柔 乐萱 青槐 夏青 涵菡 易梦 亦瑶 海莲 惜雪 妙菡 紫蓝 宛海 春竹 晓巧 绮兰 慕儿 优璇 青青 芊芊 佳雪 幽兰 慕晴 梦璐 甜瑜 芸熙 黛玉"
                .Split(' ');
        string[] manName = "伟 刚 勇 毅 俊 峰 强 军 平 保 东 文 辉 力 明 永 健 世 广 志 义 兴 良 海 山 仁 波 宁 贵 福 生 龙 元 全 国 胜 学 祥 才 发 武 新 利 清 飞 彬 富 顺 信 子 杰 涛 昌 成 康 星 光 天 达 安 岩 中 茂 进 林 有 坚 和 彪 博 诚 先 敬 震 振 壮 会 思 群 豪 心 邦 承 乐 绍 功 松 善 厚 庆 磊 民 友 裕 河 哲 江 超 浩 亮 政 谦 亨 奇 固 之 轮 翰 朗 伯 宏 言 若 鸣 朋 斌 梁 栋 维 启 克 伦 翔 旭 鹏 泽 晨 辰 士 以 建 家 致 树 炎 德 行 时 泰 盛 炜 然 弥 影 珂 棋 瑾 奕 君 梦 程 阁 乔 韬 洛 盈 铮 恒 蓝 熙 倩 牧 恩 基 煊 仪 梓 瀚 端 宗 风 恺 淇 云 睿 正 沅 丰 多 熠 书 节 楠 麒 曜 坤 蕊 远 智 硕 材 综 洋 烨 益 玉 芸 焘 凯 深 业 沛 鼎 珉 止 玺 瑞 雨 月 晔 孜 铭 昊 世炜 辰然 松杰 弥影 志珂 棋瑾 奕武 世君 梦政 国程 松阁 乔韬 洛盈 振铮 恒蓝 熙倩 文牧 飞飞 杰辰 恩基 煊明 会仪 梓瀚 梓端 宗风 恺淇 云睿 正沅 丰乐 瀚江 多熠 成书 节楠 敬睿 建麒 曜坤 新蕊 远智 硕浩 材哲 狗剩 建国 综洋 东然 心烨 祥益 玉芸 海涛 基焘 凯深 伟业 恩维 沛鼎 珉杰 止玺 子辉 浩瑞 雨涛 月晔 文兴 孜铭 昊铮 泽宇 宝玉"
        .Split(' ');

        string name = firstName[Random.Range(0, firstName.Length)];
        if (family != null)
        {
            name = family;
        }

        if (sex == 0)
        {
            if (Random.Range(0, 2) == 1)
            {
                sex = 2;
                name += womenName[Random.Range(0, womenName.Length)];
            }
            else
            {
                sex = 1;
                name += manName[Random.Range(0, manName.Length)];
            }
        }
        else if (sex == 2)
        {
            name += womenName[Random.Range(0, womenName.Length)];
        }
        else if (sex == 1)
        {
            name += manName[Random.Range(0, manName.Length)];
        }

        Dictionary<string, string> result = new Dictionary<string, string>
        {
            {"name", name},
            {"sex", sex.ToString()},
        };
        return result;
    }

    // 获取以 x 为底，y 的对数
    public float GetBaseLog(float x, float y) {
        return Mathf.Log(y) / Mathf.Log(x);
    }
}
