using System.Diagnostics.CodeAnalysis;

namespace More_Traits.ModExtensions;

public class BOT_MentalStateExtension : DefModExtension
{
    [AllowNull] public TraitDef traitDef;

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (string error in base.ConfigErrors()) yield return error;

        if (traitDef is null) yield return $"{nameof(traitDef)} can't be null!";
    }
}
