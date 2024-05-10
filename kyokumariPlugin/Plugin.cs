using BepInEx;
using BepInEx.Logging;
using HarmonyLib.Tools;
using HarmonyLib;
using System;
using static System.Net.Mime.MediaTypeNames;
using CatSystem;
using System.IO;
using CatSystem.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Collections;
using TMPro;
using UnityEngine;
using System.Reflection;

namespace kyokumariPlugin;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource gLog = new ManualLogSource("glog");
    public static string patchDir = Environment.CurrentDirectory+ "\\cnPatch\\";
    static Dictionary<string, string> myDictionary = new Dictionary<string, string>();
    private void Awake()
    {
        // Plugin startup logic
        HarmonyFileLog.Enabled = true;
        BepInEx.Logging.Logger.Sources.Add(gLog);
        gLog.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        R1();
        FontFix.Init();
        Harmony.CreateAndPatchAll(typeof(FontFix));
        Harmony _pluginTriggers = Harmony.CreateAndPatchAll(
                typeof(Triggers)
            );

    }

    public static void R1() {
        string[] lines = File.ReadAllLines(Path.Join(patchDir , "co_00.txt"));
        for (int i = 0; i < lines.Length; i=i+2)
        {
            //gLog.LogInfo(lines[i]+ lines[i + 1]);
            myDictionary.TryAdd(lines[i], lines[i + 1]);
        }
    }
    private class Triggers
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CatSystem.Module.CatSystem2.NovelCommand), nameof(CatSystem.Module.CatSystem2.NovelCommand.DisplayMessageText), new Type[] { typeof(string) })]
        static public void DisplayMessageText_HOOK(ref string text)
        {
            //gLog.LogInfo("DisplayMessageText: " + text);
            if (myDictionary.ContainsKey(text))
            {
                text = myDictionary.Get(text);
            }
        }
        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(TMPro.TMP_FontAsset), "TryAddCharacterInternal")]
        //static public void TryAddCharacterInternal_HOOK(ref uint unicode, ref TMP_Character character, ref TMP_FontAsset __instance, ref bool __result)
        //{
        //    gLog.LogInfo("TryAddCharacterInternal: " + __instance.sourceFontFile);
        //    var stackInfo = new System.Diagnostics.StackTrace(true);
        //    gLog.LogMessage(stackInfo.ToString());
        //    __result = true;
        //}
        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(TMPro.TMP_FontAsset), "CreateFontAsset", new Type[] { typeof(Font) })]
        //static public void CreateFontAsset_HOOK(ref Font font)
        //{
        //    gLog.LogInfo("CreateFontAsset: " + font.name);
        //}
        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(CatSystem.Text.TextMeshProFont), "SetCurrentFont")]
        //static public void SetCurrentFont_HOOK(ref string fontName)
        //{
        //    gLog.LogInfo("SetCurrentFont: " + fontName);
        //}
        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(CatSystem.Utility.BinaryBuffer), nameof(CatSystem.Utility.BinaryBuffer.ReadDecode))]
        //static public void ReadDecode_HOOK(ref CatSystem.Utility.BinaryBuffer __instance,ref bool __result)
        //{
        //    gLog.LogInfo("ReadDecode: " + __result);
        //    gLog.LogInfo("ReadDecode: " + __instance.ReadBuffer);
        //    gLog.LogInfo("ReadDecode: " + __instance.DataName);
        //    gLog.LogInfo("ReadDecode: " + __instance.TypeName);
        //    var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        //    byte[] array = __instance.ReadBuffer;

        //    //gLog.LogInfo("ReadDecode: " + System.Text.Encoding.UTF8.GetString(array));
        //    using (FileStream fs = new FileStream(Timestamp.ToString(), FileMode.OpenOrCreate, FileAccess.Write))
        //    {
        //        fs.Write(array, 0, array.Length);
        //    }

        //}
    }

}
