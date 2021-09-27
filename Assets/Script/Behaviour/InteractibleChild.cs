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
				[SerializeField] private Interactible parentInteractible;

				protected Interactible ParentWithAssertion => GetComponentInParent<Interactible>(transform);

				private void Awake()
				{
						if (transform.parent == null)
						{
								Debug.LogError($"Must have a parent and a component with instance of {nameof(Interactible)}");
						}
				}

				public override bool CanInteract(PlayerBehaviour sender) => ParentWithAssertion?.CanInteract(sender) ?? false;

				public override string GetTargetName() => ParentWithAssertion?.GetTargetName();

				protected override void Interact(PlayerBehaviour sender) => ParentWithAssertion?.RunInteraction(sender);

				/// <summary>
				/// Sends Collision to parent Interactible
				/// </summary>
				private void OnCollisionEnter(Collision collision)
				{
						ParentWithAssertion?.OnCollisionEnterChild(collision);
				}

				/// <summary>
				/// Sends Collision to parent Interactible
				/// </summary>
				private void OnCollisionExit(Collision collision)
				{
						ParentWithAssertion?.OnCollisionExitChild(collision);
				}

				private static T GetComponentInParent<T>(Transform reference)
				{
						if (reference.parent == null)
						{
								Debug.LogError($"Does not hava a parent! {reference.gameObject.name}");
								return default;
						}

						if (!reference.parent.TryGetComponentAllParent(out T result))
						{
								Debug.LogError($"Unable to find: {typeof(T).Name}({nameof(T)} in {reference.parent.gameObject.name})");
								return default;
						}
						return result;
				}
		}
}