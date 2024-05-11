using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

namespace kyokumariPlugin
{
    public static class FontFix
    {
        // From https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany
        public static List<TMP_FontAsset> tmpchinesefonts = new List<TMP_FontAsset>();
        public static List<string> tmpchinesefontnames = new List<string>();
        #region 字体
        public static bool AddChineseFont(string path)
        {
            if (File.Exists(path))
            {
                var AllAssets = AssetBundle.LoadFromFile(path).LoadAllAssets();
                foreach (var Asset in AllAssets)
                {
                    var TryCastFontAsset = Asset as TMP_FontAsset;
                    if (TryCastFontAsset)
                    {
                        UnityEngine.Object.DontDestroyOnLoad(TryCastFontAsset);
                        TryCastFontAsset.hideFlags |= HideFlags.HideAndDontSave;
                        tmpchinesefonts.Add(TryCastFontAsset);
                        tmpchinesefontnames.Add(TryCastFontAsset.name);
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool GetChineseFont(string fontname, out TMP_FontAsset fontAsset)
        {
            fontAsset = null;
            if (tmpchinesefonts.Count == 0)
                return false;
            if (fontname == "ZenAntique-Regular SDF" || fontname.StartsWith("HinaMincho-Regular") || fontname.StartsWith("HigashiOme-Gothic-C") || fontname == "Pretendard-Regular SDF" || fontname.StartsWith("SCDream") || fontname == "LiberationSans SDF" || fontname == "Mikodacs SDF" || fontname == "BebasKai SDF")
            {
                fontAsset = tmpchinesefonts[0];
                return true;
            }
            //if (fontname == "ZenAntique-Regular SDF" || fontname.StartsWith("HinaMincho-Regular") || fontname.StartsWith("HigashiOme-Gothic-C") || fontname == "Pretendard-Regular SDF" || fontname.StartsWith("SCDream") || fontname == "LiberationSans SDF" || fontname == "Mikodacs SDF" || fontname == "BebasKai SDF")
            return false;
        }

        static FieldInfo fi = null;
        public static TMP_FontAsset GetFontAsset(TMP_Text txt)
        {
            if (fi == null)
                fi = AccessTools.Field(typeof(TMP_Text), "m_fontAsset");
            if (fi != null)
                return fi.GetValue(txt) as TMP_FontAsset;
            return null;
        }

        public static bool IsChineseFont(TMP_FontAsset fontAsset)
            => tmpchinesefontnames.Contains(fontAsset.name);
        [HarmonyPatch(typeof(TMP_Text), nameof(TMP_Text.font), MethodType.Setter)]
        [HarmonyPrefix]
        private static bool set_font(TMP_Text __instance, ref TMP_FontAsset value)
        {
            if (IsChineseFont(GetFontAsset(__instance)))
                return false;
            string fontname = GetFontAsset(__instance).name;
            if (GetChineseFont(fontname, out TMP_FontAsset font))
                value = font;
            return true;
        }
        [HarmonyPatch(typeof(TMP_Text), nameof(TMP_Text.fontMaterial), MethodType.Setter)]
        [HarmonyPrefix]
        private static void set_fontMaterial(TMP_Text __instance, ref Material value)
        {
            if (IsChineseFont(GetFontAsset(__instance)))
                value = GetFontAsset(__instance).material;
        }

        [HarmonyPatch(typeof(TMP_Text), nameof(TMP_Text.text), MethodType.Setter)]
        [HarmonyPrefix]
        private static void set_text(TMP_Text __instance, ref Material value)
        {
            if (!IsChineseFont(GetFontAsset(__instance)))
            {
                string fontname = GetFontAsset(__instance).name;
                if (GetChineseFont(fontname, out TMP_FontAsset font))
                    __instance.font = font;
            }
        }


        #endregion

        public static void Init()
        {
            var modPath = Plugin.patchDir;
            var fontPath = Path.Join(modPath, "kyokumari_trial_font.asset");
            if (!AddChineseFont(fontPath))
            {
                Plugin.gLog.LogWarning("你没有下载中文字体"+ fontPath);
            }
        }
    }
}
