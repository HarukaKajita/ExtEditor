# Material Property Copier

## Overview

The Material Property Copier is a Unity editor tool designed to streamline the process of transferring shader property values from one source material to one or more target materials. It allows users to selectively copy properties like colors, textures, float values, and vectors, and then paste them onto other materials.

This tool is particularly useful when:
-   You need to apply a consistent look across multiple materials that share the same shader or similar shader properties.
-   You want to quickly set up a new material based on an existing one, and then tweak it.
-   You are iterating on a material's appearance and want to propagate changes to other related materials efficiently.

## Key Features

-   **Source Material Selection:** Designate a single material as the source from which properties will be read.
-   **Selective Property Copying:**
    -   Lists all compatible shader properties (Colors, Vectors, Floats/Ranges, Textures) from the source material.
    -   Allows individual selection of properties to be copied via checkboxes.
    -   Includes "Select All" and "Select None" buttons for quick selection changes.
-   **Multiple Target Materials:**
    -   Supports applying copied properties to several target materials simultaneously.
    -   Target materials can be added to a list via:
        -   Selection in the Project window ("Add Selected Materials from Project" button).
        -   Dragging and dropping material assets onto a designated area in the tool's UI.
    -   Individual target materials can be removed from the list.
-   **Copy and Paste Workflow:**
    -   A dedicated "Copy Selected Properties from Source" button caches the selected property values.
    -   A "Paste Copied Properties to Target(s)" button applies these cached values to all materials in the target list.
-   **Property Matching Logic:**
    -   Properties are matched by their internal shader property names.
    -   The tool performs a basic type check: it will only paste a property if the type (e.g., Color, Float, Texture) matches between the copied property and the property on the target material.
-   **Feedback and Error Handling:**
    -   Dialog boxes confirm successful copy and paste operations.
    -   Warnings are logged to the Unity Console if:
        -   A target material does not possess a property that was copied.
        -   There's a type mismatch for a property between the source and a target material.
-   **Undo Functionality:** The paste operation is registered with Unity's Undo system, allowing changes to be easily reverted (Ctrl+Z / Cmd+Z).

## How to Use

1.  **Open the Tool:**
    *   Navigate to `Tools > Material Property Copier` in the Unity editor menu bar. This will open the "Material Property Copier" window.

2.  **Assign the Source Material:**
    *   In the "Source Material" section of the tool window, drag a Material asset from your Project window into the object field.

3.  **Select Properties for Copying:**
    *   Once a source material is assigned, the "Shader Properties" list will populate with its available properties.
    *   Use the checkboxes next to each property name to select the ones you want to copy.
    *   You can use the "Select All" or "Select None" buttons for faster selection.

4.  **Copy the Selected Properties:**
    *   Click the **"Copy Selected Properties from Source"** button.
    *   A confirmation dialog will appear indicating how many properties were successfully copied to the tool's internal buffer.

5.  **Add Target Materials:**
    *   You can add materials to the "Target Materials" list using either of these methods:
        *   **Project Selection:** Select one or more Material assets in your Unity Project window. Then, in the Material Property Copier window, click the **"Add Selected Materials from Project"** button.
        *   **Drag and Drop:** Drag Material assets directly from your Project window and drop them onto the area labeled **"Drag & Drop Materials Here"** within the tool window.
    *   The added materials will appear in a list. You can remove any material from this list by clicking the "X" button next to its entry.

6.  **Paste Properties to Target Materials:**
    *   Once you have your desired target materials listed and properties copied, click the **"Paste Copied Properties to Target(s)"** button.
    *   The tool will iterate through each target material and attempt to apply the copied property values.
    *   A confirmation dialog will indicate how many materials were affected.
    *   **Important:** Check the Unity Console for any warnings. Warnings might indicate that certain properties could not be pasted to specific materials (e.g., if a target material uses a different shader that lacks a copied property, or if property types don't match).

7.  **Undo (If Necessary):**
    *   If the results are not what you expected, you can use Unity's standard undo command (Edit > Undo, or Ctrl+Z / Cmd+Z) to revert the changes made to the target materials.

This tool helps maintain consistency and speeds up the material setup workflow when dealing with multiple materials that share common characteristics.
