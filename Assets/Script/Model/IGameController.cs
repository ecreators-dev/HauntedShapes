using Assets.Script.Behaviour;

namespace Assets.Script.Model
{
		public interface IGameController
		{
				IAudioFader AudioFader { get; }
				bool IsCameraRotateStop { get; }
				bool IsGamepadDisconnected { get; }
				CrosshairParameters Crosshair { get; }

				void HideBlackscreen();
				void ShowBlackscreen();
				void SetStopCameraEdit(bool stopCamera);
		}
}