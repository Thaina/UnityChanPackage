using UnityEngine;

using UnityEditor;
using UnityEditor.Callbacks;
using Autodesk.Fbx;
using Unity.VisualScripting;
using System;
using System.IO;
using System.Linq;

public static class SimpleFBXEditor
{
	[MenuItem("UnityChanTools/OrderX")]
	public static void OrderX()
	{
		float count = Selection.gameObjects.Length;
		float left = (count - 1) / 2;
		float z = Selection.gameObjects.Average((selection) => selection.transform.position.z);
		foreach(var selection in Selection.gameObjects)
		{
			selection.transform.position = new Vector3(left,0,z);
			left--;
		}
	}

	[MenuItem("UnityChanTools/RenameFacePartForAnimation")]
	public static bool RenameFacePartForAnimation()
	{
		try
		{
			int instanceID = Selection.activeInstanceID;
			var assetPath = Path.GetFullPath(AssetDatabase.GetAssetPath(instanceID));
			if(!Path.GetExtension(assetPath).ToLower().EndsWith("fbx"))
				return false;

			Debug.Log(instanceID + " => " + assetPath);

			using var fbxManager = FbxManager.Create();
			using var importer = FbxImporter.Create(fbxManager,"import");
			using var exporter = FbxExporter.Create(fbxManager,"export");

			Debug.Assert(importer.Initialize(assetPath));
			Debug.Assert(exporter.Initialize(assetPath));

			using var fbxScene = FbxScene.Create(fbxManager,"imported scene");
			Debug.Assert(importer.Import(fbxScene));

			Debug.Assert(importer.IsFBX());

			Debug.Log(importer.GetFileHeaderInfo());

			var root = fbxScene.GetRootNode();
			
			foreach(var child in Enumerable.Range(0,root.GetChildCount()).Select((i) => root.GetChild(i)))
			{
				Debug.LogFormat("import : {0}",child);

				if(child.GetName().IndexOf("_face") > 0)
					child.SetName("_face");
					
				if(child.GetName() == "_face")
				{
					var count = child.GetChildCount();
					Debug.Log("face child count : " + count);

					int i = 0;
					while(child.GetGeometry() is var geom && i < geom?.GetDeformerCount())
					{
						var deformer = geom.GetDeformer(i);
						if(deformer?.GetDeformerType() == FbxDeformer.EDeformerType.eBlendShape && deformer?.GetSrcObjectCount() > 1)
						{
							foreach(var blendShp in Enumerable.Range(0,deformer.GetSrcObjectCount()).Select((n) => deformer.GetSrcObject(n)))
							{
								string name = blendShp.GetName();
								if(!name.ToLower().StartsWith("face."))
								{
									blendShp.SetName("face." + name);
									Debug.Log(name + " => " + blendShp.GetName());
								}
							}
						}

						i++;
					}
				}
				
				Debug.LogFormat("export : {0}",child);
			}

			Debug.Assert(exporter.Export(fbxScene));

			return true;
		}
		catch(Exception e)
		{
			Debug.LogException(e);
			return false;
		}
	}
}