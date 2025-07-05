using UnityEngine;
using UnityEditor;
using System;

namespace ExtEditor.BoneOverlay
{
    [Serializable]
    public class BoneOverlayState
    {
        private const string PREF_KEY_PREFIX = "ExtEditor.BoneOverlay.";
        
        // 基本設定
        private bool isEnabled = false;
        private bool enableDistanceFilter = true;
        private float maxRenderDistance = 50f;
        private float minRenderDistance = 0.1f;
        private bool distanceFadeEnabled = true;
        
        // 表示設定
        private bool showLabels = true;
        private float lineWidth = 2f;
        private float sphereSize = 0.001f;
        private float labelSize = 10f;
        
        // カラー設定
        private Color normalColor = new Color(0.5f, 0.5f, 1f, 0.8f); // 青
        private Color selectedColor = new Color(1f, 1f, 0f, 1f); // 黄色
        private Color hoverColor = new Color(0f, 1f, 0f, 1f); // 緑
        private Color lineColor = new Color(0.3f, 0.3f, 0.8f, 0.5f); // 薄い青
        
        // 検出設定
        private bool includeEmptyTransforms = false;
        private string[] boneNamePatterns = new string[] { "bone", "joint", "jnt", "bip", "spine", "neck", "head", "arm", "leg", "foot", "hand", "finger" };
        
        // プロパティ
        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    Save();
                }
            }
        }
        
        public bool EnableDistanceFilter
        {
            get => enableDistanceFilter;
            set
            {
                if (enableDistanceFilter != value)
                {
                    enableDistanceFilter = value;
                    Save();
                }
            }
        }
        
        public float MaxRenderDistance
        {
            get => maxRenderDistance;
            set
            {
                if (Mathf.Abs(maxRenderDistance - value) > 0.01f)
                {
                    maxRenderDistance = Mathf.Clamp(value, 1f, 1000f);
                    Save();
                }
            }
        }
        
        public float MinRenderDistance
        {
            get => minRenderDistance;
            set
            {
                if (Mathf.Abs(minRenderDistance - value) > 0.01f)
                {
                    minRenderDistance = Mathf.Clamp(value, 0f, maxRenderDistance);
                    Save();
                }
            }
        }
        
        public bool DistanceFadeEnabled => distanceFadeEnabled;
        public bool ShowLabels => showLabels;
        public float LineWidth => lineWidth;
        public float SphereSize => sphereSize;
        public float LabelSize => labelSize;
        
        public Color NormalColor => normalColor;
        public Color SelectedColor => selectedColor;
        public Color HoverColor => hoverColor;
        public Color LineColor => lineColor;
        
        public bool IncludeEmptyTransforms => includeEmptyTransforms;
        public string[] BoneNamePatterns => boneNamePatterns;
        
        // コンストラクタ
        public BoneOverlayState()
        {
            Load();
        }
        
        // 保存・読み込み
        public void Save()
        {
            EditorPrefs.SetBool(PREF_KEY_PREFIX + "IsEnabled", isEnabled);
            EditorPrefs.SetBool(PREF_KEY_PREFIX + "EnableDistanceFilter", enableDistanceFilter);
            EditorPrefs.SetFloat(PREF_KEY_PREFIX + "MaxRenderDistance", maxRenderDistance);
            EditorPrefs.SetFloat(PREF_KEY_PREFIX + "MinRenderDistance", minRenderDistance);
            EditorPrefs.SetBool(PREF_KEY_PREFIX + "DistanceFadeEnabled", distanceFadeEnabled);
            
            EditorPrefs.SetBool(PREF_KEY_PREFIX + "ShowLabels", showLabels);
            EditorPrefs.SetFloat(PREF_KEY_PREFIX + "LineWidth", lineWidth);
            EditorPrefs.SetFloat(PREF_KEY_PREFIX + "SphereSize", sphereSize);
            EditorPrefs.SetFloat(PREF_KEY_PREFIX + "LabelSize", labelSize);
            
            SaveColor(PREF_KEY_PREFIX + "NormalColor", normalColor);
            SaveColor(PREF_KEY_PREFIX + "SelectedColor", selectedColor);
            SaveColor(PREF_KEY_PREFIX + "HoverColor", hoverColor);
            SaveColor(PREF_KEY_PREFIX + "LineColor", lineColor);
            
            EditorPrefs.SetBool(PREF_KEY_PREFIX + "IncludeEmptyTransforms", includeEmptyTransforms);
        }
        
        public void Load()
        {
            isEnabled = EditorPrefs.GetBool(PREF_KEY_PREFIX + "IsEnabled", false);
            enableDistanceFilter = EditorPrefs.GetBool(PREF_KEY_PREFIX + "EnableDistanceFilter", true);
            maxRenderDistance = EditorPrefs.GetFloat(PREF_KEY_PREFIX + "MaxRenderDistance", 50f);
            minRenderDistance = EditorPrefs.GetFloat(PREF_KEY_PREFIX + "MinRenderDistance", 0.1f);
            distanceFadeEnabled = EditorPrefs.GetBool(PREF_KEY_PREFIX + "DistanceFadeEnabled", true);
            
            showLabels = EditorPrefs.GetBool(PREF_KEY_PREFIX + "ShowLabels", true);
            lineWidth = EditorPrefs.GetFloat(PREF_KEY_PREFIX + "LineWidth", 2f);
            sphereSize = EditorPrefs.GetFloat(PREF_KEY_PREFIX + "SphereSize", 0.1f);
            labelSize = EditorPrefs.GetFloat(PREF_KEY_PREFIX + "LabelSize", 10f);
            
            normalColor = LoadColor(PREF_KEY_PREFIX + "NormalColor", new Color(0.5f, 0.5f, 1f, 0.8f));
            selectedColor = LoadColor(PREF_KEY_PREFIX + "SelectedColor", new Color(1f, 1f, 0f, 1f));
            hoverColor = LoadColor(PREF_KEY_PREFIX + "HoverColor", new Color(0f, 1f, 0f, 1f));
            lineColor = LoadColor(PREF_KEY_PREFIX + "LineColor", new Color(0.3f, 0.3f, 0.8f, 0.5f));
            
            includeEmptyTransforms = EditorPrefs.GetBool(PREF_KEY_PREFIX + "IncludeEmptyTransforms", false);
        }
        
        private void SaveColor(string key, Color color)
        {
            EditorPrefs.SetFloat(key + ".r", color.r);
            EditorPrefs.SetFloat(key + ".g", color.g);
            EditorPrefs.SetFloat(key + ".b", color.b);
            EditorPrefs.SetFloat(key + ".a", color.a);
        }
        
        private Color LoadColor(string key, Color defaultColor)
        {
            return new Color(
                EditorPrefs.GetFloat(key + ".r", defaultColor.r),
                EditorPrefs.GetFloat(key + ".g", defaultColor.g),
                EditorPrefs.GetFloat(key + ".b", defaultColor.b),
                EditorPrefs.GetFloat(key + ".a", defaultColor.a)
            );
        }
        
        public void ResetToDefaults()
        {
            isEnabled = false;
            enableDistanceFilter = true;
            maxRenderDistance = 50f;
            minRenderDistance = 0.1f;
            distanceFadeEnabled = true;
            
            showLabels = true;
            lineWidth = 2f;
            sphereSize = 0.1f;
            labelSize = 10f;
            
            normalColor = new Color(0.5f, 0.5f, 1f, 0.8f);
            selectedColor = new Color(1f, 1f, 0f, 1f);
            hoverColor = new Color(0f, 1f, 0f, 1f);
            lineColor = new Color(0.3f, 0.3f, 0.8f, 0.5f);
            
            includeEmptyTransforms = false;
            
            Save();
        }
    }
}