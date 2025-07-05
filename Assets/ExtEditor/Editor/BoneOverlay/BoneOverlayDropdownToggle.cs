using UnityEngine;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using System;

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
            dropdownClicked += () =>
            {
                ShowDropdown(worldBound);
            };
            
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
        
        private void ShowDropdown(Rect drawPosition)
        {
            dropdownMenu = new GenericDropdownMenu();
            
            // Distance Filter Section
            dropdownMenu.AddItem("Distance Filter", state.EnableDistanceFilter, () =>
            {
                state.EnableDistanceFilter = !state.EnableDistanceFilter;
                SceneView.RepaintAll();
            });
            
            if (state.EnableDistanceFilter)
            {
                // Add distance slider as a custom item
                dropdownMenu.AddSeparator("");
                
                // Create a custom VisualElement for the slider
                var sliderContainer = new VisualElement();
                sliderContainer.style.paddingLeft = 20;
                sliderContainer.style.paddingRight = 10;
                sliderContainer.style.paddingTop = 5;
                sliderContainer.style.paddingBottom = 5;
                
                var sliderLabel = new Label($"Max Distance: {state.MaxRenderDistance:F1}m");
                sliderLabel.style.fontSize = 11;
                sliderLabel.style.marginBottom = 2;
                sliderContainer.Add(sliderLabel);
                
                var slider = new Slider(1f, 100f);
                slider.value = state.MaxRenderDistance;
                slider.style.width = 180;
                slider.RegisterValueChangedCallback(evt =>
                {
                    state.MaxRenderDistance = evt.newValue;
                    sliderLabel.text = $"Max Distance: {evt.newValue:F1}m";
                    SceneView.RepaintAll();
                });
                sliderContainer.Add(slider);
                
                dropdownMenu.contentContainer.Add(sliderContainer);
                dropdownMenu.AddSeparator("");
            }
            
            // Show Labels option
            dropdownMenu.AddItem("Show Labels", state.ShowLabels, () =>
            {
                state.ShowLabels = !state.ShowLabels;
                SceneView.RepaintAll();
            });
            
            dropdownMenu.AddSeparator("");
            
            // Additional options
            dropdownMenu.AddItem("Settings/Normal Color", false, () => ShowColorPicker("Normal", state.NormalColor, color => state.NormalColor = color));
            dropdownMenu.AddItem("Settings/Selected Color", false, () => ShowColorPicker("Selected", state.SelectedColor, color => state.SelectedColor = color));
            dropdownMenu.AddItem("Settings/Hover Color", false, () => ShowColorPicker("Hover", state.HoverColor, color => state.HoverColor = color));
            
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
            var rect = drawPosition;
            Debug.Log(rect);
            dropdownMenu.DropDown(drawPosition, this);
        }
        
        private void ShowColorPicker(string colorName, Color currentColor, Action<Color> onColorChanged)
        {
            // Create a simple color picker window
            var colorPickerWindow = EditorWindow.GetWindow<ColorPickerWindow>(true, $"Pick {colorName} Color", true);
            colorPickerWindow.Initialize(colorName, currentColor, onColorChanged);
            colorPickerWindow.ShowUtility();
        }
        
        // Simple color picker window
        private class ColorPickerWindow : EditorWindow
        {
            private Color currentColor;
            private Color originalColor;
            private Action<Color> onColorChanged;
            private string colorName;
            
            public void Initialize(string name, Color color, Action<Color> callback)
            {
                colorName = name;
                currentColor = color;
                originalColor = color;
                onColorChanged = callback;
                minSize = new Vector2(250, 100);
                maxSize = new Vector2(250, 100);
            }
            
            void OnGUI()
            {
                EditorGUILayout.LabelField($"{colorName} Color", EditorStyles.boldLabel);
                
                EditorGUI.BeginChangeCheck();
                currentColor = EditorGUILayout.ColorField(currentColor);
                if (EditorGUI.EndChangeCheck())
                {
                    onColorChanged?.Invoke(currentColor);
                    SceneView.RepaintAll();
                }
                
                EditorGUILayout.Space();
                
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Reset"))
                    {
                        currentColor = originalColor;
                        onColorChanged?.Invoke(currentColor);
                        SceneView.RepaintAll();
                    }
                    
                    if (GUILayout.Button("Close"))
                    {
                        Close();
                    }
                }
            }
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