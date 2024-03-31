using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.TraitDefModExtension
{
    public class TraitHediffLink : DefModExtension
    {
        public HediffDef hediffDef;

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors()) yield return error;

            if (hediffDef is null ) yield return $"{nameof(hediffDef)} can't be null!";
        }
    }
}
