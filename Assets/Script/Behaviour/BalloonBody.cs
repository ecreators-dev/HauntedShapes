using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(Rigidbody))]
		[RequireComponent(typeof(Collider))]
		public class BalloonBody : InteractibleChild
		{
				[Min(0)]
				[SerializeField] private float drainTimeSeconds = 15;
				[Range(0, 2)]
				[SerializeField] private float minScale = 0.01f;
				[SerializeField] private AudioSource drainAudio3d;

				private Balloon endPart;
				private Vector3 initScale;
				private float drainTime;
				private float drainMultiplier = 0;

				private bool CanDrain => drainTimeSeconds > 0;

				private void Awake()
				{
						endPart = GetComponentInParent<Balloon>();
				}

				private void Start()
				{
						initScale = transform.localScale;
				}

				protected override void Update()
				{
						if (CanDrain)
						{
								// floating up gets floating down
								// percentFilled is max(0, x)
								HandleDrainAudio();
								HandleDrainUpdate();
						}
				}


				private void HandleDrainUpdate()
				{
						float drainDuration = drainTimeSeconds / (1 + drainMultiplier);
						float percentFilled = endPart.Drain(Mathf.Lerp(endPart.FloatStrength, -Mathf.Abs(endPart.InitFloatStrength), Time.deltaTime / drainDuration));

						// once side flatten (height and depth)
						Vector3 nowScale = new Vector3
						{
								x = initScale.x * Mathf.Max(minScale, 1 - drainTime / drainTimeSeconds),
								y = initScale.y * Mathf.Max(minScale * 3, 1 - drainTime / drainTimeSeconds),
								z = initScale.z * Mathf.Max(minScale * 5, 1 - drainTime / drainTimeSeconds)
						};
						transform.localScale = Vector3.Lerp(transform.localScale, nowScale, Time.deltaTime);
				}

				private void HandleDrainAudio()
				{
						if (drainTime <= drainTimeSeconds)
						{
								// once (loop)
								PlayDrainAudioOnce();

								drainTime += Time.deltaTime;
						}
						//once (end)
						else
						{
								StopDrainAudioOnce();
						}
				}

				private void StopDrainAudioOnce()
				{
						if (drainTime != drainTimeSeconds)
						{
								drainTime = drainTimeSeconds;
						}

						if (drainAudio3d.isPlaying)
						{
								drainAudio3d.Stop();
						}
				}

				private void PlayDrainAudioOnce()
				{
						if (!drainAudio3d.isPlaying)
						{
								drainAudio3d.Play();
						}
				}

				public void Reserected()
				{
						transform.localScale = initScale;
						drainTime = 0;
						drainMultiplier = 0;
				}

				public void DrainFaster()
				{
						drainMultiplier += 1;
				}
		}
}