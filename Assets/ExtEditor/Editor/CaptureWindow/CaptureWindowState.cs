using UnityEngine;
using UnityEditor;

namespace ExtEditor.Editor.CaptureWindow
{
    /// <summary>
    /// CaptureWindowの状態管理クラス
    /// </summary>
    public class CaptureWindowState
    {
        // 言語設定
        public CaptureWindow.Language CurrentLanguage { get; set; } = CaptureWindow.Language.Japanese;
        
        // パス設定
        public CaptureWindow.PathMode PathMode { get; set; } = CaptureWindow.PathMode.Relative;
        public string OutputDirectory { get; set; } = "/Captures";
        
        // キャプチャ設定
        public CaptureWindow.CaptureSource CaptureSource { get; set; } = CaptureWindow.CaptureSource.GameView;
        public Camera CaptureCamera { get; set; }
        public RenderTexture CustomRenderTexture { get; set; }
        public Camera[] SceneCameras { get; set; }
        public string[] SceneCameraNames { get; set; }
        public int SelectedCameraIndex { get; set; } = 0;
        
        // ファイル名設定
        public string FileNameTemplate { get; set; } = "Capture_<Date>_<Time>";
        public int TakeNumber { get; set; } = 1;
        public bool UseFixedTakeDigits { get; set; } = false;
        public int TakeDigits { get; set; } = 3;
        
        // キャプチャオプション
        public bool IncludeAlpha { get; set; } = false;
        public bool UseTransparentBackground { get; set; } = false;
        public bool AutoRefreshAssets { get; set; } = true;
        
        // UI表示設定
        public bool ShowHelp { get; set; } = false;
        public bool ShowRecentCaptures { get; set; } = false;
        
        // バリデーション状態
        public bool IsOutputPathValid { get; set; } = true;
        public bool IsFileNameValid { get; set; } = true;
        public string ValidationMessage { get; set; } = "";
        
        // プレビュー
        public string ResolvedOutputPath { get; set; } = "";
        public string PreviewFileName { get; set; } = "";
        
        // 履歴
        public string[] RecentCaptures { get; set; } = new string[5];
        public string LastCapturedFilePath { get; set; } = "";
        
        /// <summary>
        /// 言語に応じたテキストを取得
        /// </summary>
        public string GetText(string japanese, string english)
        {
            return CurrentLanguage == CaptureWindow.Language.Japanese ? japanese : english;
        }
        
        /// <summary>
        /// 最近のキャプチャを追加
        /// </summary>
        public void AddToRecentCaptures(string filePath)
        {
            // 配列を一つずらして新しいエントリを先頭に追加
            for (int i = RecentCaptures.Length - 1; i > 0; i--)
            {
                RecentCaptures[i] = RecentCaptures[i - 1];
            }
            RecentCaptures[0] = filePath;
        }
        
        /// <summary>
        /// 撮影成功時の処理
        /// </summary>
        public void OnCaptureSuccess(string savedFilePath)
        {
            if (!string.IsNullOrEmpty(savedFilePath))
            {
                AddToRecentCaptures(savedFilePath);
                LastCapturedFilePath = savedFilePath;
                
                // <Take>パターンが含まれている場合は自動インクリメント
                if (FileNameTemplate.Contains("<Take>"))
                {
                    TakeNumber++;
                }
            }
        }
    }
}