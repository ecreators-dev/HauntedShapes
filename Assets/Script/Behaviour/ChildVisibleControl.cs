
using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class ChildVisibleControl : MonoBehaviour
		{
				[SerializeField] Renderer[] inTrigggerVisible;
				[SerializeField] Renderer[] offTrigggerVisible;


				private void OnTriggerEnter(Collider other)
				{
						if (IsPlayer(other))
						{
								foreach (Renderer item in inTrigggerVisible)
								{
										item.enabled = true;
								}

								foreach (Renderer item in offTrigggerVisible)
								{
										item.enabled = false;
								}
						}
				}

				private void OnTriggerExit(Collider other)
				{
						if (IsPlayer(other))
						{
								foreach (Renderer item in inTrigggerVisible)
								{
										item.enabled = false;
								}

								foreach (Renderer item in offTrigggerVisible)
								{
										item.enabled = true;
								}
						}
				}

				private bool IsPlayer(Collider other) => other.gameObject.CompareTag("Player");
		}
}