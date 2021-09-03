using Assets.Script.Behaviour;
using Assets.Script.Controller;
using Assets.Script.Model;

using UnityEngine;

namespace Assets
{
		public static class Beans
		{
				public static bool InHunt => HuntingStateBean.Instance?.InHunt ?? false;

				public static IGameController GetGameController(this Behaviour _) => GameControllerSingleton.Instance;
				public static IVoiceRecognition GetVoiceRecognizer(this Behaviour _) => VoiceRecognitionSingleton.Instance;
				public static IAudioFader GetAudioFader(this Behaviour _) => AudioFaderSingleton.Instance;

				public static void BlackscreenFade(this Behaviour _) => _.GetGameController()?.ShowBlackscreen();
				public static void NoBlackscreenFade(this Behaviour _) => _.GetGameController()?.HideBlackscreen();

				public static IInputControls InputControls(this Behaviour _)
				{
						IInputControls instance = InputControlManagerSingleton.Instance;
						if (instance is null)
						{
								Debug.LogWarning($"{nameof(InputControlManagerSingleton)} not yet instantiated or set!");
						}
						else if (instance.IsEnabled is false)
						{
								instance.Enable();
								Debug.Log($"{nameof(InputControlManagerSingleton)} now enabled!");
						}
						return instance;
				}
		}
}
