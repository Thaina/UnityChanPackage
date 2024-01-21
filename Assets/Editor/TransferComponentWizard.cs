using UnityEngine;

using UnityEditor;
using UnityEditorInternal;

using System;
using System.Linq;
using System.Collections.Generic;

using UnityChan;


public class TransferComponentWizard : ScriptableWizard
{
	public GameObject from,to;

	[MenuItem("CONTEXT/" + nameof(MonoBehaviour) + "/" + nameof(RemoveAndMoveFromPrefab))]
	public static void RemoveAndMoveFromPrefab(MenuCommand command)
	{
		Debug.Log(command.context);
		if(command.context is Component component)
		{
			ComponentUtility.CopyComponent(component);
			var go = component.gameObject;
			GameObject.DestroyImmediate(component);
			ComponentUtility.PasteComponentAsNew(go);

			foreach(var removed in PrefabUtility.GetRemovedComponents(go))
				removed.Apply(InteractionMode.UserAction);
		}
	}

	[MenuItem("CONTEXT/" + nameof(Transform) + "/" + nameof(TransferComponent))]
	public static void TransferComponent()
	{
        ScriptableWizard.DisplayWizard<TransferComponentWizard>(nameof(TransferComponent), "Apply");
	}

	private void OnWizardCreate()
	{
		if(!(from && to))
		{
			errorString = "Missing : " + string.Join(",",new[]{ from ? null : "from",to ? null : "to" }.Where((item) => !string.IsNullOrEmpty(item)));
			return;
		}

		Undo.RecordObject(to,nameof(TransferComponent));
		
		to.name = from.name;

		foreach(var (fromTranform,toTranform) in DeepJoin(from.transform,to.transform).Prepend((from.transform,to.transform)))
		{
			var fromComponents = fromTranform.gameObject.GetComponents<MonoBehaviour>();
			var toComponents = toTranform.gameObject.GetComponents<MonoBehaviour>();
			
			foreach(var component in fromComponents.Where((component) => !toComponents.Any((toComponent) => component.GetType() == toComponent.GetType())))
			{
				ComponentUtility.CopyComponent(component);
				ComponentUtility.PasteComponentAsNew(toTranform.gameObject);
			}
		}
		
		foreach(var (fromTranform,toTranform) in DeepJoin(from.transform,to.transform).Prepend((from.transform,to.transform)))
		{
			var fromSpringBone = fromTranform.gameObject.GetComponent<SpringBone>();
			var toSpringBone = toTranform.gameObject.GetComponent<SpringBone>();
			if(!(fromSpringBone && toSpringBone))
				continue;

			if(fromSpringBone.child)
			{
				var path = string.Join("/",GetRelativePath(from.transform,fromSpringBone.child));
				var toChild = to.transform.Find(path);
				Debug.Log("SpringBone.child : " + path + " get " + (toChild ? toChild : "null"));

				if(!toChild && fromSpringBone.child.gameObject.name.StartsWith("Locator_"))
				{
					path = path.Remove(path.LastIndexOf("/"));
					Debug.Log("> Move : " + fromSpringBone.child.gameObject.name + " to " + path);
					var target = GameObject.Instantiate(fromSpringBone.child.gameObject,to.transform.Find(path),false);
					toChild = target.transform;
					fromSpringBone.child.gameObject.transform.GetLocalPositionAndRotation(out var p,out var r);
					toChild.SetLocalPositionAndRotation(p,r);
					target.name = fromSpringBone.child.gameObject.name;
				}

				if(!toChild.IsChildOf(to.transform))
					throw new NotSupportedException();
				toSpringBone.child = toChild;
			}

			toSpringBone.colliders = fromSpringBone.colliders?.Length > 0 ? fromSpringBone.colliders.Select((collider) => {
				var path = string.Join("/",GetRelativePath(from.transform,collider.gameObject.transform));
				Debug.Log(path);
				var targetTransform = to.transform.Find(path);
				if(!targetTransform && collider.gameObject.name.StartsWith("Locator_"))
				{
					path = path.Remove(path.LastIndexOf("/"));
					Debug.Log("> Move : " + collider.gameObject.name + " to " + path);
					var target = GameObject.Instantiate(collider.gameObject,to.transform.Find(path),false);
					targetTransform = target.transform;
					collider.gameObject.transform.GetLocalPositionAndRotation(out var p,out var r);
					targetTransform.SetLocalPositionAndRotation(p,r);
					target.name = collider.gameObject.name;
				}

				return targetTransform.GetComponent<SpringCollider>();
			}).ToArray() : new SpringCollider[0];
		}

		if(from.GetComponent<SpringManager>() is var fromManager && to.GetComponent<SpringManager>() is var toManager)
		{
			toManager.springBones = fromManager.springBones?.Length > 0 ? fromManager.springBones.Select((springBone) => {
				var path = string.Join("/",GetRelativePath(from.transform,springBone.gameObject.transform));
				Debug.Log(path);
				var targetTransform = to.transform.Find(path);
				return targetTransform.gameObject.GetComponent<SpringBone>();
			}).ToArray() : new SpringBone[0];
		}

		if(from.GetComponent<Animator>() is var fromAnimator && to.GetComponent<Animator>() is var toAnimator)
		{
			toAnimator.runtimeAnimatorController = fromAnimator.runtimeAnimatorController;
			toAnimator.avatar = fromAnimator.avatar;
		}

		foreach(var (fromRenderer,toRenderer) in from.GetComponentsInChildren<Renderer>().Join(to.GetComponentsInChildren<Renderer>(),(outer) => outer.name,(inner) => inner.name,(outer,inner) => (outer,inner)))
		{
			if(!toRenderer.sharedMaterials.SequenceEqual(fromRenderer.sharedMaterials))
				toRenderer.sharedMaterials = fromRenderer.sharedMaterials;
		}

		foreach(var (fromBlinker,toBlinker) in from.GetComponents<AutoBlinkforSD>().Join(to.GetComponents<AutoBlinkforSD>(),(outer) => outer.name,(inner) => inner.name,(outer,inner) => (outer,inner)))
		{
			toBlinker.ref_face = to.GetComponentsInChildren<SkinnedMeshRenderer>().FirstOrDefault((renderer) => renderer.name == fromBlinker.ref_face.name);
		}
	}

	static IEnumerable<string> GetRelativePath(Transform parent,Transform target)
	{
		if(target == parent)
			return Enumerable.Empty<string>();

		if(!target.IsChildOf(parent))
			throw new System.NotSupportedException();

		return GetRelativePath(parent,target.parent).Append(target.name);
	}

	static IEnumerable<(Transform,Transform)> DeepJoin(Transform from,Transform to)
	{
		return from.OfType<Transform>().Join(to.OfType<Transform>(),(outer) => outer.name,(inner) => inner.name,(outer,inner) => {
			return (outer,inner);
		}).SelectMany((pair) => {
			var (from,to) = pair;
			return DeepJoin(from,to).Prepend(pair);
		});
	}
}