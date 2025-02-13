namespace More_Traits.Extensions;

internal static class ListExtensions
{
    internal static Span<T> AsSpanUnsafe<T>(this List<T> list) => ListConverter.AsSpan(list);

    internal static bool UnsafeContains<TItem>(this List<TItem> list, TItem item) where TItem : class
    {
        ReadOnlySpan<TItem> values = list.AsSpanUnsafe();

        int length = values.Length;
        for (int i = 0; i < length; i++)
        {
            if (values[i] == item) return true;
        }

        return false;
    }

    internal static bool UnsafeContains<TItem>(this List<TItem> list, Func<TItem, bool> predicate) where TItem : class 
    {
        ReadOnlySpan<TItem> values = list.AsSpanUnsafe();

        int length = values.Length;
        for (int i = 0; i < length; i++)
        {
            if (predicate.Invoke(values[i])) return true;
        }

        return false;
    }

    internal static bool UnsafeTryGet<TItem>(this List<TItem> list, Func<TItem, bool> predicate, out TItem? item) where TItem : class
    {
        ReadOnlySpan<TItem> values = list.AsSpanUnsafe();

        int length = values.Length;
        for (int i = 0; i < length; i++)
        {
            item = values[i];
            if (!predicate.Invoke(item)) continue;
            return true;
        }

        item = null;
        return false;
    }

    internal static int UnsafeCount<TItem>(this List<TItem> list, Func<TItem, bool> predicate) where TItem : class
    {
        ReadOnlySpan<TItem> values = list.AsSpanUnsafe();

        int count = 0;
        int length = values.Length;
        for (int i = 0; i < length; i++)
        {
            if (predicate.Invoke(values[i])) count++;
        }

        return count;
    }

    internal static TItem? UnsafeFirstOrDefault<TItem>(this List<TItem> list, Func<TItem, bool> predicate) where TItem : class
    {
        ReadOnlySpan<TItem> values = list.AsSpanUnsafe();

        int length = values.Length;
        for (int i = 0; i < length; i++)
        {
            TItem item = values[i];
            if (!predicate.Invoke(item)) continue;
            return item;
        }

        return default;
    }
}
