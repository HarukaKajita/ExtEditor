using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace ExtEditor.Editor.CaptureWindow
{
    /// <summary>
    /// キャプチャ処理のコアロジック
    /// </summary>
    public class CaptureCore
    {
        private readonly CaptureWindowState state;
        
        public CaptureCore(CaptureWindowState state)
        {
            this.state = state;
        }
        
        /// <summary>
        /// キャプチャを実行して保存
        /// </summary>
        public string CaptureAndSave()
        {
            // 事前バリデーション
            if (!ValidateBeforeCapture())
            {
                return null;
            }

            RenderTexture renderTexture = null;
            Texture2D tex = null;
            RenderTexture prevActive = RenderTexture.active;
            RenderTexture prevCamTarget = state.CaptureCamera.targetTexture;
            Color originalBackgroundColor = state.CaptureCamera.backgroundColor;
            CameraClearFlags originalClearFlags = state.CaptureCamera.clearFlags;
            
            try
            {
                // 解像度取得
                if (!GetCaptureResolution(out int width, out int height))
                {
                    Debug.LogError(state.GetText("キャプチャ解像度が無効です", "Invalid capture resolution"));
                    return null;
                }

                // リソース作成
                renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
                if (!renderTexture.Create())
                {
                    throw new Exception(state.GetText("RenderTextureの作成に失敗しました", "Failed to create RenderTexture"));
                }

                TextureFormat format = state.IncludeAlpha ? TextureFormat.RGBA32 : TextureFormat.RGB24;
                tex = new Texture2D(width, height, format, false);

                // カメラ設定
                state.CaptureCamera.targetTexture = renderTexture;
                SetupCameraForCapture();

                // 撮影
                state.CaptureCamera.Render();
                RenderTexture.active = renderTexture;
                tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                tex.Apply();

                // 保存
                string savedFilePath = SaveCapturedTexture(tex);
                
                // 保存が成功した場合のみ状態を更新
                if (!string.IsNullOrEmpty(savedFilePath))
                {
                    state.OnCaptureSuccess(savedFilePath);
                }
                
                return savedFilePath;
            }
            catch (Exception ex)
            {
                Debug.LogError($"{state.GetText("キャプチャエラー", "Capture Error")}: {ex.Message}");
                return null;
            }
            finally
            {
                // 確実なリソース解放
                if (tex != null) UnityEngine.Object.DestroyImmediate(tex);
                if (renderTexture != null)
                {
                    RenderTexture.active = null;
                    renderTexture.Release();
                }
                
                // カメラ状態復元
                state.CaptureCamera.targetTexture = prevCamTarget;
                state.CaptureCamera.backgroundColor = originalBackgroundColor;
                state.CaptureCamera.clearFlags = originalClearFlags;
                RenderTexture.active = prevActive;
            }
        }
        
        /// <summary>
        /// キャプチャ前のバリデーション
        /// </summary>
        private bool ValidateBeforeCapture()
        {
            if (state.CaptureCamera == null)
            {
                Debug.LogError(state.GetText("カメラが指定されていません", "Camera is not specified"));
                return false;
            }
            
            if (state.CaptureSource == CaptureWindow.CaptureSource.RenderTexture && state.CustomRenderTexture == null)
            {
                Debug.LogError(state.GetText("レンダーテクスチャが指定されていません", "RenderTexture is not specified"));
                return false;
            }
            
            return state.IsOutputPathValid && state.IsFileNameValid;
        }
        
        /// <summary>
        /// キャプチャ解像度を取得
        /// </summary>
        private bool GetCaptureResolution(out int width, out int height)
        {
            width = 0;
            height = 0;
            
            try
            {
                switch (state.CaptureSource)
                {
                    case CaptureWindow.CaptureSource.GameView:
                        PlayModeWindow.GetRenderingResolution(out uint widthUint, out uint heightUint);
                        width = (int)widthUint;
                        height = (int)heightUint;
                        break;

                    case CaptureWindow.CaptureSource.SceneView:
                        SceneView sceneView = SceneView.lastActiveSceneView;
                        if (sceneView?.camera != null)
                        {
                            width = (int)sceneView.camera.pixelWidth;
                            height = (int)sceneView.camera.pixelHeight;
                        }
                        break;

                    case CaptureWindow.CaptureSource.RenderTexture:
                        if (state.CustomRenderTexture != null)
                        {
                            width = state.CustomRenderTexture.width;
                            height = state.CustomRenderTexture.height;
                        }
                        break;
                }
                
                return width > 0 && height > 0;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// キャプチャ用のカメラ設定
        /// </summary>
        private void SetupCameraForCapture()
        {
            if (state.CaptureSource == CaptureWindow.CaptureSource.SceneView)
            {
                SceneView sceneView = SceneView.lastActiveSceneView;
                if (sceneView?.camera != null && sceneView.camera.clearFlags == CameraClearFlags.Skybox)
                {
                    state.CaptureCamera.clearFlags = CameraClearFlags.Skybox;
                }
            }
            
            if (state.UseTransparentBackground)
            {
                state.CaptureCamera.clearFlags = CameraClearFlags.SolidColor;
                state.CaptureCamera.backgroundColor = Color.clear;
            }
        }
        
        /// <summary>
        /// テクスチャをPNGとして保存
        /// </summary>
        private string SaveCapturedTexture(Texture2D tex)
        {
            try
            {
                string fullPath = state.ResolvedOutputPath;
                Directory.CreateDirectory(fullPath);

                string fileName = state.PreviewFileName;
                string filePath = Path.Combine(fullPath, fileName);
                
                File.WriteAllBytes(filePath, tex.EncodeToPNG());
                
                Debug.Log(state.GetText($"PNGを保存しました: {filePath}", $"PNG saved: {filePath}"));
                
                if (state.AutoRefreshAssets)
                {
                    AssetDatabase.Refresh();
                }
                
                return filePath;
            }
            catch (Exception ex)
            {
                Debug.LogError($"{state.GetText("保存エラー", "Save Error")}: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// 出力ディレクトリを開く
        /// </summary>
        public void OpenOutputDirectory()
        {
            try
            {
                string fullPath = state.ResolvedOutputPath;
                Directory.CreateDirectory(fullPath);
                EditorUtility.RevealInFinder(fullPath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"{state.GetText($"フォルダを開けませんでした: {ex.Message}", $"Cannot open folder: {ex.Message}")}");
            }
        }
    }
}