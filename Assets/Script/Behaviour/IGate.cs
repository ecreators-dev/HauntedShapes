namespace Assets.Script.Behaviour
{
		public interface IGate
		{
				bool IsOpened { get; }

				void Open();

				void Close();
		}
}