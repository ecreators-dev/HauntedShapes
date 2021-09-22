using Assets.Script.Behaviour.GhostTypes;
using Assets.Script.Model;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class Ghostbox : Equipment
		{
				private const double MINIMAL_DISTANCE = 1.5;
				private static readonly Dictionary<float, string> wordDistanceMeters = new Dictionary<float, string>
				{
						// distanz kleiner als nummer
						[1] = "Kann dich ergreifen",
						[2] = "Nah",
						[3] = "Etwas entfernt",
						[4] = "Weiter entfernt",
						[5] = "Fern",
				};

				public VoiceMaker voiceMaker;
				public List<GhostBoxTrigger> triggers;
				[SerializeField] private bool canToggleOff = true;
				[SerializeField] private bool runAsToggledOn = false;
				[SerializeField] private TMP_Text displayText;
				[SerializeField] private TMP_Text displayFrequency;
				[SerializeField] private Sprite onlineSprite;
				[SerializeField] private Sprite offlineSprite;
				[SerializeField] private SpriteRenderer displayRenderer;
				[SerializeField] private Light displayFaceLight;
				[SerializeField] private Light buttonLightOn;
				[SerializeField] private AudioSource whiteSmokeSoundLoop;
				[SerializeField] private Rigidbody RigidBody;
				[SerializeField] private string brokenText = "Akku leer";

				private readonly Queue<string> recognizedTextQueue = new Queue<string>();
				private readonly Queue<string> sayQueue = new Queue<string>();
				private IVoiceRecognition recognizer;
				private bool initialized;
				private PlayerBehaviour owner;
				private PlayerBehaviour user;
				private float toggleTimestamp;
				private Coroutine freqCo;
				private string activeText;
				[Header("UI 3D")]
				private PlayerBehaviour textInWorldTarget;

				public PlayerBehaviour OwnedByPlayer => owner;

				public bool IsUseless => IsBroken;

				public bool CanToggleOnOff => canToggleOff;

				private void Awake()
				{
						voiceMaker.PlayReadyEvent += OnChangeTextToSpeak;
				}

				private void OnChangeTextToSpeak(VoiceMaker source)
				{
						activeText = source.LastTextSpoken?.ToUpper() ?? string.Empty;
				}

				protected override void Start()
				{
						base.Start();

						canToggleOff = ShopInfo.CanToggleOff;
						if (runAsToggledOn)
						{
								SetPowered(false);
						}
				}

				protected override void Update()
				{
						UpdateVisual();

						recognizer ??= this.GetVoiceRecognizer();
						if (initialized is false && recognizer is { })
						{
								initialized = true;
								recognizer.TextRecognitionEvent += OnTextRecognized;
						}

						if (IsUseless)
						{
								if (Time.timeSinceLevelLoad - toggleTimestamp >= 30)
								{
										// toggle off
										SetPowered(false);
								}
						}

						if (initialized && recognizedTextQueue.Any())
						{
								string next = recognizedTextQueue.Dequeue();
								if (triggers is { })
								{
										if (IsPowered is false || IsUseless)
										{
												// gerät kann nicht genutzt werden
												return;
										}

										Debug.Log($"{nameof(Ghostbox)} ({gameObject.name}) hat verstanden: {next}");
										StartCoroutine(AnalyseText(next));

										// enqueue incoming text - handle fast speaker
										IEnumerator AnalyseText(string txt)
										{
												// nur ein Trigger mit der besten übereinstimmung
												GhostBoxTrigger bestTrigger = (from GhostBoxTrigger trigger in triggers
																											 where trigger.IsMatch(txt)
																											 orderby trigger.BestMatchResult(txt) descending
																											 select trigger).FirstOrDefault();
												if (bestTrigger is { })
												{
														Debug.Log($"{nameof(Ghostbox)} ({gameObject.name}) trigger akiviert");
														bestTrigger.Trigger(txt);
												}
												else
												{
														string context = txt.ToLower();
														var ghosts = FindObjectsOfType<GhostEntity>();
														foreach (GhostEntity ghost in ghosts)
														{
																string[] names = ghost.properties.ghostName.Split(' ');
																string firstName = names[0].ToLower();
																string lastName = names[1].ToLower();
																if (context.Contains(firstName) || context.Contains(lastName))
																{
																		ghost.TriggerName();
																}
														}
												}
												yield break;
										}
								}
						}

						if (sayQueue.Any())
						{
								string sayText = sayQueue.Dequeue();
								Say(sayText);
						}
				}

				private void UpdateVisual()
				{
						if (IsPowered)
						{
								ShowActive();
						}
						else
						{
								ShowInactive();
						}
				}

				private void ShowInactive()
				{
						const bool status = false;
						displayFaceLight.enabled = status;
						buttonLightOn.enabled = status;
						displayText.enabled = status;
						displayFrequency.enabled = status;
						displayRenderer.sprite = offlineSprite;

						whiteSmokeSoundLoop.Stop();
				}

				private void ShowActive()
				{
						displayText.text = activeText;

						// running!
						if (freqCo is { })
						{
								return;
						}

						Debug.Log("Start loop playback: weißes Rauschen");
						whiteSmokeSoundLoop.Play();

						const bool status = true;
						displayFaceLight.enabled = status;
						buttonLightOn.enabled = status;
						displayText.enabled = status;
						displayFrequency.enabled = status;
						displayRenderer.sprite = onlineSprite;

						const float freqSpeed = 0.2f; // sec
						const float freqStep = 0.2f;
						const float maxFreq = 2.4f * 1000f; // 2.4khz
						const float minFreq = 20; // 20hz

						freqCo = StartCoroutine(Run());

						IEnumerator Run()
						{
								WaitForSeconds waiting = new WaitForSeconds(freqSpeed);
								do
								{
										for (float f = minFreq; f <= maxFreq; f += freqStep)
										{
												float fq = f;
												string fHz = "Hz";
												if (fq > 1000)
												{
														fq /= 1000f;
														fHz = "KHz";
												}
												displayFrequency.text = $"{fq:0.0}{fHz}";
												yield return waiting;
										}
								} while (IsPowered && IsUseless is false);
								OnStop();
								yield break;
						}
				}

				private void OnStop()
				{
						displayText.text = string.Empty;
						displayFrequency.text = string.Empty;
						freqCo = null;
				}

				public void OnTextRecognized(string text)
				{
						recognizedTextQueue.Enqueue(text);
				}

				[UnityEventEditorReference]
				public void EventSayGeschwister(string question)
				{
						question = question.ToLower();
						if (FindNearestGhost() is { } data)
						{
								var sisters = data.ghost.properties.sisters ?? new List<(string firstName, bool male)>();
								if (question.Contains("wieviele") || question.Contains("hast du "))
								{
										if (sisters.Count is 0)
										{
												Say("Nein");
										}
										else if (question.Contains("bruder"))
										{
												var countBrothers = sisters.Count(e => e.male);
												if (countBrothers is 0)
												{
														Say("Nein");
												}
												else
												{
														Say(countBrothers.ToString());
												}
										}
										else if (question.Contains("schwester"))
										{
												var countSisters = sisters.Count(e => e.male is false);
												if (countSisters is 0)
												{
														Say("Nein");
												}
												else
												{
														Say(countSisters.ToString());
												}
										}
										else
										{
												if (sisters.Count is 1)
												{
														if (sisters[0].male)
														{
																Say($"einen Bruder");
														}
														else
														{
																Say($"eine Schwester");
														}
												}
												else
												{
														Say($"{sisters.Count}");
												}
										}
								}
								else if (question.Contains("namen") || question.Contains("heiß") || question.Contains("wer sind"))
								{
										if (sisters.Count is 0)
										{
												Say("Keine");
										}
										else
										{
												string list = string.Join(", ", sisters.Select(e => e.firstName));
												int last = list.LastIndexOf(',');
												if (last >= 0)
												{
														list.Remove(last, 1);
														list.Insert(last, " und");
												}
												Say(list);
										}
								}
						}
				}

				[UnityEventEditorReference]
				public void EventSaySternzeichen(string question)
				{
						if (FindNearestGhost() is { } data)
						{
								string sign = data.ghost.properties.starsign;
								Say(sign);
						}
				}

				[UnityEventEditorReference]
				public void EventSayDateOfDeath(string question)
				{
						if (FindNearestGhost() is { } data)
						{
								string date = $"{data.ghost.properties.deathDate:dddd, d. MMMM yyy}";
								Say(date);
						}
				}

				[UnityEventEditorReference]
				public void EventSayDateOfBirth(string question)
				{
						if (FindNearestGhost() is { } data)
						{
								string date = $"{data.ghost.properties.birthDate:d. MMMM yyy}";
								Say(date);
						}
				}

				[UnityEventEditorReference]
				public void EventSayGhostMotherName(string question)
				{
						if (FindNearestGhost() is { } data)
						{
								string motherName = data.ghost.properties.ghostMomFirstName;
								Say(motherName);
						}
				}

				[UnityEventEditorReference]
				public void EventSayGhostFatherName(string question)
				{
						if (FindNearestGhost() is { } data)
						{
								string fatherName = data.ghost.properties.ghostDadFirstName;
								Say(fatherName);
						}
				}

				[UnityEventEditorReference]
				public void EventSayBistDuHierGestorben(string question)
				{
						if (FindNearestGhost() is { } data)
						{
								Say(data.ghost.properties.diedHere ? "Ja" : "Nein");
						}
				}

				[UnityEventEditorReference]
				public void EventSayGhostName(string question)
				{
						if (FindNearestGhost() is { } data)
						{
								string trueGhostName = data.ghost.properties.ghostName;
								Say(trueGhostName);
						}
				}

				[UnityEventEditorReference]
				public void EventSayGhostAge(string question)
				{
						if (ChanceOf(3))
						{
								EventSayGhostExactAge(question);
						}
						else if (FindNearestGhost() is { } data)
						{
								GhostEntity ghost = data.ghost;
								string typeName = GetAgeTypeName(ghost);
								Say(typeName);
						}
				}

				private static bool ChanceOf(int chanceAmount)
				{
						int[] chances = new int[chanceAmount * 2 + 1];
						for (int i = 0; i < chances.Length; i++)
						{
								if (i % chanceAmount == 0)
								{
										chances[i] = 1;
								}
						}
						Debug.Log($"Chance: {(chances.Count(n => n is 1) / (double)chances.Length * 100):0.0} %");
						return chances[UnityEngine.Random.Range(0, chances.Length)] == 1;
				}

				[UnityEventEditorReference]
				public void EventSayGhostExactAge(string question)
				{
						if (FindNearestGhost() is { } data)
						{
								GhostEntity ghost = data.ghost;

								if (question.ToLower().Contains("wann bist du geboren"))
								{
										int birthYear = ghost.properties.birthDate.Year;
										if (birthYear < 100)
										{
												Say($"Ich bin {ghost.properties.age}");
										}
										else
										{
												Say($"{birthYear}");
										}
								}
								else
								{
										Say($"{ghost.properties.age}");
								}
						}
				}

				[UnityEventEditorReference]
				public void EventSayGhostGender(string question)
				{
						if (FindNearestGhost() is { } data)
						{
								GhostEntity ghost = data.ghost;
								string agedGenderName = GetAgedGenderName(ghost);
								Say(agedGenderName);
						}
				}

				[UnityEventEditorReference]
				public void EventSayDeathType(string question)
				{
						if (FindNearestGhost() is { } data)
						{
								GhostEntity ghost = data.ghost;
								DeathType? death = ghost.properties.deathType;
								if (UnityEngine.Random.Range(0, 10) ==
										UnityEngine.Random.Range(0, 10))
								{
										death = null;
								}

								string deathName;
								switch (death)
								{
										case DeathType.ALTERSGRENZE:
												deathName = GetRandomAnswer(
														"Zu alt",
														"Zeit gekommen",
														"meine Kindern wiedersehen");
												break;
										case DeathType.ENTFUEHRT_VERHUNGERT:
												deathName = GetRandomAnswer(
														"entführt",
														"verhungert",
														"kein Essen",
														"einsamkeit",
														"Fremder mitgenommen",
														"Wo bin ich?");
												break;
										case DeathType.ZU_TODE_GEFOLTERT:
												deathName = GetRandomAnswer(
														"Qual",
														"Aufhören!",
														"Schmerz",
														"Angst",
														"Panik",
														"weiß nichts!",
														"Stop, aufhören!");
												break;
										case DeathType.SUIZID:
												deathName = GetRandomAnswer(
														"nichts macht noch Sinn",
														"Ich schaffe das nicht",
														"Geh weg!",
														"Dunkel. Alles dunkel.",
														"Blut",
														"Bald vorbei",
														"frei von Schmerzen");
												break;
										case DeathType.DEPRESSION:
												deathName = GetRandomAnswer(
														"schwach",
														"allein",
														"schwer",
														"was noch?",
														"ich allein?",
														"keiner hilft mir");
												break;
										case DeathType.HINGERICHTET:
												deathName = GetRandomAnswer(
														"Keine Reue",
														"Mein Tod erfüllt seinen Zweck",
														"Ich bin schuldig",
														"justizfrei",
														"Ich sterbe nicht",
														"Er hat es verdient");
												break;
										case DeathType.UNGLAEUBIG_AM_ALTER_VERSTORBEN:
												deathName = GetRandomAnswer(
														"Kein Gott",
														"Mein Weg",
														"Kirche nein");
												break;
										case DeathType.UNERWARTET_VERSTORBEN:
												deathName = GetRandomAnswer(
														"Was ist das hier?",
														"Ich bin nicht tot",
														"Mach das Licht an",
														"Hallo",
														"Wer bist du?",
														"Langweilig");
												break;
										case DeathType.ERFROREN:
												deathName = GetRandomAnswer(
														"kalt",
														"eiskalt",
														"Winter",
														"starr wie eis",
														"kann nicht bewegen",
														"müde");
												break;
										case DeathType.ZU_LANGE_ALLEIN_GELASSEN:
												deathName = GetRandomAnswer(
														"allein",
														"so allein",
														"niemand hier",
														"wer ist mit mir?",
														"wo sind?",
														"ohnmacht");
												break;
										case DeathType.GEQUAELT:
												deathName = GetRandomAnswer(
														"Leid",
														"Pflaster",
														"Wunde",
														"Keine Beine",
														"Keine Arme",
														"hilflos");
												break;
										case DeathType.PANIK_ATTACKE:
												deathName = GetRandomAnswer(
														"Hilfe!",
														"Keine Luft!",
														"Hilf mir!",
														"Schnell!",
														"Keiner hilft!");
												break;
										case DeathType.SATANISCHES_RITUAL_LIEF_SCHIEF:
												deathName = GetRandomAnswer(
														"Opfer",
														"Luzifer",
														"Baltasar",
														"Satan",
														"Hölle",
														"Kommt zu mir");
												break;
										case DeathType.ABGESTUERTZT_IN_DER_DUNKELHEIT:
												deathName = GetRandomAnswer(
														"Ich falle",
														"Dunkelheit",
														"Schutz fehlt",
														"Vorbei",
														"Ganz vorbei");
												break;
										case DeathType.ERSTICKT:
												deathName = GetRandomAnswer(
														"Keine Luft",
														"Atem",
														"schwer fällt",
														"Luft",
														"Zu weit");
												break;
										case DeathType.ERDROSSELT:
												deathName = GetRandomAnswer(
														"Strick",
														"Zu eng",
														"Kette",
														"Lass los!",
														"loslassen",
														"Aufhören!");
												break;
										case DeathType.ERTRUNKEN:
												deathName = GetRandomAnswer(
														"blau",
														"das Licht",
														"keine Luft",
														"atmen",
														"möchte atmen",
														"Tiefe");
												break;
										case DeathType.ERSCHLAGEN:
												deathName = GetRandomAnswer(
														"schwach",
														"ich war schwach",
														"schlug weiter",
														"nicht schuld",
														"aufhören!",
														"Gewalt");
												break;
										default:
												deathName = GetRandomAnswer(
														"Mörder",
														"Hölle",
														"Tod",
														"Unfall",
														"Satan",
														"Dunkelheit");
												break;
								}
								Say(deathName);
						}

						string GetRandomAnswer(params string[] answers)
						{
								return answers[UnityEngine.Random.Range(0, answers.Length)];
						}
				}

				[UnityEventEditorReference]
				public void EventSayMood(string question)
				{
						if (FindNearestGhost() is { } data)
						{
								GhostEntity ghost = data.ghost;
								MoodType stimmung = ghost.properties.mood;
								string moodName;
								switch (stimmung)
								{
										case MoodType.FRIEDLICH:
												moodName = "friedlich";
												break;
										case MoodType.AENGSTLICH:
												moodName = "ängstlich";
												break;
										case MoodType.MUEDE:
												moodName = "müde";
												break;
										case MoodType.GESTRESST:
												moodName = "gestresst";
												break;
										case MoodType.GENERVT:
												moodName = "genervt";
												break;
										case MoodType.VERBITTERT:
												moodName = "verbittert";
												break;
										case MoodType.HASS:
												moodName = "Hass";
												break;
										case MoodType.AMUESIERT:
												moodName = "gut gelaunt";
												break;
										case MoodType.WUETEND:
												moodName = "wütend";
												break;
										case MoodType.ZORNIG:
												moodName = "rasend";
												break;
										case MoodType.BLUTRUENSTIG:
												moodName = "blutdurstig";
												break;
										case MoodType.TRAURIG:
												moodName = "traurig";
												break;
										case MoodType.VERLIEBT:
												moodName = "verliebt";
												break;
										case MoodType.ERBOST:
												moodName = "böse";
												break;
										default:
												moodName = "unklar";
												break;
								}
								Say(moodName);
						}
				}

				private static string GetAgedGenderName(GhostEntity ghost)
				{
						Gender gender = ghost.properties.gender;
						AgeType ageType = ghost.properties.ageType;
						string agedGenderName;
						switch (ageType)
						{
								case AgeType.CHILD:
										if (ghost.properties.age < 7)
										{
												agedGenderName = gender is Gender.MALE ? "Baby Junge" : "Baby Mädchen";
										}
										else
										{
												agedGenderName = gender is Gender.MALE ? "kleiner Junge" : "kleines Mädchen";
										}
										break;
								case AgeType.TEEN:
										agedGenderName = gender is Gender.MALE ? "Junge" : "Mädchen";
										break;
								case AgeType.ADULT:
										agedGenderName = gender is Gender.MALE ? "Mann" : "Frau";
										break;
								case AgeType.OLD:
								default:
										agedGenderName = gender is Gender.MALE ? "alter Mann" : "alte Frau";
										break;
						}

						return agedGenderName;
				}

				private static string GetAgeTypeName(GhostEntity ghost)
				{
						AgeType type = ghost.properties.ageType;
						string typeName;
						switch (type)
						{
								case AgeType.CHILD:
										typeName = "klein";
										break;
								case AgeType.TEEN:
										typeName = "jung";
										break;
								case AgeType.ADULT:
										typeName = "erwachsen";
										break;
								case AgeType.OLD:
								default:
										typeName = "alt";
										break;
						}

						return typeName;
				}

				[UnityEventEditorReference]
				public void RepeatWhatISaid(string playerText)
				{
						voiceMaker?.SayAsync($"Ich habe das von dir gehört: {playerText}");
				}

				private (GhostEntity ghost, float distance)? FindNearestGhost()
				{
						var ghosts = FindObjectsOfType<GhostEntity>();
						if (ghosts.Length is 0)
								return null;

						return ghosts.Select(d => (ghost: d, distance: GetDistance(d))).OrderBy(e => e.distance).FirstOrDefault();
				}

				[UnityEventEditorReference]
				public void EventUmfassendeOrtsangabe(string question)
				{
						if (FindNearestGhost() is { } data)
						{
								float meters = UnitsToMeters(data.distance);

								float middle = wordDistanceMeters.Keys.Average();
								float quarter = middle / 2;
								string nahFern;
								if (meters <= quarter)
								{
										nahFern = "sehr nah";
								}
								else if (meters <= middle)
								{
										nahFern = "nah";
								}
								else
								{
										nahFern = "entfernt";
								}

								(bool back, bool left) dir = GetRelativeRotationDirection(data.ghost);
								string rechtsLinks = (dir.left ? "links" : "rechts") + " " + (dir.back ? "hinter" : "vor");

								float meter = RoundDigits(UnitsToMeters(data.distance), 1);

								bool above = IsGhostAboveMyself(data.ghost, out var distanceY);
								string ueber;
								if (distanceY <= MINIMAL_DISTANCE)
								{
										ueber = null;
								}
								else
								{
										ueber = above ? "und über " : "und unter ";
								}

								if (meter <= 1.5f && distanceY <= 0.5f)
								{
										if (dir.back)
										{
												Say("Ich bin direkt hinter dir. Ich kann dich anfassen.");
										}
										else
										{
												Say("Ich bin direkt vor dir. Reich mir die Hand.");
										}
								}
								else
								{
										string sayText = $"Ich bin {nahFern}. {meter} Meter {rechtsLinks} {ueber}dir.";
										Say(sayText);
								}
						}
				}


				[UnityEventEditorReference]
				public void EventWieWeitGenau(string question)
				{
						if (FindNearestGhost() is { } data)
						{
								float distanceUnits = data.distance;
								float distanceMeter = RoundDigits(UnitsToMeters(distanceUnits), 1);

								Say($"{distanceMeter} Meter");
						}
				}

				[UnityEventEditorReference]
				public void EventObenOderUnten(string question)
				{
						if (FindNearestGhost() is { } data)
						{
								GhostEntity ghost = data.ghost;
								string text = IsGhostAboveMyself(ghost, out var distY)
										? "über" : "unter";

								// sag "bei dir", wenn du nicht wirklich über oder unter mir bist
								if (distY <= MINIMAL_DISTANCE)
										text = "bei";

								Say($"Ich bin {text} dir");
						}
				}

				private bool IsGhostAboveMyself(GhostEntity ghost, out float distanceY)
				{
						float ghostY = ghost.transform.position.y;
						float selfY = transform.position.y;
						distanceY = Mathf.Abs(ghostY - selfY);
						return ghostY > selfY;
				}

				[UnityEventEditorReference]
				public void EventVorOderHinterMir(string question)
				{
						if (FindNearestGhost() is { } data)
						{
								GhostEntity distanceNearest = data.ghost;
								(bool back, bool left) = GetRelativeRotationDirection(distanceNearest);

								string txtFront = back ? "hinten" : "vorne";
								string txtLeft = left ? "links" : "rechts";
								Say($"Ich bin von dir aus {txtFront} {txtLeft}");
						}
				}

				private (bool back, bool left) GetRelativeRotationDirection(GhostEntity distanceNearest)
				{
						Vector3 myPosition = transform.position;
						myPosition.y = 0;
						Vector3 ghostPosition = distanceNearest.transform.position;
						ghostPosition.y = 0;

						Quaternion rotation = Quaternion.LookRotation(ghostPosition - myPosition);
						float angle = (rotation.eulerAngles.y + -transform.rotation.eulerAngles.y) % 360;

						float a = angle - 180;
						bool back = a >= -90 && a <= 90;
						float b = a - 90;
						bool left = b >= -90 && b <= 90;
						return (back, left);
				}

				[UnityEventEditorReference]
				public void EventWoBistDu(string question)
				{
						if (FindNearestGhost() is { } data)
						{
								string text = GetDistanceInWords(data.distance);
								Say(text);
						}
				}

				private static string GetDistanceInWords(float distance)
				{
						float key = GetDistanceMeterForWord(distance);
						string text = wordDistanceMeters[key];
						return text;
				}

				private static float GetDistanceMeterForWord(float distanceInUnits)
				{
						float distanceNearest = distanceInUnits;
						float distanceMeters = UnitsToMeters(distanceNearest);

						var analyse = wordDistanceMeters.Select(kvp => (radius: kvp.Key, text: kvp.Value))
														.OrderBy(e => e.radius)
														.ToList();

						float key = wordDistanceMeters.Last().Key;
						float prev = 0;
						foreach (var item in analyse)
						{
								float from = prev;
								float to = item.radius;
								string text = item.text;

								if (distanceMeters >= from && distanceMeters <= to)
								{
										key = item.radius;
										break;
								}
								prev = to;
						}

						return key;
				}

				private void Say(string text)
				{
						if (voiceMaker.CanSpeak is false || sayQueue.Any())
						{
								sayQueue.Enqueue(text);
						}
						else if (voiceMaker is { })
						{
								voiceMaker.SayAsync(text);
						}
				}

				private float GetDistance(GhostEntity ghost)
				{
						// spieler ist dort wo die ghostbox ist
						float distanceRadius = (transform.position - ghost.transform.position).magnitude;
						return distanceRadius;
				}

				private static float UnitsToMeters(float distanceUnits) => 1.5f / 4f * distanceUnits;

				private static float RoundDigits(float number, int decimals)
				{
						float shift = Mathf.Pow(10, decimals);
						return Mathf.Round(number * shift) / shift;
				}

				protected override void OnEquip()
				{
				}

				protected override void OnInventory()
				{
				}

				public override bool CanInteract(PlayerBehaviour sender)
				{
						return IsTakenByPlayer is false;
				}

				public override void Interact(PlayerBehaviour sender)
				{

				}

				protected override void OnHuntStart()
				{

				}

				protected override void OnHuntStop()
				{
						// nothing yet
				}

				protected override void OnOwnerOwnedEquipment()
				{
						// nothing yet
				}

				public override EquipmentInfo GetEquipmentInfo()
				{
						if (IsBroken)
						{
								return new EquipmentInfo
								{
										Text = brokenText
								};
						}
						return null;
				}
		}
}