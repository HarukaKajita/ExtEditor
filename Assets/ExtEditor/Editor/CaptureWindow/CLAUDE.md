# CaptureWindow Tool

このファイルは、CaptureWindowツールでの作業時にClaude Codeへのガイダンスを提供します。

## ツール概要

CaptureWindowツールは、Unity内の様々なビューからスクリーンショットを撮影し、PNGファイルとして保存するエディター拡張です。Game View、Scene View、カスタムRenderTextureに対応しています。

## 主要機能

### 基本機能
- **マルチソース撮影**: Game View、Scene View、カスタムRenderTextureから撮影
- **カメラ選択**: シーンカメラから選択またはカスタムカメラを指定
- **アルファチャンネルサポート**: PNG出力時にアルファチャンネルを含める
- **透明背景**: 透明背景での撮影オプション
- **自動ファイル名**: タイムスタンプベースの命名（`Capture_yyyyMMdd_HHmmss.png`）
- **キーボードショートカット**: F9 + Cmd/Ctrl + Shift でクイック撮影

### 使用方法
1. **ツール → CaptureWindow**メニューからアクセス
2. 撮影ソースを選択（Game View/Scene View/RenderTexture）
3. ドロップダウンまたはObjectFieldからターゲットカメラを選択
4. 出力ディレクトリを設定（デフォルト: `../Captures`）
5. アルファチャンネルと背景オプションを設定
6. "Capture and Save PNG"をクリックまたはショートカットを使用
7. "Open Output Folder"で保存画像を確認

## 実装詳細

### ファイル構造
- **メインクラス**: `CaptureWindow` - EditorWindowの実装
- **単一ファイル**: 全機能が一つのファイルに集約

### 撮影メカニズム
- **RenderTexture**: すべての撮影操作にRenderTextureを使用
- **カメラ状態管理**: 撮影後に適切にカメラ状態を復元
- **複数撮影モード**: ウィンドウ撮影とショートカット撮影に対応

### 技術的特徴
- `EditorUtility.RevealInFinder()`による出力フォルダーの表示
- 出力ディレクトリの自動作成
- カメラ背景設定の撮影中の適切な管理
- キーボードショートカットの統合

## 設定詳細

### キーボードショートカット
- **ショートカット**: `F9 + Cmd/Ctrl + Shift`
- **機能**: 現在の設定でクイック撮影を実行

### 出力設定
- **デフォルト出力先**: Assetsフォルダの上位ディレクトリ（`../Captures`）
- **ファイル形式**: PNG（アルファチャンネル対応）
- **命名規則**: `Capture_yyyyMMdd_HHmmss.png`

### 撮影オプション
- **アルファチャンネル**: 透明度情報を含める
- **背景透明化**: 背景を透明にして撮影
- **カメラ選択**: Scene内のカメラまたはカスタムカメラ

## 開発ノート

### エラーハンドリング
- 出力ディレクトリ作成失敗時の処理
- カメラが見つからない場合の警告
- RenderTexture作成失敗時の処理

### パフォーマンス考慮
- RenderTextureの適切な解放
- 撮影時のカメラ状態の最小限の変更
- メモリ効率的な画像処理

### 拡張可能性
- 新しい撮影ソースの追加は`CaptureSource`enumと対応するメソッドで実装
- 出力形式の拡張は`SaveTextureToPNG`メソッドを参考に実装
- ショートカットの追加は`OnGUI`メソッドのイベント処理部分で実装

## 現状の課題

### 重要度: Critical（緊急）
- **メモリリーク**: エラーパスでRenderTextureとTexture2Dが適切に解放されない
  - **影響**: 長時間使用や撮影失敗時にメモリ不足による動作不安定
  - **改善提案**: try-finallyブロックによる確実なリソース解放処理

- **Null参照例外**: 複数箇所でnullチェックが不十分
  - **影響**: アプリケーションクラッシュや予期しない動作
  - **改善提案**: カメラやターゲットテクスチャの存在確認強化

### 重要度: High（高）
- **スレッドセーフティ問題**: 静的変数`lastInstance`への同期なしアクセス
  - **影響**: マルチスレッド環境でのデータ競合とクラッシュリスク
  - **改善提案**: ロック機構またはインスタンスベース設計への変更

- **リソース管理不備**: エラー時にテクスチャが作成されても破棄されない
  - **影響**: メモリリークとGPUメモリ不足
  - **改善提案**: using文またはDisposableパターンの実装

- **カメラ状態破損**: 例外発生時に元のカメラ設定が復元されない
  - **影響**: 撮影後にカメラの表示状態が異常になる
  - **改善提案**: finally ブロックでの確実な状態復元

### 重要度: Medium（中）
- **多言語混在**: エラーメッセージで日本語と英語が混在
  - **影響**: 一貫性のないユーザーエクスペリエンス
  - **改善提案**: 統一された言語での表示または国際化対応

- **パスセキュリティ**: 出力ディレクトリパスの検証不足
  - **影響**: 任意のディレクトリへの書き込みによるセキュリティリスク
  - **改善提案**: パス正規化と安全なディレクトリ制限

- **パフォーマンス**: 毎フレームの不要なカメラ検索
  - **影響**: エディターのレスポンス性低下
  - **改善提案**: カメラ参照のキャッシュ機能

### 具体的な改善コード例

```csharp
// メモリリーク修正
private void CaptureAndSave()
{
    RenderTexture renderTexture = null;
    Texture2D tex = null;
    RenderTexture prevActive = RenderTexture.active;
    RenderTexture prevCamTarget = captureCamera.targetTexture;
    
    try
    {
        // カメラnullチェック追加
        if (captureCamera == null)
        {
            EditorUtility.DisplayDialog("エラー", "カメラが指定されていません", "OK");
            return;
        }
        
        renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        
        // 撮影処理...
        
        // 安全なパス検証
        if (!IsValidOutputPath(outputPath))
        {
            EditorUtility.DisplayDialog("エラー", "無効な出力パスです", "OK");
            return;
        }
        
        File.WriteAllBytes(filePath, tex.EncodeToPNG());
    }
    catch (Exception ex)
    {
        EditorUtility.DisplayDialog("撮影エラー", $"撮影に失敗しました: {ex.Message}", "OK");
    }
    finally
    {
        // 確実なリソース解放
        if (tex != null) DestroyImmediate(tex);
        if (renderTexture != null)
        {
            renderTexture.Release();
            DestroyImmediate(renderTexture);
        }
        
        // カメラ状態復元
        captureCamera.targetTexture = prevCamTarget;
        RenderTexture.active = prevActive;
    }
}

// パス検証メソッド
private bool IsValidOutputPath(string path)
{
    try
    {
        var fullPath = Path.GetFullPath(path);
        var projectPath = Path.GetFullPath(Application.dataPath + "/..");
        return fullPath.StartsWith(projectPath);
    }
    catch
    {
        return false;
    }
}
```