using Assets.Script.Model;

using Microsoft.CognitiveServices.Speech;

using System;

using UnityEngine;

namespace Assets.Script.Controller
{
		// redirect to plugin impl of IVoiceRecognition
		//[RequireComponent(typeof(NeuralVoiceRecognitionLoader))]
		public class VoiceRecognitionSingleton : MonoBehaviour, IVoiceRecognition
		{
				public static IVoiceRecognition Instance { get; private set; }

				public event Action<string> TextRecognitionEvent;

				private object @lock = new object();

				private SpeechRecognizer recognizer;

				public bool IsRunning { get; private set; }
				public bool IsClientTalking { get; private set; }

				private void Awake()
				{
						if (Instance is null)
						{
								Instance = this;
						}
						else
						{
								Destroy(this);
						}
				}
				private void Start()
				{
#if UNITY_EDITOR
						this.OnPlayModeExit(StopRecognitionAsync);
#endif
						StartRecognitionAsync();
				}

#if !UNITY_EDITOR
				private void OnApplicationQuit()
				{
						StopRecognitionAsync();
				}
#endif

				public async void StartRecognitionAsync()
				{
						if (IsRunning)
								return;

						IsRunning = true;

						var config = SpeechConfig.FromSubscription("1f9274ae2b674afe862145c691845e26", "southcentralus");
						config.EnableDictation();
						config.SpeechRecognitionLanguage = "de-DE";

						recognizer = new SpeechRecognizer(config);
						recognizer.Recognized += OnRecognition;
						recognizer.SpeechStartDetected += OnStartTalking;
						recognizer.SpeechEndDetected += OnEndTalking;
						await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
				}

				public async void StopRecognitionAsync()
				{
						if (IsRunning)
						{
								IsRunning = false;
								lock (@lock)
								{
										recognizer.Recognized -= OnRecognition;
										recognizer.SpeechStartDetected -= OnStartTalking;
										recognizer.SpeechEndDetected -= OnEndTalking;
								}

								await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);

								lock (@lock)
								{
										recognizer.Dispose();
								}
						}
				}

				private void OnEndTalking(object sender, RecognitionEventArgs e)
				{
						IsClientTalking = false;
				}

				private void OnStartTalking(object sender, RecognitionEventArgs e)
				{
						IsClientTalking = true;
				}

				private void OnRecognition(object sender, SpeechRecognitionEventArgs e)
				{
						SpeechRecognitionResult result = e.Result;
						if (string.IsNullOrEmpty(result.Text.Trim()))
								return;

						Debug.Log($"Text verstanden: {result.Text}");
						OnRecognizedText(result.Text.Trim());
				}

				private void OnRecognizedText(string text)
				{
						Debug.Log($"Verstanden: {text}. Event-Handlers vorhanden (Anzahl): {TextRecognitionEvent?.GetInvocationList().Length}");
						TextRecognitionEvent?.Invoke(text);
				}
		}
}