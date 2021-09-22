using RandomNameGeneratorLibrary;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace Assets.Script.Behaviour.GhostTypes
{
		[CreateAssetMenu(fileName = "Ghost-Property", menuName = "Game/Ghost-Properties")]
		public class GhostPropertyAsset : ScriptableObject
		{
				public AgeType ageType;
				public MoodType mood;
				public int age;
				public DateTime deathDate;
				public Gender gender;
				public DateTime birthDate;
				public string starsign;
				public List<(string firstName, bool male)> sisters;
				public string ghostName;
				public string ghostMomFirstName;
				public string ghostDadFirstName;
				public bool diedHere;
				public DeathType deathType;
				public float evilMultiplicator;

				public List<string> murderNames;
				public List<Evidence> evidences;
				public List<PhysicalEvidence> physicalEvidences;

				public void Generate()
				{
						ageType = GetEnumValueRandom<AgeType>();
						mood = GetEnumValueRandom<MoodType>();
						gender = GetEnumValueRandom<Gender>();
						age = GetRandomNumberBetween(ageType);
						birthDate = GetRandomBirthDate(age);
						deathDate = birthDate.AddYears(age).AddDays(UnityEngine.Random.Range(0, 365 - 1));
						if (deathDate > DateTime.Now)
						{
								deathDate = DateTime.Now.Subtract(TimeSpan.FromDays(2));
						}
						starsign = GetStarSign(birthDate);
						sisters = GetRandomSisters(age);

						ghostName = GetRandomFirstAndLastName(gender);
						ghostMomFirstName = GetRandomFirstAndLastName(Gender.FEMALE).Split(' ')[0];
						ghostDadFirstName = GetRandomFirstAndLastName(Gender.MALE).Split(' ')[0];
						deathType = GetDeathType(ageType);

						// ist der Tote jünger als 150 ist er weniger böse
						// ist der Tode älter als 300 ist er mehr böse;
						int maxAge = Mathf.Min(age, 300);
						// ein Wert zwischen -1 und +1
						evilMultiplicator = (maxAge - 150) / 150f;

						murderNames = new int[UnityEngine.Random.Range(1, 4)].Select(_ => GetRandomFirstAndLastName(GetEnumValueRandom<Gender>())).ToList();
						diedHere = UnityEngine.Random.Range(0, 5) == UnityEngine.Random.Range(0, 5);
				}

				public void CopyPersonality(PlayerBehaviour playerBehaviour)
				{
						this.age = playerBehaviour.PlayerData.playerAge;
						this.birthDate = DateTime.Now.AddYears(-age);
						this.deathDate = DateTime.Today;
						this.deathType = DeathType.ZU_TODE_GEFOLTERT;
						this.diedHere = true;
						this.evilMultiplicator = 1;
						this.gender = playerBehaviour.PlayerData.gender;
						this.starsign = GetStarSign(birthDate);
				}

				private List<(string firstName, bool male)> GetRandomSisters(int age)
				{
						if (age > 1)
						{
								int amountSisters = UnityEngine.Random.Range(0, 3);
								if (UnityEngine.Random.Range(0, 1) == UnityEngine.Random.Range(0, 1))
								{
										amountSisters = 1;
								}
								var sistersNames = new List<(string firstName, bool male)>();
								for (int i = 0; i < amountSisters && age > (1 + i); i++)
								{
										string name;
										bool isMale = UnityEngine.Random.Range(0, 3) == UnityEngine.Random.Range(0, 3);
										if (isMale)
										{
												name = GetRandomFirstAndLastName(Gender.MALE).Split(' ')[0];
										}
										else
										{
												name = GetRandomFirstAndLastName(Gender.FEMALE).Split(' ')[0];
										}
										sistersNames.Add((name, isMale));
								}
								return sistersNames;
						}
						else
						{
								return null;
						}
				}

				private DateTime GetRandomBirthDate(int age)
				{
						int year = RNum(100, DateTime.Now.Year - age);
						int month = RNum(1, 12);
						return new DateTime(year,
								month,
								RNum(1, DateTime.DaysInMonth(year, month)));

						int RNum(int min, int maxIncl)
						{
								return UnityEngine.Random.Range(min, maxIncl);
						}
				}

				public enum StarSign
				{
						WASSERMANN = 1 * 21,
						FISCHE = 2 * 20,
						WIDDER = 3 * 21,
						STIER = 4 * 21,
						ZWILLINGE = 5 * 22,
						KREBS = 6 * 22,
						LOEWE = 7 * 24,
						JUNGFRAU = 8 * 24,
						WAAGE = 9 * 24,
						SKORPION = 10 * 24,
						SCHUETZE = 11 * 23,
						STEINBOCK = 12 * 22
				}

				private string GetStarSign(DateTime date)
				{
						var starSigns = Enum.GetValues(typeof(StarSign)).Cast<StarSign>().ToList()
								.Select((e, i) => (current: ConvertToEntry(e, i + 1), before: ConvertToEntry(e, i == 0 ? 12 : i)))
								.ToList();

						foreach (var (current, before) in starSigns)
						{
								if (date.Month == current.month)
								{
										if (date.Day >= current.beginDay)
										{
												return current.sign;
										}
										else
										{
												return before.sign;
										}
								}
						}
						return null;

						static (int beginDay, int month, string sign) ConvertToEntry(StarSign e, int month)
						{
								return (beginDay: GetDay(e, month), month: month, sign: GetName(e));

								static int GetDay(StarSign e, int month)
								{
										return (int)e / month;
								}

								static string GetName(StarSign e)
								{
										string constantName = e.ToString();
										string umlauteText = constantName.Replace("OE", "Ö").Replace("UE", "Ü");
										string formatGerman = $"{umlauteText[0].ToString().ToUpper()}{umlauteText.Substring(1).ToLower()}";
										return formatGerman;
								}
						}
				}

				private DateTime GetRandomDeathYear(int age)
				{
						long max = DateTime.Now.AddYears(-age).Ticks;
						long min = new DateTime(100, 1, 1).AddYears(age).Ticks;
						long meanwhile = (long)Mathf.Round(UnityEngine.Random.Range(min, max));
						return new DateTime(meanwhile);
				}

				private DeathType GetDeathType(AgeType ageType)
				{
						DeathType type;
						do
						{
								type = GetEnumValueRandom<DeathType>();
						} while (InvalidType());
						return type;

						bool InvalidType()
						{
								FieldInfo constant = typeof(DeathType).GetField(Enum.GetName(typeof(DeathType), type));
								if (constant.GetCustomAttribute<AgeTypeLimitAttribute>() is { } limit)
								{
										if (limit.Limit.Contains(ageType))
										{
												// valid
												return false;
										}
										// not supported age type
										return true;
								}
								// not depending on agetype (any age)
								return false;
						}
				}

				private string GetRandomFirstAndLastName(Gender gender)
				{
						const int NumberOfNames = 5;
						if (gender is Gender.MALE)
						{
								List<string> names = new System.Random(DateTime.Now.GetHashCode()).GenerateMultipleMaleFirstAndLastNames(NumberOfNames).ToList();
								return names[UnityEngine.Random.Range(0, NumberOfNames)];
						}
						else
						{
								List<string> names = new System.Random(DateTime.Now.GetHashCode()).GenerateMultipleFemaleFirstAndLastNames(NumberOfNames).ToList();
								return names[UnityEngine.Random.Range(0, NumberOfNames)];
						}
				}

				private int GetRandomNumberBetween(AgeType ageType)
				{
						var field = typeof(AgeType).GetField(Enum.GetName(typeof(AgeType), ageType));
						var range = field.GetCustomAttribute<RangeAttribute>();
						int min = (int)range.min;
						int max = Mathf.Min((int)range.max, 1000);
						return Mathf.Min(UnityEngine.Random.Range(min, max + 1), DateTime.Now.Year - 100);
				}

				private static T GetEnumValueRandom<T>() where T : Enum
				{
						var values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
						return values[UnityEngine.Random.Range(0, values.Length)];
				}

				public void CheckScenePhysicalEvidence()
				{
						IList<int> all = FindObjectsOfType<PhysicalEvidence>().ToDictionary(p => p.GetInstanceID()).Keys.ToList();
						if (physicalEvidences is { })
						{
								int found = physicalEvidences.Count(item => all.Contains(item.GetInstanceID()));
								if (found < physicalEvidences.Count)
								{
										throw new System.Exception("Physical Evidence missing in scene");
								}
						}
				}
		}
}