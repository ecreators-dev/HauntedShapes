using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;
using System;

namespace HauntedShapesRecognition
{
		public class NeuralVoiceRecognizerService : MonoBehaviour
		{
				public event Action<string> TextRecognizedEvent;

				[SerializeField]
				private bool initializeOnStart = true;

				[SerializeField]
				[Min(0)]
				private float initialSilenceSeconds = 0.2f;

				[SerializeField]
				[Min(0)]
				private float endSilenceSeconds = 0.2f;

				private SpeechConfig config;
				private SpeechRecognizer recognizer;

				public bool IsInitialized { get; private set; }

				private Task recognitionTask;

				public bool IsRunning { get; private set; }
				public bool IsTalking { get; private set; }

				protected virtual void Awake()
				{
						Debug.Log("Running Recognition Managed Plugin .Net Standard 2.1 - in Awake");
				}

				protected virtual void Start()
				{
						if (initializeOnStart)
						{
								InitializeRecognition();
						}
				}

				private void InitializeRecognition()
				{
						if (recognizer is null)
						{
								IsInitialized = true;

								// Creates an instance of a speech config with specified subscription key and service region.
								// Replace with your own subscription key // and service region (e.g., "westus").
								SetupConfig();
								SetupRecognizerService();
						}
				}

				private void SetupRecognizerService()
				{
						// https://github.com/Azure-Samples/cognitive-services-speech-sdk/blob/master/quickstart/csharp/unity/from-microphone/Assets/Scripts/HelloWorld.cs
						recognizer = new SpeechRecognizer(config);
						recognizer.Recognized += OnRecognition;
						recognizer.SpeechStartDetected += OnStartTalking;
						recognizer.SpeechEndDetected += OnEndTalking;
				}

				public void Dispose()
				{
						if (recognizer is { })
						{
								OnApplicationQuit();
						}
				}

				private void SetupConfig()
				{
						config = SpeechConfig.FromSubscription("1f9274ae2b674afe862145c691845e26", "southcentralus");
						config.EnableDictation();
						config.SpeechRecognitionLanguage = "de-DE";
						config.SpeechSynthesisLanguage = "de-DE";
						config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio24Khz48KBitRateMonoMp3);
						config.SetProperty(PropertyId.SpeechServiceConnection_InitialSilenceTimeoutMs, (initialSilenceSeconds * 1000).ToString());
						config.SetProperty(PropertyId.SpeechServiceConnection_EndSilenceTimeoutMs, (endSilenceSeconds * 1000).ToString());
						// see https://aka.ms/speech/sdkregion#standard-and-neural-voices
						config.SpeechSynthesisVoiceName = "de-DE-ConradNeural";
				}

				protected virtual void OnEndTalking(object sender, RecognitionEventArgs e)
				{
						IsTalking = false;
				}

				protected virtual void OnStartTalking(object sender, RecognitionEventArgs e)
				{
						IsTalking = true;
				}

				public virtual void StartRecognition()
				{
						if (recognizer is { } && IsRunning is false)
						{
								recognitionTask = recognizer.StartContinuousRecognitionAsync();
								IsRunning = true;
						}
				}

				public virtual async Task StopRecognition()
				{
						if (recognizer is { } && IsRunning is true)
						{
								IsRunning = false;
								await recognizer.StopContinuousRecognitionAsync();
						}
				}

				protected virtual void OnApplicationQuit()
				{
						if (recognizer is { })
						{
								Task.WaitAll(StopRecognition(), recognitionTask);
								recognizer.Recognized -= OnRecognition;
								recognizer.SpeechStartDetected -= OnStartTalking;
								recognizer.SpeechEndDetected -= OnEndTalking;
								try
								{
										recognizer.Dispose();
								}
								catch (Exception ex)
								{
										Debug.LogWarning($"[FIX LATER] Unable to dispose: {ex}");
								}
								recognizer = null;
						}
				}

				protected virtual void OnRecognition(object sender, SpeechRecognitionEventArgs e)
				{
						// Starts speech recognition, and returns after a single utterance is recognized. The end of a
						// single utterance is determined by listening for silence at the end or until a maximum of 15
						// seconds of audio is processed.  The task returns the recognition text as result. 
						// Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
						// shot recognition like command or query. 
						// For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
						SpeechRecognitionResult result = e.Result;

						// Checks result.
						switch (result.Reason)
						{
								case ResultReason.RecognizedSpeech:
										Debug.Log($"Verstanden: {result.Text}");
										RecognizeText(result.Text);
										break;
								case ResultReason.NoMatch:
										Debug.Log($"Kein Treffer: Sprachtext konnte nicht erkannt werden.");
										break;
								case ResultReason.Canceled:
										var cancellation = CancellationDetails.FromResult(result);
										Debug.LogWarning($"Abbruch: Grund = {cancellation.Reason}");

										if (cancellation.Reason is CancellationReason.Error)
										{
												Debug.LogWarning($"Abbruch: ErrorCode={cancellation.ErrorCode}");
												Debug.LogWarning($"Abbruch: ErrorDetails={cancellation.ErrorDetails}");
												Debug.LogWarning($"Abbruch: Subscription aktuell?");
										}
										break;
						}
				}

				public void RecognizeText(string text)
				{
						if (text is null) return;

						TextRecognizedEvent?.Invoke(text);
				}
		}
}
