using Assets.Script.Behaviour;
using Assets.Script.Controller;
using Assets.Script.Model;

using UnityEngine;

namespace Assets
{
		public static class Beans
		{
				private static bool wasNotInstanced;
				private static bool wasDisabled;

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
								wasNotInstanced = true;
								Debug.LogWarning($"{nameof(InputControlManagerSingleton)} not yet instantiated or set!");
						}
						else if (instance.IsEnabled is false)
						{
								wasDisabled = true;
								instance.Enable();
								Debug.LogWarning($"{nameof(InputControlManagerSingleton)} not yet enabled! Now enabled.");
						}
						else if (wasDisabled || wasNotInstanced)
						{
								Debug.LogWarning($"{nameof(InputControlManagerSingleton)} ready!");
								wasDisabled = false;
								wasNotInstanced = false;
						}
						return instance;
				}
		}
}
