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
		public abstract class PickupItem : Interactible, IPickupItem
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
				protected Transform Transform { get; set; }
				protected Rigidbody RigidBody { get; set; }

				protected virtual void Start()
				{
						UpNormal = GetUpNormalFromAxis(upAxis);
						Transform = transform;
				}

				/// <summary>
				/// <b>Need to be placed in a parent if taken before calling this method!</b>
				/// <br/>Resets local position and rotation
				/// </summary>
				public virtual void OnPlayer_NotifyItemTaken(PlayerBehaviour newUser)
				{
						CrosshairHit = CrosshairHitVisual.Instance;
						if (newUser == null)
						{
								Debug.LogError($"Parameter \"player\" was null!");
								return;
						}

						if (IsTakenByPlayer is true)
						{
								Debug.LogError($"Item already in use of player '{User.gameObject.name}'");
								return;
						}

						User = newUser;
						IsTakenByPlayer = true;
						if (TryGetComponent(out Rigidbody body))
						{
								RigidBody = body;
								DisableGravity();
						}

						Transform.localPosition = Vector3.zero;
						Transform.localRotation = Quaternion.identity;

						Debug.Log($"Picked up: {GetTargetName()}");
						OnPickedUp();
				}

				protected void DisableGravity()
				{
						RigidBody.isKinematic = true;
						Debug.Log("Disable physics gravity");
				}

				protected void EnableGravity()
				{
						RigidBody.isKinematic = false;
						Debug.Log("Enable gravity for drop");
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
								// updside!
								float oldPan = Transform.localEulerAngles.y;
								Transform.localRotation = Quaternion.FromToRotation(Vector3.up, UpNormal);
								Transform.SetParent(null);
								var euler = Transform.localEulerAngles;
								Transform.localEulerAngles = new Vector3(euler.x, oldPan, euler.z);

								if (RigidBody is { })
								{
										if (noForce is false)
										{
												Debug.Log("Drop: throw away, forward");
												RigidBody.AddForce(Transform.forward * 10, ForceMode.Impulse);
										}
										EnableGravity();
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

				protected bool IsUserOrNotTaken(PlayerBehaviour playerSender)
				{
						return User == null || User == playerSender;
				}

				public void SetParent(Transform parent)
				{
						Transform.SetParent(parent);
				}

				public virtual bool CanPickUp(PlayerBehaviour player)
				{
						return IsUnlocked && User == null;
				}
		}
}