# Texture2DArrayMaker Tool

このファイルは、Texture2DArrayMakerツールでの作業時にClaude Codeへのガイダンスを提供します。

## ツール概要

Texture2DArrayMakerツールは、複数のTexture2Dアセットを単一のTexture2DArrayアセットに結合するエディター拡張です。テクスチャの互換性検証と効率的なアレイ作成を提供します。

## 主要機能

### 基本機能
- **テクスチャアレイ作成**: 複数Texture2Dを単一Texture2DArrayに結合
- **テクスチャ検証**: すべての入力テクスチャの寸法と形式の互換性をチェック
- **アセット統合**: Texture2DArrayをUnityアセットとして作成・更新
- **自動命名**: ScriptableObjectの名前に基づいたアレイの自動命名
- **コンテキストメニュー**: 右クリックでアレイ作成・更新操作

### 使用方法
1. アセット作成: Project右クリック → Create → Texture2DArrayMaker
2. Inspectorのリストで入力テクスチャを設定
3. テクスチャが一致しない場合は検証警告が表示
4. アセット右クリック → "Create or Update Texture2DArray"
5. 出力Texture2DArrayが自動作成・更新

## 実装詳細

### ファイル構造
- **Texture2DArrayMaker.cs**: ScriptableObjectベースの単一クラス

### データ構造
```csharp
[SerializeField] List<Texture2D> inputTextures = new List<Texture2D>();
[SerializeField] bool isValid = true;
[SerializeField] string validationMessage = "";
```

### 主要メソッド
- **OnValidate()**: テクスチャ互換性チェック
- **CreateOrUpdateTexture2DArray()**: アレイ作成・更新
- **ValidateTextures()**: 詳細な検証ロジック

## 技術的特徴

### 検証システム
- **寸法チェック**: 全テクスチャの幅・高さが同一かチェック
- **形式チェック**: 全テクスチャが同じTextureFormatを使用するかチェック
- **リアルタイム検証**: `OnValidate()`による即座の互換性チェック

### 効率的な処理
- **Graphics.CopyTexture()**: 効率的なアレイ内容コピー
- **AssetDatabase.StartAssetEditing()**: バッチ処理による効率化
- **既存アセット更新**: 新規作成と既存更新の両対応

### アセット管理
- **自動パス生成**: `[AssetName]_TexArray.asset`形式での命名
- **アセット参照**: 既存アセットの適切な更新処理
- **EditorUtility.SetDirty()**: 変更の適切なマーク

## 設定詳細

### 検証要件
- **同一寸法**: すべてのテクスチャが同じwidth × height
- **同一形式**: すべてのテクスチャが同じTextureFormat
- **null チェック**: リスト内の無効なテクスチャエントリの検出

### 出力設定
- **ファイル命名**: `[ScriptableObjectの名前]_TexArray.asset`
- **保存場所**: ScriptableObjectアセットと同じディレクトリ
- **アセットタイプ**: Texture2DArray

### UI表示
- **インスペクター統合**: CreateAssetMenuによる作成メニュー統合
- **検証フィードバック**: 互換性問題の即座の表示
- **コンテキストメニュー**: アセット右クリックでの操作

## 開発ノート

### エラーハンドリング
- **null テクスチャ**: リスト内のnullエントリの適切な処理
- **寸法不一致**: 異なるサイズのテクスチャでの警告表示
- **形式不一致**: 異なるフォーマットでの詳細エラーメッセージ

### パフォーマンス最適化
- **効率的コピー**: `Graphics.CopyTexture()`による高速テクスチャコピー
- **バッチ処理**: アセット編集のバッチ化
- **メモリ管理**: 一時的なテクスチャオブジェクトの適切な管理

### 拡張可能性
- **ミップマップ対応**: 将来的なミップマップ生成オプション
- **圧縮設定**: テクスチャ圧縮オプションの追加
- **カスタム形式**: 特定用途向けの形式変換機能

### 使用場面
- **シェーダー配列**: シェーダーでのテクスチャ配列使用
- **パフォーマンス最適化**: ドローコール削減のためのテクスチャ統合
- **バッチ処理**: 類似テクスチャの効率的管理

### 制限事項
- **テクスチャ制限**: Unity Texture2DArrayの制限に従う
- **フォーマット統一**: 異なるフォーマット混在不可
- **サイズ統一**: 異なるサイズのテクスチャ混在不可

### ベストプラクティス
- **事前準備**: 入力テクスチャの寸法・形式を事前に統一
- **命名規則**: わかりやすいScriptableObject名でのアレイ識別
- **組織化**: 関連テクスチャをグループ化してアレイ作成

### 今後の改善点
- **自動リサイズ**: 異なるサイズテクスチャの自動リサイズ機能
- **形式変換**: 自動的なフォーマット統一機能
- **プレビュー**: アレイ内容のプレビュー表示