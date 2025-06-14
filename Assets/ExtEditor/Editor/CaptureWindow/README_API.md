# CaptureWindow API Documentation

CaptureWindowツールを外部スクリプトから操作するための包括的なAPIドキュメントです。

## 概要

CaptureAPIは、CaptureWindowの全機能を外部スクリプトから利用可能にするシステムです。設定の変更、キャプチャの実行、ユーティリティ機能にアクセスできます。

## 主要な機能

### 1. 設定管理 (`CaptureAPI.Settings`)

すべてのCaptureWindow設定を取得・変更できます。

```csharp
// 言語設定
CaptureAPI.Settings.Language = CaptureWindow.Language.English;

// 出力ディレクトリ
CaptureAPI.Settings.OutputDirectory = "../MyCaptures";

// ファイル名テンプレート
CaptureAPI.Settings.FileNameTemplate = "Screenshot_<Take>_<Date>";

// テイク番号
CaptureAPI.Settings.TakeNumber = 5;

// アルファチャンネル
CaptureAPI.Settings.IncludeAlpha = true;

// 透明背景
CaptureAPI.Settings.UseTransparentBackground = true;

// テイク番号自動インクリメント
CaptureAPI.Settings.AutoIncrementTake = false;

// テイク番号桁数固定
CaptureAPI.Settings.UseFixedTakeDigits = true;
CaptureAPI.Settings.TakeDigits = 4;

// アセット自動更新
CaptureAPI.Settings.AutoRefreshAssets = false;
```

### 2. キャプチャ実行 (`CaptureAPI.Capture`)

様々な方法でキャプチャを実行し、保存されたファイルの絶対パスを取得できます。

#### 基本的なキャプチャ

```csharp
// GameViewをキャプチャ
string filePath = CaptureAPI.Capture.CaptureGameView();
Debug.Log($"保存先: {filePath}");

// カメラ指定でGameViewをキャプチャ
Camera myCamera = Camera.main;
string filePath = CaptureAPI.Capture.CaptureGameView(myCamera);

// SceneViewをキャプチャ
string filePath = CaptureAPI.Capture.CaptureSceneView();

// RenderTextureをキャプチャ
RenderTexture rt = new RenderTexture(1920, 1080, 24);
string filePath = CaptureAPI.Capture.CaptureRenderTexture(rt);
```

#### カスタム設定でキャプチャ

```csharp
// カスタム設定を作成
var config = new CaptureConfig
{
    Language = CaptureWindow.Language.English,
    FileNameTemplate = "CustomCapture_<Take>_<Date>",
    TakeNumber = 10,
    IncludeAlpha = true,
    UseTransparentBackground = true,
    OutputDirectory = "../CustomOutput"
};

// 設定を適用してキャプチャ（元の設定は自動復旧）
string filePath = CaptureAPI.Capture.CaptureWithConfig(config);
```

#### 現在の設定でキャプチャ

```csharp
// 現在のCaptureWindow設定でキャプチャ
string filePath = CaptureAPI.Capture.CaptureWithCurrentSettings();
```

### 3. ユーティリティ (`CaptureAPI.Utilities`)

便利な補助機能を提供します。

```csharp
// CaptureWindowを開く
CaptureAPI.Utilities.OpenWindow();

// 出力ディレクトリをFinderで開く
CaptureAPI.Utilities.OpenOutputDirectory();

// 設定をデフォルトにリセット
CaptureAPI.Utilities.ResetToDefault();

// ファイル名プレビューを取得
string preview = CaptureAPI.Utilities.GetFileNamePreview();

// 出力パスプレビューを取得
string pathPreview = CaptureAPI.Utilities.GetOutputPathPreview();
```

## CaptureConfig クラス

キャプチャ設定を包括的に管理するクラスです。

### プロパティ

```csharp
public class CaptureConfig
{
    public CaptureWindow.Language Language;           // 言語設定
    public CaptureWindow.PathMode PathMode;           // パス指定方式
    public CaptureWindow.CaptureSource CaptureSource; // キャプチャソース
    public string OutputDirectory;                    // 出力ディレクトリ
    public string FileNameTemplate;                   // ファイル名テンプレート
    public int TakeNumber;                           // テイク番号
    public bool IncludeAlpha;                        // アルファチャンネル
    public bool UseTransparentBackground;            // 透明背景
    public bool AutoIncrementTake;                   // 自動インクリメント
    public bool UseFixedTakeDigits;                  // 桁数固定
    public int TakeDigits;                          // 桁数
    public bool AutoRefreshAssets;                   // アセット自動更新
    public Camera Camera;                           // キャプチャカメラ
    public RenderTexture RenderTexture;             // RenderTexture
}
```

### ファクトリーメソッド

```csharp
// デフォルト設定
var config = CaptureConfig.CreateDefault();

// GameView用設定
var config = CaptureConfig.CreateForGameView(myCamera);

// SceneView用設定
var config = CaptureConfig.CreateForSceneView();

// RenderTexture用設定
var config = CaptureConfig.CreateForRenderTexture(myRenderTexture);
```

## 実用例

### バッチキャプチャ

```csharp
// 全カメラを順番にキャプチャ
Camera[] cameras = FindObjectsOfType<Camera>();
foreach (Camera camera in cameras)
{
    CaptureAPI.Settings.FileNameTemplate = $"Camera_{camera.name}_<Take>";
    string filePath = CaptureAPI.Capture.CaptureGameView(camera);
    Debug.Log($"キャプチャ完了: {filePath}");
}
```

### プリセット管理

```csharp
// 高品質プリセット
var highQualityConfig = new CaptureConfig
{
    IncludeAlpha = true,
    FileNameTemplate = "HighQuality_<Name>_<Date>_<Time>",
    OutputDirectory = "../HighQualityCaptures",
    UseFixedTakeDigits = true,
    TakeDigits = 4
};

// デバッグプリセット
var debugConfig = new CaptureConfig
{
    FileNameTemplate = "Debug_<Take>",
    OutputDirectory = "../DebugCaptures",
    AutoRefreshAssets = false
};

// 用途別にキャプチャ
string highQualityPath = CaptureAPI.Capture.CaptureWithConfig(highQualityConfig);
string debugPath = CaptureAPI.Capture.CaptureWithConfig(debugConfig);
```

### エラーハンドリング

```csharp
try
{
    string filePath = CaptureAPI.Capture.CaptureGameView();
    
    if (string.IsNullOrEmpty(filePath))
    {
        Debug.LogError("キャプチャが失敗しました");
    }
    else
    {
        Debug.Log($"キャプチャ成功: {filePath}");
        
        // ファイルの存在確認
        if (System.IO.File.Exists(filePath))
        {
            var fileInfo = new System.IO.FileInfo(filePath);
            Debug.Log($"ファイルサイズ: {fileInfo.Length} bytes");
        }
    }
}
catch (System.Exception ex)
{
    Debug.LogError($"キャプチャエラー: {ex.Message}");
}
finally
{
    // 設定をクリーンアップ
    CaptureAPI.Utilities.ResetToDefault();
}
```

### RenderTextureキャプチャ

```csharp
RenderTexture renderTexture = new RenderTexture(1920, 1080, 24);
renderTexture.name = "MyRenderTexture";

try
{
    // カメラをRenderTextureにレンダリング
    Camera mainCamera = Camera.main;
    RenderTexture previousTarget = mainCamera.targetTexture;
    mainCamera.targetTexture = renderTexture;
    mainCamera.Render();
    mainCamera.targetTexture = previousTarget;
    
    // RenderTextureをキャプチャ
    var config = CaptureConfig.CreateForRenderTexture(renderTexture);
    config.FileNameTemplate = "RenderTexture_<Date>_<Time>";
    config.IncludeAlpha = true;
    
    string filePath = CaptureAPI.Capture.CaptureWithConfig(config);
    Debug.Log($"RenderTextureキャプチャ完了: {filePath}");
}
finally
{
    // リソースクリーンアップ
    renderTexture.Release();
    DestroyImmediate(renderTexture);
}
```

## パターン置換

ファイル名テンプレートでは以下のパターンが使用できます：

| パターン | 説明 | 例 |
|----------|------|-----|
| `<Take>` | テイク番号 | 001, 0001 |
| `<Name>` | カメラ名/ソース名 | MainCamera, GameView |
| `<Date>` | 日付 | 20241214 |
| `<Time>` | 時刻 | 143052 |
| `<MilliSec>` | ミリ秒 | 123 |
| `<Hour>` | 時 | 14 |
| `<Minute>` | 分 | 30 |
| `<Second>` | 秒 | 52 |
| `<Year>` | 年 | 2024 |
| `<Month>` | 月 | 12 |
| `<Day>` | 日 | 14 |
| `<UnixTime>` | Unix時刻 | 1702556452 |

## 注意事項

1. **スレッドセーフティ**: APIはメインスレッドから呼び出してください
2. **エラーハンドリング**: キャプチャ失敗時は空文字列が返される場合があります
3. **リソース管理**: RenderTextureなどのリソースは適切に解放してください
4. **設定の永続性**: API経由の設定変更はCaptureWindowに反映されます

## サンプルコード

詳細なサンプルコードは `CaptureAPISamples.cs` を参照してください。Unityメニューの `Tools/CaptureWindow/Samples/` から各サンプルを実行できます。

## トラブルシューティング

### よくある問題

1. **キャプチャが失敗する**
   - カメラが正しく設定されているか確認
   - 出力ディレクトリの権限を確認
   - ファイル名に無効な文字が含まれていないか確認

2. **ファイルが見つからない**
   - 絶対パスで確認
   - AssetDatabase.Refresh()が実行されているか確認

3. **設定が反映されない**
   - ValidateInputs()が呼ばれているか確認
   - ウィンドウが開いている場合は再描画が必要

### デバッグ方法

```csharp
// 現在の設定を確認
Debug.Log($"Language: {CaptureAPI.Settings.Language}");
Debug.Log($"OutputDirectory: {CaptureAPI.Settings.OutputDirectory}");
Debug.Log($"FileNameTemplate: {CaptureAPI.Settings.FileNameTemplate}");

// プレビューで確認
string pathPreview = CaptureAPI.Utilities.GetOutputPathPreview();
string filePreview = CaptureAPI.Utilities.GetFileNamePreview();
Debug.Log($"Path Preview: {pathPreview}");
Debug.Log($"File Preview: {filePreview}");
```