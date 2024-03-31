using More_Traits.TraitDefModExtension;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.Extensions
{
    public static class PawnExtensions
    {
        public static void TryGainMemory(this Pawn pawn, ThoughtDef thoughtDef, int forcedLevel)
        {
            pawn.needs.mood?.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(thoughtDef, forcedLevel));
        }

        public static bool HasTrait(this Pawn pawn, TraitDef traitDef) => pawn?.story?.traits?.HasTrait(traitDef) ?? false;

        public static bool CanHandlePawn(this Pawn pawn) => !pawn.Dead && pawn.story?.traits != null;

        public static void AddTraitHediffs(this Pawn pawn)
        {
            foreach (Trait trait in pawn.story.traits.allTraits)
            {
                if (!(trait.def.GetModExtension<BOT_TraitExtension>() is BOT_TraitExtension link)) continue;
                pawn.health.GetOrAddHediff(link.hediffDef);
            }
        }
    }
}
