using Microsoft.CognitiveServices.Speech;

using System;
using System.Threading.Tasks;

using UnityEngine;

namespace HauntedShapesRecognition
{
		public class NeuralTextSynthezizerService : MonoBehaviour
		{
				private SpeechConfig config;

				private void Awake()
				{
						SetupConfig();
				}

				private void SetupConfig()
				{
						config = SpeechConfig.FromSubscription("1f9274ae2b674afe862145c691845e26", "southcentralus");
						config.EnableDictation();
						config.SpeechRecognitionLanguage = "de-DE";
						config.SpeechSynthesisLanguage = "de-DE";
						config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio24Khz48KBitRateMonoMp3);

						// see https://aka.ms/speech/sdkregion#standard-and-neural-voices
						config.SpeechSynthesisVoiceName = "de-DE-ConradNeural";
				}

				public async Task<float[]> SynthesizeTextToSamples(string text)
				{
						// Creates a speech synthesizer using the default speaker as audio output.
						using (SpeechSynthesizer synthesizer = new SpeechSynthesizer(config))
						{
								// Receive a text from console input and synthesize it to speaker.
								using (SpeechSynthesisResult result = await synthesizer.SpeakTextAsync(text))
								{
										if (result.Reason == ResultReason.SynthesizingAudioCompleted)
										{
												Debug.Log($"Text wurde synthetisiert [{text}]");
												byte[] audioData = result.AudioData;
												float[] samples = AudioDataToSamples(audioData);
												return samples;
										}
										else if (result.Reason == ResultReason.Canceled)
										{
												var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
												Console.WriteLine($"Abbruch: Grund={cancellation.Reason}");

												if (cancellation.Reason == CancellationReason.Error)
												{
														Console.WriteLine($"Abbruch: ErrorCode={cancellation.ErrorCode}");
														Console.WriteLine($"Abbruch: ErrorDetails=[{cancellation.ErrorDetails}]");
														Console.WriteLine($"Abbruch: Subscription aktuell?");
												}
										}
								}
								return new float[0];
						}
				}

				public void Dispose()
				{
						// dont need it!
				}

				public virtual float[] AudioDataToSamples(byte[] audioData)
				{
						float[] samples = new float[audioData.Length / 4]; //size of a float is 4 bytes
						Buffer.BlockCopy(audioData, 0, samples, 0, audioData.Length);
						return samples;
				}

				public virtual AudioClip CreateAudioClip(string clipName, float[] sampleData)
				{
						// Assuming audio is mono because microphone input usually is
						const int channels = 1;
						// Assuming your samplerate is 44100 or change to 48000 or whatever is appropriate
						const int sampleRate = 44100;
						AudioClip clip = AudioClip.Create(clipName, sampleData.Length, channels, sampleRate, false);
						clip.SetData(sampleData, 0);
						return clip;
				}
		}
}
