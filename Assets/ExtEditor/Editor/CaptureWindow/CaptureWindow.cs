using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ExtEditor.Editor.CaptureWindow
{
    public class CaptureWindow : EditorWindow
    {
        private static CaptureWindow lastInstance;
        
        // 言語システム
        public enum Language { Japanese, English }
        private Language currentLanguage = Language.Japanese;
        
        // パス指定方式
        public enum PathMode { Relative, Absolute }
        private PathMode pathMode = PathMode.Relative;
        
        public enum CaptureSource
        {
            GameView,
            SceneView,
            RenderTexture
        }

        // 基本設定
        private Camera captureCamera;
        private string outputDirectory = "../Captures";
        private bool includeAlpha = false;
        private bool useTransparentBackground = false;
        private CaptureSource captureSource = CaptureSource.GameView;
        private RenderTexture customRenderTexture;
        private Camera[] sceneCamera;
        private string[] sceneCameraNames;
        private int selectedCameraIndex = 0;
        
        // 新機能フィールド
        private string fileNameTemplate = "Capture_<Date>_<Time>";
        private int takeNumber = 1;
        private bool showHelp = false;
        
        // バリデーション
        private bool isOutputPathValid = true;
        private bool isFileNameValid = true;
        private string validationMessage = "";
        
        // プレビュー
        private string resolvedOutputPath = "";
        private string previewFileName = "";
        
        // 外部API用フラグ
        private string lastCapturedFilePath = "";
        
        // 追加機能
        private bool autoIncrementTake = true;
        private bool useFixedTakeDigits = false;
        private int takeDigits = 3;
        private bool autoRefreshAssets = true;
        private string[] recentCaptures = new string[5];

        [MenuItem("Tools/CaptureWindow")]
        public static void ShowWindow()
        {
            GetWindow<CaptureWindow>("CaptureWindow");
        }
        
        // 言語システム
        private string GetText(string japanese, string english)
        {
            return currentLanguage == Language.Japanese ? japanese : english;
        }
        
        // パス解決メソッド
        private string ResolveOutputPath()
        {
            try
            {
                if (pathMode == PathMode.Absolute)
                {
                    return Path.GetFullPath(outputDirectory);
                }
                else
                {
                    string relativePath = outputDirectory.StartsWith("Assets/") ? outputDirectory.Substring(7) : outputDirectory.TrimStart('/', '\\');
                    return Path.GetFullPath(Path.Combine(Application.dataPath, relativePath));
                }
            }
            catch
            {
                return GetText("無効なパス", "Invalid Path");
            }
        }
        
        // パターン置換メソッド
        private string ResolvePattern(string template)
        {
            var context = new PatternContext
            {
                TakeNumber = takeNumber,
                UseFixedDigits = useFixedTakeDigits,
                DigitCount = takeDigits,
                CaptureSource = captureSource,
                CaptureCamera = captureCamera,
                CustomRenderTexture = customRenderTexture
            };
            
            return PatternResolver.ResolvePattern(template, context);
        }
        
        // バリデーションメソッド
        private void ValidateInputs()
        {
            // パスバリデーション
            var pathResult = CaptureValidator.ValidatePath(outputDirectory);
            isOutputPathValid = pathResult.IsValid;
            
            // ファイル名バリデーション
            string resolvedFileName = ResolvePattern(fileNameTemplate);
            var fileResult = CaptureValidator.ValidateFileName(resolvedFileName);
            isFileNameValid = fileResult.IsValid;
            
            // エラーメッセージ設定
            if (!isOutputPathValid)
            {
                validationMessage = pathResult.GetMessage(currentLanguage);
            }
            else if (!isFileNameValid)
            {
                validationMessage = fileResult.GetMessage(currentLanguage);
            }
            else
            {
                validationMessage = "";
            }
            
            // プレビュー更新
            resolvedOutputPath = ResolveOutputPath();
            previewFileName = resolvedFileName + ".png";
        }

        private void OnEnable()
        {
            lastInstance = this;
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
            // 言語切り替え
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(GetText("PNGキャプチャ", "PNG Capture"), EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            currentLanguage = (Language)EditorGUILayout.EnumPopup(currentLanguage, GUILayout.Width(80));
            if (EditorGUI.EndChangeCheck())
            {
                ValidateInputs();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();

            // キャプチャソース
            EditorGUI.BeginChangeCheck();
            captureSource = (CaptureSource)EditorGUILayout.EnumPopup(GetText("キャプチャソース", "Capture Source"), captureSource);
            if (EditorGUI.EndChangeCheck())
            {
                ValidateInputs();
            }

            // プルダウンメニューの更新
            EditorGUI.BeginChangeCheck();
            selectedCameraIndex = EditorGUILayout.Popup(GetText("シーンカメラ", "Scene Cameras"), selectedCameraIndex, sceneCameraNames);
            if (EditorGUI.EndChangeCheck() && selectedCameraIndex >= 0 && selectedCameraIndex < sceneCamera.Length)
            {
                captureCamera = sceneCamera[selectedCameraIndex];
                ValidateInputs();
            }

            // ObjectFieldの更新
            EditorGUI.BeginChangeCheck();
            captureCamera = (Camera)EditorGUILayout.ObjectField(GetText("ターゲットカメラ", "Target Camera"), captureCamera, typeof(Camera), true);
            if (EditorGUI.EndChangeCheck())
            {
                selectedCameraIndex = System.Array.IndexOf(sceneCamera, captureCamera);
                if (selectedCameraIndex == -1) selectedCameraIndex = 0;
                ValidateInputs();
            }
            
            if (captureSource == CaptureSource.RenderTexture)
            {
                EditorGUI.BeginChangeCheck();
                customRenderTexture = (RenderTexture)EditorGUILayout.ObjectField(GetText("レンダーテクスチャ", "Render Texture"), customRenderTexture, typeof(RenderTexture), true);
                if (EditorGUI.EndChangeCheck())
                {
                    ValidateInputs();
                }
            }
            
            EditorGUILayout.Space();
            
            // パス指定方式
            EditorGUI.BeginChangeCheck();
            pathMode = (PathMode)EditorGUILayout.EnumPopup(GetText("パス指定方式", "Path Mode"), pathMode);
            if (EditorGUI.EndChangeCheck())
            {
                ValidateInputs();
            }

            // 出力ディレクトリ
            EditorGUI.BeginChangeCheck();
            string directoryLabel = pathMode == PathMode.Relative ? 
                GetText("出力ディレクトリ (相対パス)", "Output Directory (Relative)") :
                GetText("出力ディレクトリ (絶対パス)", "Output Directory (Absolute)");
            outputDirectory = EditorGUILayout.TextField(directoryLabel, outputDirectory);
            if (EditorGUI.EndChangeCheck())
            {
                ValidateInputs();
            }
            
            // 絶対パス表示
            if (!isOutputPathValid)
            {
                EditorGUILayout.HelpBox(validationMessage, MessageType.Error);
            }
            else
            {
                EditorGUILayout.LabelField(GetText("解決されたパス", "Resolved Path"), resolvedOutputPath, EditorStyles.miniLabel);
            }
            
            EditorGUILayout.Space();
            
            // ファイル名テンプレート
            EditorGUI.BeginChangeCheck();
            fileNameTemplate = EditorGUILayout.TextField(GetText("ファイル名テンプレート", "Filename Template"), fileNameTemplate);
            if (EditorGUI.EndChangeCheck())
            {
                ValidateInputs();
            }
            
            // テイク番号
            EditorGUI.BeginChangeCheck();
            takeNumber = EditorGUILayout.IntField(GetText("テイク番号", "Take Number"), takeNumber);
            if (EditorGUI.EndChangeCheck())
            {
                takeNumber = Mathf.Max(1, takeNumber);
                ValidateInputs();
            }
            
            // テイク番号桁数固定オプション
            EditorGUI.BeginChangeCheck();
            useFixedTakeDigits = EditorGUILayout.Toggle(GetText("テイク番号桁数固定", "Fixed Take Digits"), useFixedTakeDigits);
            if (useFixedTakeDigits)
            {
                EditorGUI.indentLevel++;
                takeDigits = EditorGUILayout.IntSlider(GetText("桁数", "Digits"), takeDigits, 1, 10);
                EditorGUI.indentLevel--;
            }
            if (EditorGUI.EndChangeCheck())
            {
                ValidateInputs();
            }
            
            // ファイル名プレビュー
            if (!isFileNameValid)
            {
                EditorGUILayout.HelpBox(validationMessage, MessageType.Error);
            }
            else
            {
                // ファイル名プレビュー
                var previewStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    wordWrap = true
                };
                EditorGUILayout.LabelField(GetText("プレビュー", "Preview"), previewFileName, previewStyle);
                
                // 完全パスプレビュー
                string fullPathPreview = System.IO.Path.Combine(resolvedOutputPath, previewFileName);
                var pathStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    wordWrap = true,
                    normal = { textColor = Color.gray }
                };
                
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField(GetText("完全パス", "Full Path"), EditorStyles.miniLabel);
                EditorGUILayout.SelectableLabel(fullPathPreview, pathStyle, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2));
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.Space();

            // オプション
            includeAlpha = EditorGUILayout.Toggle(GetText("アルファチャンネルを含める", "Include Alpha Channel"), includeAlpha);
            useTransparentBackground = EditorGUILayout.Toggle(GetText("透明背景", "Transparent Background"), useTransparentBackground);

            EditorGUILayout.Space();

            // ボタン
            EditorGUI.BeginDisabledGroup(!isOutputPathValid || !isFileNameValid);
            if (GUILayout.Button(GetText("キャプチャしてPNG保存", "Capture and Save PNG")))
            {
                CaptureAndSave();
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button(GetText("出力フォルダを開く", "Open Output Folder")))
            {
                OpenOutputDirectory();
            }
            
            EditorGUILayout.Space();
            
            // 追加オプション
            autoIncrementTake = EditorGUILayout.Toggle(GetText("テイク番号自動インクリメント", "Auto Increment Take"), autoIncrementTake);
            autoRefreshAssets = EditorGUILayout.Toggle(GetText("キャプチャ後にアセット更新", "Auto Refresh Assets"), autoRefreshAssets);
            
            // 最近のキャプチャ表示
            if (recentCaptures[0] != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(GetText("最近のキャプチャ", "Recent Captures"), EditorStyles.boldLabel);
                for (int i = 0; i < recentCaptures.Length && recentCaptures[i] != null; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(Path.GetFileName(recentCaptures[i]), EditorStyles.miniLabel);
                    if (GUILayout.Button(GetText("表示", "Show"), GUILayout.Width(50)))
                    {
                        EditorUtility.RevealInFinder(recentCaptures[i]);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            
            EditorGUILayout.Space();
            
            // ヘルプセクション
            showHelp = EditorGUILayout.Foldout(showHelp, GetText("ヘルプ", "Help"));
            if (showHelp)
            {
                DrawHelpSection();
            }
        }
        
        private void DrawHelpSection()
        {
            EditorGUILayout.BeginVertical("box");
            
            // 使用方法
            EditorGUILayout.LabelField(GetText("使用方法", "Usage"), EditorStyles.boldLabel);
            var usageStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };
            EditorGUILayout.LabelField(GetText(
                "1. キャプチャソースを選択\n2. カメラを指定\n3. 出力フォルダとファイル名を設定\n4. キャプチャボタンをクリック",
                "1. Select capture source\n2. Specify camera\n3. Set output folder and filename\n4. Click capture button"
            ), usageStyle);
            
            EditorGUILayout.Space();
            
            // パターン一覧
            EditorGUILayout.LabelField(GetText("パターン一覧", "Pattern List"), EditorStyles.boldLabel);
            
            var patterns = PatternResolver.GetAvailablePatterns();
            var context = new PatternContext
            {
                TakeNumber = takeNumber,
                UseFixedDigits = useFixedTakeDigits,
                DigitCount = takeDigits,
                CaptureSource = captureSource,
                CaptureCamera = captureCamera,
                CustomRenderTexture = customRenderTexture
            };
            
            foreach (var pattern in patterns)
            {
                string example = PatternResolver.ResolvePattern(pattern.Pattern, context);
                DrawPatternHelp(pattern.Pattern, example, pattern.GetDescription(currentLanguage));
            }
            
            EditorGUILayout.Space();
            
            // オプション説明
            EditorGUILayout.LabelField(GetText("オプション", "Options"), EditorStyles.boldLabel);
            var optionsStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };
            EditorGUILayout.LabelField(GetText(
                "アルファチャンネル: 透明情報を含めるPNGを作成\n" +
                "透明背景: 背景を透明にして撮影\n" +
                "テイク番号自動インクリメント: 撮影後に自動でテイク番号を増加\n" +
                "テイク番号桁数固定: テイク番号の桁数を固定（例：001, 0001）\n" +
                "キャプチャ後にアセット更新: 撮影後にUnityのアセットデータベースを更新",
                "Alpha Channel: Create PNG with transparency\n" +
                "Transparent Background: Capture with transparent background\n" +
                "Auto Increment Take: Automatically increase take number after capture\n" +
                "Fixed Take Digits: Fix take number digits (e.g., 001, 0001)\n" +
                "Auto Refresh Assets: Refresh Unity asset database after capture"
            ), optionsStyle);
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawPatternHelp(string pattern, string example, string description)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(pattern, GUILayout.Width(80));
            EditorGUILayout.LabelField("→", GUILayout.Width(20));
            EditorGUILayout.LabelField(example, GUILayout.Width(100));
            
            var descStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                wordWrap = true
            };
            EditorGUILayout.LabelField($"({description})", descStyle);
            EditorGUILayout.EndHorizontal();
        }

        private void CaptureAndSave()
        {
            // 事前バリデーション
            if (!ValidateBeforeCapture())
            {
                return;
            }

            RenderTexture renderTexture = null;
            Texture2D tex = null;
            RenderTexture prevActive = RenderTexture.active;
            RenderTexture prevCamTarget = captureCamera.targetTexture;
            Color originalBackgroundColor = captureCamera.backgroundColor;
            CameraClearFlags originalClearFlags = captureCamera.clearFlags;
            
            try
            {
                // 解像度取得
                if (!GetCaptureResolution(out int width, out int height))
                {
                    EditorUtility.DisplayDialog(
                        GetText("エラー", "Error"),
                        GetText("キャプチャ解像度が無効です", "Invalid capture resolution"),
                        "OK");
                    return;
                }

                // リソース作成
                renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
                if (!renderTexture.Create())
                {
                    throw new Exception(GetText("RenderTextureの作成に失敗しました", "Failed to create RenderTexture"));
                }

                TextureFormat format = includeAlpha ? TextureFormat.RGBA32 : TextureFormat.RGB24;
                tex = new Texture2D(width, height, format, false);

                // カメラ設定
                captureCamera.targetTexture = renderTexture;
                SetupCameraForCapture();

                // 撮影
                captureCamera.Render();
                RenderTexture.active = renderTexture;
                tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                tex.Apply();

                // 保存
                string savedFilePath = SaveCapturedTexture(tex);
                
                // 保存が成功した場合のみ履歴に追加
                if (!string.IsNullOrEmpty(savedFilePath))
                {
                    AddToRecentCaptures(savedFilePath);
                    lastCapturedFilePath = savedFilePath;
                    
                    // テイク番号を自動インクリメント
                    if (autoIncrementTake)
                    {
                        takeNumber++;
                        ValidateInputs();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Capture Error: {ex.Message}");
                EditorUtility.DisplayDialog(
                    GetText("キャプチャエラー", "Capture Error"),
                    GetText($"撮影に失敗しました: {ex.Message}", $"Capture failed: {ex.Message}"),
                    "OK");
            }
            finally
            {
                // 確実なリソース解放
                if (tex != null) DestroyImmediate(tex);
                if (renderTexture != null)
                {
                    renderTexture.Release();
                    DestroyImmediate(renderTexture);
                }
                
                // カメラ状態復元
                captureCamera.targetTexture = prevCamTarget;
                captureCamera.backgroundColor = originalBackgroundColor;
                captureCamera.clearFlags = originalClearFlags;
                RenderTexture.active = prevActive;
            }
        }
        
        private bool ValidateBeforeCapture()
        {
            if (captureCamera == null)
            {
                EditorUtility.DisplayDialog(
                    GetText("エラー", "Error"),
                    GetText("カメラが指定されていません", "Camera is not specified"),
                    "OK");
                return false;
            }
            
            if (captureSource == CaptureSource.RenderTexture && customRenderTexture == null)
            {
                EditorUtility.DisplayDialog(
                    GetText("エラー", "Error"),
                    GetText("レンダーテクスチャが指定されていません", "RenderTexture is not specified"),
                    "OK");
                return false;
            }
            
            return isOutputPathValid && isFileNameValid;
        }
        
        private bool GetCaptureResolution(out int width, out int height)
        {
            width = 0;
            height = 0;
            
            try
            {
                switch (captureSource)
                {
                    case CaptureSource.GameView:
                        PlayModeWindow.GetRenderingResolution(out uint widthUint, out uint heightUint);
                        width = (int)widthUint;
                        height = (int)heightUint;
                        break;

                    case CaptureSource.SceneView:
                        SceneView sceneView = SceneView.lastActiveSceneView;
                        if (sceneView?.camera != null)
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
                
                return width > 0 && height > 0;
            }
            catch
            {
                return false;
            }
        }
        
        private void SetupCameraForCapture()
        {
            if (captureSource == CaptureSource.SceneView)
            {
                SceneView sceneView = SceneView.lastActiveSceneView;
                if (sceneView?.camera != null && sceneView.camera.clearFlags == CameraClearFlags.Skybox)
                {
                    captureCamera.clearFlags = CameraClearFlags.Skybox;
                }
            }
            
            if (useTransparentBackground)
            {
                captureCamera.clearFlags = CameraClearFlags.SolidColor;
                captureCamera.backgroundColor = Color.clear;
            }
        }
        
        private string SaveCapturedTexture(Texture2D tex)
        {
            try
            {
                string fullPath = ResolveOutputPath();
                Directory.CreateDirectory(fullPath);

                string fileName = ResolvePattern(fileNameTemplate) + ".png";
                string filePath = Path.Combine(fullPath, fileName);
                
                // ファイルの上書き確認
                if (File.Exists(filePath))
                {
                    bool overwrite = EditorUtility.DisplayDialog(
                        GetText("ファイルが存在します", "File Exists"),
                        GetText($"{fileName} は既に存在します。上書きしますか？", $"{fileName} already exists. Overwrite?"),
                        GetText("上書き", "Overwrite"),
                        GetText("キャンセル", "Cancel"));
                        
                    if (!overwrite) return null; // キャンセル時はnullを返す
                }
                
                File.WriteAllBytes(filePath, tex.EncodeToPNG());
                
                Debug.Log(GetText($"PNGを保存しました: {filePath}", $"PNG saved: {filePath}"));
                EditorUtility.DisplayDialog(
                    GetText("保存完了", "Save Complete"),
                    GetText($"ファイルを保存しました:\n{fileName}", $"File saved:\n{fileName}"),
                    "OK");
                
                if (autoRefreshAssets)
                {
                    AssetDatabase.Refresh();
                }
                
                return filePath; // 成功時はファイルパスを返す
            }
            catch (Exception ex)
            {
                Debug.LogError($"ファイル保存エラー: {ex.Message}");
                EditorUtility.DisplayDialog(
                    GetText("保存エラー", "Save Error"),
                    GetText($"ファイルの保存に失敗しました: {ex.Message}", $"Failed to save file: {ex.Message}"),
                    "OK");
                return null; // エラー時はnullを返す
            }
        }
        
        private void AddToRecentCaptures(string filePath)
        {
            // 配列を一つずらして新しいエントリを先頭に追加
            for (int i = recentCaptures.Length - 1; i > 0; i--)
            {
                recentCaptures[i] = recentCaptures[i - 1];
            }
            recentCaptures[0] = filePath;
        }

        private void OpenOutputDirectory()
        {
            try
            {
                string fullPath = ResolveOutputPath();
                Directory.CreateDirectory(fullPath);
                EditorUtility.RevealInFinder(fullPath);
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog(
                    GetText("エラー", "Error"),
                    GetText($"フォルダを開けませんでした: {ex.Message}", $"Cannot open folder: {ex.Message}"),
                    "OK");
            }
        }

        [MenuItem("Tools/CaptureWindow/Capture & Save")]
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
        
        public Language GetCurrentLanguage() => currentLanguage;
        public void SetCurrentLanguage(Language language) 
        { 
            currentLanguage = language;
            ValidateInputs();
        }
        
        public PathMode GetCurrentPathMode() => pathMode;
        public void SetCurrentPathMode(PathMode mode) 
        { 
            pathMode = mode;
            ValidateInputs();
        }
        
        public string GetCurrentOutputDirectory() => outputDirectory;
        public void SetCurrentOutputDirectory(string directory) 
        { 
            outputDirectory = directory;
            ValidateInputs();
        }
        
        public string GetCurrentFileNameTemplate() => fileNameTemplate;
        public void SetCurrentFileNameTemplate(string template) 
        { 
            fileNameTemplate = template;
            ValidateInputs();
        }
        
        public int GetCurrentTakeNumber() => takeNumber;
        public void SetCurrentTakeNumber(int number) 
        { 
            takeNumber = Mathf.Max(1, number);
            ValidateInputs();
        }
        
        public bool GetCurrentIncludeAlpha() => includeAlpha;
        public void SetCurrentIncludeAlpha(bool include) => includeAlpha = include;
        
        public bool GetCurrentUseTransparentBackground() => useTransparentBackground;
        public void SetCurrentUseTransparentBackground(bool use) => useTransparentBackground = use;
        
        public bool GetCurrentAutoIncrementTake() => autoIncrementTake;
        public void SetCurrentAutoIncrementTake(bool auto) => autoIncrementTake = auto;
        
        public bool GetCurrentUseFixedTakeDigits() => useFixedTakeDigits;
        public void SetCurrentUseFixedTakeDigits(bool use) 
        { 
            useFixedTakeDigits = use;
            ValidateInputs();
        }
        
        public int GetCurrentTakeDigits() => takeDigits;
        public void SetCurrentTakeDigits(int digits) 
        { 
            takeDigits = Mathf.Clamp(digits, 1, 10);
            ValidateInputs();
        }
        
        public bool GetCurrentAutoRefreshAssets() => autoRefreshAssets;
        public void SetCurrentAutoRefreshAssets(bool auto) => autoRefreshAssets = auto;
        
        public void SetCaptureSource(CaptureSource source) 
        { 
            captureSource = source;
            ValidateInputs();
        }
        
        public void SetCaptureCamera(Camera camera) 
        { 
            captureCamera = camera;
            // カメラ配列のインデックスも更新
            selectedCameraIndex = System.Array.IndexOf(sceneCamera, captureCamera);
            if (selectedCameraIndex == -1) selectedCameraIndex = 0;
            ValidateInputs();
        }
        
        public void SetCustomRenderTexture(RenderTexture renderTexture) 
        { 
            customRenderTexture = renderTexture;
            ValidateInputs();
        }
        
        public string ExecuteCaptureExternal()
        {
            CaptureAndSave();
            return lastCapturedFilePath;
        }
        
        public void OpenOutputDirectoryExternal()
        {
            OpenOutputDirectory();
        }
        
        public string GetFileNamePreview()
        {
            return previewFileName;
        }
        
        public string GetOutputPathPreview()
        {
            return resolvedOutputPath;
        }
    }
    
    // ヘルパークラス
    public static class CaptureHelper
    {
        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return "capture";
            
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
    
    // 文字列定数クラス
    public static class CaptureStrings
    {
        public static class Japanese
        {
            public const string Title = "PNGキャプチャ";
            public const string CaptureSource = "キャプチャソース";
            public const string SceneCameras = "シーンカメラ";
            public const string TargetCamera = "ターゲットカメラ";
            public const string RenderTexture = "レンダーテクスチャ";
            public const string PathMode = "パス指定方式";
            public const string OutputDirectoryRelative = "出力ディレクトリ (相対パス)";
            public const string OutputDirectoryAbsolute = "出力ディレクトリ (絶対パス)";
            public const string ResolvedPath = "解決されたパス";
            public const string FilenameTemplate = "ファイル名テンプレート";
            public const string TakeNumber = "テイク番号";
            public const string FixedTakeDigits = "テイク番号桁数固定";
            public const string Digits = "桁数";
            public const string Preview = "プレビュー";
            public const string IncludeAlpha = "アルファチャンネルを含める";
            public const string TransparentBackground = "透明背景";
            public const string AutoIncrementTake = "テイク番号自動インクリメント";
            public const string AutoRefreshAssets = "キャプチャ後にアセット更新";
            public const string CaptureAndSave = "キャプチャしてPNG保存";
            public const string OpenOutputFolder = "出力フォルダを開く";
            public const string RecentCaptures = "最近のキャプチャ";
            public const string Show = "表示";
            public const string Help = "ヘルプ";
        }
        
        public static class English
        {
            public const string Title = "PNG Capture";
            public const string CaptureSource = "Capture Source";
            public const string SceneCameras = "Scene Cameras";
            public const string TargetCamera = "Target Camera";
            public const string RenderTexture = "Render Texture";
            public const string PathMode = "Path Mode";
            public const string OutputDirectoryRelative = "Output Directory (Relative)";
            public const string OutputDirectoryAbsolute = "Output Directory (Absolute)";
            public const string ResolvedPath = "Resolved Path";
            public const string FilenameTemplate = "Filename Template";
            public const string TakeNumber = "Take Number";
            public const string FixedTakeDigits = "Fixed Take Digits";
            public const string Digits = "Digits";
            public const string Preview = "Preview";
            public const string IncludeAlpha = "Include Alpha Channel";
            public const string TransparentBackground = "Transparent Background";
            public const string AutoIncrementTake = "Auto Increment Take";
            public const string AutoRefreshAssets = "Auto Refresh Assets";
            public const string CaptureAndSave = "Capture and Save PNG";
            public const string OpenOutputFolder = "Open Output Folder";
            public const string RecentCaptures = "Recent Captures";
            public const string Show = "Show";
            public const string Help = "Help";
        }
    }
}
