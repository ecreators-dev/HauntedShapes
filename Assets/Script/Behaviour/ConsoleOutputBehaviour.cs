using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(TMP_Text))]
		public class ConsoleOutputBehaviour : MonoBehaviour
		{
				private TMP_Text text;

				[SerializeField]
				private int maxLines = 4;

				public bool showLogOutput = true;

				private readonly List<string> lines = new List<string>();

				private void Awake()
				{
						text = GetComponent<TMP_Text>();
				}

				private void Start()
				{
						Application.logMessageReceived += OnMessage;
				}

				private void Update()
				{
						text.enabled = showLogOutput;
						text.text = string.Join("\n", lines);
				}

				private void OnMessage(string logString, string stackTrace, LogType type)
				{
						// all messages same
						if (type is LogType.Error ||
								type is LogType.Exception ||
								type is LogType.Warning ||
								type is LogType.Log)
						{
								lines.Add(logString);
								if (lines.Count > maxLines)
										lines.RemoveAt(0);
						}
				}
		}
}