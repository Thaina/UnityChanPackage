//
//SpingManager.cs for unity-chan!
//
//Original Script is here:
//ricopin / SpingManager.cs
//Rocket Jump : http://rocketjump.skr.jp/unity3d/109/
//https://twitter.com/ricopin416
//
//Revised by N.Kobayashi 2014/06/24
//           Y.Ebata
//
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

using System.Linq;
using SpringBones = Unity.Animations.SpringBones;
#endif

namespace UnityChan
{
	public class SpringManager : MonoBehaviour
	{
		//Kobayashi
		// DynamicRatio is paramater for activated level of dynamic animation 
		public float dynamicRatio = 1.0f;

		//Ebata
		public float			stiffnessForce;
		public AnimationCurve	stiffnessCurve;
		public float			dragForce;
		public AnimationCurve	dragCurve;
		public SpringBone[] springBones;

		void Start ()
		{
			UpdateParameters ();
		}

#if UNITY_EDITOR
		[ContextMenu(nameof(UpdateParameters))]
		void Update ()
		{

		//Kobayashi
		if(dynamicRatio >= 1.0f)
			dynamicRatio = 1.0f;
		else if(dynamicRatio <= 0.0f)
			dynamicRatio = 0.0f;
		//Ebata
		UpdateParameters();

		}
#endif	
		private void LateUpdate ()
		{
			//Kobayashi
			if (dynamicRatio != 0.0f) {
				for (int i = 0; i < springBones.Length; i++) {
					if (dynamicRatio > springBones [i].threshold) {
						springBones [i].UpdateSpring ();
					}
				}
			}
		}

		private void UpdateParameters ()
		{
			UpdateParameter ("stiffnessForce", stiffnessForce, stiffnessCurve);
			UpdateParameter ("dragForce", dragForce, dragCurve);
		}
	
		private void UpdateParameter (string fieldName, float baseValue, AnimationCurve curve)
		{
			#if UNITY_EDITOR
			var start = curve.keys [0].time;
			var end = curve.keys [curve.length - 1].time;
			//var step	= (end - start) / (springBones.Length - 1);
		
			var prop = springBones [0].GetType ().GetField (fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
		
			for (int i = 0; i < springBones.Length; i++) {
				//Kobayashi
				if (!springBones [i].isUseEachBoneForceSettings) {
					var scale = curve.Evaluate (start + (end - start) * i / (springBones.Length - 1));
					prop.SetValue (springBones [i], baseValue * scale);
				}
			}
			#endif
		}

#if UNITY_EDITOR
		[ContextMenu(nameof(UpgradeToSpringBonePackage))]
		void UpgradeToSpringBonePackage()
		{
			if(!(gameObject.TryGetComponent(out SpringBones.SpringManager newSpringManager) && newSpringManager))
				newSpringManager = gameObject.AddComponent<SpringBones.SpringManager>();

			newSpringManager.dynamicRatio = dynamicRatio;

			var colliders = gameObject.GetComponentsInChildren<SpringCollider>();
			foreach(var collider in colliders)
			{
				if(collider.gameObject.TryGetComponent(out SpringBones.SpringSphereCollider springSphereCollider) && springSphereCollider)
					continue;

				springSphereCollider = collider.gameObject.AddComponent<SpringBones.SpringSphereCollider>();
				springSphereCollider.radius = collider.radius;
			}

			newSpringManager.springBones = springBones.Select((springBone) => {
				if(springBone.gameObject.TryGetComponent(out SpringBones.SpringBone newSpringBone) && newSpringBone)
					return newSpringBone;
					
				newSpringBone = springBone.gameObject.AddComponent<SpringBones.SpringBone>();

				newSpringBone.stiffnessForce = springBone.stiffnessForce * 700 / 0.01f;
				
				newSpringBone.dragForce = springBone.dragForce;
				newSpringBone.springForce = springBone.springForce * 1000;

				newSpringBone.radius = springBone.radius;

				newSpringBone.sphereColliders = springBone.colliders.Select((collider) => {
					return collider.gameObject.GetComponent<SpringBones.SpringSphereCollider>();
				}).ToArray();

				return newSpringBone;
			}).ToArray();

			foreach(var collider in colliders)
				GameObject.DestroyImmediate(collider);

			foreach(var springBone in gameObject.GetComponentsInChildren<SpringBone>())
				GameObject.DestroyImmediate(springBone);

			foreach(var randomWind in gameObject.GetComponentsInChildren<RandomWind>())
				GameObject.DestroyImmediate(randomWind);

			GameObject.DestroyImmediate(this);
		}
#endif
	}
}