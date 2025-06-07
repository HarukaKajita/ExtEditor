# Export Package Config

## Overview

The Export Package Config tool provides a way to define and manage configurations for exporting custom Unity packages (`.unitypackage`). It utilizes ScriptableObjects, where each asset holds a specific set of rules for determining which assets to include or exclude, how to name the output file, and where to save it.

This tool is designed to simplify and standardize the process of packaging project assets, whether for sharing with team members, distributing on the Asset Store, or creating backups. By creating different configuration assets, you can easily re-export specific sets of assets consistently.

## Key Features

-   **ScriptableObject-Based Configurations:**
    -   Create multiple `ExportPackageConfig` assets to manage various export scenarios (e.g., "CharacterModels_Export", "UI_Elements_Export").
    -   To create a new configuration: In the Project window, right-click and select `Create > Scriptableobject > ExportPackageConfig`.
-   **Dynamic Export Path:**
    -   Define a flexible export path and filename using placeholders:
        -   `<ProjectPath>`: The root directory of your Unity project (the parent of the `Assets` folder).
        -   `<AssetsPath>`: The `Assets` folder path.
        -   `<ConfigName>`: The name of the `ExportPackageConfig` ScriptableObject asset itself.
        -   `<Date>`: The current date in `yyyyMMdd` format.
        -   `<Time>`: The current time in `HHmmss` format.
    -   Example default: `<ProjectPath>/ExportedPackages/<ConfigName>_<Date>.unitypackage`
-   **Asset Inclusion Rules:**
    -   **Export Entry Assets (`exportEntryAssets`):** A list where you can drag and drop the primary assets or folders you intend to export. If a folder is added, all assets within that folder are considered entry points.
    -   **Include Dependencies (`includeDependencies`):** A boolean toggle (defaults to true). When enabled, Unity will automatically trace and include all assets that your specified `exportEntryAssets` (and assets within any specified folders) depend on.
-   **Asset Exclusion Rules:**
    -   **Exclude Assets (`excludeAssets`):** A list where you can drag and drop assets or folders that should be explicitly removed from the export list, even if they are part of an included folder or are dependencies of other included assets.
-   **Automatic Filtering:**
    -   Assets located under the `Packages/` directory (i.e., those managed by the Unity Package Manager) are automatically excluded from the export.
-   **Custom Inspector Interface:**
    -   When you select an `ExportPackageConfig` asset in the Project window, its Inspector provides:
        -   Fields to edit the `exportPathExpression`, `exportEntryAssets`, and `excludeAssets`.
        -   A button displaying the resolved export path, which also acts as a shortcut to open the target directory in your system's file explorer (the directory will be created if it doesn't exist).
        -   The main "Export unitypackage Package" button to initiate the export process.
        -   A detailed preview section:
            -   **"Entry Assets List":** Shows assets you directly specified or that are within the folders you specified.
            -   **"Dependency Assets List":** Shows assets included due to the "Include Dependencies" setting.
            -   All asset paths in these lists are clickable, allowing you to ping the asset in the Project window or select it.
        -   A "Select All" button to select all assets (both entry and dependent) that are currently determined to be part of the export.
-   **Interactive Export Process:**
    -   Clicking the export button triggers the standard Unity "Exporting package" dialog, pre-populated with the assets based on your configuration. This allows for a final review and modification before the `.unitypackage` file is created.

## How to Use

1.  **Create an Export Configuration:**
    *   In the Unity Project window, right-click.
    *   Navigate to `Create > Scriptableobject > ExportPackageConfig`.
    *   Give the newly created ScriptableObject asset a descriptive name (e.g., `MyGame_CoreSystem_Config`).

2.  **Configure the Export Settings:**
    *   Select your `ExportPackageConfig` asset. Its settings will appear in the Inspector.
    *   **出力するパッケージのパス (Export Path Expression):**
        *   Modify the `exportPathExpression` field. Use the available placeholders (like `<ConfigName>`, `<Date>`) to define the naming convention and output directory for your package.
        *   The button below this field (labeled "エクスプローラーで開く..." followed by the resolved path) allows you to verify the path and open the containing folder.
    *   **依存アセットを含める (Include Dependencies):**
        *   Check this box (default) to automatically include all dependencies of your entry assets. Uncheck it if you only want the explicitly listed assets.
    *   **パッケージに含めるアセット (Export Entry Assets):**
        *   Drag and drop the specific assets (prefabs, materials, scripts, scenes, etc.) or entire folders from your Project window into this list. These are the starting points for the export.
    *   **パッケージから除外するアセット (Exclude Assets):**
        *   Drag and drop any assets or folders into this list that you want to ensure are NOT included in the package, even if they are dependencies or within an "Export Entry Assets" folder.

3.  **Review the Asset List:**
    *   In the Inspector, scroll down to the preview section.
    *   **"エントリーアセット一覧" (Entry Assets List):** Review the assets that are directly included based on your `exportEntryAssets` list.
    *   **"依存アセット一覧" (Dependency Assets List):** If "Include Dependencies" is active, review the assets pulled in automatically.
    *   You can click on any asset path in these lists to highlight it in the Project window.
    *   Use the **"全選択" (Select All)** button to select all currently listed assets in the Project window if you want a broader overview.

4.  **Export the Package:**
    *   Click the **"unitypackageをパッケージを書き出す" (Export unitypackage Package)** button in the Inspector.
    *   The standard Unity "Exporting package" window will appear. This window will be pre-filled with all the assets identified by your configuration (entry assets + dependencies - excluded assets).
    *   You can make final adjustments in this dialog if needed (e.g., unchecking an item).
    *   Click the "Export..." button in the dialog.
    *   A file save dialog will appear, prompting you for the location and name of the `.unitypackage` file (it will default to what your `exportPathExpression` resolved to).
    *   Choose your location and click "Save."
    *   A confirmation message ("UnityPackageを書き出しました: [path]") will be logged in the Unity Console.

This tool provides a structured and reusable method for creating `.unitypackage` files, ensuring consistency and saving time.
