using Assets.Script.Behaviour.GhostTypes;

using System;
using System.Collections.Generic;

namespace Assets.Script.Behaviour
{
		[Serializable]
		public class PlayerInfo
		{
				public string playerFirstName;
				public string playerLastName;
				public int playerAge;
				public Gender gender;
				public DateTime birthDay;
				public DateTime deathDay;
				public string murderName;
				public string motherName;
				public string fatherName;
				public List<(string firstName, bool male)> sisters;
				public MoodType mood;
				public AgeType ageType;
				public bool reserectable = true;
				public int level;
				public long levelExpirience;

				public GhostEntity ghostType { get; set; }

				public bool isDead { get; set; }
		}
}