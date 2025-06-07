# (日本語訳) ExtEditor - SelectionData ツール利用ガイド

## 1. 概要

The SelectionData tool allows you to save, manage, and reuse selections of objects (both assets from the Project window and GameObjects from the Scene). You can create simple selections by manually adding objects, define selections using search queries, or build complex selections by combining other SelectionData assets.

This is particularly useful for:
- Quickly re-selecting frequently used groups of assets or GameObjects.
- Managing complex sets of objects for batch operations or exports.
- Creating dynamic collections of assets that update based on search criteria or other linked selections.

The tool consists of three main parts:
*   **SelectionData Assets:** ScriptableObjects that store the definition of your selections.
*   **SelectionData Inspector:** A custom inspector for viewing and editing SelectionData assets.
*   **SelectionData Window:** A dedicated editor window for browsing, creating, managing, and applying your saved SelectionData assets.

## 2. SelectionData アセット

A SelectionData asset (`.asset` file) holds the information about a specific selection.

### SelectionData アセットの作成:

You can create SelectionData assets in several ways:
*   **From the SelectionData Window:** (Recommended)
    *   Open the window via `Tools > ExtEditor > Selection Data Window`.
    *   Click "New Object-based", "New Query-based", or "New Combined..."
*   **From the Project Context Menu:**
    *   Right-click in the Project window and select `Create > ExtEditor > Selection Data`. This creates a basic, empty object-based SelectionData asset.

### 主なプロパティ (インスペクターで表示):

When you select a SelectionData asset, its Inspector will show the following:

*   **Description:** A user-friendly name or description for this saved selection. This is what appears in the SelectionData Window.
*   **Is Starred:** Check this to mark the selection as a "favorite". Starred items appear at the top of the list in the SelectionData Window.
*   **Selection Mode:**
    *   **Objects:** The selection is defined by a direct list of objects.
    *   **Query:** The selection is defined by a Unity Search query string.
*   **Selected Objects (appears if Mode is `Objects`):**
    *   A list where you can drag and drop assets or GameObjects.
    *   If this SelectionData is a *dynamic* combined selection, this list is read-only and shows the dynamically generated objects.
*   **Unity Search Query (appears if Mode is `Query`):**
    *   A text field for your search query (e.g., `t:Texture`, `Player`, `l:MyLabel`).
    *   The query uses a simplified version of Unity's search capabilities, primarily focusing on `AssetDatabase.FindAssets`.
*   **Combine Selection (Header):** Settings for combining other SelectionData assets.
    *   **Source Selection Data:** A list where you can add other SelectionData assets to be combined.
    *   **Combination Mode (if Sources are present):**
        *   **Static:** The `Selected Objects` list is populated once from the source SelectionData assets when the sources or mode change. The selection then behaves like a regular `Objects` mode selection and does not update automatically if the sources change later.
        *   **Dynamic:** The `Selected Objects` are re-evaluated from the source SelectionData assets every time the selection is applied or its effective objects are requested. This means changes in the source selections will be reflected automatically.
    *   **Combine Operation (if Sources are present):**
        *   **Union:** Includes all unique objects from all source SelectionData assets.
        *   **Intersection:** Includes only objects that are present in *all* source SelectionData assets.
        *   **Difference:** Includes objects from the first source SelectionData asset that are *not* present in any of the subsequent source assets.

### インスペクターでの操作:

*   **Apply Stored Selection:** Sets your current editor selection (in Project and Hierarchy windows) to the objects defined by this SelectionData asset.
*   **Set Selection From Current Editor Selection:** (Only for non-combined, `Objects` mode) Clears the `Selected Objects` list and populates it with the objects currently selected in your editor.
*   **Test Query & Select Results:** (For `Query` mode) Executes the query and selects the found objects in the editor.
*   **Cache Dynamic Selection to Static List:** (For *dynamic* combined selections) Calculates the objects from its dynamic sources and stores them in the local `Selected Objects` list. This is useful if you want to "bake" a dynamic selection. Note: you might also want to change `Combination Mode` to `Static` if you don't want it to re-evaluate later.

## 3. SelectionData ウィンドウ (`ツール > ExtEditor > Selection Data ウィンドウ`)

This window is the central hub for managing your SelectionData assets.

### ツールバー:

*   **New Object-based:** Creates a new SelectionData asset set to `Objects` mode. If you have objects selected in the editor, they will be automatically added to the new asset's `Selected Objects` list.
*   **New Query-based:** Creates a new SelectionData asset set to `Query` mode.
*   **New Combined...:** Opens a popup dialog to create a new SelectionData asset that combines other existing SelectionData assets.
    *   **Name/Description:** Name for the new combined asset.
    *   **Operation:** `Union`, `Intersection`, or `Difference`.
    *   **Mode:** `Static` or `Dynamic`.
    *   **Source SelectionData Assets:** Add slots and drag other SelectionData assets here.
*   **Search Bar:** Filters the list of SelectionData assets by their name or description.
*   **Refresh Button:** Rescans the project for SelectionData assets.

### SelectionData リスト (左ペイン):

*   Displays all SelectionData assets in your project.
*   **Starred (★):** Starred items appear first.
*   **Type Indicator:**
    *   `[O]`: Object-based
    *   `[Q]`: Query-based
    *   `[C]`: Combined
*   **Name/Description:** Click to select and view details in the right pane.
*   **Actions per item:**
    *   **Apply:** Sets your current editor selection to this SelectionData asset.
    *   **Edit Icon (pencil):** Selects the asset in the Project window, allowing you to edit its properties in the main Inspector.
    *   **Dup:** Duplicates the selected SelectionData asset.
    *   **Delete Icon (trash can):** Deletes the SelectionData asset (with confirmation).

### 詳細ビュー (右ペイン):

When you select a SelectionData asset from the list:
*   **Name/Desc:** The description of the selected item.
*   **Starred:** A toggle to change its starred status (changes are saved immediately).
*   **Type:** `Objects` or `Query`.
*   **Combination Info:** If it's a combined selection, shows the operation, mode, and lists its sources.
*   **Query:** If it's a query-based selection, shows the query string.
*   **Effective Objects:** Lists all the actual `UnityEngine.Object`s that this SelectionData asset currently resolves to.
    *   You can click on an object in this list to ping it in the Project/Hierarchy.
    *   **Select Only:** Selects *only* that specific object in the editor.

## 4. 利用例

### 例1: よく使うプレハブのセレクションを保存する

1.  In the Project window, manually select several Prefab assets you use often.
2.  Open the **SelectionData Window** (`Tools > ExtEditor > Selection Data Window`).
3.  Click **"New Object-based"** in the toolbar.
4.  A new SelectionData asset will be created, and its `Selected Objects` list will be automatically populated with your currently selected Prefabs.
5.  Give it a meaningful **Description** in its Inspector (e.g., "Core UI Prefabs").
6.  You can now quickly re-select these prefabs by finding this item in the SelectionData Window and clicking "Apply".

### 例2: 全てのテクスチャアセットの動的セレクションを作成する

1.  Open the **SelectionData Window**.
2.  Click **"New Query-based"**.
3.  Select the newly created SelectionData asset in the Project window to view its Inspector.
4.  Set its **Description** (e.g., "All Project Textures").
5.  In the **Unity Search Query** field, type `t:Texture`.
6.  Click **"Test Query & Select Results"** to see it in action.
7.  Now, whenever you "Apply" this SelectionData from the window, it will select all Texture assets currently in your project.

### 例3: セレクションの組み合わせ - 無視するモデルを除いた全てのキャラクターモデル

1.  **Create `SelectionData A` (Object-based):**
    *   Name: "All Character Models"
    *   Manually add all your character model assets to its `Selected Objects` list.
2.  **Create `SelectionData B` (Object-based):**
    *   Name: "Ignored Character Models"
    *   Manually add any character models you want to exclude (e.g., old versions, test models) to its `Selected Objects` list.
3.  **Create a Combined SelectionData:**
    *   In the **SelectionData Window**, click **"New Combined..."**.
    *   **Name/Description:** "Final Character Models"
    *   **Operation:** `Difference`
    *   **Mode:** `Dynamic` (so it updates if you change A or B)
    *   **Sources:**
        *   Add `SelectionData A` ("All Character Models") as the first source.
        *   Add `SelectionData B` ("Ignored Character Models") as the second source.
    *   Click "Create Combined SelectionData".
4.  Now, the "Final Character Models" SelectionData will always represent the models in A minus any models also present in B.

## 5. 注意点とヒント

*   The `Description` field of a SelectionData asset is primarily used for display in the SelectionData Window. The actual asset filename can be different.
*   When creating combined selections, be mindful of cycles (e.g., Data A uses Data B as a source, and Data B uses Data A). The tool has cycle detection to prevent infinite loops, but it's good practice to design your combinations logically.
*   The `ExecuteUnitySearchQuery` method in `SelectionData.cs` is a simplified search. For very complex asset queries, Unity's built-in Search window might be more powerful, but you can then save those results into an object-based SelectionData.
*   Deleting a SelectionData asset that is used as a source in another *dynamic* combined SelectionData will cause the combined one to miss those objects. For *static* combined selections, the objects are already cached.
*   Use the "Starred" feature to keep your most important or frequently used selections at the top of the list in the SelectionData Window.

以上が SelectionData ツールの概要です。

---
*このドキュメントは英語版を元に翻訳されました。*
