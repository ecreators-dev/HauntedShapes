using System;
using System.Collections;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(Rigidbody))]
		public class Balloon : HuntEventObject, IBalloon
		{
				[SerializeField] private float floatStrength = 0.8f;
				[SerializeField] private Transform body;
				[SerializeField] private AudioSource audioSource3d;
				[SerializeField] private AudioClip bounceSound3d;
				[Range(0, 1)]
				[SerializeField] private float bounceVolume = 1;

				private Vector3 initScale;

				private Rigidbody Rigidbody { get; set; }

				public float FloatStrength => FloatStrength;

				private void Awake()
				{
						Rigidbody = GetComponent<Rigidbody>();
						Rigidbody.useGravity = false;
				}

				private void FixedUpdate()
				{
						HuntingStateBean.Instance.HuntingStateChangedEvent -= OnHuntingStateChanged;
						HuntingStateBean.Instance.HuntingStateChangedEvent += OnHuntingStateChanged;

						//body.transform.localPosition = initBodyLocalPosition;
						if (IsHuntActive)
						{
								Rigidbody.useGravity = true;
								body.GetComponent<Rigidbody>().useGravity = true;
						}
						else
						{
								Rigidbody.useGravity = false;
								body.GetComponent<Rigidbody>().useGravity = false;

								Rigidbody.AddForce(Vector3.up * floatStrength);
								//Rigidbody.AddForceAtPosition(Vector3.up.normalized * floatStrength, body.position);
						}
				}

				private void OnHuntingStateChanged(bool huntActive)
				{
						if (huntActive is false)
						{
								OnHuntEnding();
						}
				}

				public void Pop()
				{
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
						OnHuntActivate();
				}

				private void PlayPopEmission()
				{

				}

				public void Reserect()
				{
						body.transform.localScale = initScale;
						gameObject.SetActive(true);

						//body.localPosition = initBodyLocalPosition;
						Rigidbody.useGravity = false;
						base.PlayToggleOffSoundInternal();
				}

				protected override void OnHuntActivate()
				{
						Rigidbody.useGravity = true;
				}

				protected override void OnHuntEnding()
				{
						Reserect();
				}

				public override bool CanInteract(PlayerBehaviour sender)
				{
						return base.CanInteract(sender);
				}

				protected override void Interact(PlayerBehaviour sender)
				{
						Pop();
				}

				public override string GetTargetName()
				{
						// nothing to display
						return null;
				}

				// self:
				private void OnCollisionEnter(Collision collision)
				{
						audioSource3d.PlayOneShot(bounceSound3d, bounceVolume);
				}

				// child:
				public override void OnCollisionEnterChild(Collision collision)
				{
						base.OnCollisionEnterChild(collision);

						audioSource3d.PlayOneShot(bounceSound3d, bounceVolume);
				}
		}
}