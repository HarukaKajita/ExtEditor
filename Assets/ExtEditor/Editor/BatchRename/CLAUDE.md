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

## 現状の課題

### 重要度: Critical（緊急）
- **設定の永続化問題**: ドメインリロード時に設定が失われる
  - **影響**: ユーザーが設定した内容が保存されず、毎回再入力が必要
  - **改善提案**: `BatchRenamerSettings.asset`ファイルの適切な読み込み・保存機能実装
  
- **Undo対応不備**: リネーム操作をUndo（取り消し）できない
  - **影響**: 間違った操作を取り消せず、手動で修正する必要がある
  - **改善提案**: `Undo.RecordObjects()`による操作前の状態記録

### 重要度: High（高）
- **アセットパス処理の非効率性**: ハードコードされたパスがプロジェクト構造変更で破綻
  - **影響**: プロジェクト移動や構造変更時にツールが動作しなくなる
  - **改善提案**: 相対パスと動的パス解決の実装

- **アセット操作の競合状態**: 進行中のインポート処理と衝突する可能性
  - **影響**: アセットデータベースの破損やクラッシュリスク
  - **改善提案**: `AssetDatabase.IsAssetImportWorkerProcess()`チェックと適切な待機処理

- **メモリリーク**: `CreateInstance`オブジェクトが適切に破棄されない
  - **影響**: 長時間使用でメモリ使用量が増加
  - **改善提案**: `using`文またはtry-finallyブロックでの確実な破棄処理

### 重要度: Medium（中）
- **ユーザーエクスペリエンス不足**: 適用前の検証プレビューが不十分
  - **影響**: 予期しない結果になるリスクが高い
  - **改善提案**: より詳細なプレビュー表示と警告機能

- **エラー報告の限定性**: エラーがログのみで、ユーザーに直接表示されない
  - **影響**: 問題が発生してもユーザーが気づかない
  - **改善提案**: `EditorUtility.DisplayDialog()`による直接的なエラー表示

- **拡張子処理のバグ**: プレビューで拡張子が重複して表示される
  - **影響**: 混乱を招く不正確なプレビュー表示
  - **改善提案**: 拡張子処理ロジックの修正と統一

### 具体的な改善コード例

```csharp
// 設定の永続化修正
private void OnEnable()
{
    string settingsPath = "Assets/ExtEditor/Editor/BatchRename/BatchRenamerSettings.asset";
    renamer = AssetDatabase.LoadAssetAtPath<BatchRenamer>(settingsPath);
    if (renamer == null)
    {
        renamer = CreateInstance<BatchRenamer>();
        Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
        AssetDatabase.CreateAsset(renamer, settingsPath);
    }
}

// Undo対応追加
private void ApplyRenaming(Object[] assetsToRename)
{
    Undo.RecordObjects(assetsToRename, "Batch Rename Assets");
    // 既存のリネーム処理...
}
```