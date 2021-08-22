using System;
using System.Globalization;
using System.Speech.Recognition;

namespace VoiceRecognition.DLL.Recognition
{
		public sealed class VoiceRecognitionEngine : IDisposable
		{
				public event Action<string> RecognizedTextEvent;
				
				private readonly SpeechRecognitionEngine engine;
				private readonly Exception error;

				public VoiceRecognitionEngine(CultureInfo lang)
				{
						try
						{
								engine = new SpeechRecognitionEngine(lang);
								engine.SetInputToDefaultAudioDevice();
								engine.LoadGrammar(new DictationGrammar());
								engine.RecognizeCompleted += RecognitionCompleted;
								error = null;
						}
						catch (Exception ex)
						{
								engine = null;
								error = ex;
						}
				}

				public void Start()
				{
						VerifyEngine();

						engine.RecognizeAsync(RecognizeMode.Multiple);
						IsRunning = true;
				}

				private void VerifyEngine()
				{
						if (engine == null)
								throw new Exception("Engine not working");
				}

				public void Stop()
				{
						VerifyEngine();

						engine.RecognizeAsyncCancel();
						IsRunning = false;
				}

				public Exception Error => error;

				public bool IsRunning { get; protected set; }

				private void RecognitionCompleted(object sender, RecognizeCompletedEventArgs e)
				{
						RecognizedTextEvent?.Invoke(e.Result.Text);
				}

				~VoiceRecognitionEngine()
				{
						try
						{
								engine?.Dispose();
						}
						catch (ObjectDisposedException)
						{
								// already disposed
						}
				}

				public void Dispose()
				{
						if (engine != null)
						{
								engine.Dispose();
						}
				}
		}
}
