using System.Collections.Generic;

namespace Assets.Script.Behaviour
{
		public static class DictionaryExtensions
		{
				public static void Decrement<T>(this Dictionary<T, int> numericDictionary, T type, int decrementStep = 1)
				{
						if (numericDictionary.TryGetValue(type, out int amount))
						{
								if (amount > 1)
								{
										numericDictionary[type] = amount - decrementStep;
								}
								else
								{
										numericDictionary.Remove(type);
								}
						}
				}

				public static void Increment<T>(this Dictionary<T, int> numericDictionary, T type, int initial = 1, int incrementStep = 1)
				{
						if (numericDictionary.TryGetValue(type, out int amount) is false)
						{
								numericDictionary[type] = initial;
						}
						else
						{
								numericDictionary[type] = amount + incrementStep;
						}
				}
		}
}