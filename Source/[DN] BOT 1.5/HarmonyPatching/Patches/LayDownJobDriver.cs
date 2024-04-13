using More_Traits.DefOfs;
using More_Traits.Extensions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using HarmonyLib;
using System.Reflection;
using More_Traits.HarmonyPatching.Patches.LovesSleeping;
using More_Traits.HarmonyPatching.Patches.SleepyHead;
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
                    Loves_Sleeping_LayDown.AddLoves_SleepingActions(toil, __instance);
                    if (Nyctophobe_CanNotSleep.NoSleepToil(__instance) is Toil noSleepToil) yield return noSleepToil;
                }
                yield return toil;
            }

            if (SleepyHead_LayDown.SleepyHeadToil(__instance) is Toil sleepyToil) yield return sleepyToil;

            yield break;
        }
    }
}
