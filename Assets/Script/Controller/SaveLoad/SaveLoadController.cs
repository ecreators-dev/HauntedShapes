using Assets.Script.Behaviour.FirstPerson;

using System.Linq;

using Object = UnityEngine.Object;

namespace Assets.Script.Controller.SaveLoad
{
		public static class SaveLoadController
		{
				public static void SaveAll(this ISaveLoad _)
				{
						var all = Object.FindObjectsOfType<Object>(true).OfType<ISaveLoad>().ToList();
						foreach (ISaveLoad item in all)
						{
								item.Save();
						}
				}

				public static void LoadAll(this ISaveLoad _)
				{
						var all = Object.FindObjectsOfType<Object>(true).OfType<ISaveLoad>().ToList();
						foreach (ISaveLoad item in all)
						{
								item.Load();
						}
				}
		}
}
