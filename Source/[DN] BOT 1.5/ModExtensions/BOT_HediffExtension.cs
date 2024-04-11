using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.ModExtensions
{
    public class BOT_HediffExtension : DefModExtension
    {
        public TraitDef traitDef;

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors()) yield return error;

            if (traitDef is null) yield return $"{nameof(traitDef)} can't be null!";
        }
    }
}
