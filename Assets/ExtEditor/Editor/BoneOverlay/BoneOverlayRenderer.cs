using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ExtEditor.BoneOverlay
{
    public class BoneOverlayRenderer
    {
        private BoneOverlayState state;
        private BoneDetector detector;
        private Transform hoveredBone;
        private Dictionary<Transform, float> boneDistanceCache;
        private int lastFrameCount = -1;
        
        // クリック検出用
        private const float CLICK_THRESHOLD = 10f;
        private float lastClickTime;
        private Transform lastClickedBone;
        
        public BoneOverlayRenderer(BoneOverlayState state, BoneDetector detector)
        {
            this.state = state;
            this.detector = detector;
            this.detector.SetState(state);
            boneDistanceCache = new Dictionary<Transform, float>();
        }
        
        public void DrawBones(SceneView sceneView, List<BoneDetector.BoneInfo> bones)
        {
            if (bones == null || bones.Count == 0) return;
            
            // カメラ情報取得
            var camera = sceneView.camera;
            if (camera == null) return;
            
            var cameraPos = camera.transform.position;
            var cameraForward = camera.transform.forward;
            
            // 距離キャッシュを更新
            UpdateDistanceCache(bones, cameraPos);
            
            // Handlesの設定
            Handles.BeginGUI();
            
            // マウス位置取得
            var mousePos = Event.current.mousePosition;
            Transform closestBone = null;
            float closestDistance = float.MaxValue;
            
            // ボーンの描画とインタラクション
            foreach (var boneInfo in bones)
            {
                var bone = boneInfo.Transform;
                if (bone == null) continue;
                
                // 距離チェック
                float distance;
                if (!boneDistanceCache.TryGetValue(bone, out distance))
                    continue;
                
                if (state.EnableDistanceFilter)
                {
                    if (distance < state.MinRenderDistance || distance > state.MaxRenderDistance)
                        continue;
                }
                
                // 視錐台カリング
                if (!IsInViewFrustum(camera, bone.position))
                    continue;
                
                // フェード計算
                float alpha = 1.0f;
                if (state.EnableDistanceFilter && state.DistanceFadeEnabled)
                {
                    float fadeRange = state.MaxRenderDistance * 0.2f;
                    alpha = Mathf.Clamp01((state.MaxRenderDistance - distance) / fadeRange);
                }
                
                // ボーンの色を取得
                Color boneColor = GetBoneColor(bone);
                boneColor.a *= alpha;
                
                // 親子間の線を描画
                if (boneInfo.Parent != null && boneDistanceCache.ContainsKey(boneInfo.Parent))
                {
                    DrawBoneLine(boneInfo.Parent.position, bone.position, alpha);
                }
                
                // ボーンの球体を描画
                var screenPos = DrawBoneSphere(camera, bone, boneColor, out bool isHovered);
                
                // マウスとの距離をチェック
                if (screenPos.HasValue)
                {
                    float screenDistance = Vector2.Distance(mousePos, screenPos.Value);
                    if (screenDistance < CLICK_THRESHOLD && screenDistance < closestDistance)
                    {
                        closestDistance = screenDistance;
                        closestBone = bone;
                    }
                }
                
                // ラベルを描画
                if (state.ShowLabels && alpha > 0.3f)
                {
                    DrawBoneLabel(camera, bone, boneColor);
                }
            }
            
            Handles.EndGUI();
            
            // ホバー状態の更新
            hoveredBone = closestBone;
            
            // クリック処理
            HandleMouseInteraction(closestBone);
            
            // 常に再描画をリクエスト（アニメーションやインタラクションのため）
            if (state.IsEnabled)
            {
                sceneView.Repaint();
            }
        }
        
        private void UpdateDistanceCache(List<BoneDetector.BoneInfo> bones, Vector3 cameraPos)
        {
            if (Time.frameCount == lastFrameCount)
                return;
                
            lastFrameCount = Time.frameCount;
            boneDistanceCache.Clear();
            
            foreach (var boneInfo in bones)
            {
                if (boneInfo.Transform != null)
                {
                    float distance = Vector3.Distance(cameraPos, boneInfo.Transform.position);
                    boneDistanceCache[boneInfo.Transform] = distance;
                }
            }
        }
        
        private bool IsInViewFrustum(Camera camera, Vector3 position)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, new Bounds(position, Vector3.one * 0.1f));
        }
        
        private Color GetBoneColor(Transform bone)
        {
            // 選択状態をチェック
            if (Selection.activeTransform == bone || 
                (Selection.transforms != null && Selection.transforms.Contains(bone)))
            {
                return state.SelectedColor;
            }
            else if (hoveredBone == bone)
            {
                return state.HoverColor;
            }
            else
            {
                return state.NormalColor;
            }
        }
        
        private void DrawBoneLine(Vector3 start, Vector3 end, float alpha)
        {
            var color = state.LineColor;
            color.a *= alpha;
            
            Handles.color = color;
            Handles.DrawAAPolyLine(state.LineWidth, start, end);
        }
        
        private Vector2? DrawBoneSphere(Camera camera, Transform bone, Color color, out bool isHovered)
        {
            isHovered = false;
            
            // スクリーン座標を計算
            var screenPos = camera.WorldToScreenPoint(bone.position);
            if (screenPos.z < 0) return null;
            
            // GUIポイントに変換
            var guiPoint = new Vector2(screenPos.x, camera.pixelHeight - screenPos.y);
            
            // 距離に基づいてサイズを調整
            float distance = boneDistanceCache.ContainsKey(bone) ? boneDistanceCache[bone] : 10f;
            float scaledSize = state.SphereSize * Mathf.Clamp(5f / distance, 0.5f, 2f);
            
            // 球体を描画
            Handles.color = color;
            
            // カスタムハンドルキャップを使用
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            var eventType = Event.current.GetTypeForControl(controlId);
            
            if (eventType == EventType.Repaint)
            {
                Handles.SphereHandleCap(
                    controlId,
                    bone.position,
                    Quaternion.identity,
                    scaledSize,
                    EventType.Repaint
                );
            }
            
            // ホバー状態チェック
            if (hoveredBone == bone)
            {
                isHovered = true;
                // ホバー時は少し大きく描画
                Handles.color = new Color(color.r, color.g, color.b, color.a * 0.3f);
                Handles.SphereHandleCap(
                    controlId,
                    bone.position,
                    Quaternion.identity,
                    scaledSize * 1.5f,
                    EventType.Repaint
                );
            }
            
            return guiPoint;
        }
        
        private void DrawBoneLabel(Camera camera, Transform bone, Color color)
        {
            var style = new GUIStyle(GUI.skin.label);
            style.fontSize = Mathf.RoundToInt(state.LabelSize);
            style.normal.textColor = color;
            style.alignment = TextAnchor.MiddleLeft;
            
            // ラベル位置を少しオフセット
            var labelPos = bone.position + camera.transform.right * state.SphereSize * 2f;
            Handles.Label(labelPos, bone.name, style);
        }
        
        private void HandleMouseInteraction(Transform closestBone)
        {
            if (closestBone == null) return;
            
            var evt = Event.current;
            
            if (evt.type == EventType.MouseDown && evt.button == 0)
            {
                // ダブルクリック検出
                float timeSinceLastClick = Time.realtimeSinceStartup - lastClickTime;
                bool isDoubleClick = (timeSinceLastClick < 0.3f && lastClickedBone == closestBone);
                
                if (isDoubleClick)
                {
                    // ダブルクリック: 階層で展開してフォーカス
                    EditorGUIUtility.PingObject(closestBone.gameObject);
                    Selection.activeTransform = closestBone;
                    SceneView.FrameLastActiveSceneView();
                }
                else
                {
                    // シングルクリック: 選択
                    if (evt.shift || evt.control || evt.command)
                    {
                        // 追加選択
                        var currentSelection = new List<Object>(Selection.objects);
                        if (currentSelection.Contains(closestBone.gameObject))
                        {
                            currentSelection.Remove(closestBone.gameObject);
                        }
                        else
                        {
                            currentSelection.Add(closestBone.gameObject);
                        }
                        Selection.objects = currentSelection.ToArray();
                    }
                    else
                    {
                        // 単独選択
                        Selection.activeTransform = closestBone;
                    }
                }
                
                lastClickTime = Time.realtimeSinceStartup;
                lastClickedBone = closestBone;
                
                evt.Use();
            }
        }
        
        public void Dispose()
        {
            boneDistanceCache?.Clear();
            detector?.ClearCache();
        }
    }
}