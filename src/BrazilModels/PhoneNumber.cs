using System.Text;
using BrazilModels.Json;

namespace BrazilModels;

using System;
using System.ComponentModel;
using System.Diagnostics;

/// <summary>
/// Basic strongly typed Phone number representation
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(StringSystemTextJsonConverter<PhoneNumber>))]
[TypeConverter(typeof(StringTypeConverter<PhoneNumber>))]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public readonly record struct PhoneNumber : IComparable<PhoneNumber>, IStringValue
#if NET8_0_OR_GREATER
    , ISpanFormattable
    , ISpanParsable<PhoneNumber>
    , IUtf8SpanFormattable
    , IUtf8SpanParsable<PhoneNumber>
#else
    , IFormattable
#endif
{
    /// <summary>
    /// String representation of the Phone number
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Create a new phone number instance
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public PhoneNumber(string phoneNumber)
    {
        ArgumentNullException.ThrowIfNull(phoneNumber);
        this.Value = Format(phoneNumber);
    }

    /// <summary>
    /// Create a new phone number instance
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public PhoneNumber(ReadOnlySpan<char> phoneNumber)
    {
        if (phoneNumber.IsEmptyOrWhiteSpace())
            throw new ArgumentException("Invalid value argument");

        this.Value = Format(phoneNumber);
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <summary>
    /// Get phoneNumber instance of a Value string
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static explicit operator PhoneNumber(string value)
        => Parse(value);

    /// <inheritdoc />
    string IFormattable.ToString(string? format, IFormatProvider? formatProvider) =>
        Value.ToString(formatProvider);

    /// <summary>
    /// Return string representation of phoneNumber
    /// </summary>
    public static implicit operator string(PhoneNumber phoneNumber)
        => phoneNumber.Value;

    /// <summary>
    /// Try to parse a Value string to a phoneNumber instance
    /// </summary>
    public static bool TryParse(ReadOnlySpan<char> value, out PhoneNumber phoneNumber)
    {
        phoneNumber = default;
        if (value.IsEmpty)
            return false;

        phoneNumber = new(value);
        return true;
    }

    /// <summary>
    /// Try to parse a Value string to a phoneNumber instance
    /// </summary>
    public static bool TryParse(string? value, out PhoneNumber phoneNumber)
    {
        phoneNumber = default;
        return value is not null && TryParse(value.AsSpan(), out phoneNumber);
    }

    /// <summary>
    /// Try to parse a Value string to a phoneNumber instance
    /// </summary>
    public static bool TryParse(ReadOnlySpan<byte> value, out PhoneNumber result) =>
        TryParse(Encoding.UTF8.GetString(value).AsSpan(), out result);

    /// <summary>
    /// Parse a Value string to a phoneNumber instance
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static PhoneNumber Parse(ReadOnlySpan<char> value) =>
        TryParse(value, out var valid)
            ? valid
            : throw new InvalidOperationException($"Invalid phone number {value}");

    /// <summary>
    /// Parse an UTF8 byte span to a phoneNumber instance
    /// </summary>
    public static PhoneNumber Parse(ReadOnlySpan<byte> value) =>
        Parse(Encoding.UTF8.GetString(value));

    /// <summary>
    /// Parse a Value string to a phoneNumber instance
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static PhoneNumber Parse(string value) =>
        TryParse(value, out var valid)
            ? valid
            : throw new InvalidOperationException($"Invalid phoneNumber {value}");

    /// <inheritdoc />
    public int CompareTo(PhoneNumber other) =>
        string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    string DebuggerDisplay() => $"PHONE{{{Value}}}";

    /// <summary>
    /// Format Phone string.
    /// </summary>
    /// <param name="value">Phone string representation</param>
    /// <returns>Formatted Phone string</returns>
    public static string Format(in ReadOnlySpan<char> value)
    {
        if (value.Length > 512)
            throw new ArgumentException("Value is too large");

        if (value.IsEmptyOrWhiteSpace())
            return string.Empty;

        Span<char> lower = stackalloc char[value.Length];
        value.ToLowerInvariant(lower);
        Span<char> clean = stackalloc char[value.Length];
        lower.RemoveNonDigits(clean, out var size);

        if (value.StartsWith("+"))
        {
            clean.OffsetRight(1);
            clean[0] = '+';
            size++;
        }


        return clean[..size].ToString();
    }

    /// <summary>
    /// Returns true if is empty
    /// </summary>
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);


#if NET8_0_OR_GREATER
    bool ISpanFormattable.TryFormat(
        Span<char> destination, out int charsWritten,
        ReadOnlySpan<char> format, IFormatProvider? provider
    )
    {
        charsWritten = 0;
        if (destination.IsEmpty) return false;

        if (destination.Length < Value.Length)
            return false;

        charsWritten = Value.Length;
        Value.CopyTo(destination);
        return true;
    }

    bool IUtf8SpanFormattable.TryFormat(
        Span<byte> utf8Destination, out int bytesWritten,
        ReadOnlySpan<char> format, IFormatProvider? provider
    )
    {
        bytesWritten = 0;
        if (utf8Destination.IsEmpty) return false;
        return Encoding.UTF8.TryGetBytes(Value, utf8Destination, out bytesWritten);
    }

    static PhoneNumber IParsable<PhoneNumber>.Parse(string s, IFormatProvider? provider) =>
        Parse(s);

    static bool IParsable<PhoneNumber>.TryParse(string? s, IFormatProvider? provider,
        out PhoneNumber result) =>
        TryParse(s, out result);

    static PhoneNumber ISpanParsable<PhoneNumber>.Parse(ReadOnlySpan<char> s,
        IFormatProvider? provider) =>
        Parse(s);

    static bool ISpanParsable<PhoneNumber>.TryParse(
        ReadOnlySpan<char> s, IFormatProvider? provider, out PhoneNumber result) =>
        TryParse(s, out result);

    static PhoneNumber IUtf8SpanParsable<PhoneNumber>.Parse(
        ReadOnlySpan<byte> utf8Text, IFormatProvider? provider) => Parse(utf8Text);

    static bool IUtf8SpanParsable<PhoneNumber>.TryParse(ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider, out PhoneNumber result) => TryParse(utf8Text, out result);

    static int IStringValue.ValueSize { get; } = 255;
#endif
}
