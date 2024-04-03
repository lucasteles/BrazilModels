using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using BrazilModels.Json;

namespace BrazilModels;

/// <summary>
/// Brazilian CPF number
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(StringSystemTextJsonConverter<Cpf>))]
[TypeConverter(typeof(StringTypeConverter<Cpf>))]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public readonly record struct Cpf : IComparable<Cpf>
#if NET8_0_OR_GREATER
    , ISpanFormattable
    , ISpanParsable<Cpf>
    , IUtf8SpanFormattable
    , IUtf8SpanParsable<Cpf>
#else
    , IFormattable
#endif
{
    /// <summary>
    /// CPF Size
    /// </summary>
    public const int DefaultLength = 11;

    /// <summary>
    /// CPF Mask
    /// </summary>
    public const string Mask = "###.###.###-##";

    /// <summary>
    /// Empty invalid CPF
    /// </summary>
    public static readonly Cpf Empty = new();

    /// <summary>
    /// CPF string representation
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Construct an Empty CPF
    /// </summary>
    public Cpf() => Value = new string('0', DefaultLength);

    /// <summary>
    /// Construct a new CPF
    /// </summary>
    /// <param name="value">A valid CPF as ReadOnlySpan of char</param>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CPF.
    /// </exception>
    public Cpf(in string value) : this(value.AsSpan()) { }

    /// <summary>
    /// Construct a new CPF
    /// </summary>
    /// <param name="value">A valid CPF a numeric value</param>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CPF.
    /// </exception>
    public Cpf(in long value) : this(value.ToString(CultureInfo.InvariantCulture)) { }

    /// <summary>
    /// Construct a new CPF
    /// </summary>
    /// <param name="value">A valid CPF as ReadOnlySpan of char</param>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CPF.
    /// </exception>
    public Cpf(in ReadOnlySpan<char> value) : this(value, true) { }

    Cpf(in ReadOnlySpan<char> value, bool validate)
    {
        Value = Format(value);

        if (validate && !Validate(value))
            throw CpfException(value);
    }

    static FormatException CpfException(in ReadOnlySpan<char> value) =>
        new($"Invalid CPF: {value}");

    /// <summary>
    /// Return a CPF string representation without special symbols
    /// </summary>
    /// <returns>CPF as string</returns>
    public override string ToString() => Value;

    /// <summary>
    /// Return a CPF string representation
    /// </summary>
    /// <param name="withMask">If true, returns CPNJ string with mask (eg. 00.000.000/0000-00)</param>
    /// <returns>CPF as string</returns>
    public string ToString(bool withMask) => Format(Value, withMask);

    /// <inheritdoc />
    public string ToString(
#if NET8_0_OR_GREATER
        [StringSyntax(StringSyntaxAttribute.NumericFormat)]
#endif
        string? format,
        IFormatProvider? formatProvider
    ) =>
        (string.IsNullOrWhiteSpace(format))
            ? Value.ToString(formatProvider)
            : ToNumber().ToString(format, formatProvider);

    /// <summary>
    /// Parse the digits to a numeric value
    /// </summary>
    /// <returns><see cref="long" /> representation of CPF.</returns>
    public long ToNumber() => long.Parse(Value);

    /// <summary>
    /// Convert CPF to string representation without mask
    /// </summary>
    /// <param name="value">A CPF structure</param>
    /// <returns>CPF as string</returns>
    public static implicit operator string(in Cpf value) => value.Value;

    /// <summary>
    /// Convert CPF to ReadOnlySpan representation without mask
    /// </summary>
    /// <param name="value">A CPF structure</param>
    /// <returns>CPF as string</returns>
    public static implicit operator ReadOnlySpan<char>(in Cpf value) => value.Value;

    /// <summary>
    /// Try to parse an string to a valid Cpf structure
    /// </summary>
    /// <param name="value">CPF string</param>
    /// <returns>Cpf structure</returns>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CPF.
    /// </exception>
    public static explicit operator Cpf(in string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (!Validate(value))
            throw CpfException(value);
        return new(value, false);
    }

    /// <summary>
    /// Convert CPF a numeric representation
    /// </summary>
    /// <param name="value">A CPF structure</param>
    /// <returns>CPF as <see cref="long"/></returns>
    public static explicit operator long(in Cpf value) => value.ToNumber();

    /// <summary>
    /// Convert CPF from number
    /// </summary>
    /// <param name="value">A CPF numeric value</param>
    /// <returns>CPF structure</returns>
    public static explicit operator Cpf(in long value) => new(value);

    /// <summary>
    /// Parses a number to Cpf
    /// </summary>
    /// <param name="value">CPF long number</param>
    /// <returns>Cpf structure</returns>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CPF.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Throws a ArgumentNullException if the passed <para name="value" /> is 0.
    /// </exception>
    public static Cpf Parse(long value)
    {
        if (value <= 0)
            throw new ArgumentException("Invalid value argument");

        return !TryParse(value, out var cpf)
            ? throw CpfException(value.ToString(CultureInfo.InvariantCulture))
            : cpf;
    }

    /// <summary>
    /// Parses a string to Cpf
    /// </summary>
    /// <param name="value">CPF string</param>
    /// <returns>Cpf structure</returns>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CPF.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Throws a ArgumentNullException if the passed <para name="value" /> is null.
    /// </exception>
    public static Cpf Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Parse(value.AsSpan());
    }

    /// <summary>
    /// Parses a char span to Cpf
    /// </summary>
    /// <param name="value">CPF string</param>
    /// <returns>Cpf structure</returns>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CPF.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Throws a ArgumentNullException if the passed <para name="value" /> is null.
    /// </exception>
    public static Cpf Parse(ReadOnlySpan<char> value)
    {
        if (value.IsEmptyOrWhiteSpace())
            throw new ArgumentException("Invalid CPF value");

        return !TryParse(value, out var cpf)
            ? throw CpfException(value)
            : cpf;
    }

    /// <summary>
    /// Parses a UTF8 byte span to CPF
    /// </summary>
    /// <param name="value">CPF UTF8 bytes</param>
    /// <returns>CPF structure</returns>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CPF.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Throws a ArgumentNullException if the passed <para name="value" /> is null.
    /// </exception>
    public static Cpf Parse(ReadOnlySpan<byte> value) => Parse(Encoding.UTF8.GetString(value));

    /// <summary>
    /// Converts a numeric representation of a CPF to the equivalent Cpf structure.
    /// </summary>
    /// <param name="value">A <see cref="long"/> containing the CPF value</param>
    /// <param name="result">A Cpf instance to contain the parsed value. If the method returns true, result
    /// contains a valid Cpf. If the method returns false, result equals Empty.
    /// </param>
    /// <returns> true if the parse operation was successful; otherwise, false.</returns>
    public static bool TryParse(long value, out Cpf result)
    {
#if NET8_0_OR_GREATER
        Span<char> stringValue = stackalloc char[DefaultLength];
        if (value.TryFormat(stringValue, out var written))
            return TryParse(stringValue[..written], out result);

        result = Empty;
        return false;

#else
        var stringValue = value.ToString(CultureInfo.InvariantCulture);
        return TryParse(stringValue, out result);
#endif
    }

    /// <summary>
    /// Converts the string representation of a CPF to the equivalent Cpf structure.
    /// </summary>
    /// <param name="value">A string containing the CPF to convert</param>
    /// <param name="result">A Cpf instance to contain the parsed value. If the method returns true, result
    /// contains a valid Cpf. If the method returns false, result equals Empty.
    /// </param>
    /// <returns> true if the parse operation was successful; otherwise, false.</returns>
    public static bool TryParse(string? value, out Cpf result)
    {
        if (!string.IsNullOrWhiteSpace(value))
            return TryParse(value.AsSpan(), out result);

        result = Empty;
        return false;
    }

    /// <summary>
    /// Converts the UTF8 byte span representation of a CPF to the equivalent CPF structure.
    /// </summary>
    /// <param name="value">A UTF8 byte span containing the CPF to convert</param>
    /// <param name="result">A CPF instance to contain the parsed value. If the method returns true, result
    /// contains a valid CPF. If the method returns false, result equals Empty.
    /// </param>
    /// <returns> true if the parse operation was successful; otherwise, false.</returns>
    public static bool TryParse(ReadOnlySpan<byte> value, out Cpf result) =>
        TryParse(Encoding.UTF8.GetString(value), out result);

    /// <summary>
    /// Converts the string representation of a CPF to the equivalent Cpf structure.
    /// </summary>
    /// <param name="value">A string containing the CPF to convert</param>
    /// <param name="result">A Cpf instance to contain the parsed value. If the method returns true, result
    /// contains a valid Cpf. If the method returns false, result equals Empty.
    /// </param>
    /// <returns> true if the parse operation was successful; otherwise, false.</returns>
    public static bool TryParse(ReadOnlySpan<char> value, out Cpf result)
    {
        var normalized = Format(value, withMask: false);
        if (!Validate(normalized))
        {
            result = Empty;
            return false;
        }

        result = new(normalized, false);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(Cpf other) =>
        string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    string DebuggerDisplay() =>
        Value == Empty ? "WARNING: EMPTY CPF!" : $"CPF{{{Format(Value, true)}}}";

    /// <summary>
    /// Validate given Cpf
    /// </summary>
    /// <param name="cpfString">Cpf string representation</param>
    /// <returns> true if the validation was successful; otherwise, false.</returns>
    public static bool Validate(in ReadOnlySpan<char> cpfString)
    {
        if (cpfString.IsEmptyOrWhiteSpace())
            return false;

        var position = 0;
        var (totalDigit1, totalDigit2) = (0, 0);
        var (dv1, dv2) = (0, 0);
        var identicalDigits = true;
        var lastDigit = -1;
        foreach (var c in cpfString)
        {
            if (!char.IsDigit(c)) continue;
            var digit = c - '0';
            if (position is not 0 && lastDigit != digit)
                identicalDigits = false;

            lastDigit = digit;
            switch (position)
            {
                case < 9:
                    totalDigit1 += digit * (DefaultLength - 1 - position);
                    totalDigit2 += digit * (DefaultLength - position);
                    break;
                case 9:
                    dv1 = digit;
                    break;
                case 10:
                    dv2 = digit;
                    break;
            }

            position++;
        }

        if (position is not DefaultLength || identicalDigits)
            return false;

        var digit1 = totalDigit1 % DefaultLength;
        digit1 = digit1 < 2 ? 0 : DefaultLength - digit1;

        if (dv1 != digit1) return false;

        totalDigit2 += digit1 * 2;
        var digit2 = totalDigit2 % DefaultLength;
        digit2 = digit2 < 2 ? 0 : DefaultLength - digit2;

        return dv2 == digit2;
    }

    /// <summary>
    /// Validate given Cpf
    /// </summary>
    /// <param name="cpfString">Cpf string representation</param>
    /// <returns> true if the validation was successful; otherwise, false.</returns>
    public static bool ValidateString(string cpfString) => Validate(cpfString);

    /// <summary>
    /// Format Cpf string.
    /// If <para name="value" /> has size smaller then expected, this function will pad the value with left 0.
    /// </summary>
    /// <param name="value">Cpf string representation</param>
    /// <param name="withMask">if true, returns formatted Cpf with mask (##.###.###/####-##), otherwise clean (##############).</param>
    /// <returns>Formatted CPF string</returns>
    public static string Format(in ReadOnlySpan<char> value, bool withMask = false) =>
        value.FormatToString(DefaultLength, withMask ? Mask : null);

#if NET8_0_OR_GREATER
    bool ISpanFormattable.TryFormat(
        Span<char> destination, out int charsWritten,
        [StringSyntax(StringSyntaxAttribute.NumericFormat)]
        ReadOnlySpan<char> format, IFormatProvider? provider
    )
    {
        charsWritten = 0;
        if (destination.IsEmpty) return false;
        if (!format.IsEmpty)
            return ToNumber().TryFormat(destination, out charsWritten, format, provider);

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
        return !format.IsEmpty
            ? ToNumber().TryFormat(utf8Destination, out bytesWritten, format, provider)
            : Encoding.UTF8.TryGetBytes(Value, utf8Destination, out bytesWritten);
    }

    static Cpf IParsable<Cpf>.Parse(string s, IFormatProvider? provider) => Parse(s);

    static bool IParsable<Cpf>.TryParse(string? s, IFormatProvider? provider, out Cpf result) =>
        TryParse(s, out result);

    static Cpf ISpanParsable<Cpf>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
        Parse(s);

    static bool ISpanParsable<Cpf>.TryParse(
        ReadOnlySpan<char> s, IFormatProvider? provider, out Cpf result) =>
        TryParse(s, out result);

    static Cpf IUtf8SpanParsable<Cpf>.Parse(
        ReadOnlySpan<byte> utf8Text, IFormatProvider? provider) => Parse(utf8Text);

    static bool IUtf8SpanParsable<Cpf>.TryParse(ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider, out Cpf result) => TryParse(utf8Text, out result);
#endif
}
