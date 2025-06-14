# BatchRename Tool

このファイルは、BatchRenameツールでの作業時にClaude Codeへのガイダンスを提供します。

## ツール概要

BatchRenameツールは、Unityプロジェクト内で複数のアセットを一括でリネームするためのエディター拡張です。プレフィックス/サフィックスの追加や文字列の検索・置換機能を提供します。

## 主要機能

### 基本機能
- **プレフィックス/サフィックス追加**: アセット名の前後にテキストを追加
- **検索・置換**: アセット名内の特定文字列を検索して置換
- **ライブプレビュー**: 変更前にリネーム結果をプレビュー表示
- **設定の永続化**: リネーム設定をScriptableObjectアセットに保存
- **安全なリネーム**: 空の名前を防ぎ、エラーを適切に処理

### 使用方法
1. **ツール → Batch Renamer**メニューからアクセス
2. Projectウィンドウでアセットを選択
3. プレフィックス、サフィックス、検索・置換文字列を設定
4. スクロール可能なプレビューエリアで変更を確認
5. "Apply Renaming to Selected Assets"をクリックして実行
6. "Save Settings"で設定を保存

## 実装詳細

### 主要クラス
- **BatchRenamer**: 設定を保存するScriptableObject
- **BatchRenamerEditor**: エディターウィンドウの実装

### 設定ファイル
- 設定は`Assets/ExtEditor/Editor/BatchRename/BatchRenamerSettings.asset`に保存
- プレフィックス、サフィックス、検索・置換文字列を保持

### 技術的特徴
- `AssetDatabase.StartAssetEditing()`/`AssetDatabase.StopAssetEditing()`による効率的なバッチ処理
- `AssetDatabase.RenameAsset()`による適切なUnityアセットリネーム
- 包括的なエラーハンドリングとログ出力
- 拡張子の自動維持

## 開発ノート

### エラーハンドリング
- 空の名前のバリデーション
- アセットリネーム失敗時の詳細ログ
- プレビュー時の例外処理

### パフォーマンス最適化
- バッチ処理APIの使用により大量アセットの効率的処理
- プレビュー表示のスクロールビュー対応

### 拡張性
- 新しいリネーム操作を追加する場合は、`BatchRenamer`クラスに新しいメソッドを追加
- UIは`BatchRenamerEditor.OnGUI()`メソッドで拡張可能