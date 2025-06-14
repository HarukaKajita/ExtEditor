using System;
using UnityEngine;

namespace ExtEditor.Editor.CaptureWindow
{
    /// <summary>
    /// ファイル名パターンの置換を処理するクラス
    /// </summary>
    public static class PatternResolver
    {
        /// <summary>
        /// パターンを解決してファイル名を生成
        /// </summary>
        public static string ResolvePattern(string template, PatternContext context)
        {
            if (string.IsNullOrEmpty(template))
                return "capture";
                
            var now = DateTime.Now;
            string result = template;
            
            // 基本パターンの置換
            result = result.Replace("<Take>", GetTakeString(context.TakeNumber, context.UseFixedDigits, context.DigitCount));
            result = result.Replace("<Name>", GetCameraName(context));
            result = result.Replace("<Date>", now.ToString("yyyyMMdd"));
            result = result.Replace("<Time>", now.ToString("HHmmss"));
            result = result.Replace("<MilliSec>", now.ToString("fff"));
            result = result.Replace("<Hour>", now.ToString("HH"));
            result = result.Replace("<Minute>", now.ToString("mm"));
            result = result.Replace("<Second>", now.ToString("ss"));
            
            // 拡張パターンの置換
            result = result.Replace("<Year>", now.ToString("yyyy"));
            result = result.Replace("<Month>", now.ToString("MM"));
            result = result.Replace("<Day>", now.ToString("dd"));
            result = result.Replace("<UnixTime>", ((DateTimeOffset)now).ToUnixTimeSeconds().ToString());
            
            return CaptureHelper.SanitizeFileName(result);
        }
        
        /// <summary>
        /// テイク番号の文字列を取得
        /// </summary>
        private static string GetTakeString(int takeNumber, bool useFixedDigits, int digitCount)
        {
            if (useFixedDigits)
            {
                string format = new string('0', Mathf.Clamp(digitCount, 1, 10));
                return takeNumber.ToString(format);
            }
            
            return takeNumber.ToString("000");
        }
        
        /// <summary>
        /// カメラ名を取得
        /// </summary>
        private static string GetCameraName(PatternContext context)
        {
            switch (context.CaptureSource)
            {
                case CaptureWindow.CaptureSource.GameView:
                    return "GameView";
                case CaptureWindow.CaptureSource.SceneView:
                    return "SceneView";
                case CaptureWindow.CaptureSource.RenderTexture:
                    return context.CustomRenderTexture?.name ?? "RenderTexture";
                default:
                    return context.CaptureCamera?.name ?? "Unknown";
            }
        }
        
        /// <summary>
        /// 利用可能なパターンの一覧を取得
        /// </summary>
        public static PatternInfo[] GetAvailablePatterns()
        {
            return new PatternInfo[]
            {
                new PatternInfo("<Take>", "テイク番号", "Take number", "001"),
                new PatternInfo("<Name>", "カメラ名", "Camera name", "MainCamera"),
                new PatternInfo("<Date>", "日付", "Date", DateTime.Now.ToString("yyyyMMdd")),
                new PatternInfo("<Time>", "時刻", "Time", DateTime.Now.ToString("HHmmss")),
                new PatternInfo("<MilliSec>", "ミリ秒", "Milliseconds", DateTime.Now.ToString("fff")),
                new PatternInfo("<Hour>", "時", "Hour", DateTime.Now.ToString("HH")),
                new PatternInfo("<Minute>", "分", "Minute", DateTime.Now.ToString("mm")),
                new PatternInfo("<Second>", "秒", "Second", DateTime.Now.ToString("ss")),
                new PatternInfo("<Year>", "年", "Year", DateTime.Now.ToString("yyyy")),
                new PatternInfo("<Month>", "月", "Month", DateTime.Now.ToString("MM")),
                new PatternInfo("<Day>", "日", "Day", DateTime.Now.ToString("dd")),
                new PatternInfo("<UnixTime>", "Unix時刻", "Unix time", ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds().ToString())
            };
        }
    }
    
    /// <summary>
    /// パターン解決に必要なコンテキスト情報
    /// </summary>
    public class PatternContext
    {
        public int TakeNumber { get; set; } = 1;
        public bool UseFixedDigits { get; set; } = false;
        public int DigitCount { get; set; } = 3;
        public CaptureWindow.CaptureSource CaptureSource { get; set; }
        public Camera CaptureCamera { get; set; }
        public RenderTexture CustomRenderTexture { get; set; }
    }
    
    /// <summary>
    /// パターン情報を表すクラス
    /// </summary>
    public class PatternInfo
    {
        public string Pattern { get; }
        public string DescriptionJapanese { get; }
        public string DescriptionEnglish { get; }
        public string Example { get; }
        
        public PatternInfo(string pattern, string descJp, string descEn, string example)
        {
            Pattern = pattern;
            DescriptionJapanese = descJp;
            DescriptionEnglish = descEn;
            Example = example;
        }
        
        public string GetDescription(CaptureWindow.Language language)
        {
            return language == CaptureWindow.Language.Japanese ? DescriptionJapanese : DescriptionEnglish;
        }
    }
}