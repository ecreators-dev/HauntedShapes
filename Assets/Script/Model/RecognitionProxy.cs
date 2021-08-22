using System;
namespace Assets.Script.Model
{
		public class RecognitionProxy<T>
				where T : RecognitionProxy<T>
		{
				public event Action<string> RecognizedTextEvent;

				private readonly Action<T> init;
				private readonly Action<T> start;
				private readonly Action<T> stop;
				private readonly Action<T> dispose;

				public RecognitionProxy(Action<T> init, Action<T> start, Action<T> stop, Action<T> dispose)
				{
						this.init = init;
						this.start = start;
						this.stop = stop;
						this.dispose = dispose;
				}

				public bool IsRunning { get; protected set; }

				protected void RecognizedText(string text)
				{
						RecognizedTextEvent?.Invoke(text);
				}

				public void Init() => this.init?.Invoke((T)this);

				public void Start() => this.start?.Invoke((T)this);

				public void Stop() => this.stop?.Invoke((T)this);

				public void Dispose() => this.dispose?.Invoke((T)this);
		}
}
