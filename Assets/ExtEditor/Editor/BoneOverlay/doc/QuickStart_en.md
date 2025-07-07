# BoneOverlay Quick Start Guide

Get started with BoneOverlay in under 2 minutes! 🚀

## 📋 Prerequisites

- Unity 2022.3 or later
- A scene with any 3D model (character, avatar, etc.)

## 🎯 3 Simple Steps

### Step 1: Enable the Toolbar

Open Scene View and click the **⋮** (three dots) menu in the top-right corner.

Navigate to: **Overlays** → **Bone Overlay Toolbar** ✓

![Enable Toolbar](images/enable-toolbar.png)

### Step 2: Turn On Bone Display

Click the **Bones** button in your Scene View toolbar. It will turn blue when active.

![Bones Button](images/bones-button.png)

✨ **That's it!** You should now see colorful disc markers on all bones in your scene.

### Step 3: Start Selecting!

- **Click** any disc or label to select a bone
- **Shift+Click** to add more bones to selection
- **Ctrl/Cmd+Click** to toggle selection

![Selecting Bones](images/selecting-bones.gif)

## 🎨 Quick Customization

Need different colors or sizes? Click the **▼** arrow next to the Bones button:

- **Sphere Size**: Make discs bigger/smaller
- **Colors**: Change colors to your preference
- **Labels**: Toggle bone names on/off

## 💡 Pro Tips

1. **Can't see bones?** → Increase "Bone Distance" in settings (default: 50m)
2. **Too cluttered?** → Turn off "Show Labels" or reduce "Bone Distance"
3. **Hard to click?** → Increase "Sphere Size" for easier selection

## ⚡ Common Shortcuts

| Action | Shortcut |
|--------|----------|
| Single Select | Click |
| Add to Selection | Shift + Click |
| Toggle Selection | Ctrl/Cmd + Click |
| Select All Bones | Select parent → Shift+G (Unity's Select Children) |

## 🆘 Quick Fixes

**No bones showing?**
- Make sure you have a model with SkinnedMeshRenderer or Animator
- Check if the Bones button is blue (active)

**Selection not working properly?**
- Update to v1.0.1 for fixed multi-selection
- Try clicking the label instead of the disc

---

Ready for more? Check out the [Full Documentation](README_en.md) for advanced features!

Happy rigging! 🎉