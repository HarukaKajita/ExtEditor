# CopySelectMenu Tool

このファイルは、CopySelectMenuツールでの作業時にClaude Codeへのガイダンスを提供します。

## ツール概要

CopySelectMenuツールは、アセットのパスやGUIDをクリップボードにコピーし、クリップボードからパスやGUIDを使用して選択を復元するエディター拡張です。Unityセッション間やプロジェクト間でのアセット選択の転送を可能にします。

## 主要機能

### 基本機能
- **パスコピー**: 選択アセットのパスをクリップボードにコピー
- **GUIDコピー**: 選択アセットのGUIDをクリップボードにコピー
- **選択復元**: クリップボードのパスやGUIDから選択を復元
- **複数アセット対応**: 複数選択アセットを同時に処理
- **クリップボード統合**: システムクリップボードを使用したデータ転送

### 使用方法
1. Projectウィンドウでアセットを選択
2. 右クリック → Assets → Copy Select Menus → 操作を選択:
   - **Copy Path**: アセットパスをクリップボードにコピー
   - **Copy GUID**: アセットGUIDをクリップボードにコピー
   - **Select with Path from Clipboard**: クリップボードのパスからアセットを選択
   - **Select with GUID from Clipboard**: クリップボードのGUIDからアセットを選択

## 実装詳細

### ファイル構造
- **CopySelectMenu.cs**: 全機能を含む単一の静的クラス

### メニュー構造
```csharp
[MenuItem("Assets/Copy Select Menus/Copy Path")]
[MenuItem("Assets/Copy Select Menus/Copy GUID")]
[MenuItem("Assets/Copy Select Menus/Select with Path from Clipboard")]
[MenuItem("Assets/Copy Select Menus/Select with GUID from Clipboard")]
```

### 主要メソッド
- **CopyPath()**: 選択アセットのパスをコピー
- **CopyGUID()**: 選択アセットのGUIDをコピー
- **SelectWithPathFromClipboard()**: パスから選択を復元
- **SelectWithGUIDFromClipboard()**: GUIDから選択を復元

## 技術的特徴

### データ処理
- **複数アセット**: 改行区切りで複数アセットを処理
- **パス取得**: `AssetDatabase.GetAssetPath()`でパス取得
- **GUID変換**: `AssetDatabase.GUIDToAssetPath()`でGUID→パス変換
- **無効フィルタ**: 無効または見つからないアセットを除外

### クリップボード操作
- **システム統合**: `EditorGUIUtility.systemCopyBuffer`使用
- **テキスト形式**: プレーンテキストでのデータ交換
- **改行区切り**: 複数項目の区切り文字として改行使用

### エラーハンドリング
- **null チェック**: 選択オブジェクトの存在確認
- **無効パス**: 存在しないパスやGUIDの処理
- **空クリップボード**: クリップボードが空の場合の処理

## 使用例

### パスのコピー・復元
```
// コピーされる内容（複数アセット）
Assets/Textures/logo.png
Assets/Materials/surface.mat
Assets/Scripts/Player.cs
```

### GUIDのコピー・復元
```
// コピーされる内容（複数GUID）
a1b2c3d4e5f6789012345678
9876543210fedcba987654321
1122334455667788aabbccdd
```

## 開発ノート

### 活用シーン
- **プロジェクト間転送**: 同じアセット構造を持つプロジェクト間での選択転送
- **チーム共有**: パスやGUIDをチームメンバーと共有
- **バックアップ選択**: 重要な選択状態の一時保存
- **スクリプト連携**: 外部スクリプトとのアセット情報交換

### 制限事項
- **アセット存在依存**: 対象アセットがプロジェクトに存在する必要
- **パス変更**: アセットの移動や名前変更でパスベース選択が無効化
- **プロジェクト依存**: GUIDは同じプロジェクト内でのみ有効

### 拡張可能性
- **JSON形式**: より構造化されたデータ形式での転送
- **フィルタ機能**: 特定タイプのアセットのみコピー
- **履歴機能**: コピー・選択履歴の保持
- **UI統合**: 専用ウィンドウでの操作インターフェース

### ベストプラクティス
- **GUID推奨**: アセット移動に強いGUIDベースの選択を推奨
- **一時保存**: 重要な選択は他の選択データツールで永続化
- **確認習慣**: 復元後の選択内容確認を習慣化

### パフォーマンス
- **軽量操作**: 単純な文字列操作のため高速
- **メモリ効率**: 一時的な文字列のみメモリ使用
- **スケーラブル**: 大量アセットでも効率的に動作

### ログ出力
- **コンソールログ**: 各操作の結果をConsoleに出力
- **成功/失敗**: 操作の成功・失敗を明確に表示
- **デバッグ情報**: 詳細な処理情報でトラブルシューティング支援

## 現状の課題

### 重要度: Medium（中）
- **文字列処理の非効率性**: ループ内での非効率な文字列連結
  - **影響**: 大量アセット処理時のパフォーマンス低下
  - **改善提案**: StringBuilder使用による効率的な文字列構築

- **エラーハンドリング不足**: クリップボード内容フォーマットの検証なし
  - **影響**: 不正なクリップボードデータで予期しない動作
  - **改善提案**: 入力データ検証と例外処理強化

- **ユーザーエクスペリエンス**: 操作失敗時のフィードバック不足
  - **影響**: 失敗原因が分からずトラブルシューティング困難
  - **改善提案**: 詳細なエラーメッセージとダイアログ表示

### 重要度: Low（低）
- **コード重複**: 類似ロジックがメソッド間で繰り返し
  - **影響**: 保守性低下とバグ修正の手間増加
  - **改善提案**: 共通処理の関数化とリファクタリング

- **国際化対応不足**: 英語メッセージのハードコード
  - **影響**: 多言語環境での利便性低下
  - **改善提案**: リソースファイル化または設定による言語切り替え

### 具体的な改善コード例

```csharp
// 文字列処理効率化
public static void CopyPath()
{
    var selectedObjects = Selection.objects;
    if (selectedObjects.Length == 0)
    {
        EditorUtility.DisplayDialog("警告", "オブジェクトが選択されていません", "OK");
        return;
    }

    var pathsBuilder = new StringBuilder();
    int validCount = 0;
    
    foreach (var obj in selectedObjects)
    {
        var path = AssetDatabase.GetAssetPath(obj);
        if (!string.IsNullOrEmpty(path))
        {
            pathsBuilder.AppendLine(path);
            validCount++;
        }
    }

    if (validCount == 0)
    {
        EditorUtility.DisplayDialog("警告", "有効なアセットパスが見つかりません", "OK");
        return;
    }
    
    var paths = pathsBuilder.ToString().TrimEnd();
    EditorGUIUtility.systemCopyBuffer = paths;
    
    Debug.Log($"{validCount}個のパスをクリップボードにコピーしました");
    EditorUtility.DisplayDialog("完了", $"{validCount}個のアセットパスをコピーしました", "OK");
}

// クリップボード検証とエラーハンドリング強化
public static void SelectWithPathFromClipboard()
{
    try
    {
        string clipboardContent = EditorGUIUtility.systemCopyBuffer;
        
        if (string.IsNullOrEmpty(clipboardContent))
        {
            EditorUtility.DisplayDialog("エラー", "クリップボードが空です", "OK");
            return;
        }
        
        // パス形式検証
        string[] paths = clipboardContent.Split(new[] { '\n', '\r' }, 
                                              StringSplitOptions.RemoveEmptyEntries);
        
        if (paths.Length == 0)
        {
            EditorUtility.DisplayDialog("エラー", "有効なパス情報がありません", "OK");
            return;
        }
        
        var validObjects = new List<UnityEngine.Object>();
        var invalidPaths = new List<string>();
        
        foreach (string path in paths)
        {
            var trimmedPath = path.Trim();
            
            // パス形式基本検証
            if (!IsValidAssetPath(trimmedPath))
            {
                invalidPaths.Add(trimmedPath);
                continue;
            }
            
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(trimmedPath);
            if (obj != null)
            {
                validObjects.Add(obj);
            }
            else
            {
                invalidPaths.Add(trimmedPath);
            }
        }
        
        if (validObjects.Count > 0)
        {
            Selection.objects = validObjects.ToArray();
            Debug.Log($"{validObjects.Count}個のオブジェクトを選択しました");
            
            if (invalidPaths.Count > 0)
            {
                Debug.LogWarning($"{invalidPaths.Count}個の無効なパスがありました: {string.Join(", ", invalidPaths.Take(3))}");
                EditorUtility.DisplayDialog("部分的成功", 
                    $"{validObjects.Count}個選択、{invalidPaths.Count}個のパスが無効でした", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("完了", $"{validObjects.Count}個のアセットを選択しました", "OK");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("エラー", "有効なアセットが見つかりませんでした", "OK");
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"パス選択エラー: {ex.Message}");
        EditorUtility.DisplayDialog("エラー", $"処理中にエラーが発生しました: {ex.Message}", "OK");
    }
}

// 共通ユーティリティ
private static bool IsValidAssetPath(string path)
{
    if (string.IsNullOrEmpty(path)) return false;
    if (!path.StartsWith("Assets/") && !path.StartsWith("Packages/")) return false;
    if (path.Contains("..") || path.Contains("//")) return false;
    return true;
}

// 共通処理の抽出
private static (List<T> validItems, List<string> invalidItems) ProcessClipboardItems<T>(
    string clipboardContent, 
    Func<string, T> converter, 
    Func<string, bool> validator = null) where T : class
{
    var validItems = new List<T>();
    var invalidItems = new List<string>();
    
    if (string.IsNullOrEmpty(clipboardContent))
        return (validItems, invalidItems);
    
    string[] items = clipboardContent.Split(new[] { '\n', '\r' }, 
                                           StringSplitOptions.RemoveEmptyEntries);
    
    foreach (string item in items)
    {
        string trimmedItem = item.Trim();
        
        if (validator != null && !validator(trimmedItem))
        {
            invalidItems.Add(trimmedItem);
            continue;
        }
        
        T converted = converter(trimmedItem);
        if (converted != null)
        {
            validItems.Add(converted);
        }
        else
        {
            invalidItems.Add(trimmedItem);
        }
    }
    
    return (validItems, invalidItems);
}
```