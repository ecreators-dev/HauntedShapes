using Assets.Script.Model;

using Microsoft.CognitiveServices.Speech;

using System;
using System.Collections;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(AudioSource))]
		public class VoiceMaker : MonoBehaviour, IVoiceMaker
		{
				public enum OutputFormat
				{
						PCM_8Bit_16khz_Mono = 17,
						PCM_16Bit_16khz_Mono = 15,
						PCM_24Bit_16khz_Mono = 16,
						PCM_48Bit_16khz_Mono = 20
				}

				public event Action<VoiceMaker> PlayReadyEvent;

				[SerializeField]
				[Range(0, 1)]
				private float volume = 1f;

				[SerializeField]
				private OutputFormat quality = OutputFormat.PCM_16Bit_16khz_Mono;

				[SerializeField]
				private string[] voices;

				[SerializeField]
				private int voiceIndex = 0;

				[SerializeField]
				[Range(-100, 200)]
				[Tooltip("Talk speed offset")]
				private float rateOffsetPercentage = -19;

				[SerializeField]
				[Range(-50, 50)]
				[Tooltip("Voice Height offset")]
				private float pitchOffsetPercentage = -20;

				private AudioSource speaker;
				private AudioClip clip;
				private volatile float[] sampleData;
				private volatile bool changedSampleData;
				private bool reading;
				private string activeText;

				private void Awake()
				{
						speaker = GetComponent<AudioSource>();
						clip = AudioClip.Create("talkClip", short.MaxValue * 2, 1, 16_000, false);
						ListVoices();
				}

				private SpeechSynthesisOutputFormat GetQuality() => (SpeechSynthesisOutputFormat)(byte)quality;


				private void Update()
				{
						if (changedSampleData && reading is false)
						{
								Debug.Log($"Prepare speaking: {activeText}");
								LastTextSpoken = activeText;
								PlayReadyEvent?.Invoke(this);

								reading = true;
								clip.SetData(sampleData, 0);
								Debug.Log($"Speaking: {activeText}");
								speaker.PlayOneShot(speaker.clip, volume);
								changedSampleData = false;
								reading = false;
								Debug.Log($"After Speaking: {activeText}");

								StartCoroutine(WaitPlayEnding());

								IEnumerator WaitPlayEnding()
								{
										yield return new WaitUntil(() => speaker.isPlaying is false);
										Debug.Log("Stopped Speaking");
										PlayReadyEvent?.Invoke(this);
								}
						}
				}

				[ContextMenu("Get Voices from Server")]
				public async void ListVoices()
				{
						SpeechConfig config = CreateConfig();
						using (var synthesizer = new SpeechSynthesizer(config))
						{
								SynthesisVoicesResult result = await synthesizer.GetVoicesAsync(config.SpeechSynthesisLanguage);
								this.voices = new string[result.Voices.Count];
								int i = 0;
								foreach (VoiceInfo voice in result.Voices)
								{
										Debug.Log(voice.ShortName);
										this.voices[i++] = voice.ShortName;
								}

								if (voiceIndex >= voices.Length)
								{
										voiceIndex = 0;
								}
						}
				}

				public bool CanSpeak => changedSampleData is false;

				public string LastTextSpoken { get; private set; }

				public bool TextAudioReady => changedSampleData && reading || LastTextSpoken is { };

				public async void SayAsync(string text)
				{
						if(changedSampleData)
						{
								Debug.Log($"Keine überlagerten Stimmen. Der Tex: {text} wird nicht gesprochen!");
								return;
						}

						SpeechConfig config = CreateConfig();
						Debug.Log($"[Say]: {text}, Speaker: {config.SpeechSynthesisVoiceName}");
						activeText = text;
						using (var synthesizer = new SpeechSynthesizer(config))
						{
								string name = config.SpeechSynthesisVoiceName;
								string ssml = $@"<speak xmlns=""http://www.w3.org/2001/10/synthesis"" xmlns:mstts=""http://www.w3.org/2001/mstts"" xmlns:emo=""http://www.w3.org/2009/10/emotionml"" version=""1.0"" xml:lang=""en-US""><voice name=""{name}""><prosody rate=""{rateOffsetPercentage}%"" pitch=""{pitchOffsetPercentage}%"">{text}</prosody></voice></speak>";
								SpeechSynthesisResult audioResult = await synthesizer.SpeakSsmlAsync(ssml).ConfigureAwait(false);
								byte[] audioData = audioResult.AudioData;
								Debug.Log($"Received data length: {audioData.Length}");

								if (audioData.Length > 0)
								{
										while (changedSampleData)
										{
												await Task.Yield();
										}

										sampleData = Convert16BitByteArrayToAudioClipData(audioData);
										changedSampleData = true;
								}
						}
				}

				private SpeechConfig CreateConfig()
				{
						var config = SpeechConfig.FromSubscription("1f9274ae2b674afe862145c691845e26", "southcentralus");
						config.SpeechSynthesisLanguage = "de-DE";
						config.SetSpeechSynthesisOutputFormat(GetQuality());
						if (voices != null && voiceIndex < voices.Length)
						{
								config.SpeechSynthesisVoiceName = voices[voiceIndex];
						}
						else
						{
								config.SpeechSynthesisVoiceName = "de-DE-ConradNeural";
						}
						return config;
				}

				public static float[] Convert16BitByteArrayToAudioClipData(byte[] source)
				{
						int x = sizeof(short);
						int convertedSize = source.Length / x;
						float[] data = new float[convertedSize];
						const short maxValue = short.MaxValue;

						for (int i = 0; i < convertedSize; i++)
						{
								int offset = i * x;
								data[i] = (float)BitConverter.ToInt16(source, offset) / maxValue;
								++i;
						}

						return data;
				}
		}
}