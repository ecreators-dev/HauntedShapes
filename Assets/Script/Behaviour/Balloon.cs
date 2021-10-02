using Assets.Script.Components;

using System;
using System.Collections;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(Rigidbody))]
		[RequireComponent(typeof(Collider))]
		public class Balloon :
				Interactible
				, IBalloon
		{
				[SerializeField] private float floatStrength = 0.8f;
				[SerializeField] private BalloonBody body;
				[SerializeField] private AudioSource bounceAudioSource3d;
				[SerializeField] private AudioClip bounceSound3d;

				public Filling Drain(float newFloatingValue)
				{
						if (CanFloatUp())
						{
								floatStrength = newFloatingValue;
						}

						return Mathf.Max(0, newFloatingValue / initFloatingStrength);

						bool CanFloatUp() => initFloatingStrength > 0;
				}

				[Range(0, 1)]
				[SerializeField] private float bounceVolume = 1;

				private Vector3 initScale;
				private float initFloatingStrength;
				private Collider Collider;
				private float initBounce;

				private Rigidbody Rigidbody { get; set; }

				private Rigidbody BodyRigidbody { get; set; }

				public float FloatStrength => floatStrength;

				public float InitFloatStrength => initFloatingStrength;

				private void Awake()
				{
						Rigidbody = GetComponent<Rigidbody>();
						BodyRigidbody = body.GetComponent<Rigidbody>();

						// fix movement
						BodyRigidbody.mass = Rigidbody.mass * 2f;

						SetUseGravity(false);
				}

				private void Start()
				{
						initFloatingStrength = FloatStrength;
						Collider = BodyRigidbody.GetComponent<Collider>();
						initBounce = this.Collider.material.bounciness;
				}

				private void SetUseGravity(bool status)
				{
						Rigidbody.useGravity = status;
						BodyRigidbody.useGravity = status;
				}

				private void FixedUpdateAddForce()
				{
						// fx: rotate upwards again
						Rigidbody.MoveRotation(Quaternion.Lerp(Rigidbody.rotation, Quaternion.identity, Time.fixedDeltaTime * this.floatStrength));

						// fx: float down or up depending on floatStrength
						BodyRigidbody.AddForce(Vector3.up * floatStrength);
						BodyRigidbody.AddForceAtPosition(Vector3.up * floatStrength * 0.2f, transform.position);
				}

				private void FixedUpdate()
				{
						//body.transform.localPosition = initBodyLocalPosition;
						if (IsHuntingActiveChanged)
						{
								if (IsHuntingActive)
								{
										SetUseGravity(true);
								}
								else
								{
										SetUseGravity(false);
								}
						}

						if (IsHuntingActive is false)
						{
								FixedUpdateAddForce();
						}

						Rigidbody.angularVelocity = Vector3.Lerp(Rigidbody.angularVelocity, Vector3.zero, Time.fixedDeltaTime);
						BodyRigidbody.angularVelocity = Vector3.Lerp(BodyRigidbody.angularVelocity, Vector3.zero, Time.fixedDeltaTime);
						if (FloatStrength < 0)
						{
								Collider.material.bounciness = 0;
						}
						else
						{
								Collider.material.bounciness = initBounce;
						}
				}

				private void OnHuntingStateChanged(bool huntActive)
				{
						if (huntActive is false && !gameObject.activeSelf)
						{
								Reserect();
								HuntingStateBean.Instance.HuntingStateChangedEvent -= OnHuntingStateChanged;
						}
				}

				public void Pop()
				{
						HuntingStateBean.Instance.HuntingStateChangedEvent += OnHuntingStateChanged;

						StartCoroutine(PopBlowUp());
				}

				private IEnumerator PopBlowUp()
				{
						base.PlayToggleOnSoundExplicitFromScript(true);
						this.initScale = body.transform.localScale;
						if (FloatStrength > 0)
						{
								PlayPopEmission();
								WaitForEndOfFrame endFrame = new WaitForEndOfFrame();
								float seconds = base.GetToggleOn().length / 3f;
								while (seconds > 0)
								{
										seconds -= Time.deltaTime;
										body.transform.localScale = Vector3.Lerp(body.transform.localScale, initScale * 1.25f, Time.deltaTime);
										yield return endFrame;
								}
						}
						gameObject.SetActive(false);
						OnHuntStarts();
						yield break;
				}

				private void PlayPopEmission()
				{

				}

				public void Reserect()
				{
						body.Reserected();
						floatStrength = initFloatingStrength;
						body.transform.localScale = initScale;
						gameObject.SetActive(true);

						//body.localPosition = initBodyLocalPosition;
						SetUseGravity(false);
						base.PlayToggleOffSoundInternal();
				}

				protected override void OnHuntStarts()
				{
						SetUseGravity(true);
				}

				protected override void OnHuntEnds()
				{
						SetUseGravity(false);
				}

				protected override void Interact(PlayerBehaviour sender)
				{
						Pop();
				}

				public override string GetTargetName()
				{
						// nothing to display
						return "?";
				}

				// collision of child:
				public override void OnCollisionEnterChild(Collision collision)
				{
						base.OnCollisionEnterChild(collision);

						HandleCollision(collision);
				}


				// self collision:
				private void OnCollisionEnter(Collision collision)
				{
						HandleCollision(collision);
				}

				private void HandleCollision(Collision collision)
				{
						if (collision.collider == body.GetComponent<Collider>())
								return;

						if (collision.gameObject.GetComponentInParent<IHeat>() is IHeat heater
								|| collision.gameObject.TryGetComponent(out heater))
						{
								if (heater.HeatDegressCelsius >= 100)
								{
										if (FloatStrength >= 0)
										{
												Pop();
												return;
										}
										else
										{
												body.DrainFaster();
										}
								}
						}

						float force = collision.impulse.magnitude;
						bounceAudioSource3d.PlayOneShot(bounceSound3d, Mathf.Max(0.2f, Mathf.Clamp01(bounceVolume * force)));
				}
		}
}