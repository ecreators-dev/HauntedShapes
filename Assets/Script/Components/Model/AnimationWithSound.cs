using UnityEngine;

namespace Assets.Script.Components
{
		[CreateAssetMenu(fileName = "Animation Sound Reference", menuName = "Game/Animation und Audio")]
		public class AnimationWithSound : ScriptableObject
		{
				public string triggerName;
				public AudioClip audio;
				public float volume;

				public void PlayAudio(Transform transform)
				{
						if (audio == null)
						{
								Debug.LogError($"Missing audio: {name}: {triggerName}");
								return;
						}
						AudioSource.PlayClipAtPoint(audio, transform.position, volume);
				}
		}
}
