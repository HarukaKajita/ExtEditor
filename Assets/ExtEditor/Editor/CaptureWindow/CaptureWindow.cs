using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ExtEditor.Editor.CaptureWindow
{
	public class CaptureWindow : EditorWindow
    {
        private enum CaptureSource
        {
            GameView,
            SceneView,
            RenderTexture
        }

        private Camera captureCamera;
        private string outputDirectory = "../Captures";
        private bool includeAlpha = false;
        private bool useTransparentBackground = false;
        private CaptureSource captureSource = CaptureSource.GameView;
        private RenderTexture customRenderTexture;
        private Camera[] sceneCamera;
        private string[] sceneCameraNames;
        private int selectedCameraIndex = 0;

        [MenuItem("Tools/CaptureWindow")]
        public static void ShowWindow()
        {
            GetWindow<CaptureWindow>("CaptureWindow");
        }

        private void OnEnable()
        {
            UpdateSceneCameras();
            
            // MainCameraを探す
            captureCamera = Camera.main;
            
            // MainCameraがない場合はシーン上の最初のカメラを使用
            if (captureCamera == null && sceneCamera.Length > 0)
            {
                captureCamera = sceneCamera[0];
            }

            // 選択中のカメラのインデックスを設定
            selectedCameraIndex = System.Array.IndexOf(sceneCamera, captureCamera);
        }

        private void UpdateSceneCameras()
        {
            // シーン内のすべてのカメラを取得
            sceneCamera = FindObjectsOfType<Camera>();
            
            // 先頭にnullを追加
            var cameraList = new List<Camera> { null };
            cameraList.AddRange(sceneCamera);
            sceneCamera = cameraList.ToArray();

            var nameList = new List<string> { "None" };
            nameList.AddRange(sceneCamera.Skip(1).Select(cam => cam != null ? cam.gameObject.name : "None"));
            sceneCameraNames = nameList.ToArray();
        }

        private void OnGUI()
        {
            GUILayout.Label("PNG Capture", EditorStyles.boldLabel);

            captureSource = (CaptureSource)EditorGUILayout.EnumPopup("Capture Source", captureSource);

            // プルダウンメニューの更新
            EditorGUI.BeginChangeCheck();
            selectedCameraIndex = EditorGUILayout.Popup("Scene Cameras", selectedCameraIndex, sceneCameraNames);
            if (EditorGUI.EndChangeCheck() && selectedCameraIndex >= 0 && selectedCameraIndex < sceneCamera.Length)
            {
                captureCamera = sceneCamera[selectedCameraIndex];
            }

            // ObjectFieldの更新
            EditorGUI.BeginChangeCheck();
            captureCamera = (Camera)EditorGUILayout.ObjectField("Target Camera", captureCamera, typeof(Camera), true);
            if (EditorGUI.EndChangeCheck())
            {
                // ObjectFieldが変更された場合、プルダウンメニューも同期
                selectedCameraIndex = System.Array.IndexOf(sceneCamera, captureCamera);
                if (selectedCameraIndex == -1) selectedCameraIndex = 0; // カメラが見つからない場合はNoneを選択
            }
            if (captureSource == CaptureSource.RenderTexture)
            {
                customRenderTexture = (RenderTexture)EditorGUILayout.ObjectField("Render Texture", customRenderTexture, typeof(RenderTexture), true);
            }

            outputDirectory = EditorGUILayout.TextField("Output Directory (Assets/...)", outputDirectory);
            includeAlpha = EditorGUILayout.Toggle("Include Alpha Channel", includeAlpha);
            useTransparentBackground = EditorGUILayout.Toggle("Transparent Background", useTransparentBackground);

            EditorGUILayout.Space();

            if (GUILayout.Button("Capture and Save PNG"))
            {
                CaptureAndSave();
            }

            if (GUILayout.Button("Open Output Folder"))
            {
                OpenOutputDirectory();
            }
        }

        private void CaptureAndSave()
        {
            if (captureCamera == null)
            {
                Debug.LogError("カメラが指定されていません。");
                return;
            }

            int width = 0;
            int height = 0;

            switch (captureSource)
            {
                case CaptureSource.GameView:
                    uint widthUint = 0;
                    uint heightUint = 0;
                    PlayModeWindow.GetRenderingResolution(out widthUint, out heightUint);
                    width = (int)widthUint;
                    height = (int)heightUint;
                    break;

                case CaptureSource.SceneView:
                    SceneView sceneView = SceneView.lastActiveSceneView;
                    if (sceneView != null && sceneView.camera != null)
                    {
                        width = (int)sceneView.camera.pixelWidth;
                        height = (int)sceneView.camera.pixelHeight;
                    }
                    break;

                case CaptureSource.RenderTexture:
                    if (customRenderTexture != null)
                    {
                        width = customRenderTexture.width;
                        height = customRenderTexture.height;
                    }
                    break;
            }

            if (width <= 0 || height <= 0)
            {
                Debug.LogError("キャプチャ解像度が無効です。");
                return;
            }

            RenderTexture renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            renderTexture.Create();

            TextureFormat format = includeAlpha ? TextureFormat.RGBA32 : TextureFormat.RGB24;
            Texture2D tex = new Texture2D(width, height, format, false);

            RenderTexture prevActive = RenderTexture.active;
            RenderTexture prevCamTarget = captureCamera.targetTexture;

            captureCamera.targetTexture = renderTexture;

            // 背景色の設定
            Color originalBackgroundColor = Camera.main != null ? Camera.main.backgroundColor : Color.black;
            CameraClearFlags originalClearFlags = captureCamera.clearFlags;

            if (captureSource == CaptureSource.SceneView)
            {
                SceneView sceneView = SceneView.lastActiveSceneView;
                if (sceneView != null)
                {
                    originalBackgroundColor = sceneView.camera.backgroundColor;
                    originalClearFlags = sceneView.camera.clearFlags;

                    if(sceneView.camera.clearFlags == CameraClearFlags.Skybox)
                        captureCamera.clearFlags = CameraClearFlags.Skybox;
                }
            }
            if (useTransparentBackground)
            {
                captureCamera.clearFlags = CameraClearFlags.SolidColor;
                captureCamera.backgroundColor = Color.clear;
            }

            captureCamera.Render();
            RenderTexture.active = renderTexture;

            // カメラの設定を元に戻す
            if (useTransparentBackground)
            {
                captureCamera.clearFlags = originalClearFlags;
                captureCamera.backgroundColor = originalBackgroundColor;
            }
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            captureCamera.targetTexture = prevCamTarget;
            RenderTexture.active = prevActive;

            // 保存先ディレクトリを作成
            string relativePath = outputDirectory.StartsWith("Assets/") ? outputDirectory.Substring(7) : outputDirectory;
            string fullPath = Path.Combine(Application.dataPath, relativePath);
            Directory.CreateDirectory(fullPath);

            string fileName = $"Capture_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string filePath = Path.Combine(fullPath, fileName);
            File.WriteAllBytes(filePath, tex.EncodeToPNG());

            Debug.Log($"PNG を保存しました: {filePath}");

            AssetDatabase.Refresh();
            DestroyImmediate(tex);
            renderTexture.Release();
            DestroyImmediate(renderTexture);
        }

        private void OpenOutputDirectory()
        {
            string fullPath = Path.Combine(Application.dataPath, outputDirectory.Replace("Assets/", ""));
            Directory.CreateDirectory(fullPath);
            // EditorUtility.RevealInFinder(fullPath);
            System.Diagnostics.Process.Start(fullPath);
        }
    }
}
