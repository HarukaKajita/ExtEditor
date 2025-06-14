# UberMaterialPropertyDrawer Tool

このファイルは、UberMaterialPropertyDrawerツールでの作業時にClaude Codeへのガイダンスを提供します。

## ツール概要

UberMaterialPropertyDrawerツールは、高度なマテリアルプロパティUI システムを提供するエディター拡張です。グループ化、カスタムコントロール、プロシージャルテクスチャ生成を含む洗練されたマテリアルインスペクター拡張を実現します。

## 主要機能

### 基本機能
- **プロパティグループ化**: マテリアルプロパティを折りたたみ可能なグループに整理
- **カスタム Enum ドローワー**: グループサポート付きの拡張enumプロパティドローワー
- **プロシージャルテクスチャ**: 組み込みグラデーションとカーブテクスチャ生成
- **ベクタープロパティドローワー**: カスタムVector2・Vector3プロパティコントロール
- **ネストグループ**: 階層的プロパティ整理のサポート
- **トグルグループ**: オン・オフ切り替え可能なグループ

### シェーダーでの使用方法
シェーダープロパティでカスタム属性を使用:
```hlsl
[Uber(GroupName)] _Property ("Display Name", Float) = 0
[Uber(GroupName, BeginGroup)] _GroupStart ("Group Name", Float) = 0
[Uber(GroupName, EndGroup)] _GroupEnd ("", Float) = 0
[Uber(GroupName, Enum, EnumTypeName)] _EnumProp ("Enum", Float) = 0
[Uber(GroupName, GradientTexture)] _GradTex ("Gradient", 2D) = "white" {}
[Uber(GroupName, CurveTexture)] _CurveTex ("Curve", 2D) = "white" {}
[Uber(GroupName, Vector2)] _Vec2 ("Vector2", Vector) = (0,0,0,0)
[Uber(GroupName, Vector3)] _Vec3 ("Vector3", Vector) = (0,0,0,0)
[Uber(GroupName, BeginToggleGroup)] _ToggleGroup ("Toggle Group", Float) = 0
[Uber(Init)] _Init ("", Float) = 0
```

## 実装詳細

### ファイル構造
- **UberDrawer.cs**: メインドローワーと委譲システム
- **BeginGroupDrawer.cs / EndGroupDrawer.cs**: グループ境界管理
- **BeginToggleGroupDrawer.cs**: トグル有効グループ
- **UberEnumDrawer.cs**: 型リフレクション付き拡張enum サポート
- **GradientTextureDrawer.cs / CurveTextureDrawer.cs**: プロシージャルテクスチャ生成
- **Vector2Drawer.cs / Vector3Drawer.cs**: カスタムベクタープロパティコントロール

### 中核クラス詳細

#### UberDrawer (メインディスパッチャー)
- **委譲システム**: 属性パラメータに基づく専用ドローワーへの委譲
- **状態管理**: 静的コレクションを使用したグループ状態とネスト管理
- **初期化処理**: `[Uber(Init)]` による状態リセット機能

#### グループ管理システム
- **BeginGroupDrawer**: フォールドアウトグループの開始処理
- **EndGroupDrawer**: グループ終了とインデント復元
- **BeginToggleGroupDrawer**: 切り替え可能グループの実装
- **ネストサポート**: スタックベースのネストグループ管理

#### プロシージャルテクスチャ
- **GradientTextureDrawer**: Gradient → Texture2D 変換とベイク
- **CurveTextureDrawer**: AnimationCurve → Texture2D 変換とベイク
- **サブアセット統合**: マテリアルのサブアセットとしてテクスチャ保存
- **形式オプション**: サイズと精度パラメータのサポート

## 技術的特徴

### 状態管理システム
```csharp
static Dictionary<string, bool> groupExpandedStates; // グループ展開状態
static Stack<int> indentLevelStack; // インデントレベルスタック  
static Dictionary<string, bool> groupToggleStates; // トグルグループ状態
```

### 型安全な Enum サポート
- **実行時型発見**: リフレクションによるEnum型の動的発見
- **名前空間サポート**: 完全修飾型名での型解決
- **エラーハンドリング**: 型が見つからない場合の適切なフォールバック

### プロシージャルテクスチャ統合
- **AssetDatabase.AddObjectToAsset()**: サブアセットとしての保存
- **自動更新**: プロパティ変更時の自動テクスチャ再生成  
- **メモリ効率**: 必要時のみテクスチャ作成

## 設定詳細

### 属性パラメータ
- **グループ名**: 第1パラメータでグループを指定
- **機能タイプ**: 第2パラメータで機能を指定（BeginGroup, EndGroup, Enum, etc.）
- **追加パラメータ**: 型名やサイズなどの追加設定

### グループ初期化
- **[Uber(Init)]**: マテリアル単位でのグループ状態リセット
- **静的状態**: 複数マテリアル間での状態共有
- **セッション永続**: エディターセッション中の状態保持

### テクスチャ生成設定
- **デフォルトサイズ**: 256x256ピクセル
- **形式**: RGBA32（アルファサポート）
- **保存場所**: マテリアルアセットのサブアセット

## 開発ノート

### 拡張ポイント
- **新しいドローワータイプ**: UberDrawer.OnGUI()に新しい分岐追加
- **カスタムグループタイプ**: 新しいグループドローワーの実装
- **テクスチャ形式**: 新しいプロシージャルテクスチャタイプの追加

### パフォーマンス考慮
- **静的コレクション**: グループ状態のエディター間共有
- **遅延テクスチャ生成**: 必要時のみテクスチャ作成
- **効率的な再描画**: 変更時のみUI更新

### エラーハンドリング
- **型解決失敗**: 存在しないEnum型でのフォールバック
- **循環参照**: グループネストでの循環検出
- **アセット作成エラー**: テクスチャ生成失敗時の適切な処理

### 互換性
- **Unity バージョン**: Unity 2022.3+ での動作確認
- **シェーダー統合**: カスタム属性のシェーダー互換性
- **マテリアル形式**: 標準マテリアルとの完全互換性

### 使用場面
- **複雑なシェーダー**: 大量プロパティを持つシェーダーのUI整理
- **アーティストワークフロー**: 直感的なマテリアル編集インターフェース
- **プロシージャル生成**: リアルタイムテクスチャ生成ワークフロー

### 制限事項
- **エディター専用**: ランタイムでは動作しない
- **マテリアル依存**: マテリアルアセットが必要
- **静的状態**: エディター再起動でグループ状態リセット

### ベストプラクティス
- **グループ設計**: 論理的なプロパティグループ化
- **命名規則**: 明確なグループ名とプロパティ名
- **初期化配置**: マテリアル上部での[Uber(Init)]配置

### 今後の機能
- **永続状態**: グループ状態の永続化
- **テーマサポート**: UI テーマのカスタマイズ
- **プリセット**: マテリアル設定プリセット機能

### トラブルシューティング
- **グループ状態異常**: [Uber(Init)]による状態リセット
- **テクスチャ生成失敗**: アセット権限とディスク容量確認
- **型解決エラー**: Enum型名とアセンブリ参照確認