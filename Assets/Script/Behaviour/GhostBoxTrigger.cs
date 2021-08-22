
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.Events;

namespace Assets.Script.Behaviour
{
		[Serializable]
		public class GhostBoxTrigger
		{
				[SerializeField]
				private List<Text> triggers = new List<Text>();

				[SerializeField]
				private UnityEvent<string> actions;

				// ohne Punktuation
				private static readonly Regex ONLY_NUMALPHA = new Regex(@"[\p{P}]+", RegexOptions.Compiled);

				public bool IsMatch(string text)
				{
						// Frage: wie alt bist du?
						// Antwort: wie alt bist du      <-- Treffer 15 / 15
						// Antwort: wie alt bist du genau <-- kein Treffer (länger)

						// Frage: wie alt bist du genau?
						// Antwort: wie alt bist du       <-- Treffer 15 / 21 
						// Antwort: wie alt bist du genau <-- Treffer 21 / 21
						string context = GetContext(text);
						return triggers.Any(t => context.Contains(t.text));
				}

				private static string GetContext(string text)
				{
						return ONLY_NUMALPHA.Replace(text.Trim().ToLower(), string.Empty);
				}

				public float BestMatchResult(string text)
				{
						string context = GetContext(text);
						float result = 0;
						foreach (var trigger in triggers)
						{
								if (context.Contains(trigger.text))
								{
										var res = trigger.text.Length / context.Length;
										if (result is 0 || res > result)
										{
												result = res;
										}
								}
						}
						return result;
				}

				public void Trigger(string playerText)
				{
						actions?.Invoke(playerText);
				}
		}
}
