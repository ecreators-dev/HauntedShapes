
using System;

using UnityEditor;

using UnityEngine;

[ExecuteAlways]
public class EditorPhysics : MonoBehaviour
{
#if UNITY_EDITOR
		private Rigidbody attachedRigidbody;
		private DateTime fallTime;
		private float activeTime;
		private Vector3 xzPosition;

		[MenuItem("Tools/Transform Tools/Align with ground %t")]
		static void Align()
		{
				Transform[] transforms = Selection.transforms;
				foreach (Transform myTransform in transforms)
				{
						if (Physics.Raycast(myTransform.position, -Vector3.up, out RaycastHit hit))
						{
								Vector3 targetPosition = hit.point;
								if (myTransform.gameObject.GetComponent<MeshFilter>() != null)
								{
										Bounds bounds = myTransform.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds;
										targetPosition.y += bounds.extents.y;
								}
								myTransform.position = targetPosition;
								Vector3 targetRotation = new Vector3(hit.normal.x, myTransform.eulerAngles.y, hit.normal.z);
								myTransform.eulerAngles = targetRotation;
						}
				}
		}

		[ContextMenu("Active Rigidbody Gravity 5 Sek")]
		public void EditorFall()
		{
				attachedRigidbody = GetComponent<Rigidbody>();
				Debug.Log($"Falling started: {gameObject.name}");
				fallTime = DateTime.Now;
				if (attachedRigidbody.useGravity is false)
				{
						Debug.Log("useGravity is false. Simulation not working without.");
				}
				EditorApplication.update = OnEditorUpdate;
				attachedRigidbody.WakeUp();
				activeTime = 0;
				Physics.autoSimulation = false;
				xzPosition = transform.position;
		}

		private void OnEditorUpdate()
		{
				if (activeTime >= 5)
				{
						Physics.autoSimulation = true;
						attachedRigidbody.Sleep();

						EditorApplication.update = null;
						TimeSpan durationFall = DateTime.Now - fallTime;
						Debug.Log($"Falling stopped: {gameObject.name} took {durationFall}");
				}
				else
				{
						attachedRigidbody.velocity = new Vector3
						{
								x = xzPosition.x,
								y = attachedRigidbody.velocity.y,
								z = xzPosition.z
						};
						transform.position = new Vector3
						{
								x = xzPosition.x,
								y = transform.position.y,
								z = xzPosition.z
						};
						Physics.Simulate(Time.deltaTime);
						Debug.Log($"Falling Update: {gameObject.name}");
				}
				activeTime += Time.deltaTime;
		}
#endif
}
