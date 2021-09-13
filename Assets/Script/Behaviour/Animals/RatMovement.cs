using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.Behaviour.Animals
{
		public class RatMovement : MonoBehaviour
		{
				[Header("Navmesh Settings")]
				[SerializeField] private NavMeshAgent agent;

				[Header("Animation Settings")]
				[Space]
				[SerializeField] private Animator animator;
				[Range(0, 25)]
				[SerializeField] private float walkSpeedMultiplier = 1;
				[Range(0, 25)]
				[SerializeField] private float idleSpeedMultiplier = 1;
				[Range(0, 25)]
				[SerializeField] private float dieSpeedMultiplier = 1;
				[Range(0, 1)]
				[SerializeField] private float volume = 1;
				[SerializeField] private AudioClip stepSound;
				[SerializeField] private AudioSource leftFrontAudio;
				[SerializeField] private AudioSource leftBackAudio;
				[SerializeField] private AudioSource rightFrontAudio;
				[SerializeField] private AudioSource rightBackAudio;

				[Header("AI Settings")]
				[Space]
				[SerializeField] private List<Transform> walkPositions;
				[Min(0)]
				[SerializeField] private float waitMinSeconds = 0;
				[Min(0)]
				[SerializeField] private float waitMaxSeconds = 6;

				// don't select next position be the same again
				private int lastPositionIndex = -1;
				private bool inJob;

				private void Start()
				{
						NavigateNext();
				}

#if UNITY_EDITOR
				private void OnDrawGizmosSelected()
				{
						// draw positions
						if (walkPositions != null)
						{
								for (int i = 0; i < walkPositions.Count; i++)
								{
										Vector3 position = walkPositions[i].position;
										Gizmos.color = Color.gray;
										Gizmos.DrawWireCube(position, Vector3.one * 0.25f);

										Handles.color = Color.yellow;
										Handles.Label(position, $"#{1 + i}");
								}
						}
				}
#endif

				public void OnAnimationEvent_Step_LeftFront() => PlayAudio(leftFrontAudio, stepSound);
				public void OnAnimationEvent_Step_LeftBack() => PlayAudio(leftBackAudio, stepSound);
				public void OnAnimationEvent_Step_RightFront() => PlayAudio(rightFrontAudio, stepSound);
				public void OnAnimationEvent_Step_RightBack() => PlayAudio(rightBackAudio, stepSound);

				private void PlayAudio(AudioSource playTo, AudioClip clip) => playTo.PlayOneShot(clip, volume);

				private enum AnimationStatus
				{
						IDLE,
						WALK,
						DIE
				}

				private AnimationStatus Status { get; set; } = AnimationStatus.IDLE;

				public void AnimateWalk()
				{
						animator.SetFloat("AnimationSpeedFactor_walking", walkSpeedMultiplier);
						animator.SetBool("Walking", true);
						animator.SetBool("Dead", false);
						Status = AnimationStatus.WALK;
				}

				public void AnimateIdle()
				{
						animator.SetFloat("AnimationSpeedFactor_idle", idleSpeedMultiplier);
						animator.SetBool("Walking", false);
						animator.SetBool("Dead", false);
						Status = AnimationStatus.IDLE;
				}

				public void Die()
				{
						animator.SetFloat("AnimationSpeedFactor_die", dieSpeedMultiplier);
						animator.SetBool("Walking", false);
						animator.SetBool("Dead", true);
						Status = AnimationStatus.DIE;
				}

				private void Update()
				{
						if (agent.pathStatus == NavMeshPathStatus.PathComplete && !inJob)
						{
								NavigateNext();
						}
				}

				private void NavigateNext()
				{
						if (walkPositions.Any())
						{
								inJob = true;
								float waitingTimeoutSeconds = NextRandomWaitingTimeout();
								int nextPosition = NextRandomLocationIndex();
								Debug.Log($"{gameObject.name} finds next position to go: {waitingTimeoutSeconds} s then position at index: {walkPositions[nextPosition].gameObject.name}");
								StartCoroutine(WaitAndWalk(waitingTimeoutSeconds, nextPosition));
						}
				}

				private float NextRandomWaitingTimeout()
				{
						return UnityEngine.Random.Range(waitMinSeconds, waitMaxSeconds);
				}

				private int NextRandomLocationIndex()
				{
						int nextPosition;
						while ((nextPosition = UnityEngine.Random.Range(0, walkPositions.Count)) == this.lastPositionIndex)
						{
								if (walkPositions.Count == 1)
										break;
						};
						lastPositionIndex = nextPosition;
						return nextPosition;
				}

				private IEnumerator WaitAndWalk(float waitingTimeoutSeconds, int nextPosition)
				{
						AnimateIdle();
						yield return new WaitForSeconds(waitingTimeoutSeconds);
						agent.SetDestination(walkPositions[nextPosition].position);
						AnimateWalk();
						inJob = false;
						yield break;
				}
		}
}