namespace More_Traits.DefOfs;

[DefOf]
public static class BOT_PreceptDefOf
{
    static BOT_PreceptDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PreceptDefOf));

    [MayRequireIdeology]
    public static PreceptDef Pain_Idealized = null!;
}
