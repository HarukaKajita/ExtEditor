using UnityEngine;
using UnityEditor;

namespace ExtEditor.Editor.CaptureWindow.Samples
{
    /// <summary>
    /// CaptureAPIの使用例を示すサンプルクラス
    /// </summary>
    public static class CaptureAPISamples
    {
        /// <summary>
        /// 基本的なキャプチャ例
        /// </summary>
        [MenuItem("Tools/CaptureWindow/Samples/Basic Capture")]
        public static void BasicCaptureExample()
        {
            // GameViewを簡単にキャプチャ
            string filePath = CaptureAPI.Capture.CaptureGameView();
            Debug.Log($"キャプチャ完了: {filePath}");
        }
        
        /// <summary>
        /// カメラ指定でキャプチャ
        /// </summary>
        [MenuItem("Tools/CaptureWindow/Samples/Camera Capture")]
        public static void CameraCaptureExample()
        {
            // MainCameraでキャプチャ
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                string filePath = CaptureAPI.Capture.CaptureGameView(mainCamera);
                Debug.Log($"カメラキャプチャ完了: {filePath}");
            }
            else
                Debug.LogWarning("MainCameraが見つかりません");
        }
        
        /// <summary>
        /// SceneViewキャプチャ例
        /// </summary>
        [MenuItem("Tools/CaptureWindow/Samples/SceneView Capture")]
        public static void SceneViewCaptureExample()
        {
            // SceneViewをキャプチャ
            string filePath = CaptureAPI.Capture.CaptureSceneView();
            Debug.Log($"SceneViewキャプチャ完了: {filePath}");
        }
        
        /// <summary>
        /// カスタム設定でキャプチャ
        /// </summary>
        [MenuItem("Tools/CaptureWindow/Samples/Custom Config Capture")]
        public static void CustomConfigCaptureExample()
        {
            // カスタム設定を作成
            var config = new CaptureConfig
            {
                Language = CaptureWindow.Language.English,
                FileNameTemplate = "CustomCapture_<Take>_<Date>",
                TakeNumber = 5,
                IncludeAlpha = true,
                UseTransparentBackground = true,
                UseFixedTakeDigits = true,
                TakeDigits = 4,
                OutputDirectory = "../CustomCaptures"
            };
            
            // 設定を適用してキャプチャ
            string filePath = CaptureAPI.Capture.CaptureWithConfig(config);
            Debug.Log($"カスタム設定キャプチャ完了: {filePath}");
        }
        
        /// <summary>
        /// 設定変更例
        /// </summary>
        [MenuItem("Tools/CaptureWindow/Samples/Settings Example")]
        public static void SettingsExample()
        {
            // 現在の設定を表示
            Debug.Log($"現在の言語: {CaptureAPI.Settings.Language}");
            Debug.Log($"現在のテイク番号: {CaptureAPI.Settings.TakeNumber}");
            Debug.Log($"現在の出力ディレクトリ: {CaptureAPI.Settings.OutputDirectory}");
            
            // 設定を変更
            CaptureAPI.Settings.Language = CaptureWindow.Language.English;
            CaptureAPI.Settings.TakeNumber = 10;
            CaptureAPI.Settings.FileNameTemplate = "Sample_<Name>_<Take>";
            CaptureAPI.Settings.IncludeAlpha = true;
            
            Debug.Log("設定を変更しました");
            
            // プレビューを確認
            string preview = CaptureAPI.Utilities.GetFileNamePreview();
            Debug.Log($"ファイル名プレビュー: {preview}");
        }
        
        /// <summary>
        /// バッチキャプチャ例
        /// </summary>
        [MenuItem("Tools/CaptureWindow/Samples/Batch Capture")]
        public static void BatchCaptureExample()
        {
            // 複数のカメラを順番にキャプチャ
            Camera[] cameras = Object.FindObjectsOfType<Camera>();
            
            foreach (Camera camera in cameras)
            {
                // 各カメラごとに設定を変更
                CaptureAPI.Settings.FileNameTemplate = $"Camera_{camera.name}_<Take>";
                
                // キャプチャ実行
                string filePath = CaptureAPI.Capture.CaptureGameView(camera);
                Debug.Log($"カメラ {camera.name} キャプチャ完了: {filePath}");
            }
        }
        
        /// <summary>
        /// RenderTextureキャプチャ例
        /// </summary>
        [MenuItem("Tools/CaptureWindow/Samples/RenderTexture Capture")]
        public static void RenderTextureCaptureExample()
        {
            // RenderTextureを作成
            RenderTexture renderTexture = new RenderTexture(1920, 1080, 24);
            renderTexture.name = "SampleRenderTexture";
            
            try
            {
                // RenderTextureにカメラをレンダリング
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    RenderTexture previousTarget = mainCamera.targetTexture;
                    mainCamera.targetTexture = renderTexture;
                    mainCamera.Render();
                    mainCamera.targetTexture = previousTarget;
                    
                    // RenderTextureをキャプチャ
                    string filePath = CaptureAPI.Capture.CaptureRenderTexture(renderTexture);
                    Debug.Log($"RenderTextureキャプチャ完了: {filePath}");
                }
            }
            finally
            {
                // リソースをクリーンアップ
                renderTexture.Release();
                Object.DestroyImmediate(renderTexture);
            }
        }
        
        /// <summary>
        /// 高度な設定例
        /// </summary>
        [MenuItem("Tools/CaptureWindow/Samples/Advanced Settings")]
        public static void AdvancedSettingsExample()
        {
            // デバッグ用の詳細設定
            CaptureAPI.Settings.Language = CaptureWindow.Language.Japanese;
            CaptureAPI.Settings.PathMode = CaptureWindow.PathMode.Absolute;
            CaptureAPI.Settings.OutputDirectory = System.IO.Path.Combine(Application.dataPath, "../Screenshots");
            CaptureAPI.Settings.FileNameTemplate = "Debug_<Name>_<Date>_<Time>_<MilliSec>";
            CaptureAPI.Settings.UseFixedTakeDigits = true;
            CaptureAPI.Settings.TakeDigits = 5;
            CaptureAPI.Settings.AutoIncrementTake = false;
            CaptureAPI.Settings.AutoRefreshAssets = false;
            
            // プレビュー確認
            string pathPreview = CaptureAPI.Utilities.GetOutputPathPreview();
            string filePreview = CaptureAPI.Utilities.GetFileNamePreview();
            
            Debug.Log($"出力パスプレビュー: {pathPreview}");
            Debug.Log($"ファイル名プレビュー: {filePreview}");
            
            // キャプチャ実行
            string filePath = CaptureAPI.Capture.CaptureWithCurrentSettings();
            Debug.Log($"高度な設定でキャプチャ完了: {filePath}");
        }
        
        /// <summary>
        /// エラーハンドリング例
        /// </summary>
        [MenuItem("Tools/CaptureWindow/Samples/Error Handling")]
        public static void ErrorHandlingExample()
        {
            try
            {
                // 無効な設定でキャプチャを試行
                CaptureAPI.Settings.OutputDirectory = "";
                CaptureAPI.Settings.FileNameTemplate = "";
                
                string filePath = CaptureAPI.Capture.CaptureGameView();
                
                if (string.IsNullOrEmpty(filePath))
                    Debug.LogError("キャプチャが失敗しました");
                else
                    Debug.Log($"キャプチャ成功: {filePath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"キャプチャエラー: {ex.Message}");
            }
            finally
            {
                // 設定をデフォルトに戻す
                CaptureAPI.Utilities.ResetToDefault();
                Debug.Log("設定をデフォルトにリセットしました");
            }
        }
        
        /// <summary>
        /// プリセット作成例
        /// </summary>
        [MenuItem("Tools/CaptureWindow/Samples/Create Presets")]
        public static void CreatePresetsExample()
        {
            // 高品質プリセット
            var highQualityConfig = CaptureConfig.CreateForGameView();
            highQualityConfig.IncludeAlpha = true;
            highQualityConfig.FileNameTemplate = "HighQuality_<Name>_<Date>_<Time>";
            highQualityConfig.OutputDirectory = "../HighQualityCaptures";
            
            // デバッグプリセット
            var debugConfig = CaptureConfig.CreateForSceneView();
            debugConfig.FileNameTemplate = "Debug_<Take>";
            debugConfig.UseFixedTakeDigits = true;
            debugConfig.TakeDigits = 3;
            debugConfig.OutputDirectory = "../DebugCaptures";
            
            // プロダクションプリセット
            var productionConfig = CaptureConfig.CreateForGameView();
            productionConfig.Language = CaptureWindow.Language.English;
            productionConfig.FileNameTemplate = "Production_<Name>_v<Take>";
            productionConfig.UseTransparentBackground = true;
            productionConfig.AutoRefreshAssets = true;
            productionConfig.OutputDirectory = "../ProductionCaptures";
            
            // 各プリセットでキャプチャ
            Debug.Log("=== 高品質キャプチャ ===");
            string highQualityPath = CaptureAPI.Capture.CaptureWithConfig(highQualityConfig);
            Debug.Log($"高品質キャプチャ: {highQualityPath}");
            
            Debug.Log("=== デバッグキャプチャ ===");
            string debugPath = CaptureAPI.Capture.CaptureWithConfig(debugConfig);
            Debug.Log($"デバッグキャプチャ: {debugPath}");
            
            Debug.Log("=== プロダクションキャプチャ ===");
            string productionPath = CaptureAPI.Capture.CaptureWithConfig(productionConfig);
            Debug.Log($"プロダクションキャプチャ: {productionPath}");
        }
        
        /// <summary>
        /// ユーティリティ機能例
        /// </summary>
        [MenuItem("Tools/CaptureWindow/Samples/Utilities")]
        public static void UtilitiesExample()
        {
            // ウィンドウを開く
            CaptureAPI.Utilities.OpenWindow();
            
            // 出力ディレクトリを開く
            CaptureAPI.Utilities.OpenOutputDirectory();
            
            // プレビュー情報を取得
            string pathPreview = CaptureAPI.Utilities.GetOutputPathPreview();
            string filePreview = CaptureAPI.Utilities.GetFileNamePreview();
            
            Debug.Log($"現在の出力パス: {pathPreview}");
            Debug.Log($"現在のファイル名: {filePreview}");
            
            // 設定をリセット
            CaptureAPI.Utilities.ResetToDefault();
            Debug.Log("設定をデフォルトにリセットしました");
        }
    }
}