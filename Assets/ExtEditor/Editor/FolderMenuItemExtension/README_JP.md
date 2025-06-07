# Folder Menu Item Extension (フォルダメニュー項目拡張)

## 概要

Folder Menu Item Extensionツールを使用すると、ユーザーは `Assets/Create/Folders/` の下にカスタムメニュー項目を動的に作成できます。これらのカスタムメニュー項目を使用して、Unityのプロジェクトウィンドウで選択したフォルダ内に、事前に定義されたフォルダ構造を迅速に生成することができます。

このツールは、`.fmi` という拡張子を持つ特別な定義ファイルを処理することで機能します。各 `.fmi` ファイルは、作成するサブフォルダのリストと、対応するメニュー項目の優先度を記述します。その後、`ScriptedImporter` がこれらの `.fmi` ファイルを読み取り、C#スクリプトを生成します。この生成されたC#スクリプトが、実際にUnityにメニュー項目を追加する役割を担います。

このシステムは、新しいフォルダ構造ごとにC#コードを作成したり変更したりする必要なく、共通のフォルダレイアウトを定義するための柔軟な方法を提供します。

## コアコンポーネント

-   **`.fmi` (Folder Menu Item) ファイル:**
    -   特定のフォルダ構造とメニューの優先度を定義する、ユーザー作成のテキストファイル。
    -   **場所:** 通常、エディタフォルダに配置されます。例: `Assets/ExtEditor/Editor/FolderMenuItemExtension/Editor/FolderMenuItemPreset/`
    -   **フォーマット:**
        -   **最初の行**は、メニュー項目の表示優先度を表す整数でなければなりません（数値が小さいほど `Assets/Create/Folders/` サブメニューの上位に表示されます）。
        -   **後続の各行**は、サブフォルダ名を指定します。ネストしたフォルダはスラッシュ `/` を使用して定義できます（例: `Art/Models/Characters`）。
-   **`FolderMenuItemImporter.cs`:**
    -   プロジェクトに `.fmi` ファイルが追加または変更されたときに自動的に処理する `ScriptedImporter`。
    -   `.fmi` ファイルの内容を読み取ります。
    -   テンプレート（`ScriptTemplate.txt`）を使用して新しいC#スクリプトを生成します（例: `MyFolders.fmi` は `MyFoldersFolderMenuItem.cs` を生成します）。
    -   この生成されたスクリプトには、メニュー項目と指定されたフォルダを作成するロジックが含まれています。
-   **`ScriptTemplate.txt`:**
    -   `FolderMenuItemImporter` が実際のメニュー項目スクリプトを生成するために使用するC#テンプレートファイル。使用する `.fmi` ファイルと同じディレクトリに存在する必要があります。
-   **生成されたC#スクリプト (例: `[YourFMIFileName]FolderMenuItem.cs`):**
    -   これらのスクリプトは `FolderMenuItemImporter` によって自動的に作成されます。
    -   `Assets/Create/Folders/[YourFMIFileName]` のようなエントリを追加する `[MenuItem]` 属性が含まれています。
    -   クリックすると、このメニュー項目はプロジェクトウィンドウで現在選択されているフォルダの下に（元の `.fmi` ファイルで定義された）フォルダ構造を作成します。

## 使い方

1.  **プリセットディレクトリの準備:**
    *   `.fmi` ファイルとテンプレート用のディレクトリがあることを確認してください。例: `Assets/ExtEditor/Editor/FolderMenuItemExtension/Editor/FolderMenuItemPreset/`
    *   このディレクトリに `ScriptTemplate.txt` が存在することを確認してください。提供されているプリセット（`AllFolders.fmi`, `RuntimeAndEditor.fmi`）には既にこの構造があります。

2.  **`.fmi` ファイルの作成または変更:**
    *   選択したプリセットディレクトリ内に新しいテキストファイルを作成し、その拡張子を `.fmi` に変更します（例: `BasicProjectSetup.fmi`）。
    *   **`.fmi` ファイルの編集:**
        *   **1行目:** メニューの優先度を表す整数を入力します（例: 高優先度の場合は `0`、低優先度の場合は `100`）。これは `Assets/Create/Folders/` サブメニュー内での順序を決定します。
        *   **後続の行:** 作成したい各サブフォルダを1行に1つずつリストします。
            ```fmi
            0
            Scripts
            Art
            Art/Materials
            Art/Prefabs
            Audio
            Scenes
            ```
    *   `.fmi` ファイルを保存します。

3.  **自動スクリプト生成:**
    *   `.fmi` ファイルを保存すると、Unityのアセットインポーターが `FolderMenuItemImporter` をトリガーします。
    *   新しいC#スクリプト（例: `BasicProjectSetupFolderMenuItem.cs`）が `.fmi` ファイルと同じディレクトリに生成されます。
    *   簡単なスクリプトコンパイルの後、新しいメニュー項目 `Assets/Create/Folders/BasicProjectSetup` が利用可能になります。

4.  **生成されたメニュー項目の使用:**
    *   Unityのプロジェクトウィンドウで、新しいフォルダ構造を作成したい親フォルダを選択します。
    *   選択したフォルダを右クリックし、`Create > Folders > BasicProjectSetup`（または `.fmi` ファイルに対応する名前）に移動します。
    *   それをクリックします。定義されたサブフォルダが、選択した親フォルダ内に作成されます。

5.  **フォルダ構造の更新:**
    *   `.fmi` ファイルの内容を変更して保存するだけです。
    *   `FolderMenuItemImporter` が再実行され、関連する生成済みC#スクリプトが更新されるため、メニュー項目の動作も更新されます。

6.  **カスタムメニュー項目の削除:**
    *   カスタムフォルダメニュー項目を完全に削除するには、**両方を削除する必要があります**:
        1.  `.fmi` 定義ファイル（例: `BasicProjectSetup.fmi`）。
        2.  対応する生成済みC#スクリプト（例: `BasicProjectSetupFolderMenuItem.cs`）。
    *   `.fmi` ファイルのみを削除した場合、生成されたスクリプトは残り、メニュー項目も残ります（ただし、更新はできなくなります）。
    *   生成されたC#スクリプトのみを削除した場合、次回 `.fmi` ファイルが再インポートされるとき（例: プロジェクトを開いたときや `.fmi` ファイルが変更されたとき）に再生成されます。

## 含まれるプリセット

このツールには、`FolderMenuItemPreset` ディレクトリにいくつかのサンプル `.fmi` ファイルが含まれています:
-   `AllFolders.fmi`: 一般的なUnityプロジェクトフォルダの包括的なセットを作成します。
-   `RuntimeAndEditor.fmi`: スクリプトを整理するための一般的なパターンである、個別の `Runtime` および `Editor` フォルダを作成します。

詳細については、そのディレクトリにあるこれらのファイルの内容と `_USAGE.txt` ファイルを確認してください。
元の `CreateFoldersMenuItem.cs` ファイルには、以前のより単純な実装からのコメントアウトされたコードが含まれており、`.fmi` システムではアクティブに使用されていません。
