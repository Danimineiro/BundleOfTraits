using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace More_Traits
{
	[HarmonyPatch]
	class BOTMiscPatches
	{
		[HarmonyPostfix, HarmonyPatch(typeof(CaravanExitMapUtility), "ExitMapAndCreateCaravan", new Type[] {typeof(IEnumerable<Pawn>), typeof(Faction), typeof(int), typeof(int), typeof(int), typeof(bool)})]
		public static void ExitMapAndCreateCaravan() => BOTGregariousCompanyCounter.BOTCalculateCompanyForGregariousPawn();

		[HarmonyPostfix, HarmonyPatch(typeof(Caravan), "Notify_Merged")]
		public static void Notify_Merged() => BOTGregariousCompanyCounter.BOTCalculateCompanyForGregariousPawn();

		[HarmonyPostfix, HarmonyPatch(typeof(WorldObject), "Destroy")]
		public static void Destroy() => BOTGregariousCompanyCounter.BOTCalculateCompanyForGregariousPawn();

		[HarmonyPostfix, HarmonyPatch(typeof(MapComponentUtility), "MapRemoved")]
		public static void MapRemoved() => BOTGregariousCompanyCounter.BOTCalculateCompanyForGregariousPawn();

		[HarmonyPostfix, HarmonyPatch(typeof(Faction), "Notify_PawnJoined")]
		public static void Notify_PawnJoined() => BOTGregariousCompanyCounter.BOTCalculateCompanyForGregariousPawn();

		[HarmonyPostfix, HarmonyPatch(typeof(Faction), "Notify_MemberCaptured")]
		public static void Notify_MemberCaptured() => BOTGregariousCompanyCounter.BOTCalculateCompanyForGregariousPawn();

		[HarmonyPostfix, HarmonyPatch(typeof(Faction), "Notify_MemberDied")]
		public static void Notify_MemberDied() => BOTGregariousCompanyCounter.BOTCalculateCompanyForGregariousPawn();

		[HarmonyPostfix, HarmonyPatch(typeof(KidnappedPawnsTracker), "Kidnap")]
		public static void Kidnap() => BOTGregariousCompanyCounter.BOTCalculateCompanyForGregariousPawn();

		[HarmonyPostfix, HarmonyPatch(typeof(PawnBanishUtility), "Banish")]
		public static void Banish() => BOTGregariousCompanyCounter.BOTCalculateCompanyForGregariousPawn();

		[HarmonyPostfix, HarmonyPatch(typeof(SavedGameLoaderNow), "LoadGameFromSaveFileNow")]
		public static void LoadGameFromSaveFileNow() => BOTGregariousCompanyCounter.BOTCalculateCompanyForGregariousPawn();

		[HarmonyPostfix, HarmonyPatch(typeof(GenGuest), "GuestRelease")]
		public static void GuestRelease() => BOTGregariousCompanyCounter.BOTCalculateCompanyForGregariousPawn();

		[HarmonyPostfix, HarmonyPatch(typeof(TraitSet), "GainTrait")]
		public static void GainTrait(Pawn ___pawn) => Current.Game.GetComponent<BOTTraitsManager>().AddPawn(___pawn);

		[HarmonyPostfix, HarmonyPatch(typeof(Pawn), "SpawnSetup")]
		public static void SpawnSetup(Pawn __instance) => Current.Game.GetComponent<BOTTraitsManager>().AddPawn(__instance);

		[HarmonyPostfix, HarmonyPatch(typeof(Pawn), "Destroy")]
		public static void Destroy(Pawn __instance) => Current.Game.GetComponent<BOTTraitsManager>().RemoveDestroyedPawnFromSets(__instance);

		[HarmonyPostfix, HarmonyPatch(typeof(Pawn), "TicksPerMove")]
		public static int TicksPerMove(int __result, Pawn __instance, bool diagonal)
		{
			if (!__instance.HasTrait(BOTTraitDefOf.BOT_Chionophile)) return __result;

			float num = __instance.GetStatValue(StatDefOf.MoveSpeed, true);
			if (RestraintsUtility.InRestraints(__instance))
			{
				num *= 0.35f;
			}
			if (__instance.carryTracker != null && __instance.carryTracker.CarriedThing != null && __instance.carryTracker.CarriedThing.def.category == ThingCategory.Pawn)
			{
				num *= 0.6f;
			}
			float num2 = num / 60f;
			float num3;
			if (num2 == 0f)
			{
				num3 = 450f;
			}
			else
			{
				num3 = 1f / num2;
				if (__instance.Spawned)
				{
					if (!__instance.Map.roofGrid.Roofed(__instance.Position))
					{
						num3 /= __instance.Map.weatherManager.CurMoveSpeedMultiplier;
					}
					num3 /= Mathf.Lerp(1f, 3.5f, __instance.Map.snowGrid.GetDepth(__instance.Position));
				}
				if (diagonal)
				{
					num3 *= 1.41421f;
				}
			}
			return Mathf.Clamp(Mathf.RoundToInt(num3), 1, 450);
		}

		[HarmonyPostfix, HarmonyPatch(typeof(Pawn_InteractionsTracker), "CheckSocialFightStart")]
		public static bool CheckSocialFightStartPatch(Pawn_InteractionsTracker __instance, ref bool __result, InteractionDef interaction, Pawn initiator)
		{
			Pawn recipient = Traverse.Create(__instance).Field("pawn").GetValue() as Pawn;
			if (recipient.HasTrait(BOTTraitDefOf.BOT_Vulgar) && interaction == InteractionDefOf.Insult)
            {
				__result = false;
				return false;
            }

			return true;
		}

		[HarmonyPostfix, HarmonyPatch(typeof(InteractionWorker_Insult), "RandomSelectionWeight")]
		public static void NegativeInteractionChanceFactor(ref float __result, Pawn initiator, Pawn recipient)
		{
			if (initiator.HasTrait(BOTTraitDefOf.BOT_Vulgar))
			{
				__result = 5f * NegativeInteractionUtility.NegativeInteractionChanceFactor(initiator, recipient) * 0.007f;
			}
		}
	}
}
