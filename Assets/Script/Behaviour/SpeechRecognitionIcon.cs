using Assets.Script.Controller;
using Assets.Script.Model;

using System;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.Behaviour
{
		public class SpeechRecognitionIcon : MonoBehaviour
		{
				[SerializeField]
				private Color runningTalking = Color.green;

				[SerializeField]
				private Color runningNotTalking = Color.yellow;

				[SerializeField]
				private Color notRunning = Color.red;

				[SerializeField]
				private Image icon;

				private void Start()
				{
						if (icon is null)
								throw new Exception($"Missing {nameof(icon)}");

						icon.color = Color.white;
				}

				private void Update()
				{
						IVoiceRecognition recognizer = VoiceRecognitionSingleton.Instance;
						if (recognizer is { })
						{
								if (recognizer.IsRunning)
								{
										if(recognizer.IsClientTalking)
										{
												SetColor(runningTalking);
										}
										else
										{
												SetColor(runningNotTalking);
										}
								}
								else
								{
										SetColor(notRunning);
								}
						}
				}

				private void SetColor(Color color)
				{
						if (icon.color.Equals(color) is false)
						{
								icon.color = color;
						}
				}
		}
}
