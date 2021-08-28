using Assets.Script.Components;

using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// You can pickup and drop
		/// </summary>
		public abstract class PickupItem : Interactible
		{
				/// <summary>
				/// Represents the player that actual captured the equipment
				/// </summary>
				protected PlayerBehaviour User { get; private set; }

				public bool IsTakenByPlayer { get; private set; }

				[CalledByPlayerBehaviour]
				public virtual void OnPlayer_ItemPickedUp(PlayerBehaviour newUser)
				{
						if (newUser is { })
						{
								if (User is null)
								{
										User = newUser;
										IsTakenByPlayer = true;
										OnPickedUp();
								}
								else
								{
										Debug.LogError($"Duplicated pickup action. Item already in use of player {User.gameObject.name}");
								}
						}
						else
						{
								Debug.LogError($"Parameter \"player\" was null!");
						}
				}

				/// <summary>
				/// After the player dropped this item
				/// </summary>
				[CalledByPlayerBehaviour]
				public void DropItem(PlayerBehaviour oldOwner)
				{
						// must not be null!
						oldOwner = oldOwner ?? throw new ArgumentNullException(nameof(oldOwner));

						if (User is { } && oldOwner == User)
						{
								// unsetting parent belongs inside here! Because only after the check, the drop may be done
								transform.SetParent(null, true);

								User = null;
								IsTakenByPlayer = false;
								PerformDrop();
						}
				}

				/// <summary>
				/// DURING interaction this item shall be taken. Updates <see cref="User"/> and <see cref="IsTakenByPlayer"/> to true.
				/// </summary>
				protected void TakeItem(PlayerBehaviour takenByPlayer)
				{
						// must not be null!
						User = takenByPlayer ?? throw new ArgumentNullException(nameof(takenByPlayer));
						IsTakenByPlayer = true;
						OnPickedUp();
				}

				/// <summary>
				/// Is called only AFTER the player script put this in his hand (animated)
				/// </summary>
				protected abstract void OnPickedUp();

				/// <summary>
				/// Is called only AFTER the player script released this item from transform - do gravity
				/// </summary>
				protected abstract void PerformDrop();

				public bool CheckBelongsTo(PlayerBehaviour player)
				{
						return User is { } && User == player;
				}

		}
}