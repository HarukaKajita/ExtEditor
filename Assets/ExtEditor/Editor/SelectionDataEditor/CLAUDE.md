# SelectionDataEditor Tool

このファイルは、SelectionDataEditorツールでの作業時にClaude Codeへのガイダンスを提供します。

## ツール概要

SelectionDataEditorツールは、Unity内のオブジェクト選択状態を保存・復元し、高度な選択操作を提供するエディター拡張です。ScriptableObjectベースで選択データを管理し、Union、Intersection、Difference操作をサポートします。

## 主要機能

### 基本機能
- **選択状態管理**: オブジェクト選択をScriptableObjectアセットとして保存・復元
- **高度な選択操作**: 複数選択セットのUnion、Intersection、Difference操作
- **クエリベース選択**: Unity Search APIを使用した動的選択
- **選択の組み合わせ**: 複数のSelectionDataアセットを組み合わせて新しい選択を作成
- **スター機能**: 重要な選択をスターマークしてリストの上位に表示
- **管理ウィンドウ**: 選択データの作成、整理、適用専用ウィンドウ

### 使用方法

#### SelectionDataの作成
- **CreateAssetMenu経由**: Project右クリック → Create → ExtEditor → Selection Data
- **管理ウィンドウ経由**: Tools → ExtEditor → Selection Data Window → Newボタン

#### SelectionDataのタイプ
1. **Object-based**: Unity オブジェクトの直接リスト
2. **Query-based**: Unity Searchクエリ使用（例: "t:Texture", "t:Material"）
3. **Combined**: 他のSelectionDataをUnion/Intersection/Difference操作で組み合わせ

#### SelectionDataの使用
- Inspectorボタンまたは管理ウィンドウで選択を適用
- 現在のエディター選択から設定
- クエリのテストと結果確認
- 重要なアイテムにスターマーク

## 実装詳細

### ファイル構造
- **SelectionData.cs**: ScriptableObjectの定義とロジック
- **SelectionDataEditor.cs**: カスタムInspectorの実装
- **SelectionDataWindow.cs**: 管理ウィンドウの実装

### データ構造
```csharp
public enum SelectionMode { Object, Query, Combined }
public enum CombineMode { Static, Dynamic }
public enum CombineOperation { Union, Intersection, Difference }
```

### 主要クラス機能
- **SelectionData**: 選択データの保存と効果的な選択オブジェクト取得
- **SelectionDataEditor**: 選択モードに応じた条件付きUI
- **SelectionDataWindow**: フィルタリング、検索、一括操作

## 技術的特徴

### 循環参照検出
- **HashSet使用**: 再帰操作での循環検出
- **安全な組み合わせ**: ネストした組み合わせでの無限ループ防止

### パフォーマンス最適化
- **遅延評価**: `GetEffectiveSelectedObjects()`での効率的な計算
- **キャッシュモード**: 静的モードでの結果キャッシュ
- **動的モード**: アクセス時の再計算

### Unity Search統合
- **クエリバリデーション**: 不正なクエリの検出と警告
- **リアルタイム結果**: 動的モードでのリアルタイム結果更新
- **タイプ安全**: 検索結果のUnityオブジェクト型チェック

## 設定詳細

### 組み合わせモード
- **Static**: パフォーマンス重視、結果をキャッシュ
- **Dynamic**: リアルタイム更新、アクセス時に再計算

### 組み合わせ操作
- **Union**: セットの結合（追加）
- **Intersection**: セットの共通部分
- **Difference**: セットの差分（減算）

### 管理ウィンドウ機能
- **フィルタリング**: タイプ、スターマーク、検索による絞り込み
- **一括操作**: 複数選択での一括適用
- **ソート**: 名前、タイプ、スターマークでのソート

## 開発ノート

### エラーハンドリング
- **循環参照**: 組み合わせ選択での循環参照検出
- **無効オブジェクト**: 削除されたオブジェクトの自動除外
- **クエリエラー**: 不正なSearchクエリの適切な処理

### 拡張性
- **新しい組み合わせ操作**: `CombineOperation`enumに追加
- **カスタムフィルター**: 管理ウィンドウのフィルタリング機能拡張
- **外部統合**: Unity Search以外のクエリエンジンとの統合

### ベストプラクティス
- **命名規則**: 選択データには説明的な名前を使用
- **組み合わせ階層**: 深すぎるネストを避けてパフォーマンス確保
- **スター活用**: よく使用する選択にスターマークを活用

### 注意点
- **オブジェクト参照**: シーンオブジェクトは保存時に失われる可能性
- **クエリ依存**: プロジェクト構造の変更がクエリ結果に影響
- **パフォーマンス**: 大量オブジェクトでの動的モード使用に注意

## 現状の課題

### 重要度: Critical（緊急）
- **無限再帰の潜在リスク**: HashSetコピー時の新インスタンス作成で循環検出を回避
  - **影響**: 循環参照のあるSelectionDataでスタックオーバーフローによるクラッシュ
  - **改善提案**: 同一HashSetインスタンスの受け渡しによる正確な循環検出

- **データ破損**: ソースアセット削除時に動的選択データが破損
  - **影響**: 削除されたアセットを参照して予期しない動作やエラー
  - **改善提案**: アセット削除時の自動クリーンアップとnull参照除去

### 重要度: High（高）
- **パフォーマンス**: メモ化なしの再帰選択解決で指数的複雑度
  - **影響**: 複雑な組み合わせ選択で動作が極端に遅くなる
  - **改善提案**: 結果キャッシュとメモ化による計算最適化

- **メモリリーク**: 大きな選択リストがガベージコレクションされない
  - **影響**: 長時間使用でメモリ使用量の継続的増加
  - **改善提案**: WeakReferenceまたは定期的なクリーンアップ機能

- **Unity Search統合の限定性**: 簡略化された検索実装で複雑クエリを処理できない
  - **影響**: 高度な検索クエリが期待通りに動作しない
  - **改善提案**: Unity Search APIの完全活用と検証機能強化

### 重要度: Medium（中）
- **UI フリーズ**: 大量選択でエディターUIが固まる
  - **影響**: 大量オブジェクト処理時のエディター応答性低下
  - **改善提案**: 非同期処理と進捗表示の実装

- **アセット削除安全性**: SelectionDataで参照されるアセットの削除保護なし
  - **影響**: 重要なアセットが誤って削除される可能性
  - **改善提案**: 削除前の参照チェックと警告表示

### 具体的な改善コード例

```csharp
// 循環検出修正
public List<UnityEngine.Object> GetEffectiveSelectedObjects(HashSet<SelectionData> visited = null)
{
    if (visited == null) visited = new HashSet<SelectionData>();
    
    if (visited.Contains(this))
    {
        Debug.LogWarning($"循環参照検出: {name}");
        return new List<UnityEngine.Object>();
    }
    visited.Add(this);

    // 修正: 同一HashSetインスタンスを渡す
    var currentSource = sourceSelectionData[i];
    List<UnityEngine.Object> currentSourceObjects = currentSource
        .GetEffectiveSelectedObjects(visited) // 同じHashSetを渡す
        .Where(o => o != null).ToList();
}

// パフォーマンス改善（メモ化）
private static Dictionary<SelectionData, List<UnityEngine.Object>> _cachedResults = 
    new Dictionary<SelectionData, List<UnityEngine.Object>>();
private static HashSet<SelectionData> _computingSet = new HashSet<SelectionData>();

public List<UnityEngine.Object> GetEffectiveSelectedObjects(HashSet<SelectionData> visited = null)
{
    // 計算中の場合は空リストを返す（循環回避）
    if (_computingSet.Contains(this))
        return new List<UnityEngine.Object>();
        
    // キャッシュチェック
    if (_cachedResults.TryGetValue(this, out var cached))
        return new List<UnityEngine.Object>(cached);
    
    _computingSet.Add(this);
    try
    {
        var result = GetEffectiveSelectedObjectsInternal(visited);
        _cachedResults[this] = result;
        return new List<UnityEngine.Object>(result);
    }
    finally
    {
        _computingSet.Remove(this);
    }
}

// アセット削除保護
[InitializeOnLoadMethod]
static void InitializeAssetDeletionProtection()
{
    AssetDatabase.deleteCancel += OnAssetDeletionCheck;
}

static bool OnAssetDeletionCheck(string assetPath, RemoveAssetOptions options)
{
    var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
    if (asset == null) return false;
    
    // SelectionDataでの参照をチェック
    var selectionDatas = AssetDatabase.FindAssets("t:SelectionData")
        .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
        .Select(path => AssetDatabase.LoadAssetAtPath<SelectionData>(path))
        .Where(sd => sd != null);
    
    foreach (var sd in selectionDatas)
    {
        if (sd.GetEffectiveSelectedObjects().Contains(asset))
        {
            return EditorUtility.DisplayDialog(
                "削除確認",
                $"アセット '{asset.name}' はSelectionData '{sd.name}' で参照されています。削除しますか？",
                "削除", "キャンセル");
        }
    }
    
    return false;
}

// キャッシュクリア機能
public static void ClearCache()
{
    _cachedResults.Clear();
    _computingSet.Clear();
}
```