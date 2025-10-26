using System.Runtime.CompilerServices;

namespace More_Traits.Extensions;

internal static class ListExtensions
{
    internal static bool UnsafeContains<TItem>(this List<TItem> list, TItem item) where TItem : class
    {
        if (list.Count == 0) return false;

        Span<TItem> values = list.AsSpan();
        ref TItem refLeft = ref Unsafe.AsRef(in values[0]);
        ref TItem refRight = ref Unsafe.Add(ref refLeft, values.Length);
        
        do
        {
            if (item == refLeft) return true;

            refLeft = ref Unsafe.Add(ref refLeft, 1);
        } while (Unsafe.IsAddressLessThan(ref refLeft, ref refRight));

        return false;
    }

    internal static bool UnsafeContains<TItem>(this List<TItem> list, Func<TItem, bool> predicate) where TItem : class
    {
        if (list.Count == 0) return false;

        Span<TItem> values = list.AsSpan();
        ref TItem refLeft = ref Unsafe.AsRef(in values[0]);
        ref TItem refRight = ref Unsafe.Add(ref refLeft, values.Length);

        do
        {
            if (predicate.Invoke(refLeft)) return true;

            refLeft = ref Unsafe.Add(ref refLeft, 1);
        } while (Unsafe.IsAddressLessThan(ref refLeft, ref refRight));

        return false;
    }

    internal static bool UnsafeTryGet<TItem>(this List<TItem> list, Func<TItem, bool> predicate, out TItem? item) where TItem : class
    {
        item = null;
        if (list.Count == 0) return false; 

        Span<TItem> values = list.AsSpan();
        ref TItem refLeft = ref Unsafe.AsRef(in values[0]);
        ref TItem refRight = ref Unsafe.Add(ref refLeft, values.Length);

        do
        {
            if (predicate.Invoke(refLeft)) 
            {
                item = refLeft;
                return true; 
            }

            refLeft = ref Unsafe.Add(ref refLeft, 1);
        } while (Unsafe.IsAddressLessThan(ref refLeft, ref refRight));

        return false;
    }

    internal static int UnsafeCount<TItem>(this List<TItem> list, Func<TItem, bool> predicate) where TItem : class
    {
        if (list.Count == 0) return 0;

        int count = 0;

        Span<TItem> values = list.AsSpan();
        ref TItem refLeft = ref Unsafe.AsRef(in values[0]);
        ref TItem refRight = ref Unsafe.Add(ref refLeft, values.Length);

        do
        {
            if (predicate.Invoke(refLeft)) count++;

            refLeft = ref Unsafe.Add(ref refLeft, 1);
        } while (Unsafe.IsAddressLessThan(ref refLeft, ref refRight));

        return count;
    }

    internal static TItem? UnsafeFirstOrNull<TItem>(this List<TItem> list, Func<TItem, bool> predicate) where TItem : class
    {
        if (list.Count == 0) return null;

        Span<TItem> values = list.AsSpan();
        ref TItem refLeft = ref Unsafe.AsRef(in values[0]);
        ref TItem refRight = ref Unsafe.Add(ref refLeft, values.Length);

        do
        {
            if (predicate.Invoke(refLeft)) return refLeft;

            refLeft = ref Unsafe.Add(ref refLeft, 1);
        } while (Unsafe.IsAddressLessThan(ref refLeft, ref refRight));

        return null;
    }
}
