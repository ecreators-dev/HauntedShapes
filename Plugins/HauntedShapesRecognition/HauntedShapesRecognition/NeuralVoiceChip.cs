using System;
using System.Collections;
using System.Threading.Tasks;

using UnityEngine;

namespace HauntedShapesRecognition
{
		[RequireComponent(typeof(NeuralTextSynthezizerService))]
		[RequireComponent(typeof(NeuralVoiceRecognizerService))]
		public class NeuralVoiceChip : MonoBehaviour
		{
				public event Action<string> TextRecognizedEvent;

				[SerializeField]
				private AudioSource speaker;

				private NeuralTextSynthezizerService speakingTalent;
				private NeuralVoiceRecognizerService hearingTalent;

				public bool IsHearing => hearingTalent.IsTalking; // you are speaking, it is hearing

				public bool IsTalking { get; private set; }

				public void SetRecognition(NeuralVoiceRecognizerService plugin)
				{
						this.hearingTalent = plugin;
				}

				private void Awake()
				{
						speakingTalent = GetComponent<NeuralTextSynthezizerService>();
						hearingTalent ??= GetComponent<NeuralVoiceRecognizerService>();
						hearingTalent.TextRecognizedEvent += OnTextRecognized;
				}

				private void OnTextRecognized(string text)
				{
						TextRecognizedEvent?.Invoke(text);
				}

				public void StartListening()
				{
						hearingTalent.StartRecognition();
				}

				public void StopListening()
				{
						hearingTalent.StopRecognition().ConfigureAwait(false);
				}

				public void SpeakInCoroutine(string textToSpeak, float speakingVolume = 1)
				{
						if (speaker is null)
						{
								Debug.Log("Fehlender AudioSource: Sprecher funktioniert nicht");
								return;
						}

						if (IsTalking)
						{
								Debug.Log("Sprecher kann nicht laufen, er spricht noch.");
								return;
						}

						IsTalking = true;
						StartCoroutine(Talk());

						IEnumerator Talk()
						{
								Debug.Log($"Sprachgenerierung startet zu text {textToSpeak}");
								Task<float[]> task = speakingTalent.SynthesizeTextToSamples(textToSpeak);
								Task.WaitAll(task);
								Debug.Log($"Sprachgenerierung endet, text {textToSpeak}");
								float[] samples = task.Result;

								if (samples is null || samples.Length is 0)
								{
										IsTalking = false;
										yield break;
								}

								AudioClip clip = speakingTalent.CreateAudioClip($"talker:{gameObject.name}", samples);
								if (clip.length is 0)
								{
										Debug.Log($"Sprachgenerierung kann die Samples nicht spielen. {textToSpeak}");
										IsTalking = false;
										yield break;
								}

								speaker.PlayOneShot(clip, speakingVolume);
								Debug.Log($"Sprachgenerierung spielt ab {textToSpeak}");

								IsTalking = false;
								yield break;
						}
				}

				public void Dispose()
				{
						hearingTalent?.Dispose();
						speakingTalent?.Dispose();
				}
		}
}
