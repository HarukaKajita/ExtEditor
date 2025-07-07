# BoneOverlay - Bone Visualization & Selection Tool

## Overview

BoneOverlay is a Unity Editor extension that visualizes bones (joints) in the Scene View, making selection and manipulation easier. It helps visualize typically invisible bone objects during rigging and animation work, supporting efficient workflows for 3D artists and developers.

### Key Features

- 🎯 **Intuitive Bone Selection** - Simply click on disc-shaped markers or labels to select bones
- 👁️ **Smart Auto-Detection** - Automatically detects bones from SkinnedMeshRenderer, Animator, and name patterns
- 🎨 **Customizable Display** - Fine-tune colors, sizes, distances, and more
- ⚡ **High Performance** - Optimized design for smooth operation even with 1000+ bones
- 🔧 **Unity Standard UI Integration** - Seamlessly integrated into Scene View toolbar
- 🐛 **Multi-Selection Support** - Fixed and improved multi-selection behavior (v1.0.1)

## System Requirements

- Unity 2022.3 or later
- Compatible with all render pipelines (Built-in, URP, HDRP)
- Editor-only (does not work at runtime)

## Installation

1. Open Unity Package Manager
2. Select "Add package from disk"
3. Select the `package.json` file to import

Alternatively, copy directly to your Assets folder.

## How to Use

### Basic Usage

1. **Enable the Toolbar**
   - Click the "⋮" menu (three dots) in the top-right corner of Scene View
   - Navigate to "Overlays" → Check "Bone Overlay Toolbar"
   - The toolbar will appear in your Scene View

2. **Enable Bone Display**
   - Click the "Bones" button (avatar mask icon) in the Scene View toolbar
   - The icon will turn blue when active, indicating bone visualization is enabled
   - Bones will appear as colored disc markers in the scene

3. **Select Bones**
   - **Single Selection**: Click on any disc marker or label to select a bone
   - **Add to Selection**: Hold Shift and click to add bones to current selection
   - **Toggle Selection**: Hold Ctrl/Cmd and click to add/remove from selection
   - Selected bones show in a different color (yellow by default)

### Customizing Settings

Click the dropdown arrow "▼" next to the "Bones" button to access detailed settings.

#### Bone Settings

- **Bone Distance** (1-100m) - Maximum distance to display bone markers
  - Default: 50m (suitable for most scenes)
  - Reduce for better performance in complex scenes
  
- **Sphere Size** (1-100mm) - Size of bone disc markers
  - Default: 5mm
  - Increase for better visibility in large scenes
  
- **Line Width** (0.1-10) - Thickness of parent-child connection lines
  - Default: 2.0
  - Thicker lines are easier to see but may clutter the view

- **Color Settings** - Customize colors for different states
  - **Normal Color**: Default disc color (green)
  - **Selected Color**: Color when bone is selected (yellow)
  - **Hover Color**: Color when hovering over bone (cyan)
  - **Line Color**: Parent-child connection line color (gray)

#### Label Settings

- **Show Labels** - Toggle bone name display on/off
- **Label Distance** (1-100m) - Maximum distance to show labels
  - Default: 30m (shorter than bone distance for clarity)
  
- **Label Size** (5-30pt) - Font size for bone names
  - Default: 10pt
  
- **Label Color** - Text color for labels
  - Default: Light blue for good contrast

#### Quick Actions

- **Reset to Defaults** - Restore all settings to default values
- **Detected Bones** - Shows count of currently detected bones

## Use Cases

### VRChat Avatar Creation

- Adjust avatar bone structure while viewing
- Easily select bones when setting up PhysBones
- Visually confirm facial bones for expression animations

[Image: Example usage with VRChat avatar]

### 3D Character Animation

- Streamline bone selection during animation creation
- Visually confirm bone chains when setting up IK
- Quickly identify rigging issues

[Image: Example usage in animation work]

### Model Debugging

- Verify bone structure of imported models
- Discover unnecessary bones or naming issues
- Better understand hierarchy structure

## Advanced Usage

### Utilizing Distance Filtering

In large scenes, adjust distance filtering to show only necessary bones:

- Set shorter distances for detailed close-up work
- Set longer distances for overview perspectives
- Configure separate distances for labels and bones

[Image: Comparison showing distance filtering effects]

### Color Customization

Customize colors based on your work context for improved visibility:

- Use brighter colors in dark scenes
- Use vibrant colors to highlight specific bones
- Adjust transparency as needed

### Performance Optimization

When working with many bones:

- Set minimum necessary distance
- Turn off label display for lighter operation
- Focus only on working area

## Troubleshooting

### Bones Not Displaying

1. **Check Toggle State**: Ensure the "Bones" button is highlighted (blue) in the toolbar
2. **Verify Distance**: Default is 50m - increase if working with large scenes
3. **Check Components**: Bones are detected from:
   - SkinnedMeshRenderer components (mesh bones)
   - Animator components (rig bones)
   - Objects with bone-related names (bone, joint, jnt, etc.)
4. **Scene View Mode**: Make sure you're in Scene View, not Game View

### Selection Issues

1. **Multi-Selection Not Working**: 
   - Ensure you're holding Shift/Ctrl BEFORE clicking
   - Update to v1.0.1 or later for fixed multi-selection
   
2. **Cannot Click Bones**:
   - Check if GameObject is not locked in the Inspector
   - Try clicking the label if the disc is hard to hit
   - Increase Sphere Size in settings for easier clicking

3. **Wrong Selection Color**:
   - Force refresh with Scene View repaint (move camera slightly)
   - Toggle the tool off and on again

### Performance Issues

1. **Too Many Bones Causing Lag** (100+ bones):
   - Reduce "Bone Distance" to 10-20m
   - Turn off "Show Labels" 
   - Focus on specific area using camera

2. **Slow Scene View**:
   - Bones are culled outside camera view automatically
   - Consider hiding unused GameObjects
   - Distance fade helps with performance

### Debug Mode

For developers experiencing issues, enable debug logging:
1. Add `BONE_OVERLAY_DEBUG` to Player Settings → Scripting Define Symbols
2. Check Console for selection operation logs

## FAQ

**Q: Does it display in Game View?**
A: No, this is a Scene View-only tool.

**Q: Can I use it at runtime?**
A: This is an editor-only feature and is not included in builds.

**Q: Can I display only specific bones?**
A: Current version displays all auto-detected bones.

**Q: Can I add custom bone detection patterns?**
A: Current version uses fixed patterns, but this is planned for future updates.

## Update History

### Version 1.0.1 (2025-01-07)
- Fixed multi-selection bug where deselection wasn't working properly
- Improved selection state synchronization with Hierarchy view
- Added immediate visual feedback for selection changes
- Added debug logging support with `BONE_OVERLAY_DEBUG` symbol

### Version 1.0.0 (2025-01-05)
- Initial release
- EditorToolbarDropdownToggle-based implementation
- Distance-based filtering with separate bone/label distances
- Comprehensive color customization
- Interactive label clicking support
- Disc-based bone visualization for better visibility

## Technical Details

- **Rendering**: Uses Unity Handles API with disc markers
- **Performance**: Optimized with frustum culling and distance-based LOD
- **Compatibility**: Works with all Unity render pipelines
- **Memory Usage**: Approximately 10KB per 100 bones

## Support

For questions, bug reports, or feature requests:

- GitHub Issues: [Report issues on the repository]
- Unity Forum: [Discussion thread]
- Email: support@exteditor.com

## License

This tool is provided under the MIT License.
See LICENSE file for full details.

---

© 2025 ExtEditor Tools. All rights reserved.