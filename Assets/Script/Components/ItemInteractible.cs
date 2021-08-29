using Assets.Script.Behaviour;

using System;

using UnityEngine;

namespace Assets.Script.Components
{
		/// <summary>
		/// An item to pickup in the players left hand. Release / drop after that thru event of player
		/// or call Drop method.
		/// </summary>
		[RequireComponent(typeof(Rigidbody))]
		[RequireComponent(typeof(Collider))]
		public class ItemInteractible : PickupItem
		{
				[Min(0)]
				[SerializeField] protected float moveSpeed = 1.2f;
				[SerializeField] private float rotationSpeed = 30;

				private bool mouseLookAround;
				private bool pickedUp;

				protected Transform Transform { get; private set; }

				protected Rigidbody RigidBody { get; private set; }

				public bool InHand { get; private set; }

				// if on ground: no parent (parent = null)
				// if hold by some one: parent != null

				protected virtual void Awake()
				{
						Transform = transform;
						RigidBody = GetComponent<Rigidbody>();
				}

				// called from player, if ready to watch
				public void SetMouseRotationEnabled(bool enabled)
				{
						if (InHand)
						{
								mouseLookAround = enabled;
						}
				}

				private void FixedUpdate()
				{
						if (IsTakenByPlayer)
						{
								// lift up - done by Transform.parent in player script (done)

								// rotate item with keys - (TODO test)
								Vector2 torque = this.InputControls().MouseDelta;
								RigidBody.angularVelocity = Vector3.zero;
								var rot = torque * rotationSpeed;
								RigidBody.AddTorque(transform.right * rot.x + Vector3.up * rot.y, ForceMode.VelocityChange);
						}
				}

				/// <summary>
				/// Interact, means to this script: pickup only
				/// </summary>
				public override bool CanInteract(PlayerBehaviour sender)
				{
						// you can pickit up, if it is not used
						return User is null && pickedUp is false;
				}

				[CalledByPlayerBehaviour]
				public override void Interact(PlayerBehaviour sender)
				{
						// unless button to drop! Because this component
						// is only to lift and drop. The player can click on
						// it to take if from floor or click it to drop it again

						if (User is { } && User == sender)
						{
								if (IsTakenByPlayer)
								{
										DropItem(sender);
								}
								else
								{
										TakeItem(sender);
								}
						}
				}

				protected override void OnPickedUp()
				{
						RigidBody.isKinematic = true;
				}

				protected override void PerformDrop()
				{
						RigidBody.isKinematic = false;
				}

				protected override void OnHuntStart()
				{
						// nothing
				}

				protected override void OnHuntStop()
				{
						// nothing
				}

				public override string GetTargetName()
				{
						return gameObject.name;
				}
		}
}
