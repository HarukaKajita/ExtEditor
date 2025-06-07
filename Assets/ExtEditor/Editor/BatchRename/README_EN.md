# Batch Renamer

## Overview

The Batch Renamer is a Unity editor tool that allows you to rename multiple assets in your project simultaneously. This is useful for organizing your assets and maintaining a consistent naming convention.

The tool provides the following renaming options:
- Add a prefix to filenames.
- Add a suffix to filenames.
- Search for a specific string within filenames and replace it with another string.

## How to Use

1.  **Select Assets:** In the Unity Project window, select all the assets you wish to rename.
2.  **Open Batch Renamer:** Navigate to `Tools > Batch Renamer` in the Unity editor menu bar to open the Batch Renamer window.
3.  **Configure Renaming Options:**
    *   **Prefix:** Enter the text you want to add to the beginning of each selected asset's name.
    *   **Suffix:** Enter the text you want to add to the end of each selected asset's name (before the file extension).
    *   **Search String:** If you want to replace a part of the filenames, enter the text you're searching for here.
    *   **Replacement String:** Enter the text that will replace the `Search String`. If `Search String` is empty, this field will be ignored.
4.  **Preview Changes:** The Batch Renamer window will display a preview of how the asset names will look after applying the current settings. Review this preview to ensure the changes are as expected.
5.  **Apply Renaming:** Once you are satisfied with the preview, click the "Apply Renaming to Selected Assets" button. The selected assets will be renamed according to your configuration.
6.  **Save Settings (Optional):**
    *   You can save your current renaming configuration (Prefix, Suffix, Search String, Replacement String) by clicking the "Save Settings" button.
    *   These settings are saved as a `BatchRenamerSettings.asset` file in the `Assets/ExtEditor/Editor/BatchRename/` directory. The tool will automatically load these settings the next time you open it.

**Note:** Always be cautious when batch renaming files. It's a good practice to back up your project or use version control before making large-scale changes.
