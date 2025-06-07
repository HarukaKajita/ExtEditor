# SelectionData Editor & Manager

## Overview

The SelectionData tool allows you to save, manage, and reuse selections of objects, supporting both assets from the Project window and GameObjects from the Scene. You can create selections by manually adding objects, defining them with search queries, or building complex selections by combining other SelectionData assets.

This tool is useful for:
- Quickly re-selecting frequently used groups of assets or GameObjects.
- Managing complex sets of objects for batch operations or exports.
- Creating dynamic collections of assets that update based on search criteria or other linked selections.

## Key Components

The tool consists of three main parts:

*   **SelectionData Assets:** These are ScriptableObjects (`.asset` files) that store the definition of your selections. Each asset holds the information about a specific selection, including its mode (object-based or query-based) and any objects or queries it contains.
*   **SelectionData Inspector:** When a SelectionData asset is selected in the Project window, a custom inspector is displayed. This inspector allows you to:
    *   Edit the description and starred status.
    *   Change the selection mode (`Objects` or `Query`).
    *   Manage the list of `Selected Objects` (for `Objects` mode).
    *   Define the `Unity Search Query` (for `Query` mode).
    *   Configure how it combines other `SelectionData` assets (Source Selection Data, Combination Mode, Combine Operation).
    *   Perform actions like applying the selection, populating from the current editor selection, testing queries, and caching dynamic selections.
*   **SelectionData Window:** Accessible via `Tools > ExtEditor > Selection Data Window`, this is a dedicated editor window for browsing, creating, managing, and applying your saved SelectionData assets. It provides a more holistic view and management interface than the individual asset inspector.

## How to Use

### SelectionData Assets

#### Creating SelectionData Assets
You can create SelectionData assets in several ways:
*   **From the SelectionData Window (Recommended):**
    *   Open via `Tools > ExtEditor > Selection Data Window`.
    *   Click "New Object-based", "New Query-based", or "New Combined..."
*   **From the Project Context Menu:**
    *   Right-click in the Project window and select `Create > ExtEditor > Selection Data`. This creates a basic, empty object-based SelectionData asset.

#### Key Properties (visible in the Inspector)
*   **Description:** A user-friendly name for the selection.
*   **Is Starred:** Marks the selection as a "favorite" for easier access in the SelectionData Window.
*   **Selection Mode:**
    *   **Objects:** Selection defined by a direct list of objects.
    *   **Query:** Selection defined by a Unity Search query string (e.g., `t:Texture`, `l:MyLabel`).
*   **Selected Objects:** (For `Objects` mode) A list where you drag and drop assets or GameObjects. Read-only for dynamic combined selections.
*   **Unity Search Query:** (For `Query` mode) Text field for your search query.
*   **Combine Selection:**
    *   **Source Selection Data:** List of other SelectionData assets to combine.
    *   **Combination Mode:**
        *   **Static:** Populates `Selected Objects` once from sources. Does not update automatically if sources change.
        *   **Dynamic:** Re-evaluates from sources each time the selection is applied. Changes in sources are reflected.
    *   **Combine Operation:** `Union` (all unique), `Intersection` (only in all), `Difference` (in first, not in subsequent).

#### Inspector Actions
*   **Apply Stored Selection:** Applies the asset's selection to the editor.
*   **Set Selection From Current Editor Selection:** (For non-combined, `Objects` mode) Populates the list with currently selected editor objects.
*   **Test Query & Select Results:** (For `Query` mode) Executes the query and selects results.
*   **Cache Dynamic Selection to Static List:** (For dynamic combined selections) "Bakes" a dynamic selection into the local `Selected Objects` list.

### SelectionData Window (`Tools > ExtEditor > Selection Data Window`)

This window is the central hub for managing SelectionData assets.

#### Toolbar
*   **New Object-based:** Creates a new object-based SelectionData. If objects are selected in the editor, they are automatically added.
*   **New Query-based:** Creates a new query-based SelectionData.
*   **New Combined...:** Opens a dialog to create a new combined SelectionData, allowing you to specify its name, operation (`Union`, `Intersection`, `Difference`), mode (`Static`, `Dynamic`), and source SelectionData assets.
*   **Search Bar:** Filters the list of SelectionData assets by name/description.
*   **Refresh Button:** Rescans the project for SelectionData assets.

#### SelectionData List (Left Pane)
*   Displays all SelectionData assets. Starred (★) items appear first.
*   Type indicators: `[O]` (Object-based), `[Q]` (Query-based), `[C]` (Combined).
*   Actions per item:
    *   **Apply:** Applies the selection to the editor.
    *   **Edit Icon (pencil):** Selects the asset in the Project window for editing in the main Inspector.
    *   **Dup:** Duplicates the asset.
    *   **Delete Icon (trash can):** Deletes the asset.

#### Detail View (Right Pane)
When a SelectionData asset is selected from the list:
*   Displays its Name/Description, Starred status (toggleable), Type, and combination info (if applicable).
*   Shows the query string for query-based selections.
*   **Effective Objects:** Lists all actual `UnityEngine.Object`s the SelectionData currently resolves to. Clicking an object pings it. "Select Only" selects only that specific object.

## Usage Examples

You can use SelectionData assets for various purposes:

*   **Saving Frequently Used Prefabs:** Select prefabs in the Project window, click "New Object-based" in the SelectionData Window, and give it a name.
*   **Dynamic Collection of All Textures:** Create a "New Query-based" SelectionData, and in its Inspector, set the query to `t:Texture`.
*   **Combining Selections (e.g., Difference):** Create one SelectionData for "All Character Models" and another for "Ignored Character Models." Then, create a "New Combined..." SelectionData using the `Difference` operation (Dynamic mode) to get "Final Character Models" (All minus Ignored).

## Notes and Tips

*   The `Description` field is for display in the SelectionData Window; the asset filename can differ.
*   Avoid circular dependencies when creating combined selections (e.g., A uses B, B uses A). The tool has cycle detection, but logical design is best.
*   The query functionality is a simplified version of Unity's search. For very complex queries, use Unity's Search window and save results into an object-based SelectionData.
*   Deleting a SelectionData asset used as a source in a *dynamic* combined selection will affect the combined one. *Static* combined selections cache their objects.
*   Use the "Starred" feature to keep important selections at the top of the SelectionData Window list.
