# SelectionData Editor & Manager (選択データエディタ＆マネージャー)

## 概要

SelectionDataツールを使用すると、オブジェクトの選択（プロジェクトウィンドウのアセットとシーンのGameObjectの両方）を保存、管理、再利用できます。手動でオブジェクトを追加して単純な選択を作成したり、検索クエリを使用して選択を定義したり、他のSelectionDataアセットを組み合わせて複雑な選択を構築したりすることができます。

これは特に次のような場合に役立ちます:
- よく使用するアセットやGameObjectのグループを迅速に再選択する。
- バッチ操作やエクスポートのために複雑なオブジェクトセットを管理する。
- 検索条件や他のリンクされた選択に基づいて更新されるアセットの動的なコレクションを作成する。

## 主要コンポーネント

このツールは主に3つの部分で構成されています:

*   **SelectionDataアセット:** 選択の定義を保存するスクリプタブルオブジェクト（`.asset`ファイル）です。各アセットは、特定の選択に関する情報（モード（オブジェクトベースまたはクエリベース）、含まれるオブジェクトやクエリなど）を保持します。
*   **SelectionDataインスペクター:** プロジェクトウィンドウでSelectionDataアセットを選択すると、カスタムインスペクターが表示されます。このインスペクターでは、以下の操作が可能です:
    *   説明とスター付きステータスの編集。
    *   選択モード（`Objects`または`Query`）の変更。
    *   `Selected Objects`（`Objects`モードの場合）のリスト管理。
    *   `Unity Search Query`（`Query`モードの場合）の定義。
    *   他の`SelectionData`アセットとの組み合わせ方法の設定（ソースSelectionData、組み合わせモード、組み合わせ操作）。
    *   選択の適用、現在のエディタ選択からの設定、クエリのテスト、動的選択のキャッシュなどのアクションの実行。
*   **SelectionDataウィンドウ:** `ツール > ExtEditor > Selection Data ウィンドウ` からアクセスでき、保存されたSelectionDataアセットの参照、作成、管理、適用を行うための専用エディタウィンドウです。個々のアセットインスペクターよりも包括的なビューと管理インターフェースを提供します。

## 使い方

### SelectionData アセット

#### SelectionData アセットの作成
SelectionDataアセットはいくつかの方法で作成できます:
*   **SelectionDataウィンドウから (推奨):**
    *   `ツール > ExtEditor > Selection Data ウィンドウ` で開きます。
    *   「New Object-based」、「New Query-based」、または「New Combined...」をクリックします。
*   **プロジェクトのコンテキストメニューから:**
    *   プロジェクトウィンドウで右クリックし、`作成 > ExtEditor > Selection Data` を選択します。これにより、基本的な空のオブジェクトベースのSelectionDataアセットが作成されます。

#### 主なプロパティ (インスペクターで表示)
*   **説明 (Description):** 保存された選択に対するユーザーフレンドリーな名前または説明。SelectionDataウィンドウに表示されます。
*   **スター付き (Is Starred):** これをチェックすると、選択が「お気に入り」としてマークされます。スター付きの項目はSelectionDataウィンドウのリストの最上部に表示されます。
*   **選択モード (Selection Mode):**
    *   **オブジェクト (Objects):** オブジェクトの直接的なリストによって選択が定義されます。
    *   **クエリ (Query):** Unityの検索クエリ文字列によって選択が定義されます。
*   **選択されたオブジェクト (Selected Objects):** (`Objects`モードの場合に表示)
    *   アセットやGameObjectをドラッグアンドドロップできるリスト。
    *   このSelectionDataが*動的な*組み合わせ選択である場合、このリストは読み取り専用となり、動的に生成されたオブジェクトが表示されます。
*   **Unity検索クエリ (Unity Search Query):** (`Query`モードの場合に表示)
    *   検索クエリ用のテキストフィールド（例: `t:Texture`, `Player`, `l:MyLabel`）。
    *   クエリはUnityの検索機能の簡易版を使用し、主に`AssetDatabase.FindAssets`に焦点を当てています。
*   **選択の組み合わせ (Combine Selection):** (ヘッダー) 他のSelectionDataアセットを組み合わせるための設定。
    *   **ソースSelectionData (Source Selection Data):** 組み合わせる他のSelectionDataアセットを追加できるリスト。
    *   **組み合わせモード (Combination Mode):** (ソースが存在する場合)
        *   **静的 (Static):** ソースやモードが変更されたときに、ソースSelectionDataアセットから`Selected Objects`リストが一度設定されます。その後、選択は通常の`Objects`モードの選択のように動作し、後でソースが変更されても自動的には更新されません。
        *   **動的 (Dynamic):** 選択が適用されるか、その有効なオブジェクトが要求されるたびに、`Selected Objects`がソースSelectionDataアセットから再評価されます。これは、ソース選択の変更が自動的に反映されることを意味します。
    *   **組み合わせ操作 (Combine Operation):** (ソースが存在する場合)
        *   **和集合 (Union):** すべてのソースSelectionDataアセットからのすべてのユニークなオブジェクトを含みます。
        *   **積集合 (Intersection):** *すべての*ソースSelectionDataアセットに存在するオブジェクトのみを含みます。
        *   **差集合 (Difference):** 最初のソースSelectionDataアセットに存在し、後続のどのソースアセットにも存在しないオブジェクトを含みます。

#### インスペクターでの操作
*   **保存された選択を適用 (Apply Stored Selection):** 現在のエディタ選択（プロジェクトおよびヒエラルキーウィンドウ内）を、このSelectionDataアセットで定義されたオブジェクトに設定します。
*   **現在のエディタ選択から設定 (Set Selection From Current Editor Selection):** （非組み合わせ、`Objects`モードのみ）`Selected Objects`リストをクリアし、エディタで現在選択されているオブジェクトを設定します。
*   **クエリをテストして結果を選択 (Test Query & Select Results):** （`Query`モードの場合）クエリを実行し、見つかったオブジェクトをエディタで選択します。
*   **動的選択を静的リストにキャッシュ (Cache Dynamic Selection to Static List):** （*動的な*組み合わせ選択の場合）動的ソースからオブジェクトを計算し、ローカルの`Selected Objects`リストに保存します。これは、動的選択を「ベイク」したい場合に便利です。注意：後で再評価されたくない場合は、`Combination Mode`を`Static`に変更することも検討してください。

### SelectionData ウィンドウ (`ツール > ExtEditor > Selection Data ウィンドウ`)

このウィンドウは、SelectionDataアセットを管理するための中心的なハブです。

#### ツールバー
*   **新規オブジェクトベース (New Object-based):** `Objects`モードに設定された新しいSelectionDataアセットを作成します。エディタでオブジェクトを選択している場合、それらは新しいアセットの`Selected Objects`リストに自動的に追加されます。
*   **新規クエリベース (New Query-based):** `Query`モードに設定された新しいSelectionDataアセットを作成します。
*   **新規組み合わせ... (New Combined...):** 他の既存のSelectionDataアセットを組み合わせる新しいSelectionDataアセットを作成するためのポップアップダイアログを開きます。
    *   **名前/説明 (Name/Description):** 新しい組み合わせアセットの名前。
    *   **操作 (Operation):** `Union`、`Intersection`、または`Difference`。
    *   **モード (Mode):** `Static`または`Dynamic`。
    *   **ソースSelectionDataアセット (Source SelectionData Assets):** ここにスロットを追加し、他のSelectionDataアセットをドラッグします。
*   **検索バー (Search Bar):** SelectionDataアセットのリストを名前または説明でフィルタリングします。
*   **更新ボタン (Refresh Button):** プロジェクト内のSelectionDataアセットを再スキャンします。

#### SelectionData リスト (左ペイン)
*   プロジェクト内のすべてのSelectionDataアセットを表示します。
*   **スター付き (★):** スター付きの項目が最初に表示されます。
*   **タイプインジケーター:**
    *   `[O]`: オブジェクトベース
    *   `[Q]`: クエリベース
    *   `[C]`: 組み合わせ
*   **名前/説明 (Name/Description):** クリックして選択し、右ペインで詳細を表示します。
*   **アイテムごとのアクション:**
    *   **適用 (Apply):** 現在のエディタ選択をこのSelectionDataアセットに設定します。
    *   **編集アイコン (鉛筆):** プロジェクトウィンドウでアセットを選択し、メインインスペクターでそのプロパティを編集できるようにします。
    *   **複製 (Dup):** 選択したSelectionDataアセットを複製します。
    *   **削除アイコン (ゴミ箱):** SelectionDataアセットを削除します（確認あり）。

#### 詳細ビュー (右ペイン)
リストからSelectionDataアセットを選択すると:
*   **名前/説明 (Name/Desc):** 選択されたアイテムの説明。
*   **スター付き (Starred):** スター付きステータスを変更するためのトグル（変更はすぐに保存されます）。
*   **タイプ (Type):** `Objects`または`Query`。
*   **組み合わせ情報 (Combination Info):** 組み合わせ選択の場合、操作、モード、およびそのソースをリスト表示します。
*   **クエリ (Query):** クエリベースの選択の場合、クエリ文字列を表示します。
*   **有効なオブジェクト (Effective Objects):** このSelectionDataアセットが現在解決する実際の`UnityEngine.Object`をすべてリスト表示します。
    *   このリスト内のオブジェクトをクリックすると、プロジェクト/ヒエラルキーでpingできます。
    *   **これのみ選択 (Select Only):** エディタでその特定のオブジェクト*のみ*を選択します。

## 利用例

SelectionDataアセットは様々な目的で使用できます:

*   **よく使うプレハブの保存:** プロジェクトウィンドウでプレハブを選択し、SelectionDataウィンドウで「New Object-based」をクリックし、名前を付けます。
*   **全てのテクスチャの動的コレクション:** 「New Query-based」のSelectionDataを作成し、インスペクターでクエリを`t:Texture`に設定します。
*   **選択の組み合わせ（例：差集合）:** 「全てのキャラクターモデル」用に1つのSelectionDataを、「無視するキャラクターモデル」用にもう1つ作成します。次に、「New Combined...」で`Difference`操作（Dynamicモード）を使用して「最終キャラクターモデル」（全てから無視するものを除いたもの）のSelectionDataを作成します。

## 注意点とヒント

*   SelectionDataアセットの`Description`フィールドは、主にSelectionDataウィンドウでの表示に使用されます。実際のアセットファイル名は異なる場合があります。
*   組み合わせ選択を作成する際は、循環（例：データAがデータBをソースとして使用し、データBがデータAを使用する）に注意してください。ツールには無限ループを防ぐための循環検出がありますが、組み合わせを論理的に設計することが推奨されます。
*   `SelectionData.cs`の`ExecuteUnitySearchQuery`メソッドは簡易的な検索です。非常に複雑なアセットクエリの場合、Unityの組み込み検索ウィンドウの方が強力かもしれませんが、その結果をオブジェクトベースのSelectionDataに保存することができます。
*   *動的な*組み合わせSelectionDataでソースとして使用されているSelectionDataアセットを削除すると、組み合わせられたものがそれらのオブジェクトを見失う原因となります。*静的な*組み合わせ選択の場合、オブジェクトは既にキャッシュされています。
*   SelectionDataウィンドウのリストの最上部に最も重要または頻繁に使用する選択を保持するために、「スター付き」機能を使用してください。
