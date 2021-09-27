using Assets.Script.Components;

using System;
using System.Collections;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(Rigidbody))]
		[RequireComponent(typeof(SphereCollider))]
		public class Balloon :
				Interactible
				, IBalloon
		{
				[SerializeField] private float floatStrength = 0.8f;
				[SerializeField] private BalloonBody body;
				[SerializeField] private AudioSource audioSource3d;
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

						//BodyRigidbody.MovePosition(transform.position + Vector3.up * floatStrength * Time.fixedDeltaTime);

						// fx: float down or up depending on floatStrength
						BodyRigidbody.AddForce(Vector3.up * floatStrength);
						BodyRigidbody.AddForceAtPosition(Vector3.up * floatStrength, transform.position);
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
						PlayPopEmission();
						WaitForEndOfFrame endFrame = new WaitForEndOfFrame();
						float seconds = base.GetToggleOn().length / 3f;
						base.PlayToggleOnSoundExplicitFromScript(true);
						this.initScale = body.transform.localScale;
						while (seconds > 0)
						{
								seconds -= Time.deltaTime;
								body.transform.localScale = Vector3.Lerp(body.transform.localScale, initScale * 1.25f, Time.deltaTime);
								yield return endFrame;
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

						if (collision.collider != body.GetComponent<Collider>())
						{
								audioSource3d.PlayOneShot(bounceSound3d, bounceVolume);
						}
				}

				// self collision:
				private void OnCollisionEnter(Collision collision)
				{
						if (collision.collider != body.GetComponent<Collider>())
						{
								audioSource3d.PlayOneShot(bounceSound3d, bounceVolume);
						}
				}
		}
}