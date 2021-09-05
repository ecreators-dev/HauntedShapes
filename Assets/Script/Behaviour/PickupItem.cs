using Assets.Script.Behaviour.FirstPerson;
using Assets.Script.Components;

using System;
using System.Collections;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// You can pickup and drop
		/// </summary>
		public abstract class PickupItem : Interactible
		{
				[SerializeField] private SnapAxis upAxis = SnapAxis.Y;

				/// <summary>
				/// Represents the player that actual captured the equipment
				/// </summary>
				public PlayerBehaviour User { get; private set; }

				public bool IsTakenByPlayer { get; private set; }

				protected ICrosshairUI CrosshairHit { get; private set; }

				protected bool IsCrosshairHovered
				{
						get
						{
								if (CrosshairHit == null)
								{
										return false;
								}
								(bool actualHit, RaycastHit hit) = CrosshairHit.GetRaycastCollidersOnlyResult();
								return actualHit && hit.collider.GetComponent<PickupItem>() == this;
						}
				}
				protected Vector3 UpNormal { get; set; } = Vector3.up;

				protected virtual void Start()
				{
						UpNormal = GetUpNormalFromAxis(upAxis);
				}

				[CalledByPlayerBehaviour]
				public virtual void OnPlayer_ItemPickedUp(PlayerBehaviour newUser)
				{
						if (newUser is { })
						{
								if (User is null)
								{
										User = newUser;
										IsTakenByPlayer = true;
										CrosshairHit = CrosshairHitVisual.Instance;

										if (TryGetComponent(out Rigidbody body))
										{
												body.isKinematic = true;
												Debug.Log("Disable physics gravity for pick up");
										}

										Transform obj = transform;
										obj.localRotation = Quaternion.identity;
										obj.localPosition = Vector3.zero;

										Debug.Log($"Picked up: {GetTargetName()}");
										OnPickedUp();
								}
								else
								{
										Debug.LogError($"Item already in use of player '{User.gameObject.name}'");
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
				public void DropItemRotated(PlayerBehaviour oldOwner, bool noForce = false)
				{
						// must not be null!
						oldOwner = oldOwner ?? throw new ArgumentNullException(nameof(oldOwner));

						if (User is { } && oldOwner == User)
						{
								User = null;
								IsTakenByPlayer = false;

								// unsetting parent belongs inside here! Because only after the check, the drop may be done
								Transform obj = transform;
								obj.SetParent(null);
								// updside!
								obj.rotation = Quaternion.FromToRotation(Vector3.up, UpNormal);

								if (TryGetComponent(out Rigidbody body))
								{
										if (noForce is false)
										{
												body.AddForce(obj.forward * 2, ForceMode.Impulse);
										}

										body.isKinematic = false;
										Debug.Log("Enable physics for drop: throw forward");
								}
								Debug.Log($"Dropped: {GetTargetName()}");
								OnPerformedDrop();
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
				protected virtual void OnPickedUp() { }

				/// <summary>
				/// Is called only AFTER the player script released this item from transform - do gravity
				/// </summary>
				protected virtual void OnPerformedDrop() { }

				public bool CheckBelongsTo(PlayerBehaviour player)
				{
						return User is { } && User == player;
				}

				protected static Vector3 GetUpNormalFromAxis(SnapAxis upAxis)
				{
						switch (upAxis)
						{
								case SnapAxis.X:
										return Vector3.right;
								case SnapAxis.Z:
										return Vector3.forward;
								case SnapAxis.All:
								case SnapAxis.Y:
								case SnapAxis.None:
								default:
										return Vector3.up;
						}
				}

		}
}