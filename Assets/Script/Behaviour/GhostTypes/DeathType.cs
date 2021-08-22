using System;

namespace Assets.Script.Behaviour.GhostTypes
{
		public enum DeathType
		{
				[AgeTypeLimit(AgeType.OLD, AgeType.ADULT)]
				ALTERSGRENZE,
				ENTFUEHRT_VERHUNGERT,
				ZU_TODE_GEFOLTERT,
				[AgeTypeLimit(AgeType.OLD, AgeType.ADULT, AgeType.TEEN)]
				SUIZID,
				[AgeTypeLimit(AgeType.OLD, AgeType.ADULT, AgeType.TEEN)]
				DEPRESSION,
				HINGERICHTET,
				[AgeTypeLimit(AgeType.OLD, AgeType.ADULT, AgeType.TEEN)]
				UNGLAEUBIG_AM_ALTER_VERSTORBEN,
				UNERWARTET_VERSTORBEN,
				ERFROREN,
				[AgeTypeLimit(AgeType.OLD, AgeType.ADULT, AgeType.TEEN)]
				ZU_LANGE_ALLEIN_GELASSEN,
				GEQUAELT,
				[AgeTypeLimit(AgeType.OLD, AgeType.ADULT, AgeType.TEEN)]
				PANIK_ATTACKE,
				[AgeTypeLimit(AgeType.OLD, AgeType.ADULT, AgeType.TEEN)]
				SATANISCHES_RITUAL_LIEF_SCHIEF,
				ABGESTUERTZT_IN_DER_DUNKELHEIT,
				ERSTICKT,
				ERDROSSELT,
				ERTRUNKEN,
				ERSCHLAGEN
		}

		[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
		public class AgeTypeLimitAttribute : Attribute
		{
				public AgeTypeLimitAttribute(params AgeType[] limit)
				{
						Limit = limit;
				}

				public AgeType[] Limit { get; }
		}
}