using Assets.Script.Model;

using System.Linq;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Only proxy delegate to parent
		/// </summary>
		public sealed class InteractibleChild : MonoBehaviour, IInteractible
		{
				private IInteractible parentInteractible;

				public bool IsPickable => parentInteractible.IsPickable;

				public string GameObjectName => parentInteractible.GameObjectName;

				public string ImplementationTypeName => parentInteractible.ImplementationTypeName;

				private void Awake()
				{
						if (transform.parent == null)
						{
								Debug.LogError($"Must have a parent and a component with {nameof(IInteractible)}");
						}
				}

				private void Start()
				{
						parentInteractible = transform.GetComponentsInParent<IInteractible>().FirstOrDefault(e => e is Component cmp && cmp.GetInstanceID() != this.GetInstanceID());
						if (parentInteractible is null)
						{
								Debug.LogError($"Must have a parent and a component with {nameof(IInteractible)}");
						}
				}

				public void Drop()
				{
						parentInteractible.Drop();
				}

				public void OnPickup(PlayerBehaviour player)
				{
						parentInteractible.OnPickup(player);
				}

				public void TouchClickUpdate()
				{
						parentInteractible.TouchClickUpdate();
				}

				public void TouchOverUpdate()
				{
						parentInteractible.TouchClickUpdate();
				}
		}
}