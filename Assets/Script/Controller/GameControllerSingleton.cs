using Assets.Script.Behaviour;
using Assets.Script.Model;

using System;
using System.Collections;

using UnityEngine;

namespace Assets.Script.Controller
{
		/// <summary>
		/// Controller Script zum
		/// - Spieler in die Lobby senden
		/// - Spieler auszuloggen
		/// </summary>
		[RequireComponent(typeof(AudioFaderSingleton))]
		public class GameControllerSingleton : MonoBehaviour, IGameController
		{
				public static IGameController Instance { get; private set; }

				[SerializeField]
				[Tooltip("Musik im Hintergrund, wenn das Spiel startet")]
				private AudioSource musicStartGameSource;
				private float musicStartGameVolume;

				[SerializeField]
				private FadeUIElement blackscreenUI;

				public IAudioFader AudioFader { get; private set; }

				private bool fadeAmbientStartMusic;
				private bool backgroundMusicInPlayingInLoop;
				private bool blackscreenFading;

				private void Awake()
				{
						if (Instance is null)
						{
								Instance = this;
						}
						else
						{
								Destroy(this);
								Debug.LogError($"Doppelter {nameof(GameControllerSingleton)}. Wird gelöscht.");
						}
				}

				/// <summary>
				/// Spielstart
				/// </summary>
				private void Start()
				{
						StartCoroutine(DelayedStartWaitForAudioFader());

						IEnumerator DelayedStartWaitForAudioFader()
						{
								WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
								IAudioFader fader = AudioFaderSingleton.Instance;
								float timeout = 0;
								const int timeoutSeconds = 2;
								while (fader is null || timeout > timeoutSeconds)
								{
										yield return waitForFixedUpdate;
										fader = AudioFaderSingleton.Instance;
										timeout += Time.fixedDeltaTime;
								}
								AudioFader = fader ?? throw new Exception($"Fehlender {nameof(AudioFaderSingleton)} nach {timeoutSeconds} Sekunden!");

								// verwendung, siehe GameReady_StartPlayBackgroundAmbientMusic()
								fadeAmbientStartMusic = true;
								yield break;
						}
				}

				private void Update()
				{
						if (fadeAmbientStartMusic && backgroundMusicInPlayingInLoop is false)
						{
								// break Update()-loop, play once
								backgroundMusicInPlayingInLoop = true;
								Debug.Log("Amient-Music is playing ...");

								GameReady_StartPlayBackgroundAmbientMusic();
						}
				}

				private void GameReady_StartPlayBackgroundAmbientMusic()
				{
						if (musicStartGameSource.clip is null)
								throw new Exception("Missing clip in ambient music! (null)");

						// setup
						musicStartGameSource.volume = 0;
						musicStartGameSource.loop = true;

						// play music (fade in, play in loop)
						musicStartGameSource.Play();
						AudioFader.FadeIn(musicStartGameSource, OnProgressMusicChanged);

						// fade out (1 to 0) with audio fade (0 to 1) in "blackscreen"
						void OnProgressMusicChanged(ProgressValue p)
						{
								blackscreenFading = p.ValueBetweenZeroAndOne < 1;
								// fades from progress 0 to 1 (unlinked from max value)
								VerifyBlackscreen();

								blackscreenUI.SetAlpha(p.Invert());
						}
				}

				private void VerifyBlackscreen()
				{
						if (blackscreenUI is null)
								throw new Exception($"Missing {nameof(blackscreenUI)}");

						if (blackscreenFading)
						{
								// Debug.LogWarning("Blackscreen Fading still running!");
								return;
						}
				}

				public void ShowBlackscreen()
				{
						VerifyBlackscreen();

						if (blackscreenFading is false)
						{
								blackscreenFading = true;
								blackscreenUI.FadeIn(() => blackscreenFading = false);
						}
				}

				public void HideBlackscreen()
				{
						VerifyBlackscreen();

						if (blackscreenFading is false)
						{
								blackscreenFading = true;
								blackscreenUI.FadeOut(() => blackscreenFading = false);
						}
				}

				public void SetStopCameraEdit(bool stopCamera)
				{
#if UNITY_EDITOR
						if (stopCamera)
						{
								MouseCursorController.Instance.HideCursorLocked();
						}
						else
						{
								MouseCursorController.Instance.ShowCursorUnlocked();
						}
#endif
				}
		}
}