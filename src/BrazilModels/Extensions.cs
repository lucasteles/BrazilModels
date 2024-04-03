using System.Globalization;

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
        decimal.TryParse(value, NumberStyles.Number, BrazilCulture.NumberFormat, out var dec)
            ? dec
            : null;

    /// <summary>
    /// Return brazilian number formatted text
    /// Use ',' for decimal separators and '.' for group separators
    /// </summary>
    public static string ToBrazilString(this decimal value, string? format = null) =>
        value.ToString(format, BrazilCulture.NumberFormat);

    /// <summary>
    /// Return brazilian number as money formatted text
    /// Use ',' for decimal separators and '.' for group separators
    /// </summary>
    public static string ToBrazilMoneyString(this decimal value, bool moneySuffix = true)
    {
        var result = value.ToString("C", BrazilCulture.NumberFormat);
        return moneySuffix ? result : result.Replace("R$ ", "");
    }
}

static class Extensions
{
    public static void RemoveNonDigits(
        in this ReadOnlySpan<char> input,
        Span<char> result, out int written
    )
    {
        var resultLength = 0;
        var leadingSpace = true;

        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];

            if (leadingSpace && char.IsWhiteSpace(c))
                continue;

            if (!char.IsDigit(c))
                continue;

            result[resultLength++] = c;
            leadingSpace = false;
        }

        written = resultLength;
    }

    public static int GetSize(this DocumentType value) =>
        value switch
        {
            DocumentType.CNPJ => Cnpj.DefaultLength,
            DocumentType.CPF => Cpf.DefaultLength,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
        };

    public static void OffsetRight(in this Span<char> value, int offset) =>
        value[..^offset].CopyTo(value[offset..]);

    public static bool PadZero(in this Span<char> value, int currentSize)
    {
        var moveSize = value.Length - currentSize;
        if (moveSize < 0) return false;
        if (moveSize is 0) return true;
        value.OffsetRight(moveSize);
        value[..moveSize].Fill('0');
        return true;
    }

    public static bool FormatClean(in this ReadOnlySpan<char> value, Span<char> result)
    {
        if (value.IsEmptyOrWhiteSpace())
        {
            result.Clear();
            return false;
        }

        value.RemoveNonDigits(result, out var realSize);
        return PadZero(result, realSize);
    }

    public static void Mask(in this ReadOnlySpan<char> value,
        Span<char> mask,
        char substituteChar = '#'
    )
    {
        var valueStep = 0;
        for (var i = 0; i < mask.Length; i++)
            if (mask[i] == substituteChar && valueStep < value.Length)
                mask[i] = value[valueStep++];
    }

    public static bool FormatMask(in this ReadOnlySpan<char> value, int size, Span<char> maskResult)
    {
        Span<char> buffer = stackalloc char[size];
        if (!value.FormatClean(buffer))
            return false;

        Mask(buffer, maskResult);
        return true;
    }

    public static bool IsEmptyOrWhiteSpace(in this ReadOnlySpan<char> value) =>
        value.IsEmpty || value.IsWhiteSpace();

    public static string FormatToString(
        this in ReadOnlySpan<char> value,
        int length,
        string? mask = null
    )
    {
        if (value.IsEmptyOrWhiteSpace() || length is 0)
            return string.Empty;

        char[] chars;
        bool ok;

        if (mask is null)
        {
            chars = new char[length];
            ok = value.FormatClean(chars);
        }
        else
        {
            chars = mask.ToCharArray();
            ok = value.FormatMask(length, chars);
        }

        return !ok ? string.Empty : new(chars);
    }
}
