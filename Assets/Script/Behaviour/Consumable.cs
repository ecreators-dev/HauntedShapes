using Assets.Script.Components;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public abstract class Consumable : Interactible
		{
				public bool Visible { get; set; } = true;

				protected MeshRenderer MeshRenderer { get; private set; }

				protected virtual void Awake()
				{
						MeshRenderer = GetComponent<MeshRenderer>();
				}

				protected override void Update()
				{
						base.Update();

						MeshRenderer.enabled = Visible;
				}
		}
}