using Assets.Script.Model;

using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[Serializable]
		public abstract class _Interactable : MonoBehaviour, IInteractible
		{
				public string GameObjectName => this.GetGameObjectName();
				public string ImplementationTypeName => this.GetImplementationTypeName();

				/// <summary>
				/// Identifies the object as pickable by player
				/// </summary>
				public abstract bool IsPickable { get; }

				public abstract void Drop();

				public abstract void OnPickup(PlayerBehaviour player);

				public abstract void TouchClickUpdate();

				public abstract void TouchOverUpdate();
		}
}