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

## 現状の課題

### 重要度: High（高）
- **例外処理**: アセット編集操作での清理なしの例外処理
  - **影響**: 例外発生時にアセットデータベースが不整合状態になる
  - **改善提案**: try-finallyブロックによる確実な清理処理

- **リソース管理**: TextureArrayオブジェクトの適切な管理不足
  - **影響**: メモリリークとGPUリソースの無駄遣い
  - **改善提案**: Disposableパターンと適切なリソース解放

- **検証ロジック**: OnValidateでの副作用と信頼性不足
  - **影響**: 不正な状態でのアセット操作と予期しない動作
  - **改善提案**: 副作用のない検証関数への分離

### 重要度: Medium（中）
- **パフォーマンス**: エラーチェックなしのGraphics.CopyTexture
  - **影響**: GPU操作失敗の検出困難とデバッグ問題
  - **改善提案**: 操作結果検証と適切なエラーハンドリング

- **ユーザーエクスペリエンス**: 大きなテクスチャ操作の進捗表示なし
  - **影響**: 長時間処理でエディターが固まったように見える
  - **改善提案**: 非同期処理と進捗バー表示

### 具体的な改善コード例

```csharp
[ContextMenu("Create or Update Texture2DArray")]
public void CreateOrUpdateTexture2DArrayContextMenu()
{
    // 事前検証
    if (!ValidateTextures())
    {
        EditorUtility.DisplayDialog("エラー", "テクスチャの検証に失敗しました", "OK");
        return;
    }
    
    // 処理時間見積もり
    int totalPixels = inputTextures.Sum(t => t != null ? t.width * t.height : 0);
    if (totalPixels > 4 * 1024 * 1024) // 4M pixels
    {
        if (!EditorUtility.DisplayDialog("処理時間警告", 
            "大きなテクスチャアレイの作成には時間がかかります。続行しますか？", 
            "続行", "キャンセル"))
            return;
    }
    
    AssetDatabase.StartAssetEditing();
    Texture2DArray textureArray = null;
    
    try
    {
        EditorUtility.DisplayProgressBar("テクスチャアレイ作成", "配列を初期化中...", 0f);
        
        textureArray = CreateOrUpdateTexture2DArray();
        if (textureArray == null)
        {
            EditorUtility.DisplayDialog("エラー", "テクスチャアレイの作成に失敗しました", "OK");
            return;
        }
        
        EditorUtility.DisplayProgressBar("テクスチャアレイ作成", "アセットを保存中...", 0.8f);
        
        var path = AssetDatabase.GetAssetPath(outputTexture);
        
        if (string.IsNullOrEmpty(path))
        {
            // 新規作成
            path = AssetDatabase.GetAssetPath(this).Replace(".asset", "_TexArray.asset");
            AssetDatabase.CreateAsset(textureArray, path);
            outputTexture = textureArray;
        }
        else
        {
            // 既存更新
            EditorUtility.CopySerialized(textureArray, outputTexture);
            DestroyImmediate(textureArray); // 一時オブジェクトを削除
        }
        
        EditorUtility.SetDirty(this);
        
        Debug.Log($"テクスチャアレイ作成完了: {inputTextures.Count}枚のテクスチャを含む配列");
        EditorUtility.DisplayDialog("完了", 
            $"テクスチャアレイを作成しました\nサイズ: {inputTextures[0].width}x{inputTextures[0].height}\n枚数: {inputTextures.Count}", 
            "OK");
    }
    catch (Exception ex)
    {
        Debug.LogError($"テクスチャアレイ作成エラー: {ex.Message}");
        EditorUtility.DisplayDialog("エラー", $"作成に失敗しました: {ex.Message}", "OK");
        
        // 一時オブジェクトのクリーンアップ
        if (textureArray != null)
            DestroyImmediate(textureArray);
    }
    finally
    {
        AssetDatabase.StopAssetEditing();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }
}

// 副作用のない検証メソッド
private bool ValidateTextures()
{
    validationMessage = "";
    
    if (inputTextures == null || inputTextures.Count == 0)
    {
        validationMessage = "入力テクスチャが設定されていません";
        return false;
    }
    
    var validTextures = inputTextures.Where(t => t != null).ToList();
    if (validTextures.Count == 0)
    {
        validationMessage = "有効なテクスチャがありません";
        return false;
    }
    
    var first = validTextures[0];
    foreach (var texture in validTextures.Skip(1))
    {
        if (texture.width != first.width || texture.height != first.height)
        {
            validationMessage = $"テクスチャサイズが一致しません: {texture.name} ({texture.width}x{texture.height}) vs {first.name} ({first.width}x{first.height})";
            return false;
        }
        
        if (texture.format != first.format)
        {
            validationMessage = $"テクスチャフォーマットが一致しません: {texture.name} ({texture.format}) vs {first.name} ({first.format})";
            return false;
        }
    }
    
    validationMessage = "検証OK";
    return true;
}

// OnValidate修正（副作用除去）
private void OnValidate()
{
    // UI更新のみ、実際の処理は行わない
    isValid = ValidateTextures();
}

// 改善されたテクスチャアレイ作成
private Texture2DArray CreateOrUpdateTexture2DArray()
{
    var validTextures = inputTextures.Where(t => t != null).ToArray();
    if (validTextures.Length == 0) return null;
    
    var first = validTextures[0];
    var textureArray = new Texture2DArray(
        first.width, 
        first.height, 
        validTextures.Length, 
        first.format, 
        false);
    
    textureArray.name = $"{name}_TextureArray";
    
    try
    {
        for (int i = 0; i < validTextures.Length; i++)
        {
            var texture = validTextures[i];
            
            EditorUtility.DisplayProgressBar(
                "テクスチャアレイ作成", 
                $"テクスチャをコピー中: {texture.name} ({i + 1}/{validTextures.Length})", 
                (float)i / validTextures.Length);
            
            // エラーチェック付きコピー
            try
            {
                Graphics.CopyTexture(texture, 0, textureArray, i);
            }
            catch (Exception ex)
            {
                Debug.LogError($"テクスチャコピーエラー {texture.name}: {ex.Message}");
                throw;
            }
        }
        
        textureArray.Apply(false);
        return textureArray;
    }
    catch
    {
        // エラー時のクリーンアップ
        if (textureArray != null)
            DestroyImmediate(textureArray);
        throw;
    }
}

// テクスチャメモリ使用量計算
private long GetEstimatedMemoryUsage()
{
    if (inputTextures.Count == 0) return 0;
    
    var first = inputTextures.FirstOrDefault(t => t != null);
    if (first == null) return 0;
    
    long pixelCount = first.width * first.height;
    int bytesPerPixel = GetBytesPerPixel(first.format);
    
    return pixelCount * bytesPerPixel * inputTextures.Count;
}

private int GetBytesPerPixel(TextureFormat format)
{
    switch (format)
    {
        case TextureFormat.RGBA32: return 4;
        case TextureFormat.RGB24: return 3;
        case TextureFormat.ARGB32: return 4;
        case TextureFormat.DXT1: return 1; // 圧縮
        case TextureFormat.DXT5: return 1; // 圧縮
        default: return 4; // デフォルト推定
    }
}
```