using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ExtEditor.Editor.CaptureWindow
{
    public class CaptureWindow : EditorWindow
    {
        private static CaptureWindow lastInstance;
        
        // 言語システム
        public enum Language { Japanese, English }
        
        // パス指定方式
        public enum PathMode { Relative, Absolute }
        
        public enum CaptureSource
        {
            GameView,
            SceneView,
            RenderTexture
        }

        // 状態管理
        private CaptureWindowState state;
        private CaptureCore captureCore;
        private CaptureWindowUI ui;

        [MenuItem("Tools/ExtEditor/CaptureWindow/Open Capture Window")]
        public static void ShowWindow()
        {
            GetWindow<CaptureWindow>("CaptureWindow");
        }

        // パス解決メソッド
        private string ResolveOutputPath()
        {
            try
            {
                // パターン置換を適用
                string resolvedDirectory = ResolvePattern(state.OutputDirectory, false);
                
                if (state.PathMode == PathMode.Absolute)
                {
                    return Path.GetFullPath(resolvedDirectory);
                }
                else
                {
                    string relativePath = resolvedDirectory.StartsWith("Assets/") ? resolvedDirectory.Substring(7) : resolvedDirectory.TrimStart('/', '\\');
                    return Path.GetFullPath(Path.Combine(Application.dataPath, relativePath));
                }
            }
            catch
            {
                return state.GetText("無効なパス", "Invalid Path");
            }
        }
        
        // パターン置換メソッド
        private string ResolvePattern(string template, bool sanitize)
        {
            var context = new PatternContext
            {
                TakeNumber = state.TakeNumber,
                UseFixedDigits = state.UseFixedTakeDigits,
                DigitCount = state.TakeDigits,
                CaptureSource = state.CaptureSource,
                CaptureCamera = state.CaptureCamera,
                CustomRenderTexture = state.CustomRenderTexture
            };
            
            return PatternResolver.ResolvePattern(template, context, sanitize);
        }
        
        // バリデーションメソッド
        private void ValidateInputs()
        {
            // パスバリデーション
            var pathResult = CaptureValidator.ValidatePath(state.OutputDirectory);
            state.IsOutputPathValid = pathResult.IsValid;
            
            // ファイル名バリデーション
            string resolvedFileName = ResolvePattern(state.FileNameTemplate, true);
            var fileResult = CaptureValidator.ValidateFileName(resolvedFileName);
            state.IsFileNameValid = fileResult.IsValid;
            
            // エラーメッセージ設定
            if (!state.IsOutputPathValid)
            {
                state.ValidationMessage = pathResult.GetMessage(state.CurrentLanguage);
            }
            else if (!state.IsFileNameValid)
            {
                state.ValidationMessage = fileResult.GetMessage(state.CurrentLanguage);
            }
            else
            {
                state.ValidationMessage = "";
            }
            
            // プレビュー更新
            state.ResolvedOutputPath = ResolveOutputPath();
            state.PreviewFileName = resolvedFileName + ".png";
        }

        private void OnEnable()
        {
            lastInstance = this;
            
            // 状態とコンポーネントの初期化
            if (state == null)
            {
                state = new CaptureWindowState();
            }
            captureCore = new CaptureCore(state);
            ui = new CaptureWindowUI(state, ValidateInputs, UpdateOutputDirectoryForPathMode, SelectOutputFolderCallback);
            ui.SetWindow(this);
            
            UpdateSceneCameras();
            
            // MainCameraを探す
            state.CaptureCamera = Camera.main;
            
            // MainCameraがない場合はシーン上の最初のカメラを使用
            if (state.CaptureCamera == null && state.SceneCameras.Length > 0)
            {
                state.CaptureCamera = state.SceneCameras[0];
            }

            // 選択中のカメラのインデックスを設定
            state.SelectedCameraIndex = System.Array.IndexOf(state.SceneCameras, state.CaptureCamera);
            
            // 初期バリデーション
            ValidateInputs();
        }

        private void OnDisable()
        {
            if (lastInstance == this)
            {
                lastInstance = null;
            }
        }

        private void UpdateSceneCameras()
        {
            // シーン内のすべてのカメラを取得
            var sceneCameras = FindObjectsOfType<Camera>();
            
            // 先頭にnullを追加
            var cameraList = new List<Camera> { null };
            cameraList.AddRange(sceneCameras);
            state.SceneCameras = cameraList.ToArray();

            var nameList = new List<string> { "None" };
            nameList.AddRange(state.SceneCameras.Skip(1).Select(cam => cam != null ? cam.gameObject.name : "None"));
            state.SceneCameraNames = nameList.ToArray();
        }

        private void OnGUI()
        {
            ui.DrawGUI();
            
            // ボタンクリックイベントを処理
            Event e = Event.current;
            if (e.type == EventType.Used)
            {
                return;
            }
            
            // CaptureWindowUIクラスのDrawActionButtonsメソッドを修正して
            // ボタンクリック時にこのメソッドを呼び出すようにする必要がある
            // 現在の実装では、UIクラス内でボタンを描画しているが、
            // クリックイベントはこのクラスで処理する必要がある
        }

        // UIから呼び出されるメソッド
        public void CaptureAndSave()
        {
            string savedFilePath = captureCore.CaptureAndSave();
            if (!string.IsNullOrEmpty(savedFilePath))
            {
                ValidateInputs(); // UIを更新
            }
        }

        public void OpenOutputDirectory()
        {
            captureCore.OpenOutputDirectory();
        }
        
        // パス指定方式切り替え時の出力フォルダ更新
        private void UpdateOutputDirectoryForPathMode()
        {
            if (string.IsNullOrEmpty(state.OutputDirectory)) return;

            var splits = state.OutputDirectory.Split("<");
            var baseDirectory = state.OutputDirectory;
            var patternDirectory = "";
            if (splits.Length > 1)
            {
                // パターンが含まれている場合は分割してベースディレクトリとパターンディレクトリを取得
                baseDirectory = splits[0];
                patternDirectory = string.Join("<", splits.Skip(1));
            }
            
            try
            {
                if (state.PathMode == PathMode.Relative)
                {
                    // 絶対パスから相対パスに変換
                    if (Path.IsPathRooted(baseDirectory))
                    {
                        string dataPath = Application.dataPath;
                        if (baseDirectory.StartsWith(dataPath))
                            baseDirectory = baseDirectory.Substring(dataPath.Length).TrimStart('/', '\\');
                        else
                            baseDirectory = Path.GetRelativePath(dataPath, baseDirectory);
                    }
                }
                else
                {
                    // 先頭にスラッシュがあるとCombineで意図しない結果になるので先頭のスラッシュを削除
                    if(Path.IsPathRooted(baseDirectory))
                        baseDirectory = baseDirectory.TrimStart('/', '\\');
                    // 相対パスから絶対パスに変換
                    baseDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, baseDirectory));
                }
                state.OutputDirectory = baseDirectory + patternDirectory;
            }
            catch
            {
                // パス変換に失敗した場合はデフォルト値を設定
                state.OutputDirectory = state.PathMode == PathMode.Relative ? "/Captures" : Path.GetFullPath(Path.Combine(Application.dataPath, "/Captures"));
            }
        }
        
        // フォルダ選択コールバック
        private void SelectOutputFolderCallback(string selectedPath)
        {
            if (state.PathMode == PathMode.Relative)
            {
                // 相対パスに変換
                string dataPath = Application.dataPath;
                if (selectedPath.StartsWith(dataPath))
                {
                    state.OutputDirectory = selectedPath.Substring(dataPath.Length).TrimStart('/', '\\');
                }
                else
                {
                    state.OutputDirectory = Path.GetRelativePath(dataPath, selectedPath);
                }
            }
            else
            {
                state.OutputDirectory = selectedPath;
            }
            
            ValidateInputs();
        }

        [MenuItem("Tools/ExtEditor/CaptureWindow/Capture & Save")]
        [Shortcut("ExtEditor/CaptureWindow/Capture & Save", KeyCode.F9, ShortcutModifiers.Action | ShortcutModifiers.Shift)]
        private static void CaptureAndSaveShortcut()
        {
            if (lastInstance != null)
            {
                lastInstance.CaptureAndSave();
                return;
            }

            var temp = CreateInstance<CaptureWindow>();
            temp.OnEnable();
            temp.CaptureAndSave();
            DestroyImmediate(temp);
        }
        
        // 外部API用メソッド
        
        public Language GetCurrentLanguage() => state.CurrentLanguage;
        public void SetCurrentLanguage(Language language) 
        { 
            state.CurrentLanguage = language;
            ValidateInputs();
        }
        
        public PathMode GetCurrentPathMode() => state.PathMode;
        public void SetCurrentPathMode(PathMode mode) 
        { 
            state.PathMode = mode;
            ValidateInputs();
        }
        
        public string GetCurrentOutputDirectory() => state.OutputDirectory;
        public void SetCurrentOutputDirectory(string directory) 
        { 
            state.OutputDirectory = directory;
            ValidateInputs();
        }
        
        public string GetCurrentFileNameTemplate() => state.FileNameTemplate;
        public void SetCurrentFileNameTemplate(string template) 
        { 
            state.FileNameTemplate = template;
            ValidateInputs();
        }
        
        public int GetCurrentTakeNumber() => state.TakeNumber;
        public void SetCurrentTakeNumber(int number) 
        { 
            state.TakeNumber = Mathf.Max(1, number);
            ValidateInputs();
        }
        
        public bool GetCurrentIncludeAlpha() => state.IncludeAlpha;
        public void SetCurrentIncludeAlpha(bool include) => state.IncludeAlpha = include;
        
        public bool GetCurrentUseTransparentBackground() => state.UseTransparentBackground;
        public void SetCurrentUseTransparentBackground(bool use) => state.UseTransparentBackground = use;
        
        
        public bool GetCurrentUseFixedTakeDigits() => state.UseFixedTakeDigits;
        public void SetCurrentUseFixedTakeDigits(bool use) 
        { 
            state.UseFixedTakeDigits = use;
            ValidateInputs();
        }
        
        public int GetCurrentTakeDigits() => state.TakeDigits;
        public void SetCurrentTakeDigits(int digits) 
        { 
            state.TakeDigits = Mathf.Clamp(digits, 1, 10);
            ValidateInputs();
        }
        
        public bool GetCurrentAutoRefreshAssets() => state.AutoRefreshAssets;
        public void SetCurrentAutoRefreshAssets(bool auto) => state.AutoRefreshAssets = auto;
        
        public void SetCaptureSource(CaptureSource source) 
        { 
            state.CaptureSource = source;
            ValidateInputs();
        }
        
        public void SetCaptureCamera(Camera camera) 
        { 
            state.CaptureCamera = camera;
            // カメラ配列のインデックスも更新
            state.SelectedCameraIndex = System.Array.IndexOf(state.SceneCameras, state.CaptureCamera);
            if (state.SelectedCameraIndex == -1) state.SelectedCameraIndex = 0;
            ValidateInputs();
        }
        
        public void SetCustomRenderTexture(RenderTexture renderTexture) 
        { 
            state.CustomRenderTexture = renderTexture;
            ValidateInputs();
        }
        
        public string ExecuteCaptureExternal()
        {
            CaptureAndSave();
            return state.LastCapturedFilePath;
        }
        
        public void OpenOutputDirectoryExternal()
        {
            OpenOutputDirectory();
        }
        
        public string GetFileNamePreview()
        {
            return state.PreviewFileName;
        }
        
        public string GetOutputPathPreview()
        {
            return state.ResolvedOutputPath;
        }
    }
    
    // ヘルパークラス
    public static class CaptureHelper
    {
        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return "Capture";
            
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }
            
            return fileName.Trim();
        }
        
        public static string GetCrossplatformPath(string path)
        {
            return path.Replace('\\', Path.DirectorySeparatorChar)
                      .Replace('/', Path.DirectorySeparatorChar);
        }
    }
}