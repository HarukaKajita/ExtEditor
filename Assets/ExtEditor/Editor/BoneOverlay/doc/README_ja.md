# BoneOverlay - ボーン可視化・選択支援ツール

## 概要

BoneOverlayは、Unity Scene Viewでボーン（ジョイント）を視覚的に表示し、選択を簡単にするエディター拡張ツールです。3Dモデルのリギングやアニメーション作業において、通常は見えにくいボーンオブジェクトを分かりやすく可視化し、効率的な作業を支援します。

### 主な特徴

- 🎯 **直感的なボーン選択** - ディスク型マーカーとラベルをクリックするだけで簡単にボーンを選択
- 👁️ **スマートな自動検出** - SkinnedMeshRenderer、Animator、名前パターンから自動的にボーンを検出
- 🎨 **カスタマイズ可能な表示** - 色、サイズ、距離など細かく調整可能
- ⚡ **高パフォーマンス** - 1000個以上のボーンでも軽快に動作する最適化設計
- 🔧 **Unity標準UI統合** - Scene Viewのツールバーにシームレスに統合
- 🐛 **複数選択対応** - 修正・改善された複数選択機能（v1.0.1）

## 動作環境

- Unity 2022.3以降
- 全レンダーパイプライン対応（Built-in、URP、HDRP）
- エディター専用（ランタイムでは動作しません）

## インストール方法

1. Unity Package Managerを開く
2. 「Add package from disk」を選択
3. `package.json`ファイルを選択してインポート

または、Assetsフォルダに直接コピーしてください。

## 使い方

### 基本的な使い方

1. **ツールバーの有効化**
   - Scene Viewの右上にある「⋮」メニュー（3つの点）をクリック
   - 「Overlays」→「Bone Overlay Toolbar」にチェックを入れる
   - Scene Viewにツールバーが表示されます

2. **ボーン表示の有効化**
   - Scene Viewのツールバーに表示される「Bones」ボタン（アバターマスクアイコン）をクリック
   - アイコンが青色にハイライトされ、ボーンの可視化が開始されます
   - ボーンが色付きのディスクマーカーとして表示されます

3. **ボーンの選択**
   - **単独選択**: ディスクマーカーまたはラベルをクリック
   - **追加選択**: Shiftキーを押しながらクリック
   - **選択の切り替え**: Ctrl/Cmdキーを押しながらクリック
   - 選択されたボーンは異なる色（デフォルト：黄色）で表示されます

### 設定のカスタマイズ

「Bones」ボタンの右側にあるドロップダウン矢印「▼」をクリックすると、詳細設定メニューにアクセスできます。

#### Bone Settings（ボーン設定）

- **Bone Distance**（1-100m）- ボーンマーカーを表示する最大距離
  - デフォルト：50m（ほとんどのシーンに適切）
  - 複雑なシーンでは距離を短くしてパフォーマンス向上
  
- **Sphere Size**（1-100mm）- ボーンディスクマーカーのサイズ
  - デフォルト：5mm
  - 大きなシーンでは視認性向上のため大きくする
  
- **Line Width**（0.1-10）- 親子接続線の太さ
  - デフォルト：2.0
  - 太い線は見やすいが、表示が混雑する可能性あり

- **カラー設定** - 各状態の色をカスタマイズ
  - **Normal Color**：デフォルトのディスク色（緑）
  - **Selected Color**：ボーン選択時の色（黄）
  - **Hover Color**：ホバー時の色（シアン）
  - **Line Color**：親子接続線の色（グレー）

#### Label Settings（ラベル設定）

- **Show Labels** - ボーン名の表示オン/オフ切り替え
- **Label Distance**（1-100m）- ラベルを表示する最大距離
  - デフォルト：30m（見やすさのためボーン距離より短め）
  
- **Label Size**（5-30pt）- ボーン名のフォントサイズ
  - デフォルト：10pt
  
- **Label Color** - ラベルのテキスト色
  - デフォルト：水色（良好なコントラスト）

#### クイックアクション

- **Reset to Defaults** - すべての設定をデフォルト値に戻す
- **Detected Bones** - 現在検出されているボーン数を表示

## 活用シーン

### VRChatアバター制作

- アバターのボーン構造を確認しながら調整
- PhysBoneの設定時にボーンを簡単に選択
- 表情アニメーションの作成時に顔のボーンを視覚的に確認

[画像: VRChatアバターでの使用例]

### 3Dキャラクターアニメーション

- アニメーション作成時のボーン選択を効率化
- IKの設定時にボーンチェーンを視覚的に確認
- リギングの問題を素早く発見

[画像: アニメーション作業での使用例]

### モデルのデバッグ

- インポートしたモデルのボーン構造を確認
- 不要なボーンや名前の問題を発見
- 階層構造の理解を助ける

## 高度な使い方

### 距離フィルタリングの活用

大規模なシーンでは、距離フィルタリングを調整することで必要なボーンのみを表示できます：

- 近くの詳細な作業時は距離を短く設定
- 全体を俯瞰する時は距離を長く設定
- ラベルとボーンで別々の距離を設定可能

[画像: 距離フィルタリングの効果を示す比較画像]

### カラーカスタマイズ

作業内容に応じて色をカスタマイズすることで、視認性を向上できます：

- 暗いシーンでは明るい色に変更
- 特定のボーンを目立たせたい場合は鮮やかな色に
- 透明度も調整可能

### パフォーマンスの最適化

多数のボーンがある場合：

- 必要最小限の距離に設定
- ラベル表示をオフにして軽量化
- 作業中のエリアのみフォーカス

## トラブルシューティング

### ボーンが表示されない

1. **トグル状態を確認**：「Bones」ボタンがハイライト（青色）されているか確認
2. **距離を確認**：デフォルトは50m - 大きなシーンでは距離を増やす
3. **コンポーネントを確認**：ボーンは以下から検出されます
   - SkinnedMeshRendererコンポーネント（メッシュボーン）
   - Animatorコンポーネント（リグボーン）
   - ボーン関連の名前を持つオブジェクト（bone、joint、jnt等）
4. **ビューモード**：Scene Viewにいることを確認（Game Viewでは動作しません）

### 選択の問題

1. **複数選択が機能しない**：
   - クリック前にShift/Ctrlを押していることを確認
   - v1.0.1以降に更新して修正された複数選択機能を使用
   
2. **ボーンをクリックできない**：
   - InspectorでGameObjectがロックされていないか確認
   - ディスクが小さい場合はラベルをクリック
   - 設定でSphere Sizeを大きくしてクリックしやすく

3. **選択色が間違っている**：
   - Scene Viewを再描画（カメラを少し動かす）
   - ツールをオフ→オンで切り替え

### パフォーマンスの問題

1. **ボーンが多すぎて重い**（100個以上）：
   - 「Bone Distance」を10-20mに減らす
   - 「Show Labels」をオフにする
   - カメラで特定エリアにフォーカス

2. **Scene Viewが遅い**：
   - カメラビュー外のボーンは自動的にカリングされます
   - 使用していないGameObjectを非表示に
   - 距離フェードがパフォーマンスを助けます

### デバッグモード

開発者向けに問題が発生した場合、デバッグログを有効化：
1. Player Settings → Scripting Define Symbolsに`BONE_OVERLAY_DEBUG`を追加
2. コンソールで選択操作のログを確認

## よくある質問

**Q: GameViewでも表示されますか？**
A: いいえ、Scene View専用のツールです。

**Q: ランタイムで使用できますか？**
A: エディター専用機能のため、ビルドには含まれません。

**Q: 特定のボーンだけを表示できますか？**
A: 現在のバージョンでは自動検出されたすべてのボーンが表示されます。

**Q: カスタムボーンの検出パターンを追加できますか？**
A: 現在のバージョンでは固定のパターンのみですが、将来的に対応予定です。

## 更新履歴

### Version 1.0.1 (2025-01-07)
- 複数選択時の選択解除が機能しないバグを修正
- Hierarchy ビューとの選択状態の同期を改善
- 選択変更時の即時視覚フィードバックを追加
- `BONE_OVERLAY_DEBUG`シンボルによるデバッグログ機能を追加

### Version 1.0.0 (2025-01-05)
- 初回リリース
- EditorToolbarDropdownToggleベースの実装
- ボーン/ラベル個別の距離フィルタリング
- 包括的なカラーカスタマイズ機能
- インタラクティブなラベルクリック対応
- 視認性向上のためのディスクベースボーン表示

## 技術仕様

- **レンダリング**：Unity Handles APIによるディスクマーカー
- **パフォーマンス**：視錐台カリングと距離ベースLODで最適化
- **互換性**：すべてのUnityレンダーパイプラインに対応
- **メモリ使用量**：100ボーンあたり約10KB

## サポート

ご質問、不具合報告、機能リクエストは以下へ：

- GitHub Issues: [リポジトリで問題を報告]
- Unity Forum: [ディスカッションスレッド]
- Email: support@exteditor.com

## ライセンス

本ツールはMITライセンスで提供されています。
詳細はLICENSEファイルをご確認ください。

---

© 2025 ExtEditor Tools. All rights reserved.