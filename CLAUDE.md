# CLAUDE.md

このファイルは、このリポジトリでコードを扱う際のClaude Code (claude.ai/code)へのガイダンスを提供します。

## プロジェクト概要

これはUnity開発者向けのモジュラー型エディターツール群を含むUnity Editor Extension Package (`com.harukakajita.exteditor`)です。プロジェクトはUnity 2022.3+を対象とし、Universal Render Pipeline (URP)を使用しています。

## ビルド・開発コマンド

**Unity操作:**
- Unity Editorでプロジェクトを開く（Unity 2022.3.22f1以降）
- アセンブリビルド: Unityの標準コンパイル機能を使用（Ctrl+Rでスクリプト再コンパイル）
- カスタムビルドスクリプトなし - Unityの組み込みシステムに依存

**ソリューションファイル:**
- メインソリューション: `ExtEditor.sln`（全ツールプロジェクトを含む）
- 個別の`.csproj`ファイルはUnityが各アセンブリに対して自動生成

## アーキテクチャ

### モジュラー設計
各ツールは以下を持つ完全に独立したモジュール:
- エディタープラットフォームのみを対象とするアセンブリ定義（`.asmdef`）
- 独立した機能と依存関係
- 二言語ドキュメント（英語/日本語READMEファイル）

### コアツール構造
- **Assets/ExtEditor/Editor/** - 10個の独立したエディターツールを含む:
  - `BatchRename/` - 高度なパターンマッチングによる一括アセット名変更
  - `CaptureWindow/` - スクリーンショット撮影（Game/Scene View、透明度サポート）
  - `MaterialPropertyCopier/` - マテリアル間でのシェーダープロパティコピー
  - `UberMaterialPropertyDrawer/` - グループ化システムを持つ高度なMaterial Inspector
  - `SelectionDataEditor/` - ScriptableObjectを使用したオブジェクト選択の保存/管理
  - `FolderMenuItemExtension/` - カスタムフォルダー構造テンプレート
  - その他4つの追加ユーティリティツール

### 主要コンポーネント
- **UberMaterialPropertyDrawer System**: 最も複雑なツールで以下の機能:
  - `UberDrawer.cs` - メインプロパティドローワーディスパッチャー
  - `BeginGroupDrawer.cs`/`EndGroupDrawer.cs` - 折りたたみ可能なプロパティグループ
  - カーブ、グラデーション、ベクターのカスタムドローワー
  - テクスチャベイク機能

## アセンブリ依存関係
- 全ツールがエディタープラットフォーム専用
- ツール間の相互依存なし
- 標準Unityパッケージ: URP 14.0.10、TextMeshPro 3.0.9、FBX Exporter 4.2.1

## ファイル場所
- パッケージマニフェスト: `Assets/ExtEditor/package.json`
- メインツール: `Assets/ExtEditor/Editor/[ツール名]/`
- 独立コンポーネント: `Assets/ExtEditor/UberMaterialPropertyDrawer/`

## 開発ノート
- 各ツールはUnity EditorWindow/MenuItemパターンに従う
- 全機能がエディター専用（ランタイムへの影響ゼロ）
- アセンブリ定義によりモジュラーコンパイルを保証
- 一貫性のためUnityのInspectorパターンとEditorGUIシステムを使用