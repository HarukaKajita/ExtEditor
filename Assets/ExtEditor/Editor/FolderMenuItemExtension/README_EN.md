# Folder Menu Item Extension

## Overview

The Folder Menu Item Extension tool allows users to dynamically create custom menu items under `Assets/Create/Folders/`. These custom menu items can then be used to quickly generate predefined folder structures within the selected folder in the Unity Project window.

The tool works by processing special definition files with an `.fmi` extension. Each `.fmi` file describes a list of subfolders to be created and a priority for its corresponding menu item. A `ScriptedImporter` then reads these `.fmi` files and generates a C# script. This generated C# script is responsible for adding the actual menu item to Unity.

This system provides a flexible way to define common folder layouts without needing to write or modify C# code for each new folder structure.

## Core Components

-   **`.fmi` (Folder Menu Item) Files:**
    -   User-created text files that define a specific folder structure and menu priority.
    -   **Location:** Typically placed in an Editor folder, for example, `Assets/ExtEditor/Editor/FolderMenuItemExtension/Editor/FolderMenuItemPreset/`.
    -   **Format:**
        -   The **first line** must be an integer representing the menu item's display priority (lower numbers appear higher in the `Assets/Create/Folders/` submenu).
        -   Each **subsequent line** specifies a subfolder name. Nested folders can be defined using a forward slash `/` (e.g., `Art/Models/Characters`).
-   **`FolderMenuItemImporter.cs`:**
    -   A `ScriptedImporter` that automatically processes any `.fmi` file when it's added or modified in the project.
    -   It reads the `.fmi` file's content.
    -   It uses a template (`ScriptTemplate.txt`) to generate a new C# script (e.g., `MyFolders.fmi` would generate `MyFoldersFolderMenuItem.cs`).
    -   This generated script contains the logic to create the menu item and the specified folders.
-   **`ScriptTemplate.txt`:**
    -   A C# template file that the `FolderMenuItemImporter` uses to generate the actual menu item scripts. It must reside in the same directory as the `.fmi` files that use it.
-   **Generated C# Scripts (e.g., `[YourFMIFileName]FolderMenuItem.cs`):**
    -   These scripts are automatically created by the `FolderMenuItemImporter`.
    -   They contain the `[MenuItem]` attribute that adds an entry like `Assets/Create/Folders/[YourFMIFileName]`.
    -   When clicked, this menu item creates the folder structure (defined in the original `.fmi` file) under the currently selected folder(s) in the Project window.

## How to Use

1.  **Prepare the Preset Directory:**
    *   Ensure you have a directory for your `.fmi` files and the template, for example, `Assets/ExtEditor/Editor/FolderMenuItemExtension/Editor/FolderMenuItemPreset/`.
    *   Make sure `ScriptTemplate.txt` is present in this directory. The provided presets (`AllFolders.fmi`, `RuntimeAndEditor.fmi`) already have this structure.

2.  **Create or Modify an `.fmi` File:**
    *   Inside your chosen preset directory, create a new text file and change its extension to `.fmi` (e.g., `BasicProjectSetup.fmi`).
    *   **Edit the `.fmi` file:**
        *   **Line 1:** Enter an integer for the menu priority (e.g., `0` for high priority, `100` for lower). This determines its order within the `Assets/Create/Folders/` submenu.
        *   **Following Lines:** List each subfolder you want to create, one per line.
            ```fmi
            0
            Scripts
            Art
            Art/Materials
            Art/Prefabs
            Audio
            Scenes
            ```
    *   Save the `.fmi` file.

3.  **Automatic Script Generation:**
    *   Upon saving the `.fmi` file, Unity's asset importer will trigger the `FolderMenuItemImporter`.
    *   A new C# script (e.g., `BasicProjectSetupFolderMenuItem.cs`) will be generated in the same directory as your `.fmi` file.
    *   The new menu item `Assets/Create/Folders/BasicProjectSetup` will become available after a brief script compilation.

4.  **Using the Generated Menu Item:**
    *   In the Unity Project window, select the parent folder where you want to create the new folder structure.
    *   Right-click on the selected folder, navigate to `Create > Folders > BasicProjectSetup` (or the name corresponding to your `.fmi` file).
    *   Click on it. The defined subfolders will be created inside the selected parent folder.

5.  **Updating a Folder Structure:**
    *   Simply modify the content of your `.fmi` file and save it.
    *   The `FolderMenuItemImporter` will re-run, updating the associated generated C# script and thus the behavior of the menu item.

6.  **Removing a Custom Menu Item:**
    *   To completely remove a custom folder menu item, you **must delete both**:
        1.  The `.fmi` definition file (e.g., `BasicProjectSetup.fmi`).
        2.  The corresponding generated C# script (e.g., `BasicProjectSetupFolderMenuItem.cs`).
    *   If you only delete the `.fmi` file, the generated script will remain, and so will the menu item (though it won't be updatable).
    *   If you only delete the generated C# script, it will be regenerated the next time the `.fmi` file is re-imported (e.g., on project open or if the `.fmi` file is touched).

## Included Presets

The tool comes with a few example `.fmi` files in the `FolderMenuItemPreset` directory:
-   `AllFolders.fmi`: Creates a comprehensive set of common Unity project folders.
-   `RuntimeAndEditor.fmi`: Creates separate `Runtime` and `Editor` folders, a common pattern for organizing scripts.

Review their content and the `_USAGE.txt` file in that directory for more examples.
The original `CreateFoldersMenuItem.cs` file contains commented-out code from a previous, simpler implementation and is not actively used by the `.fmi` system.
