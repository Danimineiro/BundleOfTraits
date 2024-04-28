using More_Traits.Extensions;
using RimWorld;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace More_Traits.ModExtensions
{
    public class TraitContainer
    {
        public TraitDef traitDef;
        public List<ThingDef> thingDefs = new List<ThingDef>();
        public List<string> devNotes = new List<string>();
        public bool ignoreDraft = false;

        public void LoadDataFromXmlCustom(XmlNode root)
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, nameof(traitDef), root);
            LoadDevNotes(root);
            LoadThingDefs(root);

            if (root[nameof(ignoreDraft)]?.FirstChild.Value is string value) ignoreDraft = ParseHelper.FromString<bool>(value);
        }

        private void LoadThingDefs(XmlNode root)
        {
            if (root.SelectSingleNode(nameof(thingDefs))?.SelectNodes("li") is XmlNodeList xmlThingDefs)
            {
                int count = xmlThingDefs.Count;
                for (int i = 0; i < count; i++)
                {
                    XmlNode node = xmlThingDefs[i];
                    node.TryGetMayRequireAttributeValues(out string mayRequireMod, out string mayRequireAnyMod);
                    DirectXmlCrossRefLoader.RegisterListWantsCrossRef(thingDefs, node.FirstChild.Value, $"{nameof(TraitContainer)}.{nameof(thingDefs)}", mayRequireMod, mayRequireAnyMod);
                }
            }
        }

        private void LoadDevNotes(XmlNode root)
        {
            if (root.SelectSingleNode(nameof(devNotes))?.SelectNodes("li") is XmlNodeList xmlDevNotes)
            {
                int count = xmlDevNotes.Count;
                for (int i = 0; i < count; i++)
                {
                    XmlNode node = xmlDevNotes[i];
                    devNotes.Add(node.FirstChild.Value);
                }
            }
        }

        private void ResolveChildNode(XmlNode root, object wanter, string fieldName)
        {
            if (!(root.SelectSingleNode(fieldName) is XmlNode child)) return;
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(wanter, fieldName, child.FirstChild.Value);
        }
    }
}
