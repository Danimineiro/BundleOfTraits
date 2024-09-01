using System.Collections.Generic;
using System.Linq;
using Verse;

namespace More_Traits.ModExtensions;

public class BOT_ThinkTreeExtension : DefModExtension
{
    public List<TraitContainer> traitContainers = [];

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (string error in base.ConfigErrors()) yield return error;

        foreach (TraitContainer container in traitContainers)
        {
            if (container.traitDef is null)
            {
                yield return $"{nameof(container.traitDef)} can't be null!";
                continue;
            }

            if (container.devNotes.Any(note => note.NullOrEmpty()) == true)
            {
                yield return $"{nameof(container.devNotes)} can't contain an empty string!";
                continue;
            }

            if (container.thingDefs.Any(def => def is null) == true)
            {
                yield return $"{nameof(container.devNotes)} can't contain a null def!";
                continue;
            }

            if (container.thingDefs.Count + container.devNotes.Count == 0)
            {
                yield return $"{nameof(container.thingDefs)} and {nameof(container.devNotes)} can't both be null or empty!";
            }
        }
    }
}
