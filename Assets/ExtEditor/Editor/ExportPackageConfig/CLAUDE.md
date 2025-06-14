# ExportPackageConfig Tool

このファイルは、ExportPackageConfigツールでの作業時にClaude Codeへのガイダンスを提供します。

## ツール概要

ExportPackageConfigツールは、Unityパッケージエクスポートの精密制御を提供するエディター拡張です。依存関係管理、高度なフィルタリング、複雑なインクルード・エクスクルードパターンをサポートします。

## 主要機能

### 基本機能
- **Unity パッケージエクスポート**: .unitypackage ファイルの設定可能なエクスポート
- **依存関係管理**: アセット依存関係の自動包含・除外制御
- **高度フィルタリング**: フォルダー含有を含む複雑なインクルード・エクスクルードパターン
- **柔軟なパス表現**: プレースホルダーを使用した設定可能出力パス
- **アセット可視化**: エクスポートに含まれるアセットの正確な表示
- **Package Manager フィルタリング**: Package Manager アセットの自動除外

### 使用方法
1. アセット作成: Project右クリック → Create → Scriptableobject → ExportPackageConfig
2. エクスポート設定を構成:
   - **Export Path Expression**: `<ProjectPath>`, `<ConfigName>`, `<Date>` などのプレースホルダー使用
   - **Include Dependencies**: 依存関係包含の切り替え
   - **Export Entry Assets**: 含めるアセット・フォルダーを追加
   - **Exclude Assets**: 除外するアセット・フォルダーを追加
3. "unitypackageをパッケージを書き出す"をクリックしてエクスポート
4. "エクスプローラーで開く"で出力ディレクトリを表示
5. Inspector でアセットリストを確認

## 実装詳細

### ファイル構造
- **ExportPackageConfig.cs**: ScriptableObject実装とカスタムエディター

### データ構造
```csharp
[SerializeField] string exportPathExpression;
[SerializeField] bool includeDependencies;
[SerializeField] List<UnityEngine.Object> exportEntryAssets;
[SerializeField] List<UnityEngine.Object> excludeAssets;
```

### プレースホルダーシステム
- **`<ProjectPath>`**: プロジェクトルートディレクトリ
- **`<AssetsPath>`**: Assetsフォルダーパス
- **`<ConfigName>`**: ScriptableObjectアセット名
- **`<Date>`**: 現在日付（yyyyMMdd）
- **`<Time>`**: 現在時刻（HHmmss）

## 技術的特徴

### 複雑な依存関係計算
- **OnValidate()**: アセット依存関係計算の複雑なロジック
- **AssetDatabase.GetDependencies()**: 依存関係解析
- **効率的重複除去**: LINQを使用したアセット重複排除

### フォルダー処理
- **AssetDatabase.FindAssets()**: フォルダー内容の再帰的発見
- **フォルダー展開**: フォルダーをすべての含有アセットに展開
- **階層処理**: ネストしたフォルダー構造の完全処理

### Package Manager 統合
- **自動除外**: Package Manager アセットの自動フィルタリング
- **パスベースフィルタ**: "Packages/" パスでの自動除外
- **クリーンエクスポート**: プロジェクト固有アセットのみ包含

## 設定詳細

### パス表現例
```
<ProjectPath>/Exports/<ConfigName>_<Date>.unitypackage
// 結果: /path/to/project/Exports/MyConfig_20241214.unitypackage
```

### 自動除外
- **Package Manager**: "Packages/" で始まるパスを自動除外
- **無効アセット**: 存在しないまたは破損したアセットを除外
- **重複アセット**: 同じアセットの複数エントリを重複除去

### カスタムエディター
- **展開可能リスト**: 大量アセットでの展開可能アセットリスト
- **リアルタイム更新**: 設定変更時の即座のアセットリスト更新
- **視覚的フィードバック**: 含まれるアセットの明確な表示

## 開発ノート

### パフォーマンス最適化
- **遅延計算**: OnValidate()での効率的な依存関係計算
- **キャッシュ戦略**: 大きなアセットリストでの効率的な処理
- **バッチ処理**: AssetDatabase操作の最適化

### エラーハンドリング
- **パス検証**: 無効な出力パスの適切な処理
- **権限チェック**: 書き込み権限の確認
- **アセット存在確認**: 削除されたアセット参照の処理

### クロスプラットフォーム対応
- **パス正規化**: 異なるOS間でのパス互換性
- **ファイル名サニタイズ**: 無効な文字の自動除去
- **パス区切り**: 適切なパス区切り文字の使用

### 拡張可能性
- **カスタムプレースホルダー**: 新しいプレースホルダーの追加
- **フィルタールール**: 高度なフィルタリングルールの実装
- **エクスポートフック**: エクスポート前後の処理フック

### 使用場面
- **アセット配布**: 特定アセットセットの配布パッケージ作成
- **バックアップ**: プロジェクトの部分的バックアップ
- **チーム共有**: チーム間でのアセット共有
- **バージョン管理**: アセットの版管理とリリース

### 制限事項
- **Unity制限**: Unityの.unitypackage形式制限に従う
- **ファイルサイズ**: 大量アセットでのファイルサイズ制限
- **パス長制限**: OS固有のパス長制限

### ベストプラクティス
- **テストエクスポート**: 本番前の小規模テストエクスポート
- **依存関係確認**: 必要な依存関係の漏れがないか確認
- **除外リスト管理**: 不要アセットの適切な除外設定

### トラブルシューティング
- **エクスポート失敗**: 権限問題やパス問題の診断
- **依存関係問題**: 不足依存関係の特定と解決
- **パフォーマンス**: 大量アセット処理時の最適化

### 将来の機能
- **増分エクスポート**: 変更されたアセットのみのエクスポート
- **圧縮オプション**: カスタム圧縮設定
- **メタデータ除外**: 特定メタデータの選択的除外

## 現状の課題

### 重要度: High（高）
- **パスインジェクション脆弱性**: ExportFilePathメソッドでのパス操作攻撃
  - **影響**: 任意の場所への不正ファイル作成リスク
  - **改善提案**: パス正規化とセキュリティ検証の実装

- **パフォーマンス**: OnValidateでの高コスト操作
  - **影響**: Inspector更新のたびに重い依存関係計算
  - **改善提案**: 遅延実行と非同期処理への変更

- **メモリ使用量**: 最適化なしでの大きなアセットリスト保存
  - **影響**: 大規模プロジェクトでメモリ不足
  - **改善提案**: ページング処理とメモリ効率化

### 重要度: Medium（中）
- **エラーハンドリング**: ファイル操作が静的に失敗
  - **影響**: エクスポート失敗時にユーザーが気づかない
  - **改善提案**: 包括的な例外処理と詳細エラー報告

- **スレッドセーフティ**: アセットリストへの同時アクセス保護なし
  - **影響**: 並行アクセスでデータ競合状態
  - **改善提案**: ロック機構または不変コレクション使用

### 具体的な改善コード例

```csharp
// パスインジェクション対策
private string ExportFilePath()
{
    var expressions = new Dictionary<string, string>
    {
        {"<ProjectPath>", SanitizePath(Application.dataPath[..^7])},
        {"<AssetsPath>", SanitizePath(Application.dataPath)},
        {"<ConfigName>", SanitizeFileName(this.name)},
        {"<Date>", DateTime.Now.ToString("yyyyMMdd")},
        {"<Time>", DateTime.Now.ToString("HHmmss")}
    };
    
    var pathExpression = exportPathExpression;
    
    // セキュリティ検証
    if (!IsValidPathExpression(pathExpression))
    {
        Debug.LogError("不正なパス表現が検出されました");
        return null;
    }
    
    foreach (var kvp in expressions)
    {
        pathExpression = pathExpression.Replace(kvp.Key, kvp.Value);
    }
    
    // 最終パス検証
    try
    {
        string fullPath = Path.GetFullPath(pathExpression);
        string projectRoot = Path.GetFullPath(Application.dataPath + "/..");
        
        if (!fullPath.StartsWith(projectRoot))
        {
            Debug.LogError($"プロジェクト外へのエクスポートは許可されません: {fullPath}");
            return null;
        }
        
        return fullPath;
    }
    catch (Exception ex)
    {
        Debug.LogError($"パス解決エラー: {ex.Message}");
        return null;
    }
}

// セキュリティ検証
private bool IsValidPathExpression(string pathExpression)
{
    if (string.IsNullOrEmpty(pathExpression)) return false;
    
    // 危険なパターンをチェック
    string[] dangerousPatterns = { "..", "~", "$", "%", "\\\\", "//" };
    foreach (string pattern in dangerousPatterns)
    {
        if (pathExpression.Contains(pattern))
        {
            Debug.LogWarning($"危険なパスパターン検出: {pattern}");
            return false;
        }
    }
    
    return true;
}

// ファイル名サニタイズ
private string SanitizeFileName(string fileName)
{
    if (string.IsNullOrEmpty(fileName)) return "Unnamed";
    
    char[] invalidChars = Path.GetInvalidFileNameChars();
    foreach (char c in invalidChars)
    {
        fileName = fileName.Replace(c, '_');
    }
    
    return fileName.Trim();
}

// パフォーマンス改善されたOnValidate
private void OnValidate()
{
    // 重い処理を遅延実行
    EditorApplication.delayCall += () => {
        if (this != null) RefreshAssetListsAsync();
    };
}

private async void RefreshAssetListsAsync()
{
    try
    {
        await Task.Run(() => {
            // バックグラウンドで依存関係計算
            var newAssetList = ComputeAssetDependencies();
            
            // メインスレッドでUI更新
            EditorApplication.delayCall += () => {
                UpdateAssetLists(newAssetList);
            };
        });
    }
    catch (Exception ex)
    {
        Debug.LogError($"アセットリスト更新エラー: {ex.Message}");
    }
}

// エラーハンドリング強化
[ContextMenu("unitypackageをパッケージを書き出す")]
public void ExportUnityPackage()
{
    try
    {
        // 事前検証
        if (!ValidateExportSettings())
        {
            EditorUtility.DisplayDialog("検証エラー", "エクスポート設定に問題があります", "OK");
            return;
        }
        
        var assets = GetAssetsToExport();
        if (assets.Count == 0)
        {
            EditorUtility.DisplayDialog("警告", "エクスポートするアセットがありません", "OK");
            return;
        }
        
        string exportPath = ExportFilePath();
        if (string.IsNullOrEmpty(exportPath))
        {
            EditorUtility.DisplayDialog("エラー", "エクスポートパスの解決に失敗しました", "OK");
            return;
        }
        
        // ディレクトリ作成
        string directory = Path.GetDirectoryName(exportPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        // 進捗表示付きエクスポート
        EditorUtility.DisplayProgressBar("エクスポート中", "パッケージを作成しています...", 0.5f);
        
        AssetDatabase.ExportPackage(
            assets.ToArray(), 
            exportPath, 
            ExportPackageOptions.Recurse | 
            (includeDependencies ? ExportPackageOptions.IncludeDependencies : ExportPackageOptions.Default));
        
        Debug.Log($"エクスポート完了: {exportPath} ({assets.Count}個のアセット)");
        
        // 結果確認
        if (File.Exists(exportPath))
        {
            long fileSize = new FileInfo(exportPath).Length;
            EditorUtility.DisplayDialog("エクスポート完了", 
                $"パッケージを作成しました\nファイル: {Path.GetFileName(exportPath)}\nサイズ: {FormatFileSize(fileSize)}\nアセット数: {assets.Count}", 
                "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("エラー", "パッケージファイルが作成されませんでした", "OK");
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"エクスポートエラー: {ex.Message}");
        EditorUtility.DisplayDialog("エクスポートエラー", $"エクスポートに失敗しました:\n{ex.Message}", "OK");
    }
    finally
    {
        EditorUtility.ClearProgressBar();
    }
}

// 設定検証
private bool ValidateExportSettings()
{
    if (string.IsNullOrEmpty(exportPathExpression))
    {
        Debug.LogError("エクスポートパス表現が設定されていません");
        return false;
    }
    
    if (exportEntryAssets.Count == 0)
    {
        Debug.LogError("エクスポートアセットが設定されていません");
        return false;
    }
    
    return true;
}

// ファイルサイズフォーマット
private string FormatFileSize(long bytes)
{
    string[] sizes = { "B", "KB", "MB", "GB" };
    double len = bytes;
    int order = 0;
    
    while (len >= 1024 && order < sizes.Length - 1)
    {
        order++;
        len = len / 1024;
    }
    
    return $"{len:0.##} {sizes[order]}";
}

// スレッドセーフなアセットリスト管理
private readonly object _assetListLock = new object();
private List<string> _cachedAssetList = new List<string>();

private List<string> GetAssetsToExportSafe()
{
    lock (_assetListLock)
    {
        return new List<string>(_cachedAssetList);
    }
}

private void UpdateAssetListsSafe(List<string> newAssets)
{
    lock (_assetListLock)
    {
        _cachedAssetList = newAssets ?? new List<string>();
    }
}
```