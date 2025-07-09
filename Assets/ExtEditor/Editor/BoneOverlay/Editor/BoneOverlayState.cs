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
        private bool showOptions = false;
        private float maxRenderDistance = 50f;
        private float minRenderDistance = 0.1f;
        private bool distanceFadeEnabled = true;
        
        // 表示設定
        private bool showLabels = true;
        private float lineWidth = 2f;
        private float sphereSize = 0.005f;
        private float labelSize = 10f;
        private float maxLabelRenderDistance = 30f; // ラベルの最大表示距離
        
        // カラー設定
        private Color normalColor = new Color(0.5f, 0.5f, 1f, 0.8f); // 青
        private Color selectedColor = new Color(1f, 1f, 0f, 1f); // 黄色
        private Color hoverColor = new Color(0f, 1f, 0f, 1f); // 緑
        private Color lineColor = new Color(0.3f, 0.3f, 0.8f, 0.5f); // 薄い青
        private Color labelColor = Color.white; // ラベルの色
        
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
        
        public bool ShowOptions
        {
            get => showOptions;
            set
            {
                if (showOptions != value)
                {
                    showOptions = value;
                    Save();
                }
            }
        }
        
        public bool ShowLabels
        {
            get => showLabels;
            set
            {
                if (showLabels != value)
                {
                    showLabels = value;
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
        
        public float LineWidth
        {
            get => lineWidth;
            set
            {
                if (Mathf.Abs(lineWidth - value) > 0.01f)
                {
                    lineWidth = Mathf.Clamp(value, 0.1f, 10f);
                    Save();
                }
            }
        }
        
        public float SphereSize
        {
            get => sphereSize;
            set
            {
                if (Mathf.Abs(sphereSize - value) > 0.0001f)
                {
                    sphereSize = Mathf.Clamp(value, 0.001f, 0.1f);
                    Save();
                }
            }
        }
        
        public float LabelSize
        {
            get => labelSize;
            set
            {
                if (Mathf.Abs(labelSize - value) > 0.01f)
                {
                    labelSize = Mathf.Clamp(value, 5f, 30f);
                    Save();
                }
            }
        }
        
        public float MaxLabelRenderDistance
        {
            get => maxLabelRenderDistance;
            set
            {
                if (Mathf.Abs(maxLabelRenderDistance - value) > 0.01f)
                {
                    maxLabelRenderDistance = Mathf.Clamp(value, 1f, 1000f);
                    Save();
                }
            }
        }
        
        public Color NormalColor
        {
            get => normalColor;
            set
            {
                if (normalColor != value)
                {
                    normalColor = value;
                    Save();
                }
            }
        }
        
        public Color SelectedColor
        {
            get => selectedColor;
            set
            {
                if (selectedColor != value)
                {
                    selectedColor = value;
                    Save();
                }
            }
        }
        
        public Color HoverColor
        {
            get => hoverColor;
            set
            {
                if (hoverColor != value)
                {
                    hoverColor = value;
                    Save();
                }
            }
        }
        
        public Color LineColor
        {
            get => lineColor;
            set
            {
                if (lineColor != value)
                {
                    lineColor = value;
                    Save();
                }
            }
        }
        
        public Color LabelColor
        {
            get => labelColor;
            set
            {
                if (labelColor != value)
                {
                    labelColor = value;
                    Save();
                }
            }
        }
        
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
            EditorPrefs.SetBool(PREF_KEY_PREFIX + "ShowOptions", showOptions);
            EditorPrefs.SetFloat(PREF_KEY_PREFIX + "MaxRenderDistance", maxRenderDistance);
            EditorPrefs.SetFloat(PREF_KEY_PREFIX + "MinRenderDistance", minRenderDistance);
            EditorPrefs.SetBool(PREF_KEY_PREFIX + "DistanceFadeEnabled", distanceFadeEnabled);
            
            EditorPrefs.SetBool(PREF_KEY_PREFIX + "ShowLabels", showLabels);
            EditorPrefs.SetFloat(PREF_KEY_PREFIX + "LineWidth", lineWidth);
            EditorPrefs.SetFloat(PREF_KEY_PREFIX + "SphereSize", sphereSize);
            EditorPrefs.SetFloat(PREF_KEY_PREFIX + "LabelSize", labelSize);
            EditorPrefs.SetFloat(PREF_KEY_PREFIX + "MaxLabelRenderDistance", maxLabelRenderDistance);
            
            SaveColor(PREF_KEY_PREFIX + "NormalColor", normalColor);
            SaveColor(PREF_KEY_PREFIX + "SelectedColor", selectedColor);
            SaveColor(PREF_KEY_PREFIX + "HoverColor", hoverColor);
            SaveColor(PREF_KEY_PREFIX + "LineColor", lineColor);
            SaveColor(PREF_KEY_PREFIX + "LabelColor", labelColor);
            
            EditorPrefs.SetBool(PREF_KEY_PREFIX + "IncludeEmptyTransforms", includeEmptyTransforms);
        }
        
        public void Load()
        {
            isEnabled = EditorPrefs.GetBool(PREF_KEY_PREFIX + "IsEnabled", false);
            showOptions = EditorPrefs.GetBool(PREF_KEY_PREFIX + "ShowOptions", false);
            maxRenderDistance = EditorPrefs.GetFloat(PREF_KEY_PREFIX + "MaxRenderDistance", 50f);
            minRenderDistance = EditorPrefs.GetFloat(PREF_KEY_PREFIX + "MinRenderDistance", 0.1f);
            distanceFadeEnabled = EditorPrefs.GetBool(PREF_KEY_PREFIX + "DistanceFadeEnabled", true);
            
            showLabels = EditorPrefs.GetBool(PREF_KEY_PREFIX + "ShowLabels", true);
            lineWidth = EditorPrefs.GetFloat(PREF_KEY_PREFIX + "LineWidth", 2f);
            sphereSize = EditorPrefs.GetFloat(PREF_KEY_PREFIX + "SphereSize", 0.005f);
            labelSize = EditorPrefs.GetFloat(PREF_KEY_PREFIX + "LabelSize", 10f);
            maxLabelRenderDistance = EditorPrefs.GetFloat(PREF_KEY_PREFIX + "MaxLabelRenderDistance", 30f);
            
            normalColor = LoadColor(PREF_KEY_PREFIX + "NormalColor", new Color(0.5f, 0.5f, 1f, 0.8f));
            selectedColor = LoadColor(PREF_KEY_PREFIX + "SelectedColor", new Color(1f, 1f, 0f, 1f));
            hoverColor = LoadColor(PREF_KEY_PREFIX + "HoverColor", new Color(0f, 1f, 0f, 1f));
            lineColor = LoadColor(PREF_KEY_PREFIX + "LineColor", new Color(0.3f, 0.3f, 0.8f, 0.5f));
            labelColor = LoadColor(PREF_KEY_PREFIX + "LabelColor", new Color(0.4f,0.7f,1f,1f));
            
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
            showOptions = false;
            maxRenderDistance = 50f;
            minRenderDistance = 0.1f;
            distanceFadeEnabled = true;
            
            showLabels = true;
            lineWidth = 2f;
            sphereSize = 0.005f;
            labelSize = 10f;
            maxLabelRenderDistance = 30f;
            
            normalColor = new Color(0.5f, 0.5f, 1f, 0.8f);
            selectedColor = new Color(1f, 1f, 0f, 1f);
            hoverColor = new Color(0f, 1f, 0f, 1f);
            lineColor = new Color(0.3f, 0.3f, 0.8f, 0.5f);
            labelColor = new Color(0.4f, 0.7f, 1f, 1f);
            
            includeEmptyTransforms = false;
            
            Save();
        }
    }
}