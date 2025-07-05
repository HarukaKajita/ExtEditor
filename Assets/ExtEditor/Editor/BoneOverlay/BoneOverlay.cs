using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace ExtEditor.BoneOverlay
{
	[Overlay(typeof(SceneView), "Bone Overlay", true)]
	[Icon("Icons/d_AvatarMask Icon")]
	public class BoneOverlay : Overlay
	{
		private BoneOverlayState state;
		private BoneOverlayRenderer renderer;
		private BoneDetector detector;
		
		private Toggle enableToggle;
		private Label statusLabel;
		private Slider distanceSlider;
		private Label distanceLabel;
		
		public override void OnCreated()
		{
			state = new BoneOverlayState();
			detector = new BoneDetector();
			detector.SetState(state);
			renderer = new BoneOverlayRenderer(state, detector);
			
			SceneView.duringSceneGui += OnSceneGUI;
			Selection.selectionChanged += OnSelectionChanged;
		}
		
		public override void OnWillBeDestroyed()
		{
			SceneView.duringSceneGui -= OnSceneGUI;
			Selection.selectionChanged -= OnSelectionChanged;
			
			renderer?.Dispose();
			state?.Save();
		}
		
		public override VisualElement CreatePanelContent()
		{
			var root = new VisualElement();
			root.style.minWidth = 200;
			root.style.paddingLeft = 4;
			root.style.paddingRight = 4;
			root.style.paddingTop = 4;
			root.style.paddingBottom = 4;
			
			// タイトル
			var titleLabel = new Label("Bone Overlay");
			titleLabel.style.fontSize = 12;
			titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
			titleLabel.style.marginBottom = 8;
			root.Add(titleLabel);
			
			// 有効/無効トグル
			enableToggle = new Toggle("Enable");
			enableToggle.value = state.IsEnabled;
			enableToggle.RegisterValueChangedCallback(evt =>
			{
				state.IsEnabled = evt.newValue;
				SceneView.RepaintAll();
			});
			root.Add(enableToggle);
			
			// 距離フィルタ設定
			var distanceContainer = new VisualElement();
			distanceContainer.style.marginTop = 8;
			
			var distanceFilterToggle = new Toggle("Distance Filter");
			distanceFilterToggle.value = state.EnableDistanceFilter;
			distanceFilterToggle.RegisterValueChangedCallback(evt =>
			{
				state.EnableDistanceFilter = evt.newValue;
				distanceSlider.SetEnabled(evt.newValue);
				SceneView.RepaintAll();
			});
			distanceContainer.Add(distanceFilterToggle);
			
			// 距離スライダー
			distanceSlider = new Slider("Max Distance", 1f, 100f);
			distanceSlider.value = state.MaxRenderDistance;
			distanceSlider.showInputField = true;
			distanceSlider.SetEnabled(state.EnableDistanceFilter);
			distanceSlider.RegisterValueChangedCallback(evt =>
			{
				state.MaxRenderDistance = evt.newValue;
				distanceLabel.text = $"Distance: {evt.newValue:F1}m";
				SceneView.RepaintAll();
			});
			distanceContainer.Add(distanceSlider);
			
			distanceLabel = new Label($"Distance: {state.MaxRenderDistance:F1}m");
			distanceLabel.style.fontSize = 10;
			distanceLabel.style.color = new Color(0.7f, 0.7f, 0.7f);
			distanceContainer.Add(distanceLabel);
			
			root.Add(distanceContainer);
			
			// ステータス表示
			statusLabel = new Label("Ready");
			statusLabel.style.marginTop = 8;
			statusLabel.style.fontSize = 10;
			statusLabel.style.color = new Color(0.7f, 0.7f, 0.7f);
			root.Add(statusLabel);
			
			return root;
		}
		
		private void OnSceneGUI(SceneView sceneView)
		{
			if (!state.IsEnabled) return;
			
			var detectedBones = detector.DetectBones();
			renderer.DrawBones(sceneView, detectedBones);
			
			// ステータス更新
			if (statusLabel != null)
			{
				statusLabel.text = $"Bones: {detectedBones.Count}";
			}
		}
		
		private void OnSelectionChanged()
		{
			if (state.IsEnabled)
			{
				SceneView.RepaintAll();
			}
		}
	}
}