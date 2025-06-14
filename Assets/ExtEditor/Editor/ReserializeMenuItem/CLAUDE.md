# ReserializeMenuItem Tool

このファイルは、ReserializeMenuItemツールでの作業時にClaude Codeへのガイダンスを提供します。

## ツール概要

ReserializeMenuItemツールは、選択したアセットとそのメタデータを強制的に再シリアライズするエディター拡張です。アセットデータベースの問題修正やシリアライズ変更の適用に使用されます。

## 主要機能

### 基本機能
- **アセット再シリアライズ**: 選択アセットのシリアライズデータを再生成
- **メタデータ含有**: アセットとメタデータの両方を再シリアライズ
- **バッチ処理**: 複数選択アセットを一度に処理
- **シンプルインターフェース**: 右クリックメニューの単一オプション

### 使用方法
1. Projectウィンドウでアセットを選択
2. 右クリック → Assets → Reserialize
3. Unityが選択されたアセットを再シリアライズ

## 実装詳細

### ファイル構造
- **ReserializeMenuItem.cs**: 単一静的クラスでの実装

### メニュー統合
```csharp
[MenuItem("Assets/Reserialize")]
public static void ReserializeAssets()
```

### 処理フロー
1. `Selection.objects`から選択オブジェクトを取得
2. LINQを使用してアセットパスを抽出
3. `AssetDatabase.ForceReserializeAssets()`で再シリアライズ実行

## 技術的特徴

### 再シリアライズオプション
- **ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata**: アセットとメタデータの完全再シリアライズ
- **包括的処理**: .meta ファイルを含む完全な再処理

### 対象アセット
- **全Unity アセット**: すべてのUnityアセットタイプに対応
- **制限なし**: 特定のファイル形式に制限されない汎用処理

### パフォーマンス
- **Unity依存**: Unity内蔵の最適化されたシリアライズエンジンを使用
- **効率的**: 必要な部分のみを再処理

## 使用場面

### 問題解決
- **アセットデータベース破損**: 破損したアセット参照の修復
- **シリアライズエラー**: シリアライズ形式の不整合修正
- **メタデータ問題**: メタファイルの不整合解決

### 開発ワークフロー
- **Unity バージョン移行**: Unity バージョン間でのアセット形式更新
- **シリアライズ変更適用**: コードでのシリアライズ変更の反映
- **データベース更新**: アセットデータベースの最新化

### メンテナンス
- **定期保守**: プロジェクトの定期的なアセット整合性チェック
- **問題予防**: 問題発生前の予防的再シリアライズ

## 開発ノート

### 安全性
- **非破壊処理**: 元のアセットデータを破壊しない安全な操作
- **Unity管理**: Unity内蔵システムによる信頼性の高い処理
- **自動バックアップ**: Unityのバージョン管理統合での自動バックアップ

### 制限事項
- **処理時間**: 大量アセットの処理には時間がかかる場合がある
- **エディター依存**: Unity エディター環境でのみ実行可能
- **一時的ロック**: 処理中の一時的なアセットアクセス制限

### 拡張可能性
- **進捗表示**: `EditorUtility.DisplayProgressBar`による進捗表示追加
- **選択的処理**: 特定タイプのみの再シリアライズ
- **ログ出力**: 処理されたアセットの詳細ログ
- **設定オプション**: 再シリアライズオプションの選択機能

### ベストプラクティス
- **バックアップ**: 重要なプロジェクトでは事前バックアップ推奨
- **段階的処理**: 大量アセットは段階的に処理
- **テスト環境**: 本番前のテスト環境での動作確認

### トラブルシューティング
- **処理中断**: 長時間処理の場合は分割処理を検討
- **メモリ不足**: 大量アセット処理時のメモリ使用量監視
- **権限問題**: 読み取り専用ファイルでの処理エラー対応

### 実装の簡潔性
- **最小限実装**: 必要最小限の機能に集中
- **Unity API活用**: Unity標準APIを最大限活用
- **保守性**: シンプルな実装による高い保守性

## 現状の課題

### 重要度: Medium（中）
- **エラーハンドリング不足**: 操作が静的に失敗する可能性
  - **影響**: 処理失敗時にユーザーが気づかない
  - **改善提案**: try-catch文と明示的なエラー表示

- **パフォーマンス**: 長時間操作のフィードバックなし
  - **影響**: 大量アセット処理時に固まったように見える
  - **改善提案**: 進捗表示と処理時間の見積もり

- **安全性**: 選択アセットの検証なし
  - **影響**: 重要なアセットの意図しない変更リスク
  - **改善提案**: アセットタイプ制限と確認ダイアログ

### 具体的な改善コード例

```csharp
[MenuItem("Assets/Reserialize")]
public static void Reserialize()
{
    var objects = Selection.objects;
    if (objects.Length == 0)
    {
        EditorUtility.DisplayDialog("警告", "アセットが選択されていません", "OK");
        return;
    }
    
    // アセットパス取得と検証
    var paths = objects.Select(AssetDatabase.GetAssetPath)
                      .Where(p => !string.IsNullOrEmpty(p))
                      .ToList();
    
    if (paths.Count == 0)
    {
        EditorUtility.DisplayDialog("エラー", "有効なアセットが見つかりません", "OK");
        return;
    }
    
    // 重要アセットのチェック
    var criticalAssets = paths.Where(IsCriticalAsset).ToList();
    if (criticalAssets.Count > 0)
    {
        bool proceed = EditorUtility.DisplayDialog(
            "重要アセット確認",
            $"重要なアセットが含まれています:\n{string.Join("\n", criticalAssets.Take(5))}\n\n続行しますか？",
            "続行", "キャンセル");
            
        if (!proceed) return;
    }
    
    // 処理時間見積もり
    float estimatedTime = EstimateReserializeTime(paths.Count);
    if (estimatedTime > 5f)
    {
        bool proceed = EditorUtility.DisplayDialog(
            "処理時間警告",
            $"推定処理時間: {estimatedTime:F1}秒\n{paths.Count}個のアセットを処理します。続行しますか？",
            "続行", "キャンセル");
            
        if (!proceed) return;
    }
    
    try
    {
        EditorUtility.DisplayProgressBar("再シリアライズ中", "アセットを処理しています...", 0);
        
        // 進捗付き処理
        for (int i = 0; i < paths.Count; i++)
        {
            var path = paths[i];
            float progress = (float)i / paths.Count;
            
            EditorUtility.DisplayProgressBar(
                "再シリアライズ中", 
                $"処理中: {Path.GetFileName(path)} ({i + 1}/{paths.Count})", 
                progress);
            
            // キャンセルチェック（長時間処理の場合）
            if (i % 10 == 0 && EditorUtility.DisplayCancelableProgressBar(
                "再シリアライズ中", 
                $"処理中: {Path.GetFileName(path)} ({i + 1}/{paths.Count})", 
                progress))
            {
                if (EditorUtility.DisplayDialog("確認", "処理をキャンセルしますか？", "キャンセル", "続行"))
                {
                    break;
                }
            }
        }
        
        // 実際の再シリアライズ実行
        AssetDatabase.ForceReserializeAssets(
            paths, 
            ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata);
        
        Debug.Log($"再シリアライズ完了: {paths.Count}個のアセット");
        EditorUtility.DisplayDialog("完了", $"{paths.Count}個のアセットを再シリアライズしました", "OK");
    }
    catch (Exception ex)
    {
        Debug.LogError($"再シリアライズエラー: {ex.Message}");
        EditorUtility.DisplayDialog("エラー", $"再シリアライズに失敗しました:\n{ex.Message}", "OK");
    }
    finally
    {
        EditorUtility.ClearProgressBar();
    }
}

// 重要アセット判定
private static bool IsCriticalAsset(string assetPath)
{
    string[] criticalExtensions = { ".unity", ".asset", ".preset", ".cs" };
    string[] criticalFolders = { "ProjectSettings", "Editor", "Scripts" };
    
    string extension = Path.GetExtension(assetPath).ToLower();
    if (criticalExtensions.Contains(extension))
        return true;
    
    return criticalFolders.Any(folder => assetPath.Contains($"/{folder}/"));
}

// 処理時間見積もり
private static float EstimateReserializeTime(int assetCount)
{
    // 経験的な見積もり（アセット数に基づく）
    const float baseTime = 0.1f; // 基本処理時間
    const float perAssetTime = 0.05f; // アセット当たりの時間
    
    return baseTime + (assetCount * perAssetTime);
}

// バッチ処理用（大量アセット対応）
public static void ReserializeLarge(IEnumerable<string> assetPaths)
{
    const int batchSize = 100; // バッチサイズ
    var paths = assetPaths.ToList();
    
    for (int i = 0; i < paths.Count; i += batchSize)
    {
        var batch = paths.Skip(i).Take(batchSize);
        
        try
        {
            AssetDatabase.ForceReserializeAssets(
                batch, 
                ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata);
                
            // バッチ間の小休止
            System.Threading.Thread.Sleep(100);
        }
        catch (Exception ex)
        {
            Debug.LogError($"バッチ {i / batchSize + 1} 処理エラー: {ex.Message}");
        }
    }
}
```