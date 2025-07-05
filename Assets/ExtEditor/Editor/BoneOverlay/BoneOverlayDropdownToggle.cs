using UnityEngine;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using System;
using UnityEditor.UIElements;

namespace ExtEditor.BoneOverlay
{
    [EditorToolbarElement(id, typeof(SceneView))]
    public class BoneOverlayDropdownToggle : EditorToolbarDropdownToggle, IAccessContainerWindow
    {
        public const string id = "ExtEditor/BoneOverlayToggle";
        
        // Static instance management
        private static BoneOverlayDropdownToggle instance;
        private static BoneOverlayState state;
        private static BoneDetector detector;
        private static BoneOverlayRenderer renderer;
        
        public EditorWindow containerWindow { get; set; }
        
        // UI Elements
        private GenericDropdownMenu dropdownMenu;
        private bool isInitialized = false;
        
        public BoneOverlayDropdownToggle()
        {
            instance = this;
            
            // Initialize if not already done
            if (state == null)
            {
                state = new BoneOverlayState();
                detector = new BoneDetector();
                detector.SetState(state);
                renderer = new BoneOverlayRenderer(state, detector);
            }
            
            // Set up the toggle
            text = "";
            tooltip = "Toggle bone visualization in Scene View";
            icon = EditorGUIUtility.IconContent("AvatarMask Icon").image as Texture2D;
            
            // Set initial value
            value = state.IsEnabled;
            
            // Register callbacks
            this.RegisterValueChangedCallback(OnToggleValueChanged);
            dropdownClicked += ShowDropdown;
            
            // Register Scene GUI callback
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            
            // Register selection changed callback
            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;
            
            isInitialized = true;
        }
        
        ~BoneOverlayDropdownToggle()
        {
            // Cleanup
            SceneView.duringSceneGui -= OnSceneGUI;
            Selection.selectionChanged -= OnSelectionChanged;
            
            if (instance == this)
            {
                renderer?.Dispose();
                state?.Save();
                instance = null;
            }
        }
        
        private void OnToggleValueChanged(ChangeEvent<bool> evt)
        {
            if (state != null)
            {
                state.IsEnabled = evt.newValue;
                SceneView.RepaintAll();
            }
        }
        
        private void ShowDropdown()
        {
            dropdownMenu = new GenericDropdownMenu();
            
            // Bone Settings Section
            dropdownMenu.AddDisabledItem("Bone Settings", false);
            
            // Bone Distance Slider
            var boneDistanceContainer = CreateSliderElement("Bone Distance", state.MaxRenderDistance, 1f, 100f, 
                (value) => state.MaxRenderDistance = value, "m");
            dropdownMenu.contentContainer.Add(boneDistanceContainer);
            
            // Sphere Size Slider
            var sphereSizeContainer = CreateSliderElement("Sphere Size", state.SphereSize * 1000f, 1f, 100f, 
                (value) => state.SphereSize = value / 1000f, "mm", 0.1f);
            dropdownMenu.contentContainer.Add(sphereSizeContainer);
            
            // Line Width Slider
            var lineWidthContainer = CreateSliderElement("Line Width", state.LineWidth, 0.1f, 10f, 
                (value) => state.LineWidth = value, "", 0.1f);
            dropdownMenu.contentContainer.Add(lineWidthContainer);
            
            // Bone Colors
            var normalColorContainer = CreateColorElement("Normal Color", state.NormalColor, 
                (color) => state.NormalColor = color);
            dropdownMenu.contentContainer.Add(normalColorContainer);
            
            var selectedColorContainer = CreateColorElement("Selected Color", state.SelectedColor, 
                (color) => state.SelectedColor = color);
            dropdownMenu.contentContainer.Add(selectedColorContainer);
            
            var hoverColorContainer = CreateColorElement("Hover Color", state.HoverColor, 
                (color) => state.HoverColor = color);
            dropdownMenu.contentContainer.Add(hoverColorContainer);
            
            var lineColorContainer = CreateColorElement("Line Color", state.LineColor, 
                (color) => state.LineColor = color);
            dropdownMenu.contentContainer.Add(lineColorContainer);
            
            dropdownMenu.AddSeparator("");
            
            // Label Settings Section
            dropdownMenu.AddDisabledItem("Label Settings", false);
            
            // Show Labels option
            dropdownMenu.AddItem("Show Labels", state.ShowLabels, () =>
            {
                state.ShowLabels = !state.ShowLabels;
                SceneView.RepaintAll();
            });
            
            if (state.ShowLabels)
            {
                // Label Distance Slider
                var labelDistanceContainer = CreateSliderElement("Label Distance", state.MaxLabelRenderDistance, 1f, 100f, 
                    (value) => state.MaxLabelRenderDistance = value, "m");
                dropdownMenu.contentContainer.Add(labelDistanceContainer);
                
                // Label Size Slider
                var labelSizeContainer = CreateSliderElement("Label Size", state.LabelSize, 5f, 30f, 
                    (value) => state.LabelSize = value, "pt", 1f);
                dropdownMenu.contentContainer.Add(labelSizeContainer);
                
                // Label Color
                var labelColorContainer = CreateColorElement("Label Color", state.LabelColor, 
                    (color) => state.LabelColor = color);
                dropdownMenu.contentContainer.Add(labelColorContainer);
            }
            
            dropdownMenu.AddSeparator("");
            
            // Reset option
            dropdownMenu.AddItem("Reset to Defaults", false, () =>
            {
                state.ResetToDefaults();
                value = state.IsEnabled;
                SceneView.RepaintAll();
            });
            
            // Status display
            if (detector != null)
            {
                var detectedBones = detector.DetectBones();
                dropdownMenu.AddSeparator("");
                dropdownMenu.AddDisabledItem($"Detected Bones: {detectedBones.Count}", false);
            }
            
            // Show the dropdown
            dropdownMenu.DropDown(worldBound, this);
        }
        
        private VisualElement CreateSliderElement(string label, float value, float min, float max, 
            Action<float> onValueChanged, string suffix = "", float step = 1f)
        {
            var container = new VisualElement();
            container.style.paddingLeft = 20;
            container.style.paddingRight = 10;
            
            var labelElement = new Label($"{label}: {value:F1}{suffix}");
            labelElement.style.fontSize = 11;
            labelElement.style.marginBottom = 2;
            container.Add(labelElement);
            
            var slider = new Slider(min, max);
            slider.value = value;
            slider.style.width = 180;
            slider.RegisterValueChangedCallback(evt =>
            {
                var roundedValue = Mathf.Round(evt.newValue / step) * step;
                onValueChanged?.Invoke(roundedValue);
                labelElement.text = $"{label}: {roundedValue:F1}{suffix}";
                SceneView.RepaintAll();
            });
            container.Add(slider);
            
            return container;
        }
        
        private VisualElement CreateColorElement(string label, Color value, Action<Color> onValueChanged)
        {
            var container = new VisualElement();
            container.style.paddingLeft = 20;
            container.style.paddingRight = 10;
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            
            var labelElement = new Label($"{label}:");
            labelElement.style.fontSize = 11;
            labelElement.style.width = 60;
            container.Add(labelElement);
            
            var colorField = new ColorField();
            colorField.value = value;
            colorField.style.flexShrink = 1;
            colorField.showAlpha = true;
            colorField.style.width = 100;
            colorField.RegisterValueChangedCallback(evt =>
            {
                onValueChanged?.Invoke(evt.newValue);
                SceneView.RepaintAll();
            });
            container.Add(colorField);
            
            return container;
        }
        
        private static void OnSceneGUI(SceneView sceneView)
        {
            if (!state.IsEnabled || detector == null || renderer == null) return;
            
            var detectedBones = detector.DetectBones();
            renderer.DrawBones(sceneView, detectedBones);
        }
        
        private static void OnSelectionChanged()
        {
            if (state != null && state.IsEnabled)
            {
                SceneView.RepaintAll();
            }
        }
        
        // Property to access current state
        public static bool IsEnabled => state?.IsEnabled ?? false;
    }
}