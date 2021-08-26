
using UnityEditor;

using UnityEngine;
using UnityEngine.Events;

namespace Assets.Script.Components
{
		/// <summary>
		/// A transform representing an area to place tributes to enable a ritual
		/// </summary>
		[RequireComponent(typeof(Collider))]
		public sealed class RitualArea : MonoBehaviour
		{
				[SerializeField] private Tribute[] allowedTributes;

				[Tooltip("Can be another transform. It's the animator to show the ritual interaction. Audio will be played in 3D from this object position.")]
				[SerializeField] private Animator ritualAnimator;
				[Tooltip("Starts an audio clip parallel to the start of the animation with a specific volume." +
						" It takes place when a tribut enteres the ritual area (Collider)")]
				[SerializeField] private AnimationWithSound tributePlacedAnimation;
				[Tooltip("Starts an audio clip parallel to the start of the animation with a specific volume." +
						" It takes place when the last and final tribut has entered the collider area. The ritual is complete.")]
				[SerializeField] private AnimationWithSound ritualStartAnimation;
				[Tooltip("Starts an audio clip parallel to the start of the animation with a specific volume." +
						" It takes place when a tribute is taken from a completed ritual (out of collider area).")]
				[SerializeField] private AnimationWithSound ritualEndedAnimation;

				[SerializeField] private UnityEvent ritualCompletingEvent;
				[SerializeField] private UnityEvent ritualEndingEvent;
				[SerializeField] private UnityEvent tributePlacedEvent;

				private TributesCounter tributesCounter;

#if UNITY_EDITOR
				private SphereCollider sphereCollider;
				private BoxCollider boxCollider;
#endif

				/// <summary>
				/// Identifiers when the ritual is complete. Means every required tribute is within the collider area.
				/// </summary>
				public bool IsRitalComplete { get; private set; }

				private void Awake()
				{
						GetComponent<Collider>().isTrigger = true;

#if UNITY_EDITOR
						sphereCollider = GetComponent<SphereCollider>();
						boxCollider = GetComponent<BoxCollider>();
#endif
				}

				private void Start()
				{
						tributesCounter = new TributesCounter(allowedTributes ?? new Tribute[0]);
				}

#if UNITY_EDITOR
				private void OnDrawGizmos()
				{
						// show the collider area in edit mode:
						Gizmos.color = IsRitalComplete ? Color.green : Color.red;
						
						if (sphereCollider != null)
						{
								Handles.DrawSolidDisc(sphereCollider.transform.position, sphereCollider.transform.up, sphereCollider.radius);
						}

						if (boxCollider != null)
						{
								Handles.DrawWireCube(boxCollider.transform.position, boxCollider.size);
						}
				}
#endif

				private void OnTriggerEnter(Collider other)
				{
						if (IsValidType(other.gameObject, out EObjectType type))
						{
								tributesCounter.PutObject(type);

								UpdateIsRitualComplete();

								if (IsRitalComplete)
								{
										PlayAnimationRitualComplete();
								}
								else
								{
										PlayAnimationTributePlaced();
								}
						}
				}

				private void UpdateIsRitualComplete()
				{
						IsRitalComplete = tributesCounter.CountNoRemainingTypes == 0;
				}

				private void OnTriggerExit(Collider other)
				{
						if (IsValidType(other.gameObject, out EObjectType type))
						{
								tributesCounter.TakeObject(type);

								bool wasComplete = IsRitalComplete;
								UpdateIsRitualComplete();

								// the ritual ends with taking the first tribut from area
								if (wasComplete && IsRitalComplete is false)
								{
										PlayAnimationRitualIncomplete();
								}
						}
				}

				private void PlayAnimationRitualIncomplete()
				{
						PlayAnimationWithSound(ritualEndedAnimation);
						ritualEndingEvent?.Invoke();
				}

				private void PlayAnimationTributePlaced()
				{
						PlayAnimationWithSound(tributePlacedAnimation);
						tributePlacedEvent?.Invoke();
				}

				private void PlayAnimationRitualComplete()
				{
						PlayAnimationWithSound(ritualStartAnimation);
						ritualCompletingEvent?.Invoke();
				}

				private void PlayAnimationWithSound(AnimationWithSound soundAndAnimationTrigger)
				{
						if (ritualAnimator is { } && soundAndAnimationTrigger is { })
						{
								soundAndAnimationTrigger.PlayAudio(ritualAnimator.transform);
								ritualAnimator.SetTrigger(soundAndAnimationTrigger.triggerName);
						}
				}

				private bool IsValidType(GameObject other, out EObjectType type)
				{
						return TryGetType(other, out type) && tributesCounter.HasType(type);
				}

				private static bool TryGetType(GameObject other, out EObjectType type)
				{
						if (other.TryGetComponent(out Interactible interactible))
						{
								type = interactible.ObjectType;
								return true;
						}
						type = EObjectType.UNDEFINED;
						return false;
				}
		}
}
