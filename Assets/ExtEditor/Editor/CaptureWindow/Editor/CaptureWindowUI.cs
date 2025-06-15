using UnityEngine;
using UnityEditor;
using System.IO;
using localization = ExtEditor.Editor.CaptureWindow.CaptureLocalization;

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
            GUILayout.Label(state.GetText(localization.TextKey.Title), EditorStyles.boldLabel);
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
                state.GetText(localization.TextKey.CaptureSource), state.CaptureSource);
            if (EditorGUI.EndChangeCheck())
            {
                validateInputs();
            }

            // プルダウンメニューの更新
            EditorGUI.BeginChangeCheck();
            state.SelectedCameraIndex = EditorGUILayout.Popup(
                state.GetText(localization.TextKey.SceneCameras), 
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
                state.GetText(localization.TextKey.TargetCamera), 
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
                    state.GetText(localization.TextKey.RenderTexture), 
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
                state.GetText(localization.TextKey.PathMode), state.PathMode);
            if (EditorGUI.EndChangeCheck())
            {
                updateOutputDirectoryForPathMode();
                validateInputs();
            }
            
            // フォルダ選択ボタン
            if (GUILayout.Button(new GUIContent(
                EditorGUIUtility.IconContent("Folder Icon").image, 
                state.GetText(localization.TextKey.SelectFolder)), 
                GUILayout.Width(30), GUILayout.Height(18)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel(
                    state.GetText(localization.TextKey.SelectFolder),
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
            string directoryLabel = state.GetText(localization.TextKey.OutputDirectory);
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
                    state.GetText(localization.TextKey.ResolvedPath),
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
                state.GetText(localization.TextKey.FilenameTemplate), 
                state.FileNameTemplate);
            if (EditorGUI.EndChangeCheck())
            {
                validateInputs();
            }
            
            // テイク番号
            EditorGUI.BeginChangeCheck();
            state.TakeNumber = EditorGUILayout.IntField(
                state.GetText(localization.TextKey.TakeNumber), 
                state.TakeNumber);
            if (EditorGUI.EndChangeCheck())
            {
                state.TakeNumber = Mathf.Max(1, state.TakeNumber);
                validateInputs();
            }
            
            // テイク番号桁数固定オプション
            EditorGUI.BeginChangeCheck();
            state.UseFixedTakeDigits = EditorGUILayout.Toggle( 
                state.GetText(localization.TextKey.FixedTakeDigits), 
                state.UseFixedTakeDigits);
            if (state.UseFixedTakeDigits)
            {
                EditorGUI.indentLevel++;
                state.TakeDigits = EditorGUILayout.IntSlider(
                    state.GetText(localization.TextKey.Digits), 
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
                    state.GetText(localization.TextKey.Preview), 
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
                EditorGUILayout.LabelField(state.GetText(localization.TextKey.FullPath));
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
                state.GetText(localization.TextKey.IncludeAlpha), 
                state.IncludeAlpha);
            state.UseTransparentBackground = EditorGUILayout.Toggle(
                state.GetText(localization.TextKey.TransparentBackground), 
                state.UseTransparentBackground);
            state.AutoRefreshAssets = EditorGUILayout.Toggle( 
                state.GetText(localization.TextKey.AutoRefreshAssets), 
                state.AutoRefreshAssets);
        }
        
        /// <summary>
        /// アクションボタンの描画
        /// </summary>
        public void DrawActionButtons()
        {
            EditorGUI.BeginDisabledGroup(!state.IsOutputPathValid || !state.IsFileNameValid);
            if (GUILayout.Button(state.GetText(localization.TextKey.CaptureAndSave)))
                window?.CaptureAndSave();
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button(state.GetText(localization.TextKey.OpenOutputFolder)))
                window?.OpenOutputDirectory();
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
                    state.GetText(localization.TextKey.RecentCaptures));
                if (state.ShowRecentCaptures)
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < state.RecentCaptures.Length && state.RecentCaptures[i] != null; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(Path.GetFileName(state.RecentCaptures[i]), EditorStyles.miniLabel);
                        if (GUILayout.Button(state.GetText(localization.TextKey.Show), GUILayout.Width(50)))
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
            state.ShowHelp = EditorGUILayout.Foldout(state.ShowHelp, state.GetText(localization.TextKey.Help));
            if (state.ShowHelp)
                DrawHelpContent();
        }
        
        /// <summary>
        /// ヘルプコンテンツの描画
        /// </summary>
        private void DrawHelpContent()
        {
            EditorGUILayout.BeginVertical("box");
            
            // 使用方法
            EditorGUILayout.LabelField(state.GetText(localization.TextKey.Usage), EditorStyles.boldLabel);
            var usageStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };
            EditorGUILayout.LabelField(state.GetText(
                localization.TextKey.UsageText
            ), usageStyle);
            
            EditorGUILayout.Space();
            
            // パターン一覧
            EditorGUILayout.LabelField(state.GetText(localization.TextKey.PatternList), EditorStyles.boldLabel);
            
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
            EditorGUILayout.LabelField(state.GetText(localization.TextKey.Options), EditorStyles.boldLabel);
            var optionsStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };
            EditorGUILayout.LabelField(state.GetText(
                localization.TextKey.OptionsText
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