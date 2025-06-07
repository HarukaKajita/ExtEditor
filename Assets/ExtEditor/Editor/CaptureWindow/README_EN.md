# Capture Window

## Overview

The Capture Window is a Unity editor tool designed to take screenshots from various sources within the editor and save them as PNG files. This tool is useful for creating promotional materials, debugging visual issues, or simply capturing moments from your game or scene.

### Key Features:

-   **Multiple Capture Sources:**
    -   **Game View:** Captures the output of a selected camera as it would appear in the Game View, using the Game View's resolution.
    -   **Scene View:** Captures the current visual state of the last active Scene View editor window.
    -   **Render Texture:** Allows capturing directly from a specified `RenderTexture` asset.
-   **Flexible Camera Selection:**
    -   Choose from a dropdown list of all cameras present in the current scene.
    -   Directly assign a `Camera` component via an object field.
-   **Customizable Output:**
    -   Specify a custom output directory for saved images (paths can be relative to the `Assets` folder or the project root).
    -   Option to include an alpha channel in the PNG, enabling transparency.
    -   Option to attempt rendering with a transparent background.
-   **Convenient File Management:**
    -   Files are automatically named with a "Capture_" prefix and a timestamp (e.g., `Capture_YYYYMMDD_HHMMSS.png`).
    -   A button to directly open the output folder in the system's file explorer.

## How to Use

1.  **Open the Tool:**
    Navigate to `Tools > CaptureWindow` in the Unity editor menu bar to open the Capture Window.

2.  **Configure Capture Settings:**

    *   **Capture Source:** Select your desired source:
        *   `GameView`: Captures from the specified `Target Camera` using the Game View's current rendering resolution.
        *   `SceneView`: Captures the content of the last active Scene View. The `Target Camera` setting influences how the scene is rendered for capture (e.g. its clear flags).
        *   `RenderTexture`: If selected, an object field will appear. Drag your desired `RenderTexture` asset into this "Render Texture" field.
    *   **Scene Cameras / Target Camera:**
        *   **Scene Cameras (Dropdown):** Select a camera from the populated list of cameras in your current scene. "None" is also an option.
        *   **Target Camera (Object Field):** Alternatively, drag a `Camera` object from your scene hierarchy or Project window directly into this field.
        *   *Note:* These two fields are synchronized. A camera must be assigned to `Target Camera` for most captures to function correctly. The tool attempts to default to `Camera.main` or the first available camera.
    *   **Output Directory:**
        *   Enter the path where the captured PNGs will be saved.
        *   Example: `Assets/Screenshots` (saves inside the Assets folder).
        *   Example: `../ProjectCaptures` (saves in a folder named `ProjectCaptures` at the same level as the `Assets` folder, i.e., project root). The default is `../Captures`.
    *   **Include Alpha Channel:**
        *   Check this box to save the PNG with an alpha channel (RGBA32 format). This is necessary for transparency.
        *   If unchecked, the image is saved without alpha (RGB24 format).
    *   **Transparent Background:**
        *   Check this box if you want the capture to have a transparent background (where applicable, e.g., if not rendering a skybox).
        *   This works by temporarily setting the `Target Camera`'s clear flags to `SolidColor` and its background color to fully transparent during the capture process. The camera's original settings are restored afterward.

3.  **Perform Capture:**
    *   Click the **"Capture and Save PNG"** button.
    *   The image will be rendered based on your settings and saved to the specified `Output Directory`.
    *   A confirmation message, including the file path, will be logged in the Unity Console.

4.  **Access Output:**
    *   Click the **"Open Output Folder"** button to quickly open the designated output directory in your operating system's file explorer.

### Important Notes:

-   Ensure a valid camera is selected in the "Target Camera" field, especially when not using the `RenderTexture` source.
-   The "Transparent Background" option works best when the camera is not rendering a skybox or a solid opaque background.
-   The resolution of the Game View capture is determined by the Game View's current resolution settings.
-   The resolution of the Scene View capture is determined by the pixel dimensions of the Scene View window.
