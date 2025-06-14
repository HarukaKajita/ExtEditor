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