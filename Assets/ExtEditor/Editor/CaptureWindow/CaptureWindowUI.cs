using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace ExtEditor.Editor.CaptureWindow
{
    /// <summary>
    /// CaptureWindowのUI描画を担当するクラス
    /// </summary>
    public class CaptureWindowUI
    {
        private readonly CaptureWindowState state;
        private readonly System.Action validateInputs;
        private readonly System.Action updateOutputDirectoryForPathMode;
        private readonly System.Action<string> selectOutputFolder;
        private CaptureWindow window;
        
        public CaptureWindowUI(CaptureWindowState state, System.Action validateInputs, 
            System.Action updateOutputDirectoryForPathMode, System.Action<string> selectOutputFolder)
        {
            this.state = state;
            this.validateInputs = validateInputs;
            this.updateOutputDirectoryForPathMode = updateOutputDirectoryForPathMode;
            this.selectOutputFolder = selectOutputFolder;
        }
        
        public void SetWindow(CaptureWindow window)
        {
            this.window = window;
        }
        
        /// <summary>
        /// UI全体を描画
        /// </summary>
        public void DrawGUI()
        {
            DrawHeader();
            EditorGUILayout.Space();
            DrawCaptureSourceSection();
            EditorGUILayout.Space();
            DrawPathSection();
            EditorGUILayout.Space();
            DrawFileNameSection();
            EditorGUILayout.Space();
            DrawOptionsSection();
            EditorGUILayout.Space();
            DrawActionButtons();
            DrawRecentCapturesSection();
            EditorGUILayout.Space();
            DrawHelpSection();
        }
        
        /// <summary>
        /// ヘッダー部分の描画
        /// </summary>
        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(state.GetText("PNGキャプチャ", "PNG Capture"), EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            state.CurrentLanguage = (CaptureWindow.Language)EditorGUILayout.EnumPopup(state.CurrentLanguage, GUILayout.Width(80));
            if (EditorGUI.EndChangeCheck())
            {
                validateInputs();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// キャプチャソース設定部分の描画
        /// </summary>
        private void DrawCaptureSourceSection()
        {
            // キャプチャソース
            EditorGUI.BeginChangeCheck();
            state.CaptureSource = (CaptureWindow.CaptureSource)EditorGUILayout.EnumPopup(
                state.GetText("キャプチャソース", "Capture Source"), state.CaptureSource);
            if (EditorGUI.EndChangeCheck())
            {
                validateInputs();
            }

            // プルダウンメニューの更新
            EditorGUI.BeginChangeCheck();
            state.SelectedCameraIndex = EditorGUILayout.Popup(
                state.GetText("シーンカメラ", "Scene Cameras"), 
                state.SelectedCameraIndex, 
                state.SceneCameraNames);
            if (EditorGUI.EndChangeCheck() && state.SelectedCameraIndex >= 0 && state.SelectedCameraIndex < state.SceneCameras.Length)
            {
                state.CaptureCamera = state.SceneCameras[state.SelectedCameraIndex];
                validateInputs();
            }

            // ObjectFieldの更新
            EditorGUI.BeginChangeCheck();
            state.CaptureCamera = (Camera)EditorGUILayout.ObjectField(
                state.GetText("ターゲットカメラ", "Target Camera"), 
                state.CaptureCamera, 
                typeof(Camera), 
                true);
            if (EditorGUI.EndChangeCheck())
            {
                state.SelectedCameraIndex = System.Array.IndexOf(state.SceneCameras, state.CaptureCamera);
                if (state.SelectedCameraIndex == -1) state.SelectedCameraIndex = 0;
                validateInputs();
            }
            
            if (state.CaptureSource == CaptureWindow.CaptureSource.RenderTexture)
            {
                EditorGUI.BeginChangeCheck();
                state.CustomRenderTexture = (RenderTexture)EditorGUILayout.ObjectField(
                    state.GetText("レンダーテクスチャ", "Render Texture"), 
                    state.CustomRenderTexture, 
                    typeof(RenderTexture), 
                    true);
                if (EditorGUI.EndChangeCheck())
                {
                    validateInputs();
                }
            }
        }
        
        /// <summary>
        /// パス設定部分の描画
        /// </summary>
        private void DrawPathSection()
        {
            // パス指定方式
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            state.PathMode = (CaptureWindow.PathMode)EditorGUILayout.EnumPopup(
                state.GetText("パス指定方式", "Path Mode"), state.PathMode);
            if (EditorGUI.EndChangeCheck())
            {
                updateOutputDirectoryForPathMode();
                validateInputs();
            }
            
            // フォルダ選択ボタン
            if (GUILayout.Button(new GUIContent(
                EditorGUIUtility.IconContent("Folder Icon").image, 
                state.GetText("フォルダ選択", "Select Folder")), 
                GUILayout.Width(30), GUILayout.Height(18)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel(
                    state.GetText("出力フォルダを選択", "Select Output Folder"),
                    string.IsNullOrEmpty(state.OutputDirectory) ? Application.dataPath : state.OutputDirectory,
                    ""
                );
                
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    selectOutputFolder(selectedPath);
                }
            }
            EditorGUILayout.EndHorizontal();

            // 出力ディレクトリ
            EditorGUI.BeginChangeCheck();
            string directoryLabel = state.GetText("出力フォルダ", "Output Directory");
            state.OutputDirectory = EditorGUILayout.TextField(directoryLabel, state.OutputDirectory);
            if (EditorGUI.EndChangeCheck())
            {
                validateInputs();
            }
            
            // パス表示
            if (!state.IsOutputPathValid)
            {
                EditorGUILayout.HelpBox(state.ValidationMessage, MessageType.Error);
            }
            else
            {
                EditorGUILayout.LabelField(
                    state.GetText("フォルダパス", "Resolved Path"), 
                    state.ResolvedOutputPath, 
                    EditorStyles.miniLabel);
            }
        }
        
        /// <summary>
        /// ファイル名設定部分の描画
        /// </summary>
        private void DrawFileNameSection()
        {
            // ファイル名テンプレート
            EditorGUI.BeginChangeCheck();
            state.FileNameTemplate = EditorGUILayout.TextField(
                state.GetText("ファイル名テンプレート", "Filename Template"), 
                state.FileNameTemplate);
            if (EditorGUI.EndChangeCheck())
            {
                validateInputs();
            }
            
            // テイク番号
            EditorGUI.BeginChangeCheck();
            state.TakeNumber = EditorGUILayout.IntField(
                state.GetText("テイク番号", "Take Number"), 
                state.TakeNumber);
            if (EditorGUI.EndChangeCheck())
            {
                state.TakeNumber = Mathf.Max(1, state.TakeNumber);
                validateInputs();
            }
            
            // テイク番号桁数固定オプション
            EditorGUI.BeginChangeCheck();
            state.UseFixedTakeDigits = EditorGUILayout.Toggle(
                state.GetText("テイク番号桁数固定", "Fixed Take Digits"), 
                state.UseFixedTakeDigits);
            if (state.UseFixedTakeDigits)
            {
                EditorGUI.indentLevel++;
                state.TakeDigits = EditorGUILayout.IntSlider(
                    state.GetText("桁数", "Digits"), 
                    state.TakeDigits, 1, 10);
                EditorGUI.indentLevel--;
            }
            if (EditorGUI.EndChangeCheck())
            {
                validateInputs();
            }
            
            // ファイル名プレビュー
            if (!state.IsFileNameValid)
            {
                EditorGUILayout.HelpBox(state.ValidationMessage, MessageType.Error);
            }
            else
            {
                // ファイル名プレビュー
                var previewStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    wordWrap = true
                };
                EditorGUILayout.LabelField(
                    state.GetText("プレビュー", "Preview"), 
                    state.PreviewFileName, 
                    previewStyle);
                
                // 完全パスプレビュー
                string fullPathPreview = Path.Combine(state.ResolvedOutputPath, state.PreviewFileName);
                var pathStyle = new GUIStyle(EditorStyles.label)
                {
                    wordWrap = true,
                    normal = { textColor = Color.gray }
                };
                
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField(state.GetText("完全パス", "Full Path"));
                EditorGUILayout.SelectableLabel(fullPathPreview, pathStyle, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2));
                EditorGUILayout.EndVertical();
            }
        }
        
        /// <summary>
        /// オプション部分の描画
        /// </summary>
        private void DrawOptionsSection()
        {
            state.IncludeAlpha = EditorGUILayout.Toggle(
                state.GetText("アルファを含める", "Include Alpha Channel"), 
                state.IncludeAlpha);
            state.UseTransparentBackground = EditorGUILayout.Toggle(
                state.GetText("背景透過", "Transparent Background"), 
                state.UseTransparentBackground);
            state.AutoRefreshAssets = EditorGUILayout.Toggle(
                state.GetText("保存後にアセット更新", "Auto Refresh Assets"), 
                state.AutoRefreshAssets);
        }
        
        /// <summary>
        /// アクションボタンの描画
        /// </summary>
        public void DrawActionButtons()
        {
            EditorGUI.BeginDisabledGroup(!state.IsOutputPathValid || !state.IsFileNameValid);
            if (GUILayout.Button(state.GetText("キャプチャしてPNG保存", "Capture and Save PNG")))
            {
                window?.CaptureAndSave();
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button(state.GetText("出力フォルダを開く", "Open Output Folder")))
            {
                window?.OpenOutputDirectory();
            }
        }
        
        /// <summary>
        /// 最近のキャプチャ表示部分の描画
        /// </summary>
        private void DrawRecentCapturesSection()
        {
            if (state.RecentCaptures[0] != null)
            {
                EditorGUILayout.Space();
                state.ShowRecentCaptures = EditorGUILayout.Foldout(
                    state.ShowRecentCaptures, 
                    state.GetText("最近のキャプチャ", "Recent Captures"));
                if (state.ShowRecentCaptures)
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < state.RecentCaptures.Length && state.RecentCaptures[i] != null; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(Path.GetFileName(state.RecentCaptures[i]), EditorStyles.miniLabel);
                        if (GUILayout.Button(state.GetText("表示", "Show"), GUILayout.Width(50)))
                        {
                            EditorUtility.RevealInFinder(state.RecentCaptures[i]);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }
        
        /// <summary>
        /// ヘルプセクションの描画
        /// </summary>
        private void DrawHelpSection()
        {
            state.ShowHelp = EditorGUILayout.Foldout(state.ShowHelp, state.GetText("ヘルプ", "Help"));
            if (state.ShowHelp)
            {
                DrawHelpContent();
            }
        }
        
        /// <summary>
        /// ヘルプコンテンツの描画
        /// </summary>
        private void DrawHelpContent()
        {
            EditorGUILayout.BeginVertical("box");
            
            // 使用方法
            EditorGUILayout.LabelField(state.GetText("使用方法", "Usage"), EditorStyles.boldLabel);
            var usageStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };
            EditorGUILayout.LabelField(state.GetText(
                "1. キャプチャソースを選択\n2. カメラを指定\n3. 出力フォルダとファイル名を設定\n4. キャプチャボタンをクリック",
                "1. Select capture source\n2. Specify camera\n3. Set output folder and filename\n4. Click capture button"
            ), usageStyle);
            
            EditorGUILayout.Space();
            
            // パターン一覧
            EditorGUILayout.LabelField(state.GetText("パターン一覧", "Pattern List"), EditorStyles.boldLabel);
            
            var patterns = PatternResolver.GetAvailablePatterns();
            var context = new PatternContext
            {
                TakeNumber = state.TakeNumber,
                UseFixedDigits = state.UseFixedTakeDigits,
                DigitCount = state.TakeDigits,
                CaptureSource = state.CaptureSource,
                CaptureCamera = state.CaptureCamera,
                CustomRenderTexture = state.CustomRenderTexture
            };
            
            foreach (var pattern in patterns)
            {
                string example = PatternResolver.ResolvePattern(pattern.Pattern, context, true);
                DrawPatternHelp(pattern.Pattern, example, pattern.GetDescription(state.CurrentLanguage));
            }
            
            EditorGUILayout.Space();
            
            // オプション説明
            EditorGUILayout.LabelField(state.GetText("オプション", "Options"), EditorStyles.boldLabel);
            var optionsStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };
            EditorGUILayout.LabelField(state.GetText(
                "アルファチャンネル: 透明情報を含めるPNGを作成\n" +
                "透明背景: 背景を透明にして撮影\n" +
                "テイク番号桁数固定: テイク番号の桁数を固定（例：001, 0001）\n" +
                "保存後にアセット更新: 撮影後にUnityのアセットデータベースを更新\n" +
                "出力フォルダ: パターン置換対応（例：Captures/<Date>）\n" +
                "フォルダ選択ボタン: パス指定方式の右のアイコンでフォルダ選択",
                "Alpha Channel: Create PNG with transparency\n" +
                "Transparent Background: Capture with transparent background\n" +
                "Fixed Take Digits: Fix take number digits (e.g., 001, 0001)\n" +
                "Auto Refresh Assets: Refresh Unity asset database after capture\n" +
                "Output Directory: Pattern replacement supported (e.g., Captures/<Date>)\n" +
                "Folder Select Button: Click folder icon to select directory"
            ), optionsStyle);
            
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// パターンヘルプの表示
        /// </summary>
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
    }
}