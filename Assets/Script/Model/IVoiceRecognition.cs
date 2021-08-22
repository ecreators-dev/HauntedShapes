using System;

namespace Assets.Script.Model
{
		public interface IVoiceRecognition
		{
				bool IsRunning { get; }

				bool IsClientTalking { get; }

				event Action<string> TextRecognitionEvent;

				void StartRecognitionAsync();
				
				void StopRecognitionAsync();
		}
}