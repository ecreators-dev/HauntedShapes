using System;
using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Assets.Script.Components
{
		/// <summary>
		/// An item to pickup in the players left hand. Release / drop after that thru event of player
		/// or call Drop method.
		/// </summary>
		[RequireComponent(typeof(Rigidbody))]
		[RequireComponent(typeof(Collider))]
		public class Item : Interactible
		{
				[Min(0)]
				[SerializeField] protected float moveSpeed = 1.2f;
				[SerializeField] private float rotationSpeed = 30;

				private bool mouseLookAround;

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

				private void Update()
				{
						// if watched:
						// rotate with mouse
						if (mouseLookAround)
						{
								Vector2Control mouseDelta = Mouse.current.delta;
								Vector2 deltaValue = mouseDelta.ReadValue();
								/*
								Vector3 currentRotEuler = Transform.localEulerAngles;
								currentRotEuler.z = 0; // roll
								currentRotEuler.y += deltaValue.x * rotationSpeed * Time.deltaTime; // pan
								currentRotEuler.x += deltaValue.y * rotationSpeed * Time.deltaTime; // tilt
								currentRotEuler.x %= 360;
								currentRotEuler.y %= 360;
								*/
								Transform.Rotate(deltaValue * rotationSpeed * Time.deltaTime, Space.Self);
						}
						else if (!Transform.localEulerAngles.Equals(Vector3.zero))
						{
								// stop, means: reset rotation
								Transform.localEulerAngles = Vector3.zero;
						}
				}

				public override bool CanInteract(GameObject sender)
				{
						// you can pickit up, if it is not used
						return Transform.parent == null && sender.TryGetComponent(out IPickupItems picker)
								&& picker.IsLeftHandEmpty;
				}

				public override void Interact(GameObject sender)
				{
						if (sender.TryGetComponent(out IPickupItems pickerPlayer) && InHand is false)
						{
								// handle drop action: sender only need to call his event to drop this item
								pickerPlayer.DropEvent += Drop;

								// align with parent
								Transform.SetParent(sender.transform, true);

								// event inside sender
								pickerPlayer.PutIntoLeftHand(this);

								// animate to hand:
								StartCoroutine(MoveToOwner());

								InHand = true;

								RigidBody.isKinematic = true;
								IEnumerator MoveToOwner()
								{
										Vector3 localDestination = Vector3.zero;

										float dist = Vector3.Distance(Transform.localPosition, localDestination);
										if (dist == 0) dist = 1;
										float s = moveSpeed / dist;

										WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
										yield return waitForFixedUpdate;

										const float MIN_DISTANCE = 0.01f;
										while (Vector3.Distance(Transform.localPosition, localDestination) > MIN_DISTANCE)
										{
												Vector3 dir = localDestination - Transform.localPosition;
												RigidBody.MovePosition(Transform.localPosition + dir.normalized * s * Time.fixedDeltaTime);
												yield return waitForFixedUpdate;
										}
										// complete
										RigidBody.MovePosition(localDestination);
										yield break;
								}
						}
				}

				/// <summary>
				/// Is called after the player let the item drop
				/// </summary>
				public void Drop(IPickupItems sender)
				{
						InHand = false;
						sender.DropEvent -= Drop;
						transform.SetParent(null, true);

						// let the item fall like normal. no matter how rotated
						transform.localRotation = Quaternion.Euler(0, 0, 0);

						// gravity takes control
						RigidBody.isKinematic = false;
				}
		}

		public interface IPickupItems
		{
				event Action<IPickupItems> DropEvent;

				bool IsLeftHandEmpty { get; }

				void PutIntoLeftHand(Item item);
		}
}
