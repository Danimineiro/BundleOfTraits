namespace More_Traits.ModExtensions;

public class BOT_MentalStateExtension : DefModExtension
{
    public TraitDef traitDef = null!;

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (string error in base.ConfigErrors()) yield return error;

        if (traitDef is null) yield return $"{nameof(traitDef)} can't be null!";
    }
}
