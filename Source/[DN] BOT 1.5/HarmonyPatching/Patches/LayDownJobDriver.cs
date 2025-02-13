using RimWorld;
using System.Collections.Generic;
using Verse.AI;
using More_Traits.HarmonyPatching.Patches.Communal;
using More_Traits.HarmonyPatching.Patches.LovesSleeping;
using More_Traits.HarmonyPatching.Patches.Nyctophobe;

namespace More_Traits.HarmonyPatching.Patches
{
    public class LayDownJobDriver
    {
        public static IEnumerable<Toil> MakeNewToils_PostFix(IEnumerable<Toil> values, JobDriver_LayDown __instance)
        {
            foreach (Toil toil in values)
            {
                if (toil.debugName == "LayDown")
                {
                    Communal_Toils_LayDown.AddCommunalActions(toil, __instance);
                    Loves_Sleeping_LayDown.AddLoves_SleepingActions(toil, __instance);
                    if (Nyctophobe_CanNotSleep.NoSleepToil(__instance) is Toil noSleepToil) yield return noSleepToil;
                }
                yield return toil;
            }

            yield break;
        }
    }
}
