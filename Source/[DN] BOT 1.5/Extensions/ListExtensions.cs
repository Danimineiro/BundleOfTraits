using System;
using System.Collections.Generic;

namespace More_Traits.Extensions;

public static class ListExtensions
{
    public static Span<T> AsSpanUnsafe<T>(this List<T> list)
    {
        return ListConverter.AsSpan(list);
    }
}

