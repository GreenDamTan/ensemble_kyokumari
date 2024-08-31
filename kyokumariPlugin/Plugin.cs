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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using static CatSystem.Emote.Inertia;

namespace kyokumariPlugin;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource gLog = new ManualLogSource("glog");
    public static string patchDir = Environment.CurrentDirectory+ "\\cnPatch\\";
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

    private static void R1() {
        string[] co_list = {
            "co_00.txt","co_01.txt","co_02.txt","co_03.txt","co_04.txt",
            "co_05.txt","co_06.txt","co_07.txt","co_08.txt","co_09.txt",
            "co_10.txt","co_11.txt","co_12.txt","co_13.txt","co_14.txt",
            "co_15.txt","co_16.txt","co_17.txt","co_18.txt","co_19.txt",
            "co_20.txt"
        };
        for (int i = 0; i < co_list.Length; i++)
        {
            string path = Path.Join(patchDir, "text", co_list[i] );
            if (!File.Exists(path))
            {
                gLog.LogInfo("Not Exists " + co_list[i]);
                continue;
            }
            string[] lines = File.ReadAllLines(path);
            gLog.LogInfo("Read "+ co_list[i]);
            for (int j = 0; j < lines.Length; j = j + 2)
            {
                //gLog.LogInfo(lines[j]+ lines[j + 1]);
                myDictionary.TryAdd(lines[j].Replace("\t", ""), lines[j + 1].Replace("\t", ""));
            }
        }
    }
    static Dictionary<string, string> myDictionary = new Dictionary<string, string>();
    private static string ReplaceTextFromDictionary(string beforeText) {
        string afterText;
        if (myDictionary.TryGetValue(beforeText, out afterText))
        {
            return afterText;
        }
        return beforeText;
    }
    private class Triggers
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CatSystem.Module.CatSystem2.NovelCommand), nameof(CatSystem.Module.CatSystem2.NovelCommand.DisplayMessageText), new Type[] { typeof(string) })]
        static private void DisplayMessageText_HOOK(ref string text)
        {
            //gLog.LogInfo("DisplayMessageText: " + text);
            text = ReplaceTextFromDictionary(text);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CatSystem.Module.CatSystem2.NovelCommand), nameof(CatSystem.Module.CatSystem2.NovelCommand.DisplayMessageName), new Type[] { typeof(string), typeof(bool) })]
        static private void DisplayMessageName_HOOK(ref string text, ref bool autoface)
        {
            text = ReplaceTextFromDictionary(text);
        }

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(CatSystem.AtxImageBase), nameof(CatSystem.AtxImageBase.Load), new Type[] { typeof(Stream), typeof(string) })]
        //static public void CatSystem_AtxImageBase_Load_HOOK(ref Stream stream, ref string baseName)
        //{
        //    gLog.LogInfo("CatSystem_AtxImageBase_Load: " + baseName);
        //    //var stackInfo = new System.Diagnostics.StackTrace(true);
        //    //gLog.LogMessage(stackInfo.ToString());
        //}

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(CatSystem.File.FileManager), nameof(CatSystem.File.FileManager.ReadAtxImage), new Type[] { typeof(string), typeof(string), typeof(bool) })]
        //static public void CatSystem_File_FileManager_ReadAtxImage_HOOK( ref string dataType, ref string filename, ref bool texreadonly)
        //{
        //    gLog.LogInfo(string.Format("CatSystem_File_FileManager_ReadAtxImage_Load: {0} {1} {2}", dataType, filename, texreadonly));
        //    var stackInfo = new System.Diagnostics.StackTrace(true);
        //    gLog.LogMessage(stackInfo.ToString());
        //}

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
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CatSystem.Module.ModuleFeScript), nameof(CatSystem.Module.ModuleFeScript.LoadScript), new Type[] { typeof(string), typeof(int[]) })]
        static private void LoadScript_HOOK(ref string scriptName, ref int[] bootParam)
        {
            if (scriptName == "title"||scriptName== "wmlogo")
            {
                ChangeWindowTitle();
            }
        }
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
    }
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool SetWindowText(IntPtr hwnd, String lpString);
    [DllImport("user32.dll")]
    private static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, IntPtr lParam);
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    private static void ChangeWindowTitle()
    {
        IntPtr unityHWnd = IntPtr.Zero;
        string UNITY_WND_CLASSNAME = "UnityWndClass";
        EnumThreadWindows(GetCurrentThreadId(), (hWnd, lParam) =>
        {
            var classText = new StringBuilder(UNITY_WND_CLASSNAME.Length + 1);
            GetClassName(hWnd, classText, classText.Capacity);

            if (classText.ToString() == UNITY_WND_CLASSNAME)
            {
                unityHWnd = hWnd;
                return false;
            }
            return true;
        }, IntPtr.Zero);
        gLog.LogInfo("unityHWnd:"+unityHWnd.ToInt64().ToString());

        if (unityHWnd != IntPtr.Zero)
        {
            SetWindowText(unityHWnd,"旭光的mariage");
        }
    }
}
