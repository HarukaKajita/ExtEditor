using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

namespace ExtEditor.BoneOverlay
{
    [Overlay(typeof(SceneView), "", true)]
    [Icon("Icons/d_AvatarMask Icon")]
    public class BoneOverlayToolbar : ToolbarOverlay
    {
        // Constructor that passes the toolbar element IDs
        public BoneOverlayToolbar() : base(BoneOverlayDropdownToggle.id)
        {
        }
    }
}