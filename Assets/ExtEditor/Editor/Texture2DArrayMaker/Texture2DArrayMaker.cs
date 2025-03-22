using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.Editor.Texture2DArrayMaker
{
	[CreateAssetMenu(fileName = "Texture2DArrayMaker", menuName = "Texture2DArrayMaker")]
	public class Texture2DArrayMaker : ScriptableObject
	{
		[SerializeField] private List<Texture2D> inputTextures = new();
		[SerializeField] private Texture2DArray outputTexture;
		[HideInInspector]
		[SerializeField] private bool isValid = false;
		
		[ContextMenu("Create or Update Texture2DArray")]
		private void CreateOrUpdateTexture2DArrayContextMenu()
		{
			
			try
			{
				AssetDatabase.StartAssetEditing();
				var textureArray = CreateOrUpdateTexture2DArray();
				var path = AssetDatabase.GetAssetPath(outputTexture);
				var isNotAsset = string.IsNullOrEmpty(path);

				if (isNotAsset)
				{
					path = AssetDatabase.GetAssetPath(this).Replace(".asset", "_TexArray.asset");
					AssetDatabase.CreateAsset(outputTexture, path);
				}
				else
				{
					EditorUtility.CopySerialized(textureArray, outputTexture);
				}
			}
			finally
			{
				AssetDatabase.StopAssetEditing();
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
			
		}
		private Texture2DArray CreateOrUpdateTexture2DArray()
		{
			OnValidate();
			if (!isValid)
				throw new Exception("Textures are not valid");
			var first = inputTextures.FirstOrDefault();
			if (first == null)
				throw new Exception("Textures are empty");
			
			inputTextures = inputTextures.Where(texture => texture != null).ToList();
			
			var width = first.width;
			var height = first.height;
			var format = first.format;
			var textureName = name + "_TexArray";
			
			var textureArray = new Texture2DArray(width, height, inputTextures.Count, format, true);
			if(textureArray.name != textureName)
				textureArray.name = textureName;
			
			for (var i = 0; i < inputTextures.Count; i++)
				Graphics.CopyTexture(inputTextures[i], 0, 0, textureArray, i, 0);
				// outputTexture.SetPixels(inputTextures[i].GetPixels(), i);
				
			textureArray.Apply();
			return textureArray;
		}
		
		private void OnValidate()
		{
			var first = inputTextures.FirstOrDefault();
			if (first == null)
			{
				isValid = false;
				Debug.LogWarning("Textures are empty");
				return;
			}
			var width = first.width;
			var height = first.height;
			var format = first.format;
			
			foreach (var texture in inputTextures)
			{
				if (width != texture.width || height != texture.height)
				{
					isValid = false;
					Debug.LogWarning("All textures must have the same width and height");
				}
				if(format != texture.format)
				{
					isValid = false;
					Debug.LogWarning("All textures must have the same format");
				}
			}
			isValid = true;
		}
	}
}
