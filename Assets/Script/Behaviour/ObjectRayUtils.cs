
using UnityEngine;

namespace Assets.Script.Behaviour
{
		public static class ObjectRayUtils
		{
				/// <summary>
				/// Identifiers the first hit object with Collider, that will hit from Mouse Position. From Camera.main
				/// </summary>
				public static Ray ObjectRayMousePosition(this MonoBehaviour _)
				{
						return Camera.main.ScreenPointToRay(Input.mousePosition);
				}

				/// <summary>
				/// Identifiers the first hit object with Collider, that will hit from Screen Position Center. From Camera.main
				/// </summary>
				public static Ray ObjectRayScreenCenter(this MonoBehaviour _)
				{
						// at screen center:
						return Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
				}

				/// <summary>
				/// Get nearest Object by Component and radius
				/// </summary>
				public static T FindComponentAroundRadiusOf<T>(this MonoBehaviour instance, int radius)
				{
						Collider[] hitColliders = Physics.OverlapSphere(instance.transform.position, radius);
						return GetClosestAround(hitColliders, radius);

						T GetClosestAround(Collider[] foundInRange, float radius)
						{
								T closest = default;
								float distance = radius;
								Vector3 currentPos = instance.transform.position;
								foreach (Collider c in foundInRange)
								{
										// not self!
										if (c.gameObject == instance.gameObject)
												continue;

										if (c.TryGetComponent(out T current))
										{
												Vector3 t = c.transform.position - currentPos;
												float currentDistance = t.magnitude;
												if (currentDistance < distance)
												{
														closest = current;
														distance = currentDistance;
												}
										}
								}
								return closest;
						}
				}
		}
}