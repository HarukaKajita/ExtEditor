# CaptureWindow - Unity高機能スクリーンショットツール

## 商品説明

**Unityエディターで最も高機能なスクリーンショット撮影ツール**

CaptureWindowは、Unity開発者のためのプロフェッショナルなスクリーンショット撮影エディター拡張です。GameView、SceneView、RenderTextureの撮影に対応し、柔軟なファイル命名、多言語対応、外部スクリプトからの操作など、開発現場で求められる全ての機能を搭載しています。

### ✨ 主な特徴
- 🎯 **3種類の撮影ソース対応**：GameView / SceneView / RenderTexture
- 🌐 **日本語・英語対応**：完全な2言語インターフェース
- 🎨 **高度なファイル命名**：12種類のパターン置換でカスタム命名
- 🔧 **外部API完備**：スクリプトからの完全制御が可能
- 💎 **透明背景対応**：アルファチャンネル付きPNG出力
- 🚀 **プロダクション対応**：バッチ処理・自動化ワークフロー対応

---

## 導入方法

### 必要環境
- **Unity 2022.3.22f1以降**
- **Universal Render Pipeline (URP) 14.0.10以降**（推奨）

### インストール手順

1. **UnityPackageのインポート**
   ```
   Assets → Import Package → Custom Package → [ファイル名].unitypackage
   ```

2. **ツールの起動**
   ```
   Unity メニューバー → Tools → CaptureWindow
   ```

3. **動作確認**
   - CaptureWindowが開いたら導入完了
   - 初回起動時は日本語で表示されます

### アンインストール方法
```
Assets/ExtEditor/Editor/CaptureWindow/ フォルダを削除
```

---

## 機能一覧

### 🎯 基本撮影機能

#### **マルチソース撮影**
- **GameView撮影**：実際のゲーム画面をキャプチャ
- **SceneView撮影**：エディターのシーンビューをキャプチャ  
- **RenderTexture撮影**：カスタムRenderTextureをキャプチャ

#### **高品質出力**
- **アルファチャンネル対応**：透明度情報を含むPNG出力
- **透明背景撮影**：背景を透明にした撮影
- **高解像度対応**：任意の解像度での撮影

### 🎨 高度なファイル命名システム

#### **12種類のパターン置換**
| パターン | 説明 | 例 |
|----------|------|-----|
| `<Take>` | テイク番号 | 001, 0001 |
| `<Name>` | カメラ名/ソース名 | MainCamera, GameView |
| `<Date>` | 日付 | 20241214 |
| `<Time>` | 時刻 | 143052 |
| `<MilliSec>` | ミリ秒 | 123 |
| `<Hour>` | 時 | 14 |
| `<Minute>` | 分 | 30 |
| `<Second>` | 秒 | 52 |
| `<Year>` | 年 | 2024 |
| `<Month>` | 月 | 12 |
| `<Day>` | 日 | 14 |
| `<UnixTime>` | Unix時刻 | 1702556452 |

#### **柔軟な設定オプション**
- **テイク番号管理**：1-10桁での桁数固定、自動インクリメント
- **パス指定**：相対パス/絶対パス選択可能
- **プレビュー機能**：保存前のファイル名・パス確認

### 🌐 多言語対応

- **日本語・英語完全対応**
- **UIの動的切り替え**
- **エラーメッセージも多言語対応**
- **ヘルプシステム内蔵**

### 🔧 外部スクリプト操作API

#### **設定管理API**
```csharp
// 基本設定
CaptureAPI.Settings.Language = CaptureWindow.Language.English;
CaptureAPI.Settings.OutputDirectory = "../Screenshots";
CaptureAPI.Settings.FileNameTemplate = "Game_<Take>_<Date>";

// 高度な設定
CaptureAPI.Settings.UseFixedTakeDigits = true;
CaptureAPI.Settings.TakeDigits = 4;
CaptureAPI.Settings.IncludeAlpha = true;
```

#### **キャプチャ実行API**
```csharp
// 基本キャプチャ（戻り値は保存されたファイルの絶対パス）
string filePath = CaptureAPI.Capture.CaptureGameView();
string filePath = CaptureAPI.Capture.CaptureSceneView();
string filePath = CaptureAPI.Capture.CaptureRenderTexture(myRenderTexture);

// カスタム設定でキャプチャ
var config = new CaptureConfig
{
    FileNameTemplate = "Production_<Name>_v<Take>",
    IncludeAlpha = true,
    UseTransparentBackground = true
};
string filePath = CaptureAPI.Capture.CaptureWithConfig(config);
```

#### **ユーティリティAPI**
```csharp
// プレビュー取得
string filePreview = CaptureAPI.Utilities.GetFileNamePreview();
string pathPreview = CaptureAPI.Utilities.GetOutputPathPreview();

// ウィンドウ操作
CaptureAPI.Utilities.OpenWindow();
CaptureAPI.Utilities.OpenOutputDirectory();
```

### 🚀 プロダクション機能

#### **バッチ処理対応**
```csharp
// 全カメラを順番にキャプチャ
Camera[] cameras = FindObjectsOfType<Camera>();
foreach (Camera camera in cameras)
{
    CaptureAPI.Settings.FileNameTemplate = $"Camera_{camera.name}_<Take>";
    string filePath = CaptureAPI.Capture.CaptureGameView(camera);
}
```

#### **プリセット管理**
- **設定プリセット**：用途別設定の保存・読み込み
- **バッチ設定**：複数設定での連続撮影
- **自動化対応**：CI/CDパイプライン統合可能

### 🛡️ 安全性・信頼性

#### **堅牢なエラーハンドリング**
- **バリデーション機能**：不正な設定の事前チェック
- **セキュリティ対策**：パスインジェクション攻撃防止
- **メモリ管理**：適切なリソース解放でメモリリーク防止

#### **ユーザーフレンドリー**
- **リアルタイムプレビュー**：設定変更の即座反映
- **詳細ヘルプ**：折りたたみ式ヘルプシステム内蔵
- **履歴機能**：最近の撮影ファイル管理

### 🎮 キーボードショートカット

- **F9 + Cmd/Ctrl + Shift**：現在の設定でクイック撮影

---

## 活用場面

### 🎯 ゲーム開発
- **プロモーション素材作成**：高品質なゲーム画面キャプチャ
- **バグレポート**：問題箇所の正確な記録
- **進捗共有**：開発状況の視覚的共有

### 🏢 チーム開発
- **アセット管理**：アセットの視覚的記録
- **品質管理**：一定品質でのスクリーンショット撮影
- **ドキュメント作成**：技術資料用画像作成

### 🔄 自動化ワークフロー
- **CI/CD統合**：自動テスト結果の画像出力
- **バッチ処理**：大量スクリーンショットの自動生成
- **品質チェック**：自動的なビジュアル回帰テスト

---

## 注意事項

### システム要件
- **Unity 2022.3.22f1以降**が必要です
- **エディター専用**ツールです（ランタイムでは動作しません）
- **Windows/Mac**両対応ですが、一部機能はプラットフォーム依存します

### 使用上の注意
- **大量撮影時**はメモリ使用量にご注意ください
- **外部API使用時**はメインスレッドから呼び出してください
- **RenderTexture使用時**は適切にリソースを解放してください

### パフォーマンス
- **高解像度撮影**時は処理時間が長くなる場合があります
- **アセット自動更新**を無効にすることで高速化できます

---

## 利用規約（Terms of Service）

### 著作権・配布に関して
- 本データの **再配布、販売を禁止** します
- 本データの **著作権はHarukaKajitaに帰属** します
- 購入者は個人または組織内での使用に限定されます

### 免責事項
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

### 使用許諾
- 商用・非商用問わず、製作物への使用は自由です
- 本ツールを使用して作成されたコンテンツの権利は制作者に帰属します
- 本ツール自体の改変・逆コンパイルは禁止します

---

## 更新履歴

### v1.0.0 - 2024年12月14日
- 販売開始

---

## サポート

### 技術サポート
- **ドキュメント**：パッケージ内にAPI仕様書・サンプルコード完備
- **サンプル**：12種類の実用的なサンプルコードを提供

### お問い合わせ
技術的な質問や不具合報告は、購入プラットフォームのメッセージ機能をご利用ください。

---

*CaptureWindow - Unity開発者のためのプロフェッショナルスクリーンショットツール*