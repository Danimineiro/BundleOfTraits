using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace More_Traits.ModExtensions;

public class BOT_HediffExtension : DefModExtension
{
    public TraitDef traitDef = null!;
    public List<Color> stageColors = null!;

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (string error in base.ConfigErrors()) yield return error;

        if (traitDef is null) yield return $"{nameof(traitDef)} can't be null!";
    }
}
