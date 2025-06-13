using System.Xml;

namespace More_Traits.Extensions;

public static class XmlNodeExtensions
{
    public static bool TryGetMayRequireAttributeValues(this XmlNode node, out string? mayRequireMod, out string? mayRequireAnyMod)
    {
        mayRequireMod = null;
        mayRequireAnyMod = null;

        if (node.Attributes is not XmlAttributeCollection attributes) return false;
        mayRequireMod = attributes["MayRequire"]?.Value.ToLower();
        mayRequireAnyMod = attributes["MayRequireAnyOf"]?.Value.ToLower();

        return true;
    }
}
