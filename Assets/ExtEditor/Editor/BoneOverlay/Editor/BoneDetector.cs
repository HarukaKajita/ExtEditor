using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace ExtEditor.BoneOverlay
{
    public class BoneDetector
    {
        private BoneOverlayState state;
        private HashSet<Transform> cachedBones;
        private int lastFrameCount = -1;
        
        // 除外されたボーンの数を追跡
        public int ExcludedBonesCount { get; private set; }
        
        public class BoneInfo
        {
            public Transform Transform { get; set; }
            public Transform Parent { get; set; }
            public List<Transform> Children { get; set; }
            public bool IsSkinnedMeshBone { get; set; }
            public bool IsAnimatorBone { get; set; }
            public int Depth { get; set; }
            
            public BoneInfo()
            {
                Children = new List<Transform>();
            }
        }
        
        private Dictionary<Transform, BoneInfo> boneInfoCache;
        
        public BoneDetector()
        {
            cachedBones = new HashSet<Transform>();
            boneInfoCache = new Dictionary<Transform, BoneInfo>();
        }
        
        public void SetState(BoneOverlayState state)
        {
            this.state = state;
        }
        
        public List<BoneInfo> DetectBones()
        {
            // フレームごとにキャッシュをチェック
            if (Time.frameCount == lastFrameCount && boneInfoCache.Count > 0)
            {
                return boneInfoCache.Values.ToList();
            }
            
            lastFrameCount = Time.frameCount;
            cachedBones.Clear();
            boneInfoCache.Clear();
            ExcludedBonesCount = 0;  // 除外カウントをリセット
            
            // シーン内の全てのGameObjectを取得
            var allTransforms = Object.FindObjectsOfType<Transform>();
            
            // SkinnedMeshRendererのボーンを収集
            var skinnedMeshRenderers = Object.FindObjectsOfType<SkinnedMeshRenderer>();
            foreach (var smr in skinnedMeshRenderers)
            {
                if (smr.bones != null)
                {
                    foreach (var bone in smr.bones)
                    {
                        if (bone != null)
                        {
                            // 非表示または選択不可のボーンをチェック
                            if (IsExcludedBone(bone))
                            {
                                ExcludedBonesCount++;
                                continue;
                            }
                            
                            cachedBones.Add(bone);
                            MarkBoneHierarchy(bone, true, false);
                        }
                    }
                }
            }
            
            // Animatorのボーンを収集
            var animators = Object.FindObjectsOfType<Animator>();
            foreach (var animator in animators)
            {
                if (animator.avatar != null && animator.avatar.isHuman)
                {
                    // Humanoidボーンを収集
                    CollectHumanoidBones(animator);
                }
                else if (animator.transform != null)
                {
                    // Generic Animatorの場合、階層を探索
                    SearchAnimatorHierarchy(animator.transform);
                }
            }
            
            // 名前パターンに基づいてボーンを検出
            if (state != null && state.BoneNamePatterns != null)
            {
                foreach (var transform in allTransforms)
                {
                    if (transform == null) continue;
                    
                    // 除外されるボーンはスキップ
                    if (IsExcludedBone(transform))
                    {
                        ExcludedBonesCount++;
                        continue;
                    }
                    
                    string lowerName = transform.name.ToLower();
                    foreach (var pattern in state.BoneNamePatterns)
                    {
                        if (lowerName.Contains(pattern.ToLower()))
                        {
                            // 子Transformがあるか、既にボーンとして認識されている場合
                            if (transform.childCount > 0 || cachedBones.Contains(transform))
                            {
                                cachedBones.Add(transform);
                                MarkBoneHierarchy(transform, false, false);
                                break;
                            }
                            else if (state.IncludeEmptyTransforms)
                            {
                                cachedBones.Add(transform);
                                MarkBoneHierarchy(transform, false, false);
                                break;
                            }
                        }
                    }
                }
            }
            
            // BoneInfo構造を構築
            BuildBoneInfoStructure();
            
            return boneInfoCache.Values.ToList();
        }
        
        private void MarkBoneHierarchy(Transform bone, bool isSkinnedMeshBone, bool isAnimatorBone)
        {
            if (bone == null || IsExcludedBone(bone)) return;
            
            if (!boneInfoCache.ContainsKey(bone))
            {
                var info = new BoneInfo
                {
                    Transform = bone,
                    IsSkinnedMeshBone = isSkinnedMeshBone,
                    IsAnimatorBone = isAnimatorBone
                };
                boneInfoCache[bone] = info;
            }
            else
            {
                boneInfoCache[bone].IsSkinnedMeshBone |= isSkinnedMeshBone;
                boneInfoCache[bone].IsAnimatorBone |= isAnimatorBone;
            }
            
            // 親も含める（ルートまで）
            if (bone.parent != null && !cachedBones.Contains(bone.parent))
            {
                // 親が除外対象でないかチェック
                if (!IsExcludedBone(bone.parent))
                {
                    bool shouldIncludeParent = false;
                    
                    // 親が他の子ボーンを持っているかチェック
                    foreach (Transform sibling in bone.parent)
                    {
                        if (sibling != bone && cachedBones.Contains(sibling))
                        {
                            shouldIncludeParent = true;
                            break;
                        }
                    }
                    
                    if (shouldIncludeParent)
                    {
                        cachedBones.Add(bone.parent);
                        MarkBoneHierarchy(bone.parent, false, false);
                    }
                }
            }
        }
        
        private void CollectHumanoidBones(Animator animator)
        {
            // Humanoidボーンを取得
            var humanBones = System.Enum.GetValues(typeof(HumanBodyBones)).Cast<HumanBodyBones>()
                .Where(b => b != HumanBodyBones.LastBone);
            
            foreach (var humanBone in humanBones)
            {
                var bone = animator.GetBoneTransform(humanBone);
                if (bone != null)
                {
                    // 除外チェック
                    if (IsExcludedBone(bone))
                    {
                        ExcludedBonesCount++;
                        continue;
                    }
                    
                    cachedBones.Add(bone);
                    MarkBoneHierarchy(bone, false, true);
                }
            }
        }
        
        private void SearchAnimatorHierarchy(Transform root)
        {
            if (root == null) return;
            
            // Animatorの階層内でボーンらしきものを探す
            var transforms = root.GetComponentsInChildren<Transform>();
            foreach (var t in transforms)
            {
                if (t.childCount > 0 && state != null && state.BoneNamePatterns != null)
                {
                    // 除外チェック
                    if (IsExcludedBone(t))
                    {
                        ExcludedBonesCount++;
                        continue;
                    }
                    
                    string lowerName = t.name.ToLower();
                    foreach (var pattern in state.BoneNamePatterns)
                    {
                        if (lowerName.Contains(pattern.ToLower()))
                        {
                            cachedBones.Add(t);
                            MarkBoneHierarchy(t, false, true);
                            break;
                        }
                    }
                }
            }
        }
        
        private void BuildBoneInfoStructure()
        {
            // 親子関係を構築
            foreach (var kvp in boneInfoCache)
            {
                var bone = kvp.Key;
                var info = kvp.Value;
                
                // 親を設定
                if (bone.parent != null && boneInfoCache.ContainsKey(bone.parent))
                {
                    info.Parent = bone.parent;
                    boneInfoCache[bone.parent].Children.Add(bone);
                }
                
                // 深度を計算
                info.Depth = CalculateDepth(bone);
            }
        }
        
        private int CalculateDepth(Transform bone)
        {
            int depth = 0;
            Transform current = bone.parent;
            
            while (current != null && boneInfoCache.ContainsKey(current))
            {
                depth++;
                current = current.parent;
            }
            
            return depth;
        }
        
        private bool IsExcludedBone(Transform bone)
        {
            if (bone == null || bone.gameObject == null) return true;
            
            // 非表示のボーンを除外
            if (SceneVisibilityManager.instance.IsHidden(bone.gameObject, true))
                return true;
            
            // 選択不可のボーンを除外
            if (SceneVisibilityManager.instance.IsPickingDisabled(bone.gameObject, true))
                return true;
            
            return false;
        }
        
        public void ClearCache()
        {
            cachedBones.Clear();
            boneInfoCache.Clear();
            lastFrameCount = -1;
        }
    }
}