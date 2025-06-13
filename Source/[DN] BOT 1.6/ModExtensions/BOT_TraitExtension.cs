using System.Diagnostics.CodeAnalysis;

namespace More_Traits.ModExtensions;

public class BOT_TraitExtension : DefModExtension
{
    [AllowNull] public HediffDef hediffDef;

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (string error in base.ConfigErrors()) yield return error;

        if (hediffDef is null) yield return $"{nameof(hediffDef)} can't be null!";
    }
}
