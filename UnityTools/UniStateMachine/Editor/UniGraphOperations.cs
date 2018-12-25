﻿using System.Linq;
using Boo.Lang;
using UniEditorTools;
using UniStateMachine.Nodes;
using UnityEditor;
using UnityEngine;
using XNode;

namespace UniStateMachine.EditorTools
{
	
	public class UniGraphOperations 
	{

		[MenuItem("Assets/UniGraph/Cleanup UniGraph")]
		public static void CleanUpSelectedUniGraph()
		{
			var selections = Selection.objects;
			foreach (var selection in selections)
			{
				if(selection is UniNodesGraph graph)
					CleanUpUniGraph(graph);
			}
		}
		
		[MenuItem("Assets/UniGraph/Stop UniGraph")]
		public static void StopUniGraph()
		{
			var selections = Selection.objects;
			foreach (var selection in selections)
			{
				if(selection is UniNodesGraph graph)
					graph.Dispose();
			}
		}

		public static void CleanUpUniGraph(UniNodesGraph graph)
		{

			var assetPath = AssetDatabase.GetAssetPath(graph);
			if (string.IsNullOrEmpty(assetPath))
				return;

			var assets = AssetDatabase.LoadAllAssetsAtPath(assetPath).ToList();
			var nodes = graph.nodes;
			
			assets.Remove(graph);

			var itemsToRemove = new List<Node>();
			
			foreach (var node in nodes)
			{
				if (!node)
					itemsToRemove.Add(node);
			}

			foreach (var node in itemsToRemove)
			{
				nodes.Remove(node);
			}
			
			foreach (var asset in assets)
			{
				if (!asset || !nodes.Contains(asset))
				{
					nodes.Remove(asset as Node);
					asset.DestroyNestedAsset();
					continue;
				}
				
				Debug.LogFormat("FIND ASSET {0}",asset.name);
			}
			
			EditorUtility.SetDirty(graph);
		}
		
	}
}
