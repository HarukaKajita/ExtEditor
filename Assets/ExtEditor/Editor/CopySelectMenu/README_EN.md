# Copy Select Menu

## Overview

The Copy Select Menu tool enhances the Unity editor by adding convenient context menu options for managing asset selections. It allows users to easily copy asset paths or Globally Unique Identifiers (GUIDs) to the clipboard and, conversely, to select assets in the Project window based on a list of paths or GUIDs from the clipboard.

This tool is particularly useful for:
-   Quickly obtaining asset references for use in scripts or external documentation.
-   Sharing specific asset lists with team members.
-   Saving and restoring complex selections of assets.
-   Automating parts of asset management workflows.

## Features

The following menu items are added under the `Assets > Copy Select Menus` path in the main menu bar and also appear in the context menu when you right-click assets in the Project window:

### 1. Copy Path
-   **Functionality:** Copies the full project-relative path (e.g., `Assets/Folder/MyAsset.mat`) of each selected asset to the system clipboard.
-   **Multiple Selections:** If multiple assets are selected, their paths are copied, with each path on a new line.
-   **Feedback:** A confirmation message listing the copied paths is logged to the Unity Console.

### 2. Copy GUID
-   **Functionality:** Copies the unique GUID of each selected asset to the system clipboard.
-   **Multiple Selections:** If multiple assets are selected, their GUIDs are copied, with each GUID on a new line.
-   **Feedback:** A confirmation message listing the copied GUIDs is logged to the Unity Console.

### 3. Select with Path from Clipboard
-   **Functionality:** Reads a newline-separated list of asset paths from the system clipboard. It then attempts to locate and select these assets in the Project window.
-   **Error Handling:** Paths that do not correspond to any asset in the project are ignored.
-   **Feedback:** A confirmation message listing the paths used for selection is logged to the Unity Console.

### 4. Select with GUID from Clipboard
-   **Functionality:** Reads a newline-separated list of asset GUIDs from the system clipboard. It then attempts to locate and select these assets in the Project window.
-   **Error Handling:** GUIDs that do not correspond to any asset in the project are ignored.
-   **Feedback:** A confirmation message listing the GUIDs used for selection is logged to the Unity Console.

## How to Use

### Copying Asset Information:
1.  **Select Asset(s):** In the Unity Project window, select one or more assets whose information you want to copy.
2.  **Access Menu:**
    *   Right-click on any of the selected assets to open the context menu, then navigate to `Copy Select Menus`.
    *   Alternatively, with the asset(s) selected, go to the main Unity menu bar and click `Assets > Copy Select Menus`.
3.  **Choose Action:**
    *   Click `Copy Path` to copy asset paths.
    *   Click `Copy GUID` to copy asset GUIDs.
4.  The selected information is now available on your system clipboard.

### Selecting Assets from Clipboard:
1.  **Prepare Clipboard:** Ensure your system clipboard contains the asset paths or GUIDs you wish to select. Each path or GUID should be on a new line. (You might have this data from a previous "Copy" operation, a text file, etc.)
2.  **Access Menu:**
    *   Right-click anywhere in the Project window (or on any asset) to open the context menu, then navigate to `Copy Select Menus`.
    *   Alternatively, go to the main Unity menu bar and click `Assets > Copy Select Menus`.
3.  **Choose Action:**
    *   Click `Select with Path from Clipboard` if your clipboard contains asset paths.
    *   Click `Select with GUID from Clipboard` if your clipboard contains asset GUIDs.
4.  The tool will attempt to find and select the corresponding assets in your Project window.

This tool does not have its own editor window and relies solely on these menu integrations.
