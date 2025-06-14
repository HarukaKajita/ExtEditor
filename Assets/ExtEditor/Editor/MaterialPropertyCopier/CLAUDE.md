# MaterialPropertyCopier Tool

このファイルは、MaterialPropertyCopierツールでの作業時にClaude Codeへのガイダンスを提供します。

## ツール概要

MaterialPropertyCopierツールは、マテリアル間でシェーダープロパティを選択的にコピーするエディター拡張です。複数のターゲットマテリアルに対して、特定のプロパティのみを転送できます。

## 主要機能

### 基本機能
- **マテリアルプロパティ転送**: ソースマテリアルから複数のターゲットマテリアルへプロパティをコピー
- **選択的コピー**: チェックボックスでコピーするプロパティを個別選択
- **プロパティタイプサポート**: Color、Vector、Float、Range、Textureプロパティに対応
- **複数ターゲット**: 複数の宛先マテリアルをサポート
- **ドラッグ&ドロップ**: マテリアルを直接ターゲットリストにドラッグ可能
- **タイプセーフティ**: ソースとターゲット間でプロパティタイプの一致を検証

### 使用方法
1. **ツール → Material Property Copier**メニューからアクセス
2. ObjectFieldでソースマテリアルを設定
3. チェックボックスで希望プロパティを選択（Select All/Select Noneボタン利用可能）
4. "Copy Selected Properties from Source"をクリック
5. ターゲットマテリアルを追加:
   - Projectでマテリアルを選択し"Add Selected Materials"をクリック
   - マテリアルをドラッグ&ドロップエリアにドラッグ
   - ターゲットリストの個別ObjectFieldを使用
6. "Paste Copied Properties to Target(s)"をクリック

## 実装詳細

### 主要クラス
- **MaterialPropertyCopier**: EditorWindowの単一クラス実装
- **全機能統合**: ソース選択、プロパティコピー、ターゲット管理を一つのクラスで実装

### プロパティ管理
- **プロパティ検査**: `ShaderUtil`を使用してシェーダープロパティを検査
- **タイプ判定**: プロパティタイプの自動判定と互換性チェック
- **辞書ベース**: コピーしたプロパティをタイプ情報付きで辞書に保存

### UI機能
- **ドラッグ&ドロップ**: `DragAndDrop` APIを使用した直感的なマテリアル追加
- **アルファベット順ソート**: プロパティは表示名でアルファベット順にソート
- **Undo対応**: プロパティ貼り付け操作でUndo/Redoサポート

## 技術的特徴

### プロパティタイプサポート
- **Color**: カラープロパティの完全サポート
- **Vector**: Vector4プロパティの転送
- **Float/Range**: 数値プロパティの処理
- **Texture**: テクスチャプロパティの参照転送

### エラーハンドリング
- **タイプ不一致**: プロパティタイプが一致しない場合の詳細ログ
- **null参照**: マテリアルやプロパティのnullチェック
- **無効プロパティ**: 存在しないプロパティへのアクセス保護

### バッチ処理
- **複数ターゲット**: 一度に複数のマテリアルに対してプロパティを適用
- **効率的転送**: プロパティタイプごとの最適化された転送メソッド
- **選択的適用**: 選択されたプロパティのみを処理

## 開発ノート

### データ構造
```csharp
Dictionary<string, object> copiedProperties; // プロパティ名と値のペア
Dictionary<string, ShaderPropertyType> propertyTypes; // プロパティタイプ情報
List<Material> targetMaterials; // ターゲットマテリアルリスト
```

### 主要メソッド
- **CopySelectedProperties()**: 選択プロパティをソースからコピー
- **PasteCopiedProperties()**: コピーしたプロパティをターゲットに適用
- **ValidatePropertyType()**: プロパティタイプの互換性チェック

### 拡張ポイント
- **新しいプロパティタイプ**: `ShaderPropertyType`enumに新しいタイプを追加
- **カスタムUI**: プロパティ表示のカスタマイズは`DrawPropertyList()`メソッドで実装
- **バッチ操作**: 大量マテリアル処理用のプログレスバー追加可能

### 注意点
- **シェーダー互換性**: 同じシェーダーまたは互換性のあるシェーダー間でのみ使用推奨
- **プロパティ名**: プロパティ名の完全一致が必要
- **アセットvsプロシージャル**: アセットベースとプロシージャルマテリアル両方に対応

## 現状の課題

### 重要度: High（高）
- **型安全性の不備**: 無効になったテクスチャ参照の検証なし
  - **影響**: 削除されたテクスチャを参照してエラーや予期しない動作
  - **改善提案**: テクスチャ有効性チェックとAssetDatabase.Contains()による検証

- **Undo記録の不整合**: 修正されたマテリアルのみがUndo記録される
  - **影響**: 一部のマテリアルのみUndo可能で一貫性のない状態
  - **改善提案**: 全ターゲットマテリアルの事前Undo記録

- **パフォーマンス**: 貼り付け操作時の非効率なプロパティタイプ検索
  - **影響**: 大量プロパティ処理時の動作遅延
  - **改善提案**: プロパティタイプのキャッシュ機能実装

### 重要度: Medium（中）
- **UI応答性**: 大量マテリアル操作時の進捗表示なし
  - **影響**: 処理中にエディターが固まったように見える
  - **改善提案**: `EditorUtility.DisplayProgressBar()`による進捗表示

- **メモリ使用量**: コピーしたプロパティが無期限に保存される
  - **影響**: 長時間使用でメモリ使用量の増加
  - **改善提案**: プロパティデータの定期的な清理機能

- **エラー回復**: 部分的な失敗で一部マテリアルのみ変更される
  - **影響**: 予期しない混在状態で一貫性が失われる
  - **改善提案**: トランザクション的な処理と全体ロールバック機能

### 具体的な改善コード例

```csharp
// 型安全性向上
case ShaderUtil.ShaderPropertyType.TexEnv:
    Texture tex = sourceMaterial.GetTexture(propInfo.propertyName);
    // テクスチャ有効性検証
    if (tex != null && AssetDatabase.Contains(tex))
    {
        value = tex;
    }
    else if (tex != null)
    {
        Debug.LogWarning($"テクスチャ {tex.name} は有効なアセットではありません。スキップします。");
        continue;
    }
    break;

// Undo記録の改善
void PastePropertiesToTargets()
{
    // 全ターゲットマテリアルを事前記録
    var validTargets = targetMaterials.Where(m => m != null).ToArray();
    if (validTargets.Length == 0)
    {
        EditorUtility.DisplayDialog("警告", "有効なターゲットマテリアルがありません", "OK");
        return;
    }
    
    Undo.RecordObjects(validTargets, "Paste Material Properties");
    
    // 進捗表示付きで処理
    for (int i = 0; i < validTargets.Length; i++)
    {
        var targetMat = validTargets[i];
        
        EditorUtility.DisplayProgressBar(
            "プロパティ貼り付け中", 
            $"{targetMat.name} ({i + 1}/{validTargets.Length})", 
            (float)i / validTargets.Length);
            
        try
        {
            PastePropertiesToMaterial(targetMat);
        }
        catch (Exception ex)
        {
            Debug.LogError($"マテリアル {targetMat.name} への貼り付けに失敗: {ex.Message}");
        }
    }
    
    EditorUtility.ClearProgressBar();
}

// プロパティタイプキャッシュ
private Dictionary<string, ShaderUtil.ShaderPropertyType> _propertyTypeCache = 
    new Dictionary<string, ShaderUtil.ShaderPropertyType>();

private ShaderUtil.ShaderPropertyType GetCachedPropertyType(Material material, string propertyName)
{
    var key = $"{material.shader.name}_{propertyName}";
    if (!_propertyTypeCache.TryGetValue(key, out var type))
    {
        // 実際のタイプ取得処理
        type = GetPropertyType(material, propertyName);
        _propertyTypeCache[key] = type;
    }
    return type;
}
```