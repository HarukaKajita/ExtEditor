using UnityEngine;

namespace ExtEditor.Editor.CaptureWindow
{
    /// <summary>
    /// CaptureWindowの設定を保存するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "CaptureSettings", menuName = "ExtEditor/Capture Settings")]
    public class CaptureSettings : ScriptableObject
    {
        [Header("基本設定")]
        public CaptureWindow.Language language = CaptureWindow.Language.Japanese;
        public CaptureWindow.PathMode pathMode = CaptureWindow.PathMode.Relative;
        public string outputDirectory = "../Captures";
        public string fileNameTemplate = "Capture_<Date>_<Time>";
        
        [Header("テイク番号設定")]
        public int takeNumber = 1;
        public bool autoIncrementTake = true;
        public bool useFixedTakeDigits = false;
        [Range(1, 10)]
        public int takeDigits = 3;
        
        [Header("キャプチャオプション")]
        public bool includeAlpha = false;
        public bool useTransparentBackground = false;
        public bool autoRefreshAssets = true;
        
        [Header("履歴")]
        public string[] recentCaptures = new string[5];
        
        /// <summary>
        /// 設定をCaptureWindowに適用
        /// </summary>
        public void ApplyToWindow(CaptureWindow window)
        {
            // Private fields are not accessible, so this would need reflection
            // or the CaptureWindow class would need public setters
        }
        
        /// <summary>
        /// CaptureWindowから設定を取得
        /// </summary>
        public void LoadFromWindow(CaptureWindow window)
        {
            // Private fields are not accessible, so this would need reflection
            // or the CaptureWindow class would need public getters
        }
        
        /// <summary>
        /// デフォルト設定にリセット
        /// </summary>
        [ContextMenu("Reset to Default")]
        public void ResetToDefault()
        {
            language = CaptureWindow.Language.Japanese;
            pathMode = CaptureWindow.PathMode.Relative;
            outputDirectory = "../Captures";
            fileNameTemplate = "Capture_<Date>_<Time>";
            takeNumber = 1;
            autoIncrementTake = true;
            useFixedTakeDigits = false;
            takeDigits = 3;
            includeAlpha = false;
            useTransparentBackground = false;
            autoRefreshAssets = true;
            recentCaptures = new string[5];
        }
    }
}