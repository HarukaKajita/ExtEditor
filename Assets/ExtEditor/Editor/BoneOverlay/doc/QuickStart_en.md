# BoneOverlay Quick Start Guide

This guide explains how to get started with the BoneOverlay tool in the shortest time possible.

## 📋 Prerequisites

- Unity 2022.3 or later installed
- A 3D model (with SkinnedMeshRenderer) placed in the scene

## 🚀 Get Started in 3 Steps

### Step 1: Show the Toolbar

Click the "⋮" menu in the top-right of Scene View, then check "Overlays" → "Bone Overlay Toolbar".

[Image: Overlays menu operation]

### Step 2: Enable Bone Display

Click the "Bones" button that appears in the Scene View toolbar.

[Image: Clicking the Bones button]

### Step 3: Select Bones

Click on the displayed bone spheres or labels to select them.

[Image: Selecting a bone]

## ✨ That's It!

That's all you need to know for basic usage. Bones are now visually displayed and easily selectable.

## 📝 Common Operations

### Multiple Selection
- **Shift + Click**: Add to selection
- **Ctrl/Cmd + Click**: Toggle add/remove from selection

### Display Adjustment (Optional)
Click the "▼" next to the "Bones" button to open the settings menu:

- **Sphere Size**: Adjust bone sphere size
- **Show Labels**: Toggle object name display
- **Bone Distance**: Maximum display distance

[Image: Settings menu]

## 💡 Helpful Tips

1. **Change to Visible Colors**: If the background is dark, adjust colors to be brighter in the settings menu
2. **Labels Are Clickable**: You can click not only spheres but also object name labels
3. **Distance Filter**: In large scenes, set a shorter distance to show only necessary parts

## ❓ Troubleshooting

### Bones Not Showing
→ Check if there's a model with SkinnedMeshRenderer in the scene

### Cannot Select
→ Verify Scene View is in the appropriate mode (Move/Rotate/Scale tools, etc.)

### Heavy/Laggy Performance
→ Try setting "Bone Distance" shorter (e.g., 10m) in the settings menu

---

For detailed features and settings, please refer to the [Complete Manual](README_en.md).