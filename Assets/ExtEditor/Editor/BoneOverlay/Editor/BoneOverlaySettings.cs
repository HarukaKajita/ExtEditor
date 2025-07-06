using UnityEngine;
using UnityEditor;

namespace ExtEditor.BoneOverlay
{
    // 将来の拡張用: ScriptableObjectベースの設定システム
    // 現在はBoneOverlayStateがEditorPrefsで設定を管理していますが、
    // より複雑な設定やプリセット機能が必要になった場合にこのクラスを使用します
    [CreateAssetMenu(fileName = "BoneOverlaySettings", menuName = "ExtEditor/BoneOverlay Settings", order = 100)]
    public class BoneOverlaySettings : ScriptableObject
    {
        [Header("Display Settings")]
        public bool showLabels = true;
        public float lineWidth = 2f;
        public float sphereSize = 0.01f;
        public float labelSize = 10f;
        
        [Header("Distance Filter")]
        public bool enableDistanceFilter = true;
        public float maxRenderDistance = 50f;
        public float minRenderDistance = 0.1f;
        public bool distanceFadeEnabled = true;
        
        [Header("Colors")]
        public Color normalColor = new Color(0.5f, 0.5f, 1f, 0.8f);
        public Color selectedColor = new Color(1f, 1f, 0f, 1f);
        public Color hoverColor = new Color(0f, 1f, 0f, 1f);
        public Color lineColor = new Color(0.3f, 0.3f, 0.8f, 0.5f);
        
        [Header("Detection Settings")]
        public bool includeEmptyTransforms = false;
        public string[] boneNamePatterns = new string[] 
        { 
            "bone", "joint", "jnt", "bip", "spine", "neck", "head", 
            "arm", "leg", "foot", "hand", "finger", "toe", "clavicle",
            "shoulder", "elbow", "wrist", "hip", "knee", "ankle"
        };
        
        [Header("Performance")]
        public int maxBonesPerFrame = 1000;
        public bool useLOD = true;
        public float lodDistance1 = 20f; // フル詳細
        public float lodDistance2 = 40f; // 簡易表示
        
        // デフォルト設定を取得
        public static BoneOverlaySettings GetDefault()
        {
            var settings = CreateInstance<BoneOverlaySettings>();
            return settings;
        }
        
        // BoneOverlayStateに設定を適用
        public void ApplyToState(BoneOverlayState state)
        {
            if (state == null) return;
            
            // 将来の実装: StateにSettingsの値を適用
            // 現在はStateが直接設定を管理しているため、この機能は未使用
        }
        
        // BoneOverlayStateから設定を読み込み
        public void LoadFromState(BoneOverlayState state)
        {
            if (state == null) return;
            
            // 将来の実装: StateからSettingsに値を読み込み
            // 現在はStateが直接設定を管理しているため、この機能は未使用
        }
    }
}