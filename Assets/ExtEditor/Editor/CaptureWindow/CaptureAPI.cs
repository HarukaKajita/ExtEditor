using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace ExtEditor.Editor.CaptureWindow
{
    /// <summary>
    /// CaptureWindowの機能を外部スクリプトから操作するためのAPI
    /// </summary>
    public static class CaptureAPI
    {
        /// <summary>
        /// キャプチャ設定
        /// </summary>
        public static class Settings
        {
            private static CaptureWindow GetOrCreateWindow()
            {
                var window = EditorWindow.GetWindow<CaptureWindow>(false, "CaptureWindow", false);
                if (window == null)
                {
                    window = ScriptableObject.CreateInstance<CaptureWindow>();
                }
                return window;
            }
            
            /// <summary>
            /// 言語設定を取得/設定
            /// </summary>
            public static CaptureWindow.Language Language
            {
                get => GetCurrentLanguage();
                set => SetCurrentLanguage(value);
            }
            
            /// <summary>
            /// パス指定方式を取得/設定
            /// </summary>
            public static CaptureWindow.PathMode PathMode
            {
                get => GetCurrentPathMode();
                set => SetCurrentPathMode(value);
            }
            
            /// <summary>
            /// 出力ディレクトリを取得/設定
            /// </summary>
            public static string OutputDirectory
            {
                get => GetCurrentOutputDirectory();
                set => SetCurrentOutputDirectory(value);
            }
            
            /// <summary>
            /// ファイル名テンプレートを取得/設定
            /// </summary>
            public static string FileNameTemplate
            {
                get => GetCurrentFileNameTemplate();
                set => SetCurrentFileNameTemplate(value);
            }
            
            /// <summary>
            /// テイク番号を取得/設定
            /// </summary>
            public static int TakeNumber
            {
                get => GetCurrentTakeNumber();
                set => SetCurrentTakeNumber(value);
            }
            
            /// <summary>
            /// アルファチャンネル含有フラグを取得/設定
            /// </summary>
            public static bool IncludeAlpha
            {
                get => GetCurrentIncludeAlpha();
                set => SetCurrentIncludeAlpha(value);
            }
            
            /// <summary>
            /// 透明背景フラグを取得/設定
            /// </summary>
            public static bool UseTransparentBackground
            {
                get => GetCurrentUseTransparentBackground();
                set => SetCurrentUseTransparentBackground(value);
            }
            
            /// <summary>
            /// テイク番号自動インクリメントフラグを取得/設定
            /// </summary>
            public static bool AutoIncrementTake
            {
                get => GetCurrentAutoIncrementTake();
                set => SetCurrentAutoIncrementTake(value);
            }
            
            /// <summary>
            /// テイク番号桁数固定フラグを取得/設定
            /// </summary>
            public static bool UseFixedTakeDigits
            {
                get => GetCurrentUseFixedTakeDigits();
                set => SetCurrentUseFixedTakeDigits(value);
            }
            
            /// <summary>
            /// テイク番号桁数を取得/設定
            /// </summary>
            public static int TakeDigits
            {
                get => GetCurrentTakeDigits();
                set => SetCurrentTakeDigits(value);
            }
            
            /// <summary>
            /// アセット自動更新フラグを取得/設定
            /// </summary>
            public static bool AutoRefreshAssets
            {
                get => GetCurrentAutoRefreshAssets();
                set => SetCurrentAutoRefreshAssets(value);
            }
            
            // 内部実装メソッド
            private static CaptureWindow.Language GetCurrentLanguage()
            {
                var window = GetOrCreateWindow();
                return window.GetCurrentLanguage();
            }
            
            private static void SetCurrentLanguage(CaptureWindow.Language language)
            {
                var window = GetOrCreateWindow();
                window.SetCurrentLanguage(language);
            }
            
            private static CaptureWindow.PathMode GetCurrentPathMode()
            {
                var window = GetOrCreateWindow();
                return window.GetCurrentPathMode();
            }
            
            private static void SetCurrentPathMode(CaptureWindow.PathMode pathMode)
            {
                var window = GetOrCreateWindow();
                window.SetCurrentPathMode(pathMode);
            }
            
            private static string GetCurrentOutputDirectory()
            {
                var window = GetOrCreateWindow();
                return window.GetCurrentOutputDirectory();
            }
            
            private static void SetCurrentOutputDirectory(string directory)
            {
                var window = GetOrCreateWindow();
                window.SetCurrentOutputDirectory(directory);
            }
            
            private static string GetCurrentFileNameTemplate()
            {
                var window = GetOrCreateWindow();
                return window.GetCurrentFileNameTemplate();
            }
            
            private static void SetCurrentFileNameTemplate(string template)
            {
                var window = GetOrCreateWindow();
                window.SetCurrentFileNameTemplate(template);
            }
            
            private static int GetCurrentTakeNumber()
            {
                var window = GetOrCreateWindow();
                return window.GetCurrentTakeNumber();
            }
            
            private static void SetCurrentTakeNumber(int takeNumber)
            {
                var window = GetOrCreateWindow();
                window.SetCurrentTakeNumber(takeNumber);
            }
            
            private static bool GetCurrentIncludeAlpha()
            {
                var window = GetOrCreateWindow();
                return window.GetCurrentIncludeAlpha();
            }
            
            private static void SetCurrentIncludeAlpha(bool includeAlpha)
            {
                var window = GetOrCreateWindow();
                window.SetCurrentIncludeAlpha(includeAlpha);
            }
            
            private static bool GetCurrentUseTransparentBackground()
            {
                var window = GetOrCreateWindow();
                return window.GetCurrentUseTransparentBackground();
            }
            
            private static void SetCurrentUseTransparentBackground(bool useTransparentBackground)
            {
                var window = GetOrCreateWindow();
                window.SetCurrentUseTransparentBackground(useTransparentBackground);
            }
            
            private static bool GetCurrentAutoIncrementTake()
            {
                var window = GetOrCreateWindow();
                return window.GetCurrentAutoIncrementTake();
            }
            
            private static void SetCurrentAutoIncrementTake(bool autoIncrementTake)
            {
                var window = GetOrCreateWindow();
                window.SetCurrentAutoIncrementTake(autoIncrementTake);
            }
            
            private static bool GetCurrentUseFixedTakeDigits()
            {
                var window = GetOrCreateWindow();
                return window.GetCurrentUseFixedTakeDigits();
            }
            
            private static void SetCurrentUseFixedTakeDigits(bool useFixedTakeDigits)
            {
                var window = GetOrCreateWindow();
                window.SetCurrentUseFixedTakeDigits(useFixedTakeDigits);
            }
            
            private static int GetCurrentTakeDigits()
            {
                var window = GetOrCreateWindow();
                return window.GetCurrentTakeDigits();
            }
            
            private static void SetCurrentTakeDigits(int takeDigits)
            {
                var window = GetOrCreateWindow();
                window.SetCurrentTakeDigits(takeDigits);
            }
            
            private static bool GetCurrentAutoRefreshAssets()
            {
                var window = GetOrCreateWindow();
                return window.GetCurrentAutoRefreshAssets();
            }
            
            private static void SetCurrentAutoRefreshAssets(bool autoRefreshAssets)
            {
                var window = GetOrCreateWindow();
                window.SetCurrentAutoRefreshAssets(autoRefreshAssets);
            }
        }
        
        /// <summary>
        /// キャプチャ実行機能
        /// </summary>
        public static class Capture
        {
            /// <summary>
            /// GameViewをキャプチャ
            /// </summary>
            /// <returns>保存されたファイルの絶対パス</returns>
            public static string CaptureGameView()
            {
                return ExecuteCapture(CaptureWindow.CaptureSource.GameView, null, null);
            }
            
            /// <summary>
            /// GameViewをキャプチャ（カメラ指定）
            /// </summary>
            /// <param name="camera">キャプチャに使用するカメラ</param>
            /// <returns>保存されたファイルの絶対パス</returns>
            public static string CaptureGameView(Camera camera)
            {
                return ExecuteCapture(CaptureWindow.CaptureSource.GameView, camera, null);
            }
            
            /// <summary>
            /// SceneViewをキャプチャ
            /// </summary>
            /// <returns>保存されたファイルの絶対パス</returns>
            public static string CaptureSceneView()
            {
                return ExecuteCapture(CaptureWindow.CaptureSource.SceneView, null, null);
            }
            
            /// <summary>
            /// RenderTextureをキャプチャ
            /// </summary>
            /// <param name="renderTexture">キャプチャするRenderTexture</param>
            /// <returns>保存されたファイルの絶対パス</returns>
            public static string CaptureRenderTexture(RenderTexture renderTexture)
            {
                return ExecuteCapture(CaptureWindow.CaptureSource.RenderTexture, null, renderTexture);
            }
            
            /// <summary>
            /// カスタム設定でキャプチャ実行
            /// </summary>
            /// <param name="config">キャプチャ設定</param>
            /// <returns>保存されたファイルの絶対パス</returns>
            public static string CaptureWithConfig(CaptureConfig config)
            {
                // 設定を一時的に適用
                var originalConfig = SaveCurrentConfig();
                try
                {
                    ApplyConfig(config);
                    return ExecuteCapture(config.CaptureSource, config.Camera, config.RenderTexture);
                }
                finally
                {
                    // 元の設定に戻す
                    ApplyConfig(originalConfig);
                }
            }
            
            /// <summary>
            /// 設定を保持したままキャプチャ実行
            /// </summary>
            /// <returns>保存されたファイルの絶対パス</returns>
            public static string CaptureWithCurrentSettings()
            {
                var window = EditorWindow.GetWindow<CaptureWindow>(false, "CaptureWindow", false);
                return window.ExecuteCaptureExternal();
            }
            
            private static string ExecuteCapture(CaptureWindow.CaptureSource source, Camera camera, RenderTexture renderTexture)
            {
                var window = EditorWindow.GetWindow<CaptureWindow>(false, "CaptureWindow", false);
                window.SetCaptureSource(source);
                
                if (camera != null)
                {
                    window.SetCaptureCamera(camera);
                }
                
                if (renderTexture != null)
                {
                    window.SetCustomRenderTexture(renderTexture);
                }
                
                return window.ExecuteCaptureExternal();
            }
            
            private static CaptureConfig SaveCurrentConfig()
            {
                return new CaptureConfig
                {
                    Language = Settings.Language,
                    PathMode = Settings.PathMode,
                    OutputDirectory = Settings.OutputDirectory,
                    FileNameTemplate = Settings.FileNameTemplate,
                    TakeNumber = Settings.TakeNumber,
                    IncludeAlpha = Settings.IncludeAlpha,
                    UseTransparentBackground = Settings.UseTransparentBackground,
                    AutoIncrementTake = Settings.AutoIncrementTake,
                    UseFixedTakeDigits = Settings.UseFixedTakeDigits,
                    TakeDigits = Settings.TakeDigits,
                    AutoRefreshAssets = Settings.AutoRefreshAssets
                };
            }
            
            private static void ApplyConfig(CaptureConfig config)
            {
                Settings.Language = config.Language;
                Settings.PathMode = config.PathMode;
                Settings.OutputDirectory = config.OutputDirectory;
                Settings.FileNameTemplate = config.FileNameTemplate;
                Settings.TakeNumber = config.TakeNumber;
                Settings.IncludeAlpha = config.IncludeAlpha;
                Settings.UseTransparentBackground = config.UseTransparentBackground;
                Settings.AutoIncrementTake = config.AutoIncrementTake;
                Settings.UseFixedTakeDigits = config.UseFixedTakeDigits;
                Settings.TakeDigits = config.TakeDigits;
                Settings.AutoRefreshAssets = config.AutoRefreshAssets;
            }
        }
        
        /// <summary>
        /// ユーティリティ機能
        /// </summary>
        public static class Utilities
        {
            /// <summary>
            /// CaptureWindowを開く
            /// </summary>
            public static void OpenWindow()
            {
                CaptureWindow.ShowWindow();
            }
            
            /// <summary>
            /// 出力ディレクトリを開く
            /// </summary>
            public static void OpenOutputDirectory()
            {
                var window = EditorWindow.GetWindow<CaptureWindow>(false, "CaptureWindow", false);
                window.OpenOutputDirectoryExternal();
            }
            
            /// <summary>
            /// 設定をデフォルトにリセット
            /// </summary>
            public static void ResetToDefault()
            {
                Settings.Language = CaptureWindow.Language.Japanese;
                Settings.PathMode = CaptureWindow.PathMode.Relative;
                Settings.OutputDirectory = "../Captures";
                Settings.FileNameTemplate = "Capture_<Date>_<Time>";
                Settings.TakeNumber = 1;
                Settings.IncludeAlpha = false;
                Settings.UseTransparentBackground = false;
                Settings.AutoIncrementTake = true;
                Settings.UseFixedTakeDigits = false;
                Settings.TakeDigits = 3;
                Settings.AutoRefreshAssets = true;
            }
            
            /// <summary>
            /// ファイル名プレビューを取得
            /// </summary>
            /// <returns>プレビューファイル名</returns>
            public static string GetFileNamePreview()
            {
                var window = EditorWindow.GetWindow<CaptureWindow>(false, "CaptureWindow", false);
                return window.GetFileNamePreview();
            }
            
            /// <summary>
            /// 出力パスプレビューを取得
            /// </summary>
            /// <returns>プレビュー出力パス</returns>
            public static string GetOutputPathPreview()
            {
                var window = EditorWindow.GetWindow<CaptureWindow>(false, "CaptureWindow", false);
                return window.GetOutputPathPreview();
            }
        }
    }
    
    /// <summary>
    /// キャプチャ設定を表すクラス
    /// </summary>
    [System.Serializable]
    public class CaptureConfig
    {
        public CaptureWindow.Language Language = CaptureWindow.Language.Japanese;
        public CaptureWindow.PathMode PathMode = CaptureWindow.PathMode.Relative;
        public CaptureWindow.CaptureSource CaptureSource = CaptureWindow.CaptureSource.GameView;
        public string OutputDirectory = "../Captures";
        public string FileNameTemplate = "Capture_<Date>_<Time>";
        public int TakeNumber = 1;
        public bool IncludeAlpha = false;
        public bool UseTransparentBackground = false;
        public bool AutoIncrementTake = true;
        public bool UseFixedTakeDigits = false;
        public int TakeDigits = 3;
        public bool AutoRefreshAssets = true;
        public Camera Camera;
        public RenderTexture RenderTexture;
        
        /// <summary>
        /// デフォルト設定を作成
        /// </summary>
        public static CaptureConfig CreateDefault()
        {
            return new CaptureConfig();
        }
        
        /// <summary>
        /// GameView用設定を作成
        /// </summary>
        public static CaptureConfig CreateForGameView(Camera camera = null)
        {
            return new CaptureConfig
            {
                CaptureSource = CaptureWindow.CaptureSource.GameView,
                Camera = camera
            };
        }
        
        /// <summary>
        /// SceneView用設定を作成
        /// </summary>
        public static CaptureConfig CreateForSceneView()
        {
            return new CaptureConfig
            {
                CaptureSource = CaptureWindow.CaptureSource.SceneView
            };
        }
        
        /// <summary>
        /// RenderTexture用設定を作成
        /// </summary>
        public static CaptureConfig CreateForRenderTexture(RenderTexture renderTexture)
        {
            return new CaptureConfig
            {
                CaptureSource = CaptureWindow.CaptureSource.RenderTexture,
                RenderTexture = renderTexture
            };
        }
    }
}