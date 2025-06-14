# FolderMenuItemExtension Tool

このファイルは、FolderMenuItemExtensionツールでの作業時にClaude Codeへのガイダンスを提供します。

## ツール概要

FolderMenuItemExtensionツールは、事前定義されたフォルダー構造を作成するカスタム右クリックメニューアイテムを生成するエディター拡張です。`.fmi`ファイルを使用してフォルダー作成テンプレートを定義します。

## 主要機能

### 基本機能
- **カスタムフォルダー作成**: 事前定義されたフォルダー構造の生成
- **テンプレートベースシステム**: `.fmi`ファイルによるフォルダー作成テンプレートの定義
- **動的メニュー生成**: 自動的にC#スクリプトを生成してメニューアイテムを追加
- **優先度制御**: `.fmi`ファイルの最初の行でメニュー優先度を設定
- **柔軟なフォルダーリスト**: テキスト形式での任意フォルダー構造定義

### 使用方法
1. FolderMenuItemPresetフォルダーに`.fmi`拡張子のテキストファイルを作成
2. 最初の行: 優先度番号（小さい数字ほどメニュー上位に表示）
3. 以降の行: 作成するフォルダー名
4. システムが自動的にC#スクリプトを生成してメニューアイテムを作成
5. Projectウィンドウのフォルダー右クリック → Assets → Create → Folders → [メニュー名]

## 実装詳細

### ファイル構造
- **CreateFoldersMenuItem.cs**: 生成されるメニューアイテムのテンプレート
- **FolderMenuItemImporter.cs**: `.fmi`ファイルを処理するカスタムインポーター

### ファイル形式
```
1
Textures
Materials
Models
Scripts
```

### 技術的特徴
- **ScriptedImporter**: `.fmi`ファイル用のカスタムインポーター
- **コード生成**: テンプレート文字列置換によるC#スクリプト生成
- **自動リフレッシュ**: スクリプト生成後のAssetDatabase自動更新

## 設定詳細

### メニュー設定
- **生成メニューパス**: `Assets/Create/Folders/[filename]`
- **優先度オフセット**: 優先度に21を加算してUnityデフォルト"Folder"オプションの下に配置
- **対象**: Project内のフォルダー上での右クリックメニュー

### テンプレートシステム
- **プレースホルダー**:
  - `FOLDERMENUITEMCLASSNAME`: 生成クラス名
  - `FOLDERMENUITEMNAME`: メニュー表示名
  - `FOLDERLIST`: フォルダー作成コード
  - `PRIORITY`: メニュー優先度

### 生成ファイル
- **出力場所**: インポーターと同じディレクトリ
- **ファイル名**: `[fmiファイル名].cs`
- **内容**: MenuItemアトリビュート付きの静的メソッド

## 開発ノート

### インポーター動作
1. `.fmi`ファイルの内容を読み込み
2. 最初の行を優先度として解析
3. 残りの行をフォルダー名として処理
4. テンプレートファイルから文字列置換でC#コード生成
5. 生成されたスクリプトをAssetDatabaseに追加

### エラーハンドリング
- **ファイル読み込みエラー**: 不正な`.fmi`ファイル形式の処理
- **優先度解析**: 数値以外の優先度行の処理
- **空ファイル**: 内容のない`.fmi`ファイルの処理

### 拡張性
- **新しいプレースホルダー**: テンプレートシステムに新機能追加可能
- **カスタムメニューパス**: メニュー配置場所のカスタマイズ
- **追加メタデータ**: `.fmi`ファイルへの追加情報統合

### 制限事項
- **静的生成**: 実行時の動的フォルダー作成は不可
- **単一階層**: ネストしたフォルダー構造は個別のフォルダー作成として処理
- **Unity依存**: Unityエディター環境でのみ動作

### ベストプラクティス
- **命名規則**: `.fmi`ファイル名がメニュー項目名になるため、わかりやすい名前を使用
- **優先度設定**: よく使用するテンプレートには小さい優先度番号を設定
- **フォルダー命名**: 作成されるフォルダー名にはUnityの命名規則に従う

## 現状の課題

### 重要度: High（高）
- **ファイルシステム競合状態**: 適切な同期なしでのファイル操作
  - **影響**: 同時アクセス時のファイル破損やアクセスエラー
  - **改善提案**: ファイルロック機構またはアトミック操作の実装

- **リソースリーク**: StreamWriterがすべてのコードパスで破棄されない
  - **影響**: ファイルハンドルリークによるシステムリソース不足
  - **改善提案**: using文による確実なリソース解放

- **セキュリティ**: ファイルパス検証不足でディレクトリトラバーサル攻撃の可能性
  - **影響**: 意図しないディレクトリへのファイル作成リスク
  - **改善提案**: パス正規化と安全なディレクトリ制限

### 重要度: Medium（中）
- **エラーハンドリング**: ファイル操作失敗時の静的失敗
  - **影響**: 問題発生時にユーザーが気づかない
  - **改善提案**: 明示的なエラー表示とログ機能

- **コード生成検証**: 生成されたコードの検証やエラーチェックなし
  - **影響**: 不正なC#コードが生成されてコンパイルエラー
  - **改善提案**: 生成コードの構文チェックと検証機能

### 具体的な改善コード例

```csharp
// リソースリークとエラーハンドリング修正
public override void OnImportAsset(AssetImportContext ctx)
{
    try
    {
        string[] lines = File.ReadAllLines(ctx.assetPath);
        if (lines.Length == 0)
        {
            Debug.LogError($"空のファイル: {ctx.assetPath}");
            return;
        }

        // 優先度解析
        if (!int.TryParse(lines[0], out int priority))
        {
            Debug.LogError($"無効な優先度: {lines[0]} in {ctx.assetPath}");
            return;
        }

        // パス検証
        string scriptFilePath = Path.Combine(Path.GetDirectoryName(ctx.assetPath), 
                                           Path.GetFileNameWithoutExtension(ctx.assetPath) + ".cs");
        
        // セキュリティ: パス正規化と検証
        string fullScriptPath = Path.GetFullPath(scriptFilePath);
        string assetsPath = Path.GetFullPath(Application.dataPath);
        if (!fullScriptPath.StartsWith(assetsPath))
        {
            Debug.LogError($"無効なスクリプトパス: {fullScriptPath}");
            return;
        }

        // テンプレート処理...
        string templateText = GenerateTemplate(lines, priority);

        // 生成コード検証
        if (!ValidateGeneratedCode(templateText))
        {
            Debug.LogError("生成されたコードが無効です");
            return;
        }

        // ファイル書き込み（原子的操作）
        string tempPath = scriptFilePath + ".tmp";
        using (var sw = File.CreateText(tempPath))
        {
            sw.Write(templateText);
        }

        // 原子的ファイル置換
        if (File.Exists(scriptFilePath))
            File.Delete(scriptFilePath);
        File.Move(tempPath, scriptFilePath);

        AssetDatabase.ImportAsset(GetProjectRelativePath(scriptFilePath));
    }
    catch (Exception ex)
    {
        Debug.LogError($"FMIファイル処理エラー {ctx.assetPath}: {ex.Message}");
        EditorUtility.DisplayDialog("エラー", $"フォルダーメニュー生成に失敗しました: {ex.Message}", "OK");
    }
}

// 生成コード検証
private bool ValidateGeneratedCode(string code)
{
    // 基本的な構文チェック
    if (string.IsNullOrEmpty(code)) return false;
    if (!code.Contains("class ")) return false;
    if (!code.Contains("[MenuItem(")) return false;
    
    // より詳細な検証可能
    return true;
}

// プロジェクト相対パス取得
private string GetProjectRelativePath(string absolutePath)
{
    string assetsPath = Application.dataPath;
    if (absolutePath.StartsWith(assetsPath))
        return "Assets" + absolutePath.Substring(assetsPath.Length);
    return absolutePath;
}

// ファイルロック付き安全な書き込み
private void SafeWriteFile(string filePath, string content)
{
    const int maxRetries = 3;
    const int retryDelayMs = 100;
    
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            using var sw = new StreamWriter(fs);
            sw.Write(content);
            return; // 成功
        }
        catch (IOException) when (i < maxRetries - 1)
        {
            Thread.Sleep(retryDelayMs);
        }
    }
    
    throw new IOException($"ファイル書き込みに失敗: {filePath}");
}
```