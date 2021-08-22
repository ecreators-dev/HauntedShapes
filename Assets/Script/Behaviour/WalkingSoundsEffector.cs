
using UnityEditor;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(Collider))]
		public class WalkingSoundsEffector : MonoBehaviour
		{
				[Header("Step Sound Audio")]
				[Tooltip("Put a walking sound into: played when trigger from the front")]
				[SerializeField] private AudioClip frontAudio;
				[Tooltip("Put a walking sound into: played when trigger from the back")]
				[SerializeField] private AudioClip backAudio;
				private Vector3 position;
				private Vector3 front;
				private Vector3 back;
				private const float handleOffset = 0.5f;
				private const float handleSize = 0.125f;

				private void Awake()
				{
						Collider myCollider = GetComponent<Collider>();
						myCollider.isTrigger = true;
				}

#if UNITY_EDITOR
				private void OnDrawGizmos()
				{
						position = transform.position;
						front = position + transform.forward.normalized * handleOffset;
						Handles.color = Color.green;
						Handles.DrawWireCube(front, Vector3.one * handleSize);
						Handles.color = Color.white;
						Handles.Label(front, $"Front: {GetAudioLabel(frontAudio)}");

						back = transform.position + transform.forward.normalized * -handleOffset;
						Handles.color = Color.blue;
						Handles.DrawWireCube(back, Vector3.one * handleSize);
						Handles.color = Color.white;
						Handles.Label(back, $"Back: {GetAudioLabel(backAudio)}");

						Handles.color = Color.gray;
						Handles.DrawDottedLine(front, back, 0.125f);

						Handles.color = Color.gray;
						Handles.DrawWireDisc(position, Vector3.forward, 0.125f);
						Handles.DrawWireDisc(position, Vector3.forward, 0.125f / 2f);
				}
#endif

				private string GetAudioLabel(AudioClip clip)
				{
						if (clip == null || clip.name is null)
						{
								return "<?>";
						}
						if (string.IsNullOrWhiteSpace(clip.name))
						{
								return "''";
						}
						return clip.name;
				}

				private void OnTriggerExit(Collider other)
				{
						if (other.TryGetComponent(out IStepSoundProvider stepSoundProvider))
						{
								if (IsBehindMe(other.transform))
								{
										stepSoundProvider.SetStepSound(backAudio);
								}
								else
								{
										stepSoundProvider.SetStepSound(frontAudio);
								}
						}
				}

				private bool IsBehindMe(Transform other)
				{
						Vector3 toOther = other.position - transform.position;
						Vector3 forward = transform.TransformDirection(Vector3.forward);
						return Vector3.Dot(forward, toOther) < 0;
				}

				private enum EffectorSide
				{
						FRONT,
						BACK
				}
		}
}