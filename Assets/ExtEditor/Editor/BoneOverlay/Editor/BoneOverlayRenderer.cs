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
        
        // ラベルインタラクション用
        private Transform clickedLabelBone = null;
        
        // パフォーマンス最適化用キャッシュ
        private Plane[] frustumPlanes;
        private int lastFrustumUpdateFrame = -1;
        private Vector3 lastCameraPos;
        private Quaternion lastCameraRot;
        private static GUIStyle labelStyle;
        private List<BoneDetector.BoneInfo> sortedBonesCache;
        private bool needsSort = true;
        private Transform previousHoveredBone;
        
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
            
            // 距離キャッシュを更新
            UpdateDistanceCache(bones, cameraPos);
            
            // カメラが移動した場合のみソートを実行
            if (needsSort || sortedBonesCache == null || sortedBonesCache.Count != bones.Count)
            {
                sortedBonesCache = bones.OrderByDescending(b => 
                    boneDistanceCache.TryGetValue(b.Transform, out float d) ? d : float.MaxValue
                ).ToList();
                needsSort = false;
            }
            
            var sortedBones = sortedBonesCache;
            
            // マウス位置取得
            var mousePos = Event.current.mousePosition;
            Transform closestBone = null;
            float closestDepth = float.MaxValue;
            
            // ラベルクリックの結果をリセット
            clickedLabelBone = null;
            
            // ボーンの描画とインタラクション
            var labelsToRender = new List<(Transform bone, Color color)>();
            
            foreach (var boneInfo in sortedBones)
            {
                var bone = boneInfo.Transform;
                if (bone == null) continue;
                
                // 距離チェック
                float distance;
                if (!boneDistanceCache.TryGetValue(bone, out distance))
                    continue;
                
                // 距離フィルタリング（常に有効）
                if (distance < state.MinRenderDistance || distance > state.MaxRenderDistance)
                    continue;
                
                // 視錐台カリング
                if (!IsInViewFrustum(camera, bone.position))
                    continue;
                
                // フェード計算
                float alpha = 1.0f;
                if (state.DistanceFadeEnabled)
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
                var screenPosAndSize = DrawBoneSphere(camera, bone, boneColor, out bool isHovered);
                
                // マウスとの距離をチェック
                if (screenPosAndSize.HasValue)
                {
                    var screenPos = new Vector2(screenPosAndSize.Value.x, screenPosAndSize.Value.y);
                    var screenRadius = screenPosAndSize.Value.z;
                    float screenDistance = Vector2.Distance(mousePos, screenPos);
                    
                    // マウスが円の範囲内にあり、かつ最も手前（カメラに近い）の場合
                    if (screenDistance < screenRadius && distance < closestDepth)
                    {
                        closestDepth = distance;
                        closestBone = bone;
                    }
                }
                
                // ラベルを描画リストに追加（距離フィルタリング付き）
                if (state.ShowLabels && distance <= state.MaxLabelRenderDistance)
                {
                    // ラベル用のアルファ計算
                    float labelAlpha = 1.0f;
                    if (state.DistanceFadeEnabled)
                    {
                        float labelFadeRange = state.MaxLabelRenderDistance * 0.2f;
                        labelAlpha = Mathf.Clamp01((state.MaxLabelRenderDistance - distance) / labelFadeRange);
                    }
                    
                    if (labelAlpha > 0.1f)
                    {
                        var labelColor = state.LabelColor;
                        labelColor.a *= labelAlpha * alpha;
                        labelsToRender.Add((bone, labelColor));
                    }
                }
            }
            
            // ラベルをHandles.Buttonで描画
            foreach (var (bone, color) in labelsToRender)
            {
                DrawBoneLabel(camera, bone, color);
            }
            
            // ボーンがクリックされていないが、ラベルがクリックされている場合は、ラベルのボーンをクリックとみなす
            if (closestBone == null && clickedLabelBone != null)
            {
                closestBone = clickedLabelBone;
            }
            
            // ホバー状態の更新
            hoveredBone = closestBone;
            
            // クリック処理
            HandleMouseInteraction(closestBone);
            
            // ホバー状態が変わった、またはマウスが動いている時のみ再描画
            if (state.IsEnabled && (hoveredBone != previousHoveredBone || Event.current.type == EventType.MouseMove))
            {
                sceneView.Repaint();
                previousHoveredBone = hoveredBone;
            }
        }
        
        private void UpdateDistanceCache(List<BoneDetector.BoneInfo> bones, Vector3 cameraPos)
        {
            if (Time.frameCount == lastFrameCount)
                return;
                
            lastFrameCount = Time.frameCount;
            
            // カメラ位置が変わった場合はソートが必要
            if (lastCameraPos != cameraPos)
            {
                needsSort = true;
            }
            
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
            // 視錐台平面をフレーム単位でキャッシュ
            if (frustumPlanes == null || lastFrustumUpdateFrame != Time.frameCount || 
                lastCameraPos != camera.transform.position || lastCameraRot != camera.transform.rotation)
            {
                frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
                lastFrustumUpdateFrame = Time.frameCount;
                lastCameraPos = camera.transform.position;
                lastCameraRot = camera.transform.rotation;
            }
            
            return GeometryUtility.TestPlanesAABB(frustumPlanes, new Bounds(position, Vector3.one * 0.1f));
        }
        
        private Color GetBoneColor(Transform bone)
        {
            // 選択状態をチェック
            if (Selection.activeGameObject == bone.gameObject || 
                (Selection.gameObjects != null && Selection.gameObjects.Contains(bone.gameObject)))
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
        
        private Vector3? DrawBoneSphere(Camera camera, Transform bone, Color color, out bool isHovered)
        {
            isHovered = false;
            
            // スクリーン座標を計算
            var screenPos = camera.WorldToScreenPoint(bone.position);
            if (screenPos.z < 0) return null;
            
            // 透視投影カメラの場合
            float screenRadius;
            if (!camera.orthographic)
            {
                // 半径ぶんだけカメラの “右方向” にオフセットした点をスクリーン変換
                Vector3 offsetPos = bone.position + camera.transform.right * state.SphereSize;
                Vector3 edgeOnScreen = camera.WorldToScreenPoint(offsetPos);

                // 中心→端 の長さがピクセル半径
                float pixelRadius = (edgeOnScreen - screenPos).magnitude;
                screenRadius = pixelRadius;  // 直径
            }
            // 正射影カメラの場合
            else
            {
                // orthographicSize = 画面の半縦幅(world) なので
                float pixelsPerUnit = camera.pixelHeight / (camera.orthographicSize * 2f);
                screenRadius = state.SphereSize * pixelsPerUnit;   // 直径
            }
            
            // GUIポイントに変換
            var guiPoint = new Vector2(screenPos.x, camera.pixelHeight - screenPos.y);
            
            // 距離に基づいてサイズを調整
            float distance = boneDistanceCache.ContainsKey(bone) ? boneDistanceCache[bone] : 10f;
            float scaledSize = state.SphereSize * Mathf.Clamp(5f / distance, 0.5f, 2f)/2;
            var dirForCamera = (bone.position - camera.transform.position).normalized;
            // 円を描画
            Handles.color = color;
            
            // カスタムハンドルキャップを使用
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            var eventType = Event.current.GetTypeForControl(controlId);
            
            if (eventType == EventType.Repaint)
            {
                Handles.DrawSolidDisc(bone.position,dirForCamera, scaledSize);
            }
            
            // ホバー状態チェック
            if (hoveredBone == bone)
            {
                isHovered = true;
                // ホバー時は少し大きく描画
                Handles.color = new Color(color.r, color.g, color.b, color.a * 0.3f);
                Handles.DrawSolidDisc(bone.position,dirForCamera, scaledSize * 1.2f);
            }
            
            return new Vector3(guiPoint.x, guiPoint.y, screenRadius);
        }
        
        private void DrawBoneLabel(Camera camera, Transform bone, Color color)
        {
            // ラベル位置を少しオフセット
            var labelPos = bone.position + camera.transform.up * state.SphereSize;
            
            // ラベルのスタイルを再利用
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle("Label");
                labelStyle.alignment = TextAnchor.LowerLeft;
                labelStyle.padding = new RectOffset(4, 4, 2, 2);
            }
            
            // 動的なプロパティのみ更新
            labelStyle.fontSize = Mathf.RoundToInt(state.LabelSize);
            labelStyle.normal.textColor = color;
            
            // コンテンツを作成
            var content = new GUIContent(bone.name);
            
            // GUI描画とクリック検出
            Handles.BeginGUI();
            
            // スクリーン座標に変換
            var screenPos = camera.WorldToScreenPoint(labelPos) + new Vector3(0, 10, 0);
            if (screenPos.z > 0)
            {
                var guiPoint = new Vector2(screenPos.x, camera.pixelHeight - screenPos.y);
                var size = labelStyle.CalcSize(content);
                var rect = new Rect(guiPoint.x - size.x * 0.5f, guiPoint.y - size.y * 0.5f, size.x, size.y);
                
                // 左クリックのみを検出
                var evt = Event.current;
                if (evt.type == EventType.MouseDown && evt.button == 0 && rect.Contains(evt.mousePosition))
                {
                    clickedLabelBone = bone;
                    evt.Use(); // 左クリックのみイベントを消費
                }
                
                // ラベルを描画
                var oldContentColor = GUI.contentColor;
                GUI.contentColor = color;
                GUI.Label(rect, content, labelStyle);
                GUI.contentColor = oldContentColor;
            }
            
            Handles.EndGUI();
        }
        
        private void HandleMouseInteraction(Transform closestBone)
        {
            var evt = Event.current;
            
            if (evt.type == EventType.MouseDown && evt.button == 0)
            {
                if (closestBone != null)
                {
                    // ボーンがある場合の選択処理
                    if (evt.shift || evt.control || evt.command)
                    {
                        // 追加選択
                        var currentSelection = new List<Object>(Selection.gameObjects);
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
                        Selection.activeGameObject = closestBone.gameObject;
                    }
                    evt.Use();
                }
                else if (!evt.shift && !evt.control && !evt.command)
                {
                    // ボーンがない場合、かつ修飾キーが押されていない場合は選択を解除
                    Selection.activeGameObject = null;
                    // 空クリックの場合はイベントを消費しない（Unity標準の動作に任せる）
                }
                
                // 選択変更後に再描画（クリック時のみ）
                SceneView.RepaintAll();
            }
        }
        
        public void Dispose()
        {
            boneDistanceCache?.Clear();
            detector?.ClearCache();
        }
    }
}