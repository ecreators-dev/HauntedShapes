using Assets.Script.Behaviour;

namespace Assets.Script.Model
{
		public interface IGameController
		{
				IAudioFader AudioFader { get; }
				bool IsCameraRotateStop { get; }

				void HideBlackscreen();
				void ShowBlackscreen();
				void SetStopCameraEdit(bool stopCamera);
		}
}