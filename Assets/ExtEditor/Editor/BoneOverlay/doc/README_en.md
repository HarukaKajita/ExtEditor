# BoneOverlay - Bone Visualization & Selection Tool

## Overview

BoneOverlay is a Unity Editor extension that visualizes bones (joints) in the Scene View, making selection and manipulation easier. It helps visualize typically invisible bone objects during rigging and animation work, supporting efficient workflows for 3D artists and developers.

### Key Features

- 🎯 **Intuitive Bone Selection** - Simply click on spheres or labels to select bones
- 👁️ **Smart Auto-Detection** - Automatically detects bones from SkinnedMeshRenderer, Animator, and name patterns
- 🎨 **Customizable Display** - Fine-tune colors, sizes, distances, and more
- ⚡ **High Performance** - Optimized design for smooth operation even with numerous bones
- 🔧 **Unity Standard UI Integration** - Seamlessly integrated into Scene View toolbar

[Image: Screenshot showing bones visualized in Scene View]

## System Requirements

- Unity 2022.3 or later
- Universal Render Pipeline (URP) compatible
- Editor-only (does not work at runtime)

## Installation

1. Open Unity Package Manager
2. Select "Add package from disk"
3. Select the `package.json` file to import

Alternatively, copy directly to your Assets folder.

## How to Use

### Basic Usage

1. **Enable the Toolbar**
   - Click the "⋮" menu in the top-right of Scene View
   - Check "Overlays" → "Bone Overlay Toolbar"
   
   [Image: Screenshot showing Overlays menu operation]

2. **Enable Bone Display**
   - Click the "Bones" button in the Scene View toolbar
   - The icon will highlight and bone visualization begins
   
   [Image: Toolbar with Bones button enabled]

3. **Select Bones**
   - Click on displayed spheres or labels to select bones
   - Hold Shift/Ctrl while clicking for multiple selection
   
   [Image: Animated GIF showing bone selection]

### Customizing Settings

Click the "▼" next to the "Bones" button to open the detailed settings menu.

[Image: Dropdown menu opened]

#### Bone Settings

- **Bone Distance** - Maximum distance to display bones (1-100m)
- **Sphere Size** - Size of bone spheres (1-100mm)
- **Line Width** - Thickness of parent-child lines (0.1-10)
- **Color Settings**
  - Normal Color - Default color
  - Selected Color - Color when selected
  - Hover Color - Color when hovering
  - Line Color - Connection line color

#### Label Settings

- **Show Labels** - Toggle object name labels
- **Label Distance** - Maximum distance to display labels (1-100m)
- **Label Size** - Font size for labels (5-30pt)
- **Label Color** - Label text color

[Image: Comparison showing different settings]

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

1. Check if "Bones" toggle is enabled in Scene View
2. Verify distance settings are appropriate (default: 50m)
3. Confirm objects have SkinnedMeshRenderer or Animator components

### Cannot Select

1. Check if bones are not locked
2. Verify no other tools are interfering
3. Ensure Scene View is in appropriate mode

### Performance Issues

1. Reduce display distance
2. Turn off label display
3. Hide unnecessary objects

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

### Version 1.0.0 (2025-01-05)
- Initial release
- EditorToolbarDropdownToggle-based implementation
- Distance-based filtering
- Color customization features
- Label interaction support

## Support

For questions or bug reports, please contact:

- Email: [Support email address]
- Discord: [Discord server link]
- Twitter: [@Username]

## License

This tool is provided under [License Name] license.
Please check the LICENSE file for details.

---

© 2025 [Creator Name]. All rights reserved.