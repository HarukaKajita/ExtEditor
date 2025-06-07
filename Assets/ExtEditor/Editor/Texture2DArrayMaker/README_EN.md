# Texture2D Array Maker

## Overview

The Texture2D Array Maker is a ScriptableObject-based utility for Unity that simplifies the creation and updating of `Texture2DArray` assets. A `Texture2DArray` is a GPU resource that holds multiple 2D textures (slices) of the same size and format within a single texture object. This is commonly used for optimizing rendering by reducing texture swaps, for techniques like texture atlases for animation, terrain splat maps, or material variations.

This tool allows you to define a list of input `Texture2D` assets and then generate or update a `Texture2DArray` from them.

## Key Features

-   **ScriptableObject Configuration:**
    -   Each `Texture2DArrayMaker` asset you create in your project holds the configuration for generating a specific `Texture2DArray`.
    -   Create new configurations via `Assets > Create > Texture2DArrayMaker`.
-   **Input Texture List:**
    -   Specify a list of `Texture2D` assets that will become the slices of the `Texture2DArray`.
-   **Output `Texture2DArray`:**
    -   Optionally assign an existing `Texture2DArray` asset to be updated.
    -   If left unassigned, a new `Texture2DArray` asset will be created.
-   **Validation:**
    -   Before creation, the tool checks if all input textures:
        -   Have the same width and height.
        -   Have the same `TextureFormat`.
    -   Warnings are logged to the console if validation fails.
-   **Creation and Update:**
    -   A context menu item "Create or Update Texture2DArray" on the `Texture2DArrayMaker` asset triggers the process.
    -   If no output asset is specified, a new `Texture2DArray` is created in the same directory as the maker asset, named `[MakerAssetName]_TexArray.asset`.
    -   If an output asset is specified, its contents are updated with the new array data.
    -   The pixel data from each input texture is copied to a corresponding slice in the `Texture2DArray`.

## How to Use

1.  **Create a `Texture2DArrayMaker` Asset:**
    *   In your Unity Project window, right-click.
    *   Navigate to `Create > Texture2DArrayMaker`.
    *   Give the newly created ScriptableObject asset a descriptive name (e.g., `FlameAnimation_ArrayMaker`).

2.  **Configure the Maker Asset:**
    *   Select the `Texture2DArrayMaker` asset you just created.
    *   In the Inspector:
        *   **Input Textures:** Drag and drop the `Texture2D` assets you want to include as slices into this list.
            *   **Important:** All textures in this list **must** have the same dimensions (width and height) and the same `TextureFormat`. The tool will use the first texture in the list as the reference for these properties.
        *   **Output Texture (Optional):**
            *   If you want to update an existing `Texture2DArray` asset, drag it into this field.
            *   If you want to create a new `Texture2DArray` asset, leave this field empty (None).

3.  **Generate or Update the `Texture2DArray`:**
    *   Ensure the `Texture2DArrayMaker` asset is selected in the Project window.
    *   Right-click on this selected `Texture2DArrayMaker` asset.
    *   From the context menu that appears, choose **"Create or Update Texture2DArray"**.

4.  **Check the Results:**
    *   If the "Output Texture" field was empty:
        *   A new `Texture2DArray` asset will be created in the same folder as your `Texture2DArrayMaker` asset. Its name will be based on the maker's name (e.g., `FlameAnimation_ArrayMaker_TexArray.asset`).
        *   The "Output Texture" field on your `Texture2DArrayMaker` asset will now be automatically populated with a reference to this newly created array.
    *   If the "Output Texture" field pointed to an existing `Texture2DArray`:
        *   That asset's content will be updated with the new slices from your input textures.
    *   Look at the Unity Console for any warning messages, especially if the input textures did not meet the validation criteria (same size and format).

By using this tool, you can easily manage and regenerate `Texture2DArray` assets as your source textures change, which is useful for various advanced rendering techniques and optimizations.
