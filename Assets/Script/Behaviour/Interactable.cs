using Assets.Script.Model;

using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[Serializable]
		public abstract class Interactable : MonoBehaviour, IInteractible
		{
				public string GameObjectName => this.GetGameObjectName();
				public string ImplementationTypeName => this.GetImplementationTypeName();

				public abstract bool IsPickable { get; }

				public abstract void Drop();

				public abstract void OnPickup(PlayerBehaviour player);

				public abstract void TouchClickUpdate();

				public abstract void TouchOverUpdate();
		}
}