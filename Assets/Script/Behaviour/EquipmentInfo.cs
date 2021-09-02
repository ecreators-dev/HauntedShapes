using System;
using System.Collections.Generic;

namespace Assets.Script.Behaviour
{
		public class EquipmentInfo : IEquatable<EquipmentInfo>
		{
				public string Text { get; set; }

				public string TimerText { get; set; }

				public override bool Equals(object obj)
				{
						return Equals(obj as EquipmentInfo);
				}

				public bool Equals(EquipmentInfo other)
				{
						return other != null &&
									 Text == other.Text &&
									 TimerText == other.TimerText;
				}

				public override int GetHashCode()
				{
						int hashCode = 702292887;
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Text);
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TimerText);
						return hashCode;
				}
		}
}