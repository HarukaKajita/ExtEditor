# ExtEditor - SelectionData ツールの使い方

## 1. 概要

SelectionData ツールを使用すると、オブジェクトの選択状態（Project ウィンドウのアセットおよび Scene の GameObject の両方）を保存、管理、再利用できます。オブジェクトを手動で追加してシンプルなセレクションを作成したり、検索クエリで定義したり、他の SelectionData アセットを組み合わせて複雑なセレクションを構築したりできます。

特に次のような場面で役立ちます:
- 頻繁に使用するアセットや GameObject のグループを素早く再選択する
- バッチ操作やエクスポートのために複雑なオブジェクト集合を管理する
- 検索条件や他のセレクションに基づいて更新される動的なアセット集を作成する

このツールは主に次の 3 つの要素で構成されます:
* **SelectionData アセット**: セレクション定義を保存する ScriptableObject
* **SelectionData インスペクター**: SelectionData アセットを表示・編集するためのカスタムインスペクター
* **SelectionData ウィンドウ**: 保存した SelectionData アセットを閲覧、作成、管理、適用する専用エディターウィンドウ

## 2. SelectionData アセット

SelectionData アセット（拡張子 `.asset`）には特定のセレクション情報が保存されます。

### SelectionData アセットの作成方法

SelectionData アセットは以下の方法で作成できます:
* **SelectionData ウィンドウから** (推奨)
    * `Tools > ExtEditor > Selection Data Window` からウィンドウを開きます。
    * "New Object-based"、"New Query-based"、または "New Combined..." をクリックします。
* **Project ウィンドウのコンテキストメニューから**
    * Project ウィンドウを右クリックし `Create > ExtEditor > Selection Data` を選択します。空のオブジェクトベースの SelectionData アセットが作成されます。

### インスペクターで表示される主なプロパティ

SelectionData アセットを選択すると、インスペクターには次の項目が表示されます:

* **Description**: セレクションの名称や説明。SelectionData ウィンドウに表示されます。
* **Is Starred**: お気に入りとしてマークするためのチェック。スター付きアイテムはウィンドウのリスト上部に表示されます。
* **Selection Mode**:
    * **Objects**: オブジェクトのリストでセレクションを定義します。
    * **Query**: Unity Search のクエリ文字列でセレクションを定義します。
* **Selected Objects** (Mode が `Objects` の場合に表示)
    * アセットや GameObject をドラッグ＆ドロップできるリスト
    * この SelectionData が *Dynamic* な組み合わせセレクションの場合、このリストは読み取り専用で動的に生成されたオブジェクトを表示します。
* **Unity Search Query** (Mode が `Query` の場合に表示)
    * 検索クエリ用のテキストフィールド（例: `t:Texture`, `Player`, `l:MyLabel`）
    * 検索には Unity の検索機能を簡略化したものを使用しており、主に `AssetDatabase.FindAssets` が用いられます。
* **Combine Selection (ヘッダー)** 他の SelectionData アセットを組み合わせる設定
    * **Source Selection Data**: 組み合わせ対象の SelectionData アセットを追加するリスト
    * **Combination Mode** (ソースがある場合)
        * **Static**: ソースやモードの変更時に一度だけ `Selected Objects` が更新され、その後は通常の `Objects` モードと同様に自動更新されません。
        * **Dynamic**: 選択を適用するたび、または有効オブジェクトを取得するたびにソース SelectionData から再評価されます。ソース側の変更が自動的に反映されます。
    * **Combine Operation** (ソースがある場合)
        * **Union**: すべてのソース SelectionData から重複を除いたオブジェクトを含めます。
        * **Intersection**: すべてのソースに共通するオブジェクトのみを含めます。
        * **Difference**: 最初のソースに存在し、その後のソースには存在しないオブジェクトを含めます。

### インスペクター上の操作

* **Apply Stored Selection**: SelectionData で定義されたオブジェクトを現在のエディター選択（Project と Hierarchy）に設定します。
* **Set Selection From Current Editor Selection** (組み合わせでない `Objects` モードのみ): `Selected Objects` をクリアし、現在エディターで選択しているオブジェクトでリストを更新します。
* **Test Query & Select Results** (`Query` モード用): クエリを実行し、見つかったオブジェクトをエディターで選択します。
* **Cache Dynamic Selection to Static List** (*Dynamic* な組み合わせセレクション用): 動的ソースから得たオブジェクトをローカルの `Selected Objects` に保存します。動的セレクションを"焼き付け"たいときに便利です。必要に応じて `Combination Mode` を `Static` に変更すると再評価されなくなります。

## 3. SelectionData ウィンドウ (`Tools > ExtEditor > Selection Data Window`)

このウィンドウは SelectionData アセットを管理する中心的な場所です。

### ツールバー

* **New Object-based**: `Objects` モードの新しい SelectionData アセットを作成します。エディターでオブジェクトを選択している場合、それらが自動的に `Selected Objects` に追加されます。
* **New Query-based**: `Query` モードの新しい SelectionData アセットを作成します。
* **New Combined...**: 既存の SelectionData アセットを組み合わせた新しいアセットを作成するポップアップを開きます。
    * **Name/Description**: 新しい組み合わせアセットの名前
    * **Operation**: `Union`, `Intersection`, `Difference` のいずれか
    * **Mode**: `Static` または `Dynamic`
    * **Source SelectionData Assets**: スロットを追加して他の SelectionData アセットをドラッグします
* **Search Bar**: 名前や説明で SelectionData アセットをフィルターします。
* **Refresh Button**: プロジェクトを再スキャンして SelectionData アセットを更新します。

### SelectionData リスト (左ペイン)

* プロジェクト内のすべての SelectionData アセットを表示します。
* **Starred (★)**: スター付きアイテムは先頭に表示されます。
* **Type Indicator**
    * `[O]`: Object-based
    * `[Q]`: Query-based
    * `[C]`: Combined
* **Name/Description**: クリックすると右ペインに詳細が表示されます。
* **Actions per item**
    * **Apply**: この SelectionData アセットの内容を現在のエディター選択に設定します。
    * **Edit Icon (pencil)**: Project ウィンドウでアセットを選択し、メインインスペクターで編集できるようにします。
    * **Dup**: 選択した SelectionData アセットを複製します。
    * **Delete Icon (trash can)**: 確認の上で SelectionData アセットを削除します。

### 詳細ビュー (右ペイン)

リストから SelectionData アセットを選択すると次が表示されます:
* **Name/Desc**: 選択したアイテムの説明
* **Starred**: スター状態の切り替え (即時保存)
* **Type**: `Objects` または `Query`
* **Combination Info**: 組み合わせセレクションの場合、操作・モード・ソース一覧を表示
* **Query**: クエリベースセレクションの場合、クエリ文字列を表示
* **Effective Objects**: この SelectionData が現在解決する `UnityEngine.Object` の一覧
    * オブジェクトをクリックすると Project/Hierarchy でピンを打てます
    * **Select Only**: そのオブジェクトだけをエディターで選択します

## 4. 利用例

### 例 1: よく使う Prefab のセレクションを保存する

1. Project ウィンドウで、よく使う Prefab アセットを複数選択します。
2. **SelectionData ウィンドウ** (`Tools > ExtEditor > Selection Data Window`) を開きます。
3. ツールバーの **"New Object-based"** をクリックします。
4. 新しい SelectionData アセットが作成され、現在選択中の Prefab が自動的に `Selected Objects` リストに追加されます。
5. インスペクターでわかりやすい **Description** (例: "Core UI Prefabs") を付けます。
6. SelectionData ウィンドウでこのアイテムを見つけて "Apply" をクリックすれば、すぐにこれらの Prefab を再選択できます。

### 例 2: すべての Texture アセットを動的に選択する

1. **SelectionData ウィンドウ**を開きます。
2. **"New Query-based"** をクリックします。
3. Project ウィンドウで新しく作成された SelectionData アセットを選択し、インスペクターを表示します。
4. **Description** を設定します (例: "All Project Textures")。
5. **Unity Search Query** フィールドに `t:Texture` と入力します。
6. **"Test Query & Select Results"** をクリックして動作を確認します。
7. 以降、この SelectionData をウィンドウから "Apply" すると、プロジェクト内のすべての Texture アセットが選択されます。

### 例 3: 無視するモデルを除いた全キャラクターモデルを選択する

1. **`SelectionData A` (Object-based) を作成**
    * Name: "All Character Models"
    * すべてのキャラクターモデルアセットを `Selected Objects` リストに追加します
2. **`SelectionData B` (Object-based) を作成**
    * Name: "Ignored Character Models"
    * 除外したいモデル (古い版やテスト用など) を `Selected Objects` リストに追加します
3. **Combined SelectionData を作成**
    * **SelectionData ウィンドウ**で **"New Combined..."** をクリックします
    * **Name/Description**: "Final Character Models"
    * **Operation**: `Difference`
    * **Mode**: `Dynamic` (A または B を変更すると更新されるように)
    * **Sources**:
        * `SelectionData A` ("All Character Models") を最初のソースとして追加
        * `SelectionData B` ("Ignored Character Models") を 2 番目のソースとして追加
    * "Create Combined SelectionData" をクリックします
4. これで "Final Character Models" SelectionData は常に A から B を除いたモデルを表すようになります。

## 5. 注意点とヒント

* `Description` フィールドは主に SelectionData ウィンドウで表示するためのもので、実際のアセットファイル名とは異なっていてもかまいません。
* 組み合わせセレクションを作成する際は循環参照に注意してください (例: Data A が Data B をソースにし、Data B が Data A をソースにする)。ツールにはループ検出機能がありますが、論理的な組み合わせを心掛けましょう。
* `SelectionData.cs` の `ExecuteUnitySearchQuery` メソッドは簡易検索です。より複雑な検索には Unity 標準の Search ウィンドウが強力ですが、結果をオブジェクトベースの SelectionData に保存して利用できます。
* 他の *Dynamic* な Combined SelectionData のソースとして使用されている SelectionData アセットを削除すると、その Combined はそのオブジェクトを取得できなくなります。*Static* モードの場合は既にキャッシュされています。
* よく使うセレクションは "Starred" 機能を使って SelectionData ウィンドウの上部に表示させると便利です。

以上が SelectionData ツールの概要です。
