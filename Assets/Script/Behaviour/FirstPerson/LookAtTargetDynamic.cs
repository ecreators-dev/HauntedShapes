using UnityEditor;

using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		[RequireComponent(typeof(Camera))]
		public class LookAtTargetDynamic : MonoBehaviour
		{
				// this camera is only moving vertically (tilt)
				// a player transform in root is rotating (pan)
				// this script will let this camera look on a certain
				// point in distance, dermitted by crosshair
				private Camera cam;
				[SerializeField] private CrosshairHitVisual crosshair;

				private void Awake()
				{
						cam = GetComponent<Camera>();
				}

#if UNITY_EDITOR
				private void OnDrawGizmos()
				{
						if (crosshair is { })
						{
								(bool actualHit, RaycastHit hit) = crosshair.GetRaycastCollidersOnlyResult();
								if (actualHit)
								{
										Handles.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.2f);
										Handles.DrawSolidDisc(hit.point, hit.normal, 0.5f);

										Gizmos.color = Color.white;
										Gizmos.DrawLine(hit.point, hit.point + hit.normal * 0.25f);
								}
						}
				}
#endif

				private void Update()
				{
						// where is the player look at
						cam = CameraMoveType.Instance.GetCamera();
						crosshair ??= (CrosshairHitVisual)CrosshairHitVisual.Instance;
						crosshair.RaycastCollidersOnly(cam);
				}
		}
}