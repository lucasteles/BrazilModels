using System;
using System.Text.RegularExpressions;

namespace BrazilModels;

static class Extensions
{
    public static ReadOnlySpan<char> ClearString(in this ReadOnlySpan<char> value) =>
        Regex.Replace(value.ToString(), "[^0-9a-zA-Z]+", string.Empty).AsSpan().Trim();

    public static ReadOnlySpan<char> FormatMask(in this ReadOnlySpan<char> value, int size, string mask) =>
        value.IsEmptyOrWhiteSpace()
            ? string.Empty
            : value.FormatClean(size)
                .Mask(mask);

    public static ReadOnlySpan<char> FormatClean(in this ReadOnlySpan<char> value, int size) =>
        value.IsEmptyOrWhiteSpace()
            ? string.Empty
            : value.ClearString().PadZero(size);

    public static ReadOnlySpan<char> PadZero(in this ReadOnlySpan<char> value, int totalWidth)
    {
        Span<char> destination = new char[totalWidth];
        destination.Fill('0');
        var step = Math.Max(totalWidth - value.Length, 0);
        value.CopyTo(destination[step..]);
        return destination;
    }

    public static ReadOnlySpan<char> Mask(in this ReadOnlySpan<char> value, string mask, char substituteChar = '#')
    {
        Span<char> result = new char[mask.Length];
        mask.CopyTo(result);

        var valueStep = 0;
        for (var i = 0; i < mask.Length; i++)
            if (mask[i] == substituteChar && valueStep < value.Length)
                result[i] = value[valueStep++];

        return result;
    }

    public static bool IsEmptyOrWhiteSpace(in this ReadOnlySpan<char> value) =>
        value.IsEmpty || value.IsWhiteSpace();
}
