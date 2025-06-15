using System.Collections.Generic;

namespace ExtEditor.Editor.CaptureWindow
{
    /// <summary>
    /// CaptureWindowの多言語対応文字列を管理するクラス
    /// </summary>
    public static class CaptureLocalization
    {
        private static readonly Dictionary<CaptureWindow.Language, Dictionary<TextKey, string>> localizedTexts = new()
        {
            { CaptureWindow.Language.Japanese, Japanese.Texts },
            { CaptureWindow.Language.English, English.Texts }
        };
        public enum TextKey
        {
            Title,
            CaptureSource,
            SceneCameras,
            TargetCamera,
            RenderTexture,
            PathMode,
            SelectFolder,
            OutputDirectory,
            OutputDirectoryRelative,
            OutputDirectoryAbsolute,
            ResolvedPath,
            FilenameTemplate,
            TakeNumber,
            FixedTakeDigits,
            Digits,
            Preview,
            FullPath,
            IncludeAlpha,
            TransparentBackground,
            AutoIncrementTake,
            AutoRefreshAssets,
            CaptureAndSave,
            OpenOutputFolder,
            RecentCaptures,
            Show,
            Help,

            Error,
            CameraNotSpecified,
            RenderTextureNotSpecified,
            InvalidCaptureResolution,
            RenderTextureCreationFailed,
            CaptureError,
            CaptureFailed,
            SaveError,
            CannotOpenFolder,
            InvalidPath,

            FileExists,
            FileExistsMessage,
            Overwrite,
            Cancel,
            SaveComplete,
            FileSaved,
            PNGSaved,

            Usage,
            UsageText,
            PatternList,
            Options,
            OptionsText,
        }
        /// <summary>
        /// 日本語文字列定数
        /// </summary>
        private static class Japanese
        {
            internal static readonly Dictionary<TextKey, string> Texts = new()
            {
                { TextKey.Title, "PNGキャプチャ" },
                { TextKey.CaptureSource, "キャプチャソース" },
                { TextKey.SceneCameras, "シーンカメラ" },
                { TextKey.TargetCamera, "ターゲットカメラ" },
                { TextKey.RenderTexture, "レンダーテクスチャ" },
                { TextKey.PathMode, "パス指定方式" },
                { TextKey.SelectFolder, "フォルダ選択" },
                { TextKey.OutputDirectory, "出力フォルダ" },
                { TextKey.OutputDirectoryRelative, "出力ディレクトリ (相対パス)" },
                { TextKey.OutputDirectoryAbsolute, "出力ディレクトリ (絶対パス)" },
                { TextKey.ResolvedPath, "解決されたパス" },
                { TextKey.FilenameTemplate, "ファイル名テンプレート" },
                { TextKey.TakeNumber, "テイク番号" },
                { TextKey.FixedTakeDigits, "テイク番号桁数固定" },
                { TextKey.Digits, "桁数" },
                { TextKey.Preview, "プレビュー" },
                { TextKey.FullPath, "フルパス" },
                { TextKey.IncludeAlpha, "アルファチャンネルを含める" },
                { TextKey.TransparentBackground, "透明背景" },
                { TextKey.AutoIncrementTake, "テイク番号自動インクリメント" },
                { TextKey.AutoRefreshAssets, "キャプチャ後にアセット更新" },
                { TextKey.CaptureAndSave, "キャプチャしてPNG保存" },
                { TextKey.OpenOutputFolder, "出力フォルダを開く" },
                { TextKey.RecentCaptures, "最近のキャプチャ" },
                { TextKey.Show, "表示" },
                { TextKey.Help, "ヘルプ" },

                // エラーメッセージ
                { TextKey.Error, "エラー" },
                { TextKey.CameraNotSpecified, "カメラが指定されていません" },
                { TextKey.RenderTextureNotSpecified, "レンダーテクスチャが指定されていません" },
                { TextKey.InvalidCaptureResolution, "キャプチャ解像度が無効です" },
                { TextKey.RenderTextureCreationFailed, "RenderTextureの作成に失敗しました" },
                { TextKey.CaptureError, "キャプチャエラー" },
                { TextKey.CaptureFailed, "撮影に失敗しました" },
                { TextKey.SaveError, "保存エラー" },
                { TextKey.CannotOpenFolder, "フォルダを開けませんでした" },
                { TextKey.InvalidPath, "無効なパス" },

                { TextKey.FileExists, "ファイルが存在します" },
                { TextKey.FileExistsMessage, "は既に存在します。上書きしますか？" },
                { TextKey.Overwrite, "上書き" },
                { TextKey.Cancel, "キャンセル" },
                { TextKey.SaveComplete, "保存完了" },
                { TextKey.FileSaved, "ファイルを保存しました" },
                { TextKey.PNGSaved, "PNGを保存しました" },

                { TextKey.Usage, "使い方" },
                { TextKey.UsageText, "1. キャプチャソースを選択\n2. カメラを指定\n3. 出力フォルダとファイル名を設定\n4. キャプチャボタンをクリック" },
                { TextKey.PatternList, "パターン一覧" },
                { TextKey.Options, "オプション" },
                { TextKey.OptionsText, "アルファチャンネル: 透明情報を含めるPNGを作成\n" +
                                       "透明背景: 背景を透明にして撮影\n" +
                                       "テイク番号桁数固定: テイク番号の桁数を固定（例：001, 0001）\n" +
                                       "保存後にアセット更新: 撮影後にUnityのアセットデータベースを更新\n" +
                                       "出力フォルダ: パターン置換対応（例：Captures/<Date>）\n" +
                                       "フォルダ選択ボタン: パス指定方式の右のアイコンでフォルダ選択"}
            };
        }
        
        /// <summary>
        /// 英語文字列定数
        /// </summary>
        private static class English
        {
            internal static readonly Dictionary<TextKey, string> Texts = new()
            {
                { TextKey.Title, "PNG Capture" },
                { TextKey.CaptureSource, "Capture Source" },
                { TextKey.SceneCameras, "Scene Cameras" },
                { TextKey.TargetCamera, "Target Camera" },
                { TextKey.RenderTexture, "Render Texture" },
                { TextKey.PathMode, "Path Mode" },
                { TextKey.SelectFolder, "Select Folder" },
                { TextKey.OutputDirectory, "Output Directory" },
                { TextKey.OutputDirectoryRelative, "Output Directory (Relative)" },
                { TextKey.OutputDirectoryAbsolute, "Output Directory (Absolute)" },
                { TextKey.ResolvedPath, "Resolved Path" },
                { TextKey.FilenameTemplate, "Filename Template" },
                { TextKey.TakeNumber, "Take Number" },
                { TextKey.FixedTakeDigits, "Fixed Take Digits" },
                { TextKey.Digits, "Digits" },
                { TextKey.Preview, "Preview" },
                { TextKey.FullPath, "Full Path" },
                { TextKey.IncludeAlpha, "Include Alpha Channel" },
                { TextKey.TransparentBackground, "Transparent Background" },
                { TextKey.AutoIncrementTake, "Auto Increment Take" },
                { TextKey.AutoRefreshAssets, "Auto Refresh Assets" },
                { TextKey.CaptureAndSave, "Capture and Save PNG" },
                { TextKey.OpenOutputFolder, "Open Output Folder" },
                { TextKey.RecentCaptures, "Recent Captures" },
                { TextKey.Show, "Show" },
                { TextKey.Help, "Help" },

                // エラーメッセージ
                { TextKey.Error, "Error" },
                { TextKey.CameraNotSpecified, "Camera is not specified" },
                { TextKey.RenderTextureNotSpecified, "RenderTexture is not specified" },
                { TextKey.InvalidCaptureResolution, "Invalid capture resolution" },
                { TextKey.RenderTextureCreationFailed, "Failed to create RenderTexture" },
                { TextKey.CaptureError, "Capture Error" },
                { TextKey.CaptureFailed, "Capture failed" },
                { TextKey.SaveError, "Save Error" },
                { TextKey.CannotOpenFolder, "Cannot open folder" },
                { TextKey.InvalidPath, "Invalid Path" },

                // 確認ダイアログ
                { TextKey.FileExists, "File Exists" },
                { TextKey.FileExistsMessage, " already exists. Overwrite?" },
                { TextKey.Overwrite, "Overwrite" },
                { TextKey.Cancel, "Cancel" },
                { TextKey.SaveComplete, "Save Complete" },
                { TextKey.FileSaved, "File saved:" },
                { TextKey.PNGSaved, "PNG saved" },

                { TextKey.Usage, "Usage" },
                {
                    TextKey.UsageText,
                    "1. Select capture source\n2. Specify camera\n3. Set output folder and filename\n4. Click capture button"
                },
                { TextKey.PatternList, "Pattern List" },
                { TextKey.Options, "Options" },
                {
                    TextKey.OptionsText, "Alpha Channel: Create PNG with transparency\n" +
                                         "Transparent Background: Capture with transparent background\n" +
                                         "Fixed Take Digits: Fix take number digits (e.g., 001, 0001)\n" +
                                         "Auto Refresh Assets: Refresh Unity asset database after capture\n" +
                                         "Output Directory: Pattern replacement supported (e.g., Captures/<Date>)\n" +
                                         "Folder Select Button: Click folder icon to select directory"
                }
            };
        }
        
        /// <summary>
        /// 指定された言語に対応する文字列を取得
        /// </summary>
        public static string GetText(TextKey textKey, CaptureWindow.Language language)
        {
            return localizedTexts[language][textKey];
        }
    }
}