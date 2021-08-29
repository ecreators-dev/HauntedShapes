
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Assets.MiniFirstPersonController.Scripts
{
		public class PlayerMovement : MonoBehaviour
		{
				[Header("Walking")]
				[SerializeField] private float walkSpeed = 5;

				[Header("Running")]
				[SerializeField] private bool canRun = true;
				[SerializeField] private float runSpeed = 9;
				[SerializeField] private KeyCode runningKey = KeyCode.LeftShift;

				public bool IsRunning { get; private set; }
				private Rigidbody RigidBody { get; set; }

				[SerializeField] private List<Func<float>> speedOverrides = new List<Func<float>>();

				private void Awake()
				{
						RigidBody = GetComponent<Rigidbody>();
				}

				private void FixedUpdate()
				{
						IsRunning = canRun && Input.GetKeyDown(runningKey);

						float targetMovingSpeed = IsRunning ? runSpeed : walkSpeed;
						if (speedOverrides.Any())
						{
								targetMovingSpeed = speedOverrides.Last().Invoke();
						}

						Vector2 inputAxis = this.InputControls().InputAxis;
						Vector2 targetVelocity = inputAxis * targetMovingSpeed;
						RigidBody.velocity = transform.rotation * new Vector3(targetVelocity.x, -9.81f, targetVelocity.y);
				}

				public void AddSpeedOverrideOnce(Func<float> speedOverride)
				{
						if (!speedOverrides.Contains(speedOverride))
						{
								speedOverrides.Add(speedOverride);
						}
				}

				public void RemoveSpeedOverride(Func<float> speedOverride)
				{
						if (speedOverrides.Contains(speedOverride))
						{
								speedOverrides.Remove(speedOverride);
						}
				}
		}
}