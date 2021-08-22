using System;

namespace Assets.Script.Model
{
		public interface IVoiceRecognizer
		{
				event Action<string> RecognizedTextEvent;

				bool IsRunning { get; }

				void Init();
				void Start();
				void Stop();
				void Dispose();
		}
}