using Assets.Script.Behaviour;

using System;

using UnityEngine;

namespace Assets.Script.Components
{
		public abstract class Interactible : MonoBehaviour
		{
				[SerializeField] protected EObjectType identifier = EObjectType.UNDEFINED;
				[SerializeField] private bool locked;

				private bool oldHuntingStatus;

				public EObjectType ObjectType => identifier;

				protected bool IsHuntingActive { get; private set; }

				protected bool IsHuntingActiveChanged { get; private set; }

				public bool IsLocked => locked;

				public bool IsUnlocked => !locked;

				protected void Lock()
				{
						bool old = locked;
						locked = true;
						if (old != locked)
						{
								OnLockedStateChanged(old, locked);
						}
				}

				protected void Unlock()
				{
						bool old = locked;
						locked = false;
						if (old != locked)
						{
								OnLockedStateChanged(old, locked);
						}
				}

				protected virtual void OnLockedStateChanged(bool old, bool locked)
				{
				}

				public abstract bool CanInteract(PlayerBehaviour sender);

				public abstract void Interact(PlayerBehaviour sender);

				protected virtual void Update()
				{
						IsHuntingActive = Beans.InHunt;
						IsHuntingActiveChanged = IsHuntingActive != oldHuntingStatus;

						if (IsHuntingActiveChanged)
						{
								if (oldHuntingStatus)
								{
										OnHuntStop();
								}
								else
								{
										OnHuntStart();
								}
						}

						oldHuntingStatus = IsHuntingActive;
				}

				protected abstract void OnHuntStart();

				protected abstract void OnHuntStop();
		}
}
