# BlendShapeFixpostProcessor Tool

このファイルは、BlendShapeFixpostProcessorツールでの作業時にClaude Codeへのガイダンスを提供します。

## ツール概要

BlendShapeFixpostProcessorツールは、モデルインポート時に不正に順序付けられたBlendShapeフレームウェイトを自動的に修正するエディター拡張です。AssetPostprocessorを使用して自動処理を行います。

## 主要機能

### 基本機能
- **BlendShapeウェイトソート**: インポート時にBlendShapeフレームウェイトの順序を自動修正
- **自動処理**: AssetPostprocessor経由でモデルインポート時に自動実行
- **ウェイト検証**: BlendShapeフレームがウェイト順にソートされているかチェック
- **スマート再構築**: 修正が必要なBlendShapeのみを再構築
- **完全データ保持**: 頂点、法線、タンジェントを維持
- **マルチメッシュサポート**: インポートされたモデル内のすべてのSkinnedMeshRendererを処理

### 動作原理
1. モデルインポート時に自動実行
2. 各SkinnedMeshRendererのBlendShapeを検査
3. フレームウェイトが昇順でない場合に修正処理を実行
4. BlendShapeを再構築してウェイト順にソート

## 実装詳細

### 主要クラス
- **BlendShapeFixPostProcessor**: AssetPostprocessorを継承した単一クラス

### 処理フロー
1. **OnPostprocessModel()**: モデルインポート時のフック
2. **BlendShape検出**: SkinnedMeshRendererでBlendShapeを持つメッシュを特定
3. **ウェイト検証**: フレームウェイトが正しくソートされているかチェック
4. **データ収集**: BlendShape情報（頂点、法線、タンジェント、ウェイト）を収集
5. **再構築**: `Mesh.ClearBlendShapes()`と`Mesh.AddBlendShapeFrame()`で再構築

### データ構造
```csharp
// BlendShapeデータを保持するカスタム構造
struct BlendShapeData {
    Vector3[] vertices;
    Vector3[] normals;
    Vector3[] tangents;
    float weight;
}
```

## 技術的特徴

### 効率的な処理
- **条件付き処理**: BlendShapeを持つメッシュのみを処理
- **ソートチェック**: 既に正しく順序付けられている場合はスキップ
- **バッチ処理**: すべてのフレームを一度に再構築

### データ整合性
- **完全データ保持**: 頂点位置、法線、タンジェントを完全に保持
- **ウェイトベースソート**: フレームウェイトによる昇順ソート
- **フレーム名維持**: BlendShapeフレーム名を保持

### 自動化
- **ユーザー介入不要**: 手動操作が不要
- **透明処理**: エラーがない限りログ出力なし（現在はコメントアウト）
- **統合処理**: Unityのインポートパイプラインに完全統合

## 開発ノート

### 対象アセット
- **FBXファイル**: 主にFBXインポート時に動作
- **BlendShape含有モデル**: BlendShapeを含むすべてのモデル
- **SkinnedMeshRenderer**: スキンメッシュでのみ動作

### エラーハンドリング
- **null チェック**: メッシュとBlendShapeデータの存在確認
- **データ検証**: フレーム数とデータ整合性の確認
- **例外保護**: 処理中の例外からの保護

### パフォーマンス考慮
- **必要時のみ処理**: 問題のあるBlendShapeのみを修正
- **メモリ効率**: 一時的なデータ構造の最小化
- **処理速度**: 効率的なソートアルゴリズム

### 拡張可能性
- **ログ機能**: 現在コメントアウトされているログ機能の有効化
- **カスタムソート**: ウェイト以外の基準でのソート
- **選択的処理**: 特定の条件下でのみ処理を実行

### 注意点
- **非可逆処理**: 一度処理されたBlendShapeは元に戻せない
- **インポート時実行**: アセット再インポート時に毎回実行
- **データ依存**: 元のBlendShapeデータの品質に依存

### 今後の改善点
- **ログ出力**: 処理されたBlendShapeの詳細ログ
- **設定オプション**: 処理の有効/無効切り替え
- **統計情報**: 修正されたBlendShapeの統計表示

## 現状の課題

### 重要度: High（高）
- **メモリ使用量**: 境界チェックなしでの大配列割り当て
  - **影響**: 大きなメッシュで OutOfMemoryException やエディタークラッシュ
  - **改善提案**: メッシュサイズ制限と段階的処理

- **パフォーマンス**: フレームソートチェックのO(n²)複雑度（実際はO(n)だが改善余地あり）
  - **影響**: 大量BlendShapeフレームで処理時間大幅増加
  - **改善提案**: 効率的なソートアルゴリズムとバイナリ検索

- **データ損失リスク**: バックアップや検証なしでのメッシュ変更
  - **影響**: 処理失敗時に元データの復元不可
  - **改善提案**: 処理前のバックアップ作成と検証機能

### 重要度: Medium（中）
- **エラー回復なし**: 操作失敗時にメッシュが未定義状態に残る
  - **影響**: 部分的な処理失敗でメッシュが破損状態
  - **改善提案**: ロールバック機能と整合性チェック

- **サイレント処理**: ユーザーに高コスト処理のフィードバックなし
  - **影響**: 長時間処理で進捗不明、処理内容不明
  - **改善提案**: 進捗表示と処理結果の詳細ログ

### 具体的な改善コード例

```csharp
void OnPostprocessModel(GameObject importedModel)
{
    var skinnedMeshRenderers = importedModel.GetComponentsInChildren<SkinnedMeshRenderer>();
    
    foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
    {
        Mesh mesh = skinnedMeshRenderer.sharedMesh;
        
        // 安全性チェック追加
        if (mesh == null || mesh.blendShapeCount == 0) continue;
        
        // メモリ使用量チェック
        const int maxVertices = 65536; // 一般的な制限
        if (mesh.vertexCount > maxVertices)
        {
            Debug.LogWarning($"大きなメッシュ {mesh.name} ({mesh.vertexCount} 頂点) はスキップされました");
            continue;
        }
        
        // 処理が必要かチェック（既存のロジック）
        bool needsFix = NeedsBlendShapeFix(mesh);
        if (!needsFix) continue;
        
        // ユーザー通知
        Debug.Log($"BlendShape修正中: {mesh.name} ({mesh.blendShapeCount} shapes)");
        
        try
        {
            // バックアップ作成
            var backupData = CreateBlendShapeBackup(mesh);
            
            // 修正処理
            FixBlendShapeFrameOrder(mesh);
            
            // 検証
            if (!ValidateBlendShapeData(mesh))
            {
                RestoreBlendShapeBackup(mesh, backupData);
                Debug.LogError($"BlendShape修正失敗、復元しました: {mesh.name}");
            }
            else
            {
                Debug.Log($"BlendShape修正完了: {mesh.name}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"BlendShape処理エラー {mesh.name}: {ex.Message}");
        }
    }
}

// バックアップ作成
private struct BlendShapeBackup
{
    public Vector3[][] vertices;
    public Vector3[][] normals;
    public Vector3[][] tangents;
    public float[] weights;
    public string[] names;
}

private BlendShapeBackup CreateBlendShapeBackup(Mesh mesh)
{
    var backup = new BlendShapeBackup
    {
        vertices = new Vector3[mesh.blendShapeCount][],
        normals = new Vector3[mesh.blendShapeCount][],
        tangents = new Vector3[mesh.blendShapeCount][],
        weights = new float[mesh.blendShapeCount],
        names = new string[mesh.blendShapeCount]
    };
    
    for (int i = 0; i < mesh.blendShapeCount; i++)
    {
        backup.names[i] = mesh.GetBlendShapeName(i);
        int frameCount = mesh.GetBlendShapeFrameCount(i);
        
        for (int frame = 0; frame < frameCount; frame++)
        {
            backup.weights[i] = mesh.GetBlendShapeFrameWeight(i, frame);
            backup.vertices[i] = new Vector3[mesh.vertexCount];
            backup.normals[i] = new Vector3[mesh.vertexCount];
            backup.tangents[i] = new Vector3[mesh.vertexCount];
            
            mesh.GetBlendShapeFrameVertices(i, frame, 
                backup.vertices[i], backup.normals[i], backup.tangents[i]);
        }
    }
    
    return backup;
}

// 効率的なソートチェック
private bool NeedsBlendShapeFix(Mesh mesh)
{
    for (int i = 0; i < mesh.blendShapeCount; i++)
    {
        int frameCount = mesh.GetBlendShapeFrameCount(i);
        if (frameCount <= 1) continue;
        
        for (int frame = 1; frame < frameCount; frame++)
        {
            float prevWeight = mesh.GetBlendShapeFrameWeight(i, frame - 1);
            float currWeight = mesh.GetBlendShapeFrameWeight(i, frame);
            if (prevWeight > currWeight)
                return true;
        }
    }
    return false;
}

// 検証機能
private bool ValidateBlendShapeData(Mesh mesh)
{
    try
    {
        // 基本検証
        if (mesh.blendShapeCount == 0) return true;
        
        // ソート検証
        return !NeedsBlendShapeFix(mesh);
    }
    catch
    {
        return false;
    }
}
```