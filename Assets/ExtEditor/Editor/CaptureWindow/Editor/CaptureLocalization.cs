namespace ExtEditor.Editor.CaptureWindow
{
    /// <summary>
    /// CaptureWindowの多言語対応文字列を管理するクラス
    /// </summary>
    public static class CaptureLocalization
    {
        /// <summary>
        /// 日本語文字列定数
        /// </summary>
        public static class Japanese
        {
            // UI要素
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
            
            // エラーメッセージ
            public const string Error = "エラー";
            public const string CameraNotSpecified = "カメラが指定されていません";
            public const string RenderTextureNotSpecified = "レンダーテクスチャが指定されていません";
            public const string InvalidCaptureResolution = "キャプチャ解像度が無効です";
            public const string RenderTextureCreationFailed = "RenderTextureの作成に失敗しました";
            public const string CaptureError = "キャプチャエラー";
            public const string CaptureFailed = "撮影に失敗しました";
            public const string CannotOpenFolder = "フォルダを開けませんでした";
            public const string InvalidPath = "無効なパス";
            
            // 確認ダイアログ
            public const string FileExists = "ファイルが存在します";
            public const string FileExistsMessage = "は既に存在します。上書きしますか？";
            public const string Overwrite = "上書き";
            public const string Cancel = "キャンセル";
            public const string SaveComplete = "保存完了";
            public const string FileSaved = "ファイルを保存しました:";
            public const string PNGSaved = "PNGを保存しました";
            
            // ヘルプ
            public const string Usage = "使用方法";
            public const string UsageText = "1. キャプチャソースを選択\n2. カメラを指定\n3. 出力フォルダとファイル名を設定\n4. キャプチャボタンをクリック";
            public const string PatternList = "パターン一覧";
            public const string Options = "オプション";
            public const string OptionsText = "アルファチャンネル: 透明情報を含めるPNGを作成\n" +
                                           "透明背景: 背景を透明にして撮影\n" +
                                           "テイク番号桁数固定: テイク番号の桁数を固定（例：001, 0001）\n" +
                                           "保存後にアセット更新: 撮影後にUnityのアセットデータベースを更新\n" +
                                           "出力フォルダ: パターン置換対応（例：Captures/<Date>）\n" +
                                           "フォルダ選択ボタン: パス指定方式の右のアイコンでフォルダ選択";
        }
        
        /// <summary>
        /// 英語文字列定数
        /// </summary>
        public static class English
        {
            // UI要素
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
            
            // エラーメッセージ
            public const string Error = "Error";
            public const string CameraNotSpecified = "Camera is not specified";
            public const string RenderTextureNotSpecified = "RenderTexture is not specified";
            public const string InvalidCaptureResolution = "Invalid capture resolution";
            public const string RenderTextureCreationFailed = "Failed to create RenderTexture";
            public const string CaptureError = "Capture Error";
            public const string CaptureFailed = "Capture failed";
            public const string CannotOpenFolder = "Cannot open folder";
            public const string InvalidPath = "Invalid Path";
            
            // 確認ダイアログ
            public const string FileExists = "File Exists";
            public const string FileExistsMessage = " already exists. Overwrite?";
            public const string Overwrite = "Overwrite";
            public const string Cancel = "Cancel";
            public const string SaveComplete = "Save Complete";
            public const string FileSaved = "File saved:";
            public const string PNGSaved = "PNG saved";
            
            // ヘルプ
            public const string Usage = "Usage";
            public const string UsageText = "1. Select capture source\n2. Specify camera\n3. Set output folder and filename\n4. Click capture button";
            public const string PatternList = "Pattern List";
            public const string Options = "Options";
            public const string OptionsText = "Alpha Channel: Create PNG with transparency\n" +
                                           "Transparent Background: Capture with transparent background\n" +
                                           "Fixed Take Digits: Fix take number digits (e.g., 001, 0001)\n" +
                                           "Auto Refresh Assets: Refresh Unity asset database after capture\n" +
                                           "Output Directory: Pattern replacement supported (e.g., Captures/<Date>)\n" +
                                           "Folder Select Button: Click folder icon to select directory";
        }
        
        /// <summary>
        /// 指定された言語に対応する文字列を取得
        /// </summary>
        public static string GetText(CaptureWindow.Language language, string japaneseText, string englishText)
        {
            return language == CaptureWindow.Language.Japanese ? japaneseText : englishText;
        }
    }
}