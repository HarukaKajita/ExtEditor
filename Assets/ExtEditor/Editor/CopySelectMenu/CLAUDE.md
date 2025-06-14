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