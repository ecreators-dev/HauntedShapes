using Assets.Script.Components;
using Assets.Script.Model;

using System.Linq;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Only proxy delegate to parent
		/// </summary>
		public class InteractibleChild : Interactible
		{
				private Interactible parentInteractible;

				private void Awake()
				{
						if (transform.parent == null)
						{
								Debug.LogError($"Must have a parent and a component with {nameof(Interactible)}");
						}
				}

				private void Start()
				{
						parentInteractible = transform.GetComponentsInParent<Interactible>().FirstOrDefault(e => e is Component cmp && cmp.GetInstanceID() != this.GetInstanceID());
						if (parentInteractible is null)
						{
								Debug.LogError($"Must have a parent and a component with {nameof(Interactible)}");
						}
				}

				protected Interactible InteractibleParent => parentInteractible;

				public override bool CanInteract(PlayerBehaviour sender) => parentInteractible.CanInteract(sender);

				public override string GetTargetName() => parentInteractible.GetTargetName();

				protected override void Interact(PlayerBehaviour sender) => parentInteractible.RunInteraction(sender);

				private void OnCollisionEnter(Collision collision)
				{
						parentInteractible.OnCollisionEnterChild(collision);		
				}

				private void OnCollisionExit(Collision collision)
				{
						parentInteractible.OnCollisionExitChild(collision);		
				}
		}
}