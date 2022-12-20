using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BrazilModels;

/// <summary>
/// Brazil extensions
/// </summary>
public static class BrazilExtensions
{
    /// <summary>
    /// Converts the string representation of a number to its Decimal equivalent.
    /// Use ',' for decimal separators and '.' for group separators
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static decimal? TryParseDecimalBrazil(this string value) =>
        decimal.TryParse(value, NumberStyles.Number, NumberFormat, out var dec) ? dec : null;

    /// <summary>
    /// Return brazilian number formatted text
    /// Use ',' for decimal separators and '.' for group separators
    /// </summary>
    public static string ToBrazilString(this decimal value, string? format = null) =>
        value.ToString(format, NumberFormat);

    /// <summary>
    /// Return brazilian number as money formatted text
    /// Use ',' for decimal separators and '.' for group separators
    /// </summary>
    public static string ToBrazilMoneyString(this decimal value, bool moneySuffix = true)
    {
        var result = value.ToString("C", NumberFormat);
        return moneySuffix ? result : result.Replace("R$ ", "");
    }

    /// <summary>
    /// NumberFormatInfo using ',' for decimal separators and '.' for group separators
    /// </summary>
    /// {}
    public static readonly NumberFormatInfo NumberFormat =
        new()
        {
            CurrencyDecimalDigits = 2,
            CurrencyDecimalSeparator = ",",
            CurrencyGroupSeparator = ".",
            CurrencyNegativePattern = 9,
            CurrencyPositivePattern = 2,
            CurrencySymbol = "R$",
            NaNSymbol = "NaN",
            NegativeInfinitySymbol = "-\u221E",
            NegativeSign = "-",
            NumberDecimalDigits = 3,
            NumberDecimalSeparator = ",",
            NumberGroupSeparator = ".",
            NumberNegativePattern = 1,
            PerMilleSymbol = "\u2030",
            PercentDecimalDigits = 3,
            PercentDecimalSeparator = ",",
            PercentGroupSeparator = ".",
            PercentNegativePattern = 1,
            PercentPositivePattern = 1,
            PercentSymbol = "%",
            PositiveInfinitySymbol = "\u221E",
            PositiveSign = "\u002B",
            NativeDigits = new[]
            {
                "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
            },
            DigitSubstitution = DigitShapes.None,
            CurrencyGroupSizes = new[]
            {
                3
            },
            NumberGroupSizes = new[]
            {
                3
            },
            PercentGroupSizes = new[]
            {
                3
            },
        };
}

static class Extensions
{
    public static ReadOnlySpan<char> ClearString(in this ReadOnlySpan<char> value) =>
        Regex.Replace(value.ToString(), "[^0-9a-zA-Z]+", string.Empty).AsSpan().Trim();

    public static ReadOnlySpan<char> FormatMask(in this ReadOnlySpan<char> value, int size,
        string mask) =>
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
        value[..(totalWidth - step)].CopyTo(destination[step..]);
        return destination;
    }

    public static ReadOnlySpan<char> Mask(in this ReadOnlySpan<char> value, string mask,
        char substituteChar = '#')
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
