# 旭光のマリアージュ 正式版本 测试补丁
给群里想翻的群友用

插件基于BepInEx开发

格式一看就懂，文件夹里面的记事本打开

第一行被匹配原文，第二行替换文本

修改后需要重写启动游戏

图还是试玩版的图，不过正式版也能用

![测试图](/cnpatch/20240511023425.jpg)

## 关于getresource_comment

我直接拿试玩版的偷过来换掉，傻逼才去硬嗑playdrm

![正式版](/cnpatch/20240831193229.jpg)

虽然能跑起来的话基本上也能dump出来

```csharp 
[HarmonyPrefix]
[HarmonyPatch(typeof(CatSystem.StringVariable), nameof(CatSystem.StringVariable.SetVariable), new Type[] { typeof(int), typeof(string) })]
static private void SetVariable_HOOK(ref int no, ref string str)
{
    if (str == "5A51610A-D960-43D7")
    {
        str = "{CB22F1FC-79CC-49B9-92A1-8BB882CA249E}";
    }
    gLog.LogInfo(str);
}
```

## 关于字体
字体包中包括的字在cnPatch/kyokumari_trial_font下

不在其中的字不能正常展示

需要新增字在更变文件后需要重新生成kyokumari_trial_font.asset

不过推荐是新增一个文件走fallback list，反正常用字应该都有了

重生成一次肯定几个小时没了