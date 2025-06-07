# BlendShape Fix Postprocessor

## Overview

The `BlendShapeFixPostProcessor` is an automated Unity editor tool that runs when 3D models are imported or re-imported into the project. Its primary function is to ensure that the frames within each blend shape are correctly sorted by their weight.

In some 3D modeling workflows or with certain file formats, blend shape frames might be exported in an incorrect order (e.g., a frame intended for 100% influence appearing before a frame for 50% influence). This can lead to incorrect interpolation or visual artifacts when these blend shapes are animated or manipulated in Unity.

This postprocessor script automatically inspects all `SkinnedMeshRenderer` components in an imported model. For each blend shape, it checks if its frames are sorted by weight. If an incorrectly sorted blend shape is found, the script will:
1.  Read all frame data (weight, delta vertices, delta normals, delta tangents).
2.  Sort the frames based on their weight in ascending order.
3.  Clear the existing (incorrect) blend shape data from the mesh.
4.  Re-add the blend shape frames to the mesh in the correct order.

## How to Use

This tool operates automatically. There are no manual steps required to use it beyond ensuring it is part of your project.

1.  **Integration:** Make sure the `BlendShapeFixPostProcessor.cs` script is located within an `Editor` folder in your Unity project, typically `Assets/ExtEditor/Editor/BlendShapeFixpostProcessor/`.
2.  **Import Models:** When you import a new 3D model (e.g., FBX, DAE, OBJ) that contains blend shapes, or when you re-import an existing model, this script will automatically trigger as part of Unity's asset import pipeline.
3.  **Automatic Correction:** If the script detects any blend shapes with frames that are not sorted by weight, it will automatically correct the order. This process is generally silent. A `Debug.Log` message indicating that a fix has been applied is commented out in the script but can be enabled for diagnostic purposes if needed.

There are no user-configurable settings for this tool. It is designed to be a "set it and forget it" solution to a common blend shape import problem.
