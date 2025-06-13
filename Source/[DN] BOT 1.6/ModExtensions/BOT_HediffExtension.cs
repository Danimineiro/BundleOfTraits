using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace More_Traits.ModExtensions;

public class BOT_HediffExtension : DefModExtension
{
    [AllowNull] public TraitDef traitDef;
    [AllowNull] public List<Color> stageColors;

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (string error in base.ConfigErrors()) yield return error;

        if (traitDef is null) yield return $"{nameof(traitDef)} can't be null!";
    }
}
