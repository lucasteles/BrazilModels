using System.Text;
using BrazilModels.Json;

namespace BrazilModels;

using System;
using System.ComponentModel;
using System.Diagnostics;

/// <summary>
/// Basic strongly typed E-mail representation
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(StringSystemTextJsonConverter<Email>))]
[TypeConverter(typeof(StringTypeConverter<Email>))]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public readonly record struct Email : IComparable<Email>, IStringValue
#if NET8_0_OR_GREATER
    , ISpanFormattable
    , ISpanParsable<Email>
    , IUtf8SpanFormattable
    , IUtf8SpanParsable<Email>
#else
    , IFormattable
#endif
{
    /// <summary>
    /// String representation of the Email
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Create a new email instance
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Email(string email)
    {
        ArgumentNullException.ThrowIfNull(email);
        this.Value = email.ToLowerInvariant();
    }

    /// <summary>
    /// Create a new email instance
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Email(ReadOnlySpan<char> email)
    {
        if (email.IsEmptyOrWhiteSpace())
            throw new ArgumentException("Invalid value argument");

        this.Value = email.ToString().ToLowerInvariant();
    }

    /// <summary>
    /// Returns true if is empty
    /// </summary>
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <inheritdoc />
    string IFormattable.ToString(string? format, IFormatProvider? formatProvider) =>
        Value.ToString(formatProvider);

    /// <summary>
    /// Get Email instance of a Value string
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static explicit operator Email(string value)
        => Parse(value);

    /// <summary>
    /// Return string representation of Email
    /// </summary>
    public static implicit operator string(Email email)
        => email.Value;

    /// <summary>
    /// Try to parse a char span to an Email instance
    /// </summary>
    public static bool TryParse(ReadOnlySpan<char> value, out Email email)
    {
        email = default;
        if (value.IsEmpty || !IsValid(value))
            return false;

        email = new(value);
        return true;
    }

    /// <summary>
    /// Try to parse a Value string to an Email instance
    /// </summary>
    public static bool TryParse(string? value, out Email email)
    {
        email = default;
        return value is not null && TryParse(value.AsSpan(), out email);
    }

    /// <summary>
    /// Try to parse a UTF8 byte span to an Email instance
    /// </summary>
    public static bool TryParse(ReadOnlySpan<byte> value, out Email result) =>
        TryParse(Encoding.UTF8.GetString(value).AsSpan(), out result);

    /// <summary>
    /// Parse a Value string to an Email instance
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static Email Parse(string value) =>
        TryParse(value, out var valid)
            ? valid
            : throw new InvalidOperationException($"Invalid E-mail {value}");

    /// <summary>
    /// Parse a Value string to an Email instance
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static Email Parse(ReadOnlySpan<char> value) =>
        TryParse(value, out var valid)
            ? valid
            : throw new InvalidOperationException($"Invalid E-mail {value}");

    /// <summary>
    /// Parse an UTF8 byte span to an Email instance
    /// </summary>
    public static Email Parse(ReadOnlySpan<byte> value) => Parse(Encoding.UTF8.GetString(value));

    /// <summary>
    /// Validate Email string
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsValid(ReadOnlySpan<char> value)
    {
        const string at = "@";
        if (value.IsEmptyOrWhiteSpace())
            return false;

        var index = value.IndexOf(at, StringComparison.OrdinalIgnoreCase);

        return index > 0 &&
               index != value.Length - 1 &&
               index == value.LastIndexOf(at, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Validate Email string
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsValid(string? value) => value is not null && IsValid(value.AsSpan());

    /// <inheritdoc />
    public int CompareTo(Email other) =>
        string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    string DebuggerDisplay() => $"EMAIL{{{Value}}}";


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
        return !utf8Destination.IsEmpty && Encoding.UTF8.TryGetBytes(Value, utf8Destination, out bytesWritten);
    }

    static Email IParsable<Email>.Parse(string s, IFormatProvider? provider) => Parse(s);

    static bool IParsable<Email>.TryParse(string? s, IFormatProvider? provider, out Email result) =>
        TryParse(s, out result);

    static Email ISpanParsable<Email>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
        Parse(s);

    static bool ISpanParsable<Email>.TryParse(
        ReadOnlySpan<char> s, IFormatProvider? provider, out Email result) =>
        TryParse(s, out result);

    static Email IUtf8SpanParsable<Email>.Parse(
        ReadOnlySpan<byte> utf8Text, IFormatProvider? provider) => Parse(utf8Text);

    static bool IUtf8SpanParsable<Email>.TryParse(ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider, out Email result) => TryParse(utf8Text, out result);

    static int IStringValue.ValueSize => 255;
#endif
}
