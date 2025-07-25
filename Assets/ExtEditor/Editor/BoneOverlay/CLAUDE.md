# BoneOverlay Tool

このファイルは、BoneOverlayツールでの作業時にClaude Codeへのガイダンスを提供します。

## ツール概要

BoneOverlayツールは、Unity SceneViewでジョイント（ボーン）を可視化し、選択を簡単にするツールバーベースのエディター拡張です。Unity 2022.3以降のEditorToolbarDropdownToggle APIを使用し、Scene Viewのツールバーに統合されています。ジョイントは基本的にEmptyObjectで選択が難しいため、視覚的な表現とインタラクティブな選択機能を提供します。

## 主要機能

### 基本機能
- **EditorToolbarDropdownToggleベース**: Scene Viewツールバーに統合（アイコン付きトグル）
- **ボーン自動検出**: SkinnedMeshRenderer、Animator、名前パターンから推定（重複排除）
- **インタラクティブ可視化**: クリック可能な球体と親子間の線分
- **距離ベースフィルタリング**: 常時有効、ボーンとラベルで個別設定可能
- **選択状態による色変更**: 通常/選択/ホバーで異なる色表示
- **クリック機能**: シングルクリックで選択、Shift/Ctrlで複数選択
- **カラーカスタマイズ**: ドロップダウンメニュー内で直接色設定
- **ラベルインタラクション**: ラベルクリックでもオブジェクト選択可能
- **調整可能パラメータ**: 球体サイズ、線の太さ、ラベルサイズ、各種色設定

### 距離フィルタリング（常時有効）
- **ボーン表示距離**: 最大50m（デフォルト）、調整可能
- **ラベル表示距離**: 最大30m（デフォルト）、ボーンとは独立して設定
- **距離フェード**: 遠距離でのスムーズなフェードアウト
- **パフォーマンス最適化**: 視錐台カリングと組み合わせ

### 使用方法
1. Scene Viewのツールバーで`Overlays`メニューから`Bone Overlay Toolbar`を有効化
2. ツールバーに表示される`Bones`トグル（AvatarMaskアイコン）をオンにする
3. ドロップダウン矢印をクリックして設定メニューを開く
4. 各種設定を調整:
   - Bone Settings: ボーン表示距離、球体サイズ、線の太さ、ボーン関連の色
   - Label Settings: ラベル表示のオン/オフ、表示距離、サイズ、ラベル色
5. シーン内のボーンが自動的に可視化される
6. ボーンの選択方法:
   - **クリック選択**: ボーンの球体またはラベルをクリック
     - 通常クリック: 単独選択
     - Shift/Ctrl+クリック: 複数選択の追加/削除
   - **選択解除**: 空領域をクリック

### 特徴
- **深度順描画**: 重なったボーンで手前のボーンが優先される
- **精確な選択**: スクリーン位置と深度を考慮した選択ロジック

## 実装詳細

### ファイル構造
- **BoneOverlayDropdownToggle.cs**: メインツールバー要素（EditorToolbarDropdownToggle）
- **BoneOverlayToolbar.cs**: ツールバーオーバーレイ（ToolbarOverlay）
- **BoneOverlayState.cs**: 設定の永続化（EditorPrefs使用）
- **BoneDetector.cs**: ボーン検出ロジック
- **BoneOverlayRenderer.cs**: Gizmo描画とインタラクション
- **BoneOverlay_Legacy.cs**: 旧Overlayベース実装（無効化）

### アーキテクチャ特徴
- **ツールバー統合**: EditorToolbarDropdownToggleでScene Viewツールバーに統合
- **ドロップダウンUI**: GenericDropdownMenuを使用した設定メニュー
- **静的インスタンス管理**: ツールバー要素の生存期間管理
- **キャッシング**: フレームごとのボーン検出結果をキャッシュ
- **イベント駆動**: Selection変更やSceneGUIイベントに反応

### ボーン検出アルゴリズム
1. SkinnedMeshRendererのbonesプロパティから収集
2. Animatorコンポーネント（Humanoid/Generic）から収集
3. 名前パターンマッチング（"bone", "joint", "jnt"等）
4. 階層関係を分析して親ボーンも含める
5. 重複を排除して最終リストを生成

### 描画システム
- **Handles API使用**: DrawAAPolyLine（線）、SphereHandleCap（球体）
- **ラベル描画**: GUI.Button（Handles.BeginGUI/EndGUI内）でクリック可能なラベル
- **スクリーン座標変換**: 球体のピクセル半径を正確に計算（透視/正投影対応）
- **距離ベースLOD**: 近距離でフル詳細、遠距離で簡易表示
- **アルファブレンディング**: 距離によるフェード効果

## 技術的特徴

### パフォーマンス
- **最適化**: 視錐台カリング、距離カリング、キャッシング
- **スケーラビリティ**: 1000+ボーンでも動作可能
- **フレームレート維持**: 描画制限とLODシステム

### 拡張性
- **設定システム**: EditorPrefsとScriptableObjectの両対応
- **カラーカスタマイズ**: 各状態の色を個別設定可能
- **検出パターン**: ボーン名パターンをカスタマイズ可能

## 最新の変更内容

### 2025-01-05 更新
- **Distance Filter常時有効化**: トグルを削除し、距離フィルタリングは常にオン
- **独立した距離設定**: ボーンとラベルで別々の表示距離を設定可能
- **拡張された調整機能**:
  - Sphere Size: 球体の大きさを調整可能（1-100mm）
  - Line Width: 線分の太さを調整可能（0.1-10）
  - Label Size: ラベルのフォントサイズを調整可能（5-30pt）
  - Label Color: ラベルの色を個別に設定可能
- **改善されたUI**: カラーピッカーウィンドウを廃止し、ドロップダウン内で完結
- **ラベルインタラクション**: GUI.Buttonを使用してラベルをクリック可能に
- **UIレイアウト**: 色設定を各セクション（Bone Settings、Label Settings）に統合
- **球体クリック判定の改善**: スクリーン座標での正確な半径計算
- **ラベル位置調整**: 球体の上部に表示（camera.transform.up使用）
- **デフォルトラベル色**: 水色系（0.4f, 0.7f, 1f）に変更

### 2025-01-07 更新 - 複数選択バグ修正とドキュメント改善
- **バグ修正**: BoneOverlayRenderer.cs の HandleMouseInteraction メソッドで、選択解除時に `closestBone.gameObject.transform` を削除しようとしていたバグを修正
  - 修正前: `currentSelection.Remove(closestBone.gameObject.transform);`
  - 修正後: `currentSelection.Remove(closestBone.gameObject);`
- **選択状態の即時反映**: HandleMouseInteraction の最後に `SceneView.RepaintAll()` を追加
- **コード最適化**: Selection API を `Selection.transforms` から `Selection.gameObjects` に統一
- **デバッグ機能削除**: 簡潔性のためデバッグログコードを削除
- **ドキュメント全面改善**:
  - README: よりユーザーフレンドリーな説明、具体的なトラブルシューティング追加
  - QuickStart: 2分以内で使い始められる簡潔なガイドに改善
  - TechnicalSpecs: 最新のアーキテクチャとバグ修正内容を反映

### 2025-01-08 更新 - 深度順描画と選択優先度改善
- **深度順描画の実装**: ボーンをカメラからの距離でソート
  - 遠いボーンから順に描画（手前のボーンが上に描画される）
  - OrderByDescendingを使用した効率的なソート
- **選択優先度の改善**: 同じスクリーン位置のボーンで手前を優先
  - closestDepth変数を追加して深度を追跡
  - スクリーン距離での選択を廃止し、円内の最も手前のボーンを選択
- **矩形選択機能の削除**: Unity標準機能との競合回避のため削除

### 2025-01-09 更新 - パフォーマンス最適化とコード品質改善
- **パフォーマンス最適化**:
  - 視錐台平面の計算をフレーム単位でキャッシュ
  - GUIStyleをstaticフィールドで再利用（毎フレーム生成を回避）
  - ソート処理をカメラ移動時のみ実行
  - Repaint呼び出しをホバー状態変化時のみに制限
- **コード品質改善**:
  - labelColorのデフォルトアルファを1.0に修正（以前は0で透明）
  - 常にtrueだったEnableDistanceFilterプロパティを削除
  - 未使用のBoneOverlaySettingsクラスを削除
- **ユーザビリティ改善**:
  - 空クリックでの選択解除実装
  - ツールバーオーバーレイに適切な名前を設定

## 現在の課題

### Critical
- なし

### High
- **大量ボーン時の最適化**: 空間分割（Octree）の実装検討
- **Undo/Redo対応**: 選択操作の履歴管理

### Medium
- **プリセット機能**: 設定の保存/読み込み
- **ローカライゼーション**: 日本語/英語切り替え
- **アニメーション対応**: 再生中のボーン追従
- **DrawBonesメソッドの分割**: 155行の巨大メソッドを機能別に分割
- **設定保存のバッチ化**: プロパティ変更時の頻繁なEditorPrefs保存を最適化

## 開発ノート

### 追加予定機能
- ボーンチェーンごとの色分け
- ウェイトペイント表示モード
- ボーン階層の一括選択
- ボーン名の一括リネーム

### API使用例
```csharp
// 外部からBoneOverlayを制御する場合
bool isEnabled = BoneOverlayDropdownToggle.IsEnabled;

// ツールバーオーバーレイの取得
var toolbar = SceneView.lastActiveSceneView.overlays
    .FirstOrDefault(o => o is BoneOverlayToolbar) as BoneOverlayToolbar;
if (toolbar != null)
{
    // ツールバーの表示/非表示制御
    toolbar.displayed = true;
}
```

### ドロップダウンメニューの構造
```
Bones [▼]
├─ Bone Settings
│  ├─ Bone Distance: [50.0]m ──────
│  ├─ Sphere Size: [5.0]mm ────────
│  ├─ Line Width: [2.0] ───────────
│  ├─ Normal Color: [■] 
│  ├─ Selected Color: [■]
│  ├─ Hover Color: [■]
│  └─ Line Color: [■]
├─ Label Settings  
│  ├─ □ Show Labels
│  ├─ Label Distance: [30.0]m ─────
│  ├─ Label Size: [10.0]pt ────────
│  └─ Label Color: [■]
├─ [Reset to Defaults]
└─ Detected Bones: 42
```

### 注意事項
- Unity 2022.3以降でのみ動作（EditorToolbarDropdownToggle API依存）
- エディター専用機能（ランタイムでは使用不可）
- SceneViewでのみ動作（GameViewでは非表示）
- ツールバーオーバーレイはScene Viewのツールバーカスタマイズから有効化が必要