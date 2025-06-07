# SelectionData Editor

## Overview

The SelectionData Editor is a Unity editor window extension that allows developers to manage and edit `SelectionData` ScriptableObject assets. `SelectionData` assets are used to store collections of string identifiers, which can be useful for various purposes suchs as defining tags, categories, or other sets of selectable items.

## Features

- **Create New SelectionData Assets**: Easily create new `SelectionData` assets directly from the editor window.
- **Edit Existing SelectionData Assets**: Modify the list of string identifiers within a `SelectionData` asset.
- **Intuitive UI**: User-friendly interface for adding, removing, and reordering identifiers.
- **Automatic Asset Saving**: Changes made to `SelectionData` assets are automatically saved.

## Getting Started

### Prerequisites

- Unity 2020.3 or later.

### Installation

1.  Place the `SelectionDataEditor.cs` script in an `Editor` folder within your Unity project.
2.  Ensure that the `SelectionData.cs` script (defining the `ScriptableObject`) is also present in your project.

### Opening the Editor

To open the SelectionData Editor window, navigate to **Window > SelectionData Editor** in the Unity menu bar.

## How to Use

### Creating a New SelectionData Asset

1.  Open the SelectionData Editor window.
2.  Click the "Create New SelectionData" button.
3.  A file dialog will appear. Choose a location and name for your new `SelectionData` asset (e.g., `MyTags.asset`).
4.  The new asset will be created and automatically selected for editing.

### Editing an Existing SelectionData Asset

1.  Open the SelectionData Editor window.
2.  Drag and drop an existing `SelectionData` asset from your Project window onto the "Drop SelectionData Asset Here" area in the editor window.
    Alternatively, click the "Load SelectionData" button (if available, depending on the implementation) and select an asset using the object picker.
3.  The editor will display the list of identifiers stored in the selected asset.

### Managing Identifiers

-   **Adding an Identifier**:
    -   Type the new identifier string into the text field at the bottom of the list.
    -   Click the "+" (Add) button or press Enter.
-   **Removing an Identifier**:
    -   Click the "-" (Remove) button next to the identifier you want to remove.
-   **Reordering Identifiers** (If supported by the implementation):
    -   Drag and drop identifiers within the list to change their order.

### Saving Changes

Changes are typically saved automatically when you modify the list of identifiers (e.g., add, remove, or reorder). Some implementations might also include an explicit "Save" button.

## Scripting API

### `SelectionData.cs`

This script defines the `ScriptableObject` that holds the data.

```csharp
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSelectionData", menuName = "Data/SelectionData")]
public class SelectionData : ScriptableObject
{
    public List<string> identifiers = new List<string>();
}
```

### `SelectionDataEditor.cs` (Example Snippets)

This script creates the editor window.

**Initializing the Window:**

```csharp
using UnityEditor;
using UnityEngine;

public class SelectionDataEditor : EditorWindow
{
    private SelectionData currentSelectionData;
    // ... other variables

    [MenuItem("Window/SelectionData Editor")]
    public static void ShowWindow()
    {
        GetWindow<SelectionDataEditor>("SelectionData Editor");
    }

    // ... OnGUI and other methods
}
```

**Drawing the UI for Identifiers:**

```csharp
// Inside OnGUI method
if (currentSelectionData != null)
{
    // ...
    for (int i = 0; i < currentSelectionData.identifiers.Count; i++)
    {
        EditorGUILayout.BeginHorizontal();
        currentSelectionData.identifiers[i] = EditorGUILayout.TextField(currentSelectionData.identifiers[i]);
        if (GUILayout.Button("-", GUILayout.Width(25)))
        {
            // Remove identifier logic
            // Remember to use SerializedObject and SerializedProperty for robust undo/dirtying
        }
        EditorGUILayout.EndHorizontal();
    }
    // ... UI for adding new identifiers
}
```

## Troubleshooting

-   **Editor Window Not Appearing**: Ensure `SelectionDataEditor.cs` is in an `Editor` folder. Check for console errors.
-   **Changes Not Saving**: Verify that `EditorUtility.SetDirty()` is called on the `SelectionData` asset after modifications, and `AssetDatabase.SaveAssets()` is called if you need to force an immediate save to disk. For lists managed via `SerializedProperty`, `ApplyModifiedProperties()` is key.

## Future Improvements

-   Support for reordering identifiers via drag-and-drop.
-   Enhanced search and filtering capabilities for large lists of identifiers.
-   Batch operations (e.g., import/export identifiers from/to CSV).

---

*This documentation provides a general guide. Specific features and UI elements might vary based on the exact implementation of the `SelectionDataEditor.cs` script.*
