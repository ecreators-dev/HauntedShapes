using Assets.Script.Behaviour;

namespace Assets.Script.Model
{
		public interface IGameController
		{
				IAudioFader AudioFader { get; }

				void HideBlackscreen();
				void ShowBlackscreen();
				void SetStopCameraEdit(bool stopCamera);
		}
}