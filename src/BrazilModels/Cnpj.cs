using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text;
using BrazilModels.Json;

namespace BrazilModels;

/// <summary>
/// Brazilian CNPJ number
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(StringSystemTextJsonConverter<Cnpj>))]
[TypeConverter(typeof(StringTypeConverter<Cnpj>))]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public readonly record struct Cnpj : IComparable<Cnpj>, IStringValue, IEquatable<CpfCnpj>
#if NET8_0_OR_GREATER
    , ISpanFormattable
    , ISpanParsable<Cnpj>
    , IUtf8SpanFormattable
    , IUtf8SpanParsable<Cnpj>
    , IEqualityOperators<Cnpj, Cnpj, bool>
    , IEqualityOperators<Cnpj, CpfCnpj, bool>
#else
    , IFormattable
#endif
{
    /// <summary>
    /// CNPJ Size
    /// </summary>
    public const byte DefaultLength = 14;

    /// <summary>
    /// CNPJ Mask
    /// </summary>
    public const string Mask = "##.###.###/####-##";

    static readonly byte[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2, };

    static readonly byte[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2, };

    /// <summary>
    /// Empty invalid CNPJ
    /// </summary>
    public static Cnpj Empty { get; } = new();

    /// <summary>
    /// CNPJ string representation
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Construct an Empty CNPJ
    /// </summary>
    public Cnpj() => Value = Value = new string('0', DefaultLength);

    /// <summary>
    /// Construct new CNPJ
    /// </summary>
    /// <param name="value">A valid string CNPJ value</param>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CNPJ.
    /// </exception>
    public Cnpj(in string value) : this(value.AsSpan()) { }

    /// <summary>
    /// Construct a new CNPJ
    /// </summary>
    /// <param name="value">A valid CNPJ as ReadOnlySpan of char</param>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CNPJ.
    /// </exception>
    public Cnpj(in ReadOnlySpan<char> value) : this(value, true) { }

    /// <summary>
    /// Construct new CNPJ
    /// </summary>
    /// <param name="value">A valid numeric CNPJ value</param>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CNPJ.
    /// </exception>
    public Cnpj(in long value) : this(value.ToString(CultureInfo.InvariantCulture)) { }

    Cnpj(in ReadOnlySpan<char> value, bool validate)
    {
        Value = Format(value);

        if (validate && !Validate(value))
            throw CnpjException(value);
    }

    /// <summary>
    /// Returns true if is empty
    /// </summary>
    public bool IsEmpty => Value == Empty.Value;

    /// <inheritdoc />
    public bool Equals(CpfCnpj other) => other.Equals(this);

    /// <summary>
    /// Return a CNPJ string representation without special symbols
    /// </summary>
    /// <returns>CNPJ as string</returns>
    public override string ToString() => Value;

    /// <summary>
    /// Return a CNPJ string representation
    /// </summary>
    /// <param name="withMask">If true, returns CNPJ string with mask (eg. 00.000.000/0000-00)</param>
    /// <returns>CNPJ as string</returns>
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
    /// <returns><see cref="long" /> representation of CNPJ.</returns>
    public long ToNumber() => long.Parse(Value);

    static FormatException CnpjException(in ReadOnlySpan<char> value) =>
        new($"Invalid CNPJ: {value}");

    /// <summary>
    /// Convert CNPJ to string representation without a mask
    /// </summary>
    /// <param name="cnpj">A CNPJ structure</param>
    /// <returns>CNPJ as string</returns>
    public static implicit operator string(in Cnpj cnpj) => cnpj.Value;

    /// <summary>
    /// Convert CNPJ to ReadOnlySpan representation without a mask
    /// </summary>
    /// <param name="cnpj">A CNPJ structure</param>
    /// <returns>CNPJ as string</returns>
    public static implicit operator ReadOnlySpan<char>(in Cnpj cnpj) => cnpj.Value;

    /// <summary>
    /// Try to parse a string to a valid Cnpj structure
    /// </summary>
    /// <param name="value">CNPJ string</param>
    /// <returns>Cnpj structure</returns>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CNPJ.
    /// </exception>
    public static explicit operator Cnpj(in string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (!Validate(value))
            throw CnpjException(value);
        return new(value, false);
    }

    /// <summary>
    /// Convert CNPJ a numeric representation
    /// </summary>
    /// <param name="value">A CNPJ structure</param>
    /// <returns>CNPJ as <see cref="long"/></returns>
    public static explicit operator long(in Cnpj value) => value.ToNumber();

    /// <summary>
    /// Convert CNPJ from number
    /// </summary>
    /// <param name="value">A CNPJ numeric value</param>
    /// <returns>CNPJ structure</returns>
    public static explicit operator Cnpj(in long value) => new(value);

    /// <summary>
    /// Parses a string to Cnpj
    /// </summary>
    /// <param name="value">CNPJ string</param>
    /// <returns>Cnpj structure</returns>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CNPJ.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Throws an ArgumentNullException if the passed <para name="value" /> is null.
    /// </exception>
    public static Cnpj Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Parse(value.AsSpan());
    }

    /// <summary>
    /// Parses a char span to Cnpj
    /// </summary>
    /// <param name="value">CNPJ string</param>
    /// <returns>Cnpj structure</returns>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CNPJ.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Throws an ArgumentNullException if the passed <para name="value" /> is null.
    /// </exception>
    public static Cnpj Parse(ReadOnlySpan<char> value) =>
        !TryParse(value, out var cnpj)
            ? throw CnpjException(value)
            : cnpj;

    /// <summary>
    /// Parses a UTF8 byte span to Cnpj
    /// </summary>
    /// <param name="value">CNPJ UTF8 bytes</param>
    /// <returns>Cnpj structure</returns>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CNPJ.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Throws an ArgumentNullException if the passed <para name="value" /> is null.
    /// </exception>
    public static Cnpj Parse(ReadOnlySpan<byte> value) => Parse(Encoding.UTF8.GetString(value));

    /// <summary>
    /// Parses a number to Cnpj
    /// </summary>
    /// <param name="value">CNPJ long number</param>
    /// <returns>Cnpj structure</returns>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CNPJ.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Throws an ArgumentNullException if the passed <para name="value" /> is 0.
    /// </exception>
    public static Cnpj Parse(long value)
    {
        if (value <= 0)
            throw new ArgumentException("Invalid value argument");

        return !TryParse(value, out var cnpj)
            ? throw CnpjException(value.ToString(CultureInfo.InvariantCulture))
            : cnpj;
    }

    /// <summary>
    /// Converts the char span representation of a CNPJ to the equivalent Cnpj structure.
    /// </summary>
    /// <param name="value">A string containing the CNPJ to convert</param>
    /// <param name="result">A Cnpj instance to contain the parsed value. If the method returns true, result
    /// contains a valid Cnpj. If the method returns false, result equals Empty.
    /// </param>
    /// <returns> true if the parse operation was successful; otherwise, false.</returns>
    public static bool TryParse(ReadOnlySpan<char> value, out Cnpj result)
    {
        var normalized = Format(value);
        if (!Validate(normalized))
        {
            result = Empty;
            return false;
        }

        result = new(normalized, false);
        return true;
    }

    /// <summary>
    /// Converts the UTF8 byte span representation of a CNPJ to the equivalent Cnpj structure.
    /// </summary>
    /// <param name="value">A UTF8 byte span containing the CNPJ to convert</param>
    /// <param name="result">A Cnpj instance to contain the parsed value. If the method returns true, result
    /// contains a valid Cnpj. If the method returns false, result equals Empty.
    /// </param>
    /// <returns> true if the parse operation was successful; otherwise, false.</returns>
    public static bool TryParse(ReadOnlySpan<byte> value, out Cnpj result) =>
        TryParse(Encoding.UTF8.GetString(value).AsSpan(), out result);

    /// <summary>
    /// Converts the string representation of a CNPJ to the equivalent Cnpj structure.
    /// </summary>
    /// <param name="value">A string containing the CNPJ to convert</param>
    /// <param name="result">A Cnpj instance to contain the parsed value. If the method returns true, result
    /// contains a valid Cnpj. If the method returns false, result equals Empty.
    /// </param>
    /// <returns> true if the parse operation was successful; otherwise, false.</returns>
    public static bool TryParse(string? value, out Cnpj result)
    {
        if (!string.IsNullOrWhiteSpace(value))
            return TryParse(value.AsSpan(), out result);

        result = Empty;
        return false;
    }

    /// <summary>
    /// Converts a numeric representation of a CNPJ to the equivalent Cnpj structure.
    /// </summary>
    /// <param name="value">A <see cref="long"/> containing the CNPJ value</param>
    /// <param name="result">A Cnpj instance to contain the parsed value. If the method returns true, result
    /// contains a valid Cnpj. If the method returns false, result equals Empty.
    /// </param>
    /// <returns> true if the parse operation was successful; otherwise, false.</returns>
    public static bool TryParse(long value, out Cnpj result)
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

    /// <inheritdoc />
    public int CompareTo(Cnpj other) =>
        string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    string DebuggerDisplay() =>
        Value == Empty ? "WARNING: INVALID CNPJ!" : $"CNPJ{{{Format(Value, true)}}}";

    /// <summary>
    /// Validate given Cnpj
    /// </summary>
    /// <param name="cnpjString">Cnpj string representation</param>
    /// <returns> true if the validation was successful; otherwise, false.</returns>
    public static bool Validate(in ReadOnlySpan<char> cnpjString)
    {
        if (cnpjString.IsEmptyOrWhiteSpace())
            return false;

        var position = 0;
        var (totalDigit1, totalDigit2) = (0, 0);
        var identicalDigits = true;
        var lastDigit = -1;

        foreach (var c in cnpjString)
        {
            if (!char.IsDigit(c))
                continue;

            var digit = c - '0';
            if (position is not 0 && lastDigit != digit)
                identicalDigits = false;

            lastDigit = digit;
            switch (position)
            {
                case < 12:
                    totalDigit1 += digit * multiplier1[position];
                    totalDigit2 += digit * multiplier2[position];
                    break;
                case 12:
                    {
                        var dv1 = (totalDigit1 % 11);
                        dv1 = dv1 < 2 ? 0 : 11 - dv1;
                        if (digit != dv1) return false;
                        totalDigit2 += dv1 * multiplier2[12];
                        break;
                    }
                case 13:
                    {
                        var dv2 = (totalDigit2 % 11);
                        dv2 = dv2 < 2 ? 0 : 11 - dv2;
                        if (digit != dv2) return false;
                        break;
                    }
            }

            position++;
        }

        return position is DefaultLength && !identicalDigits;
    }

    /// <summary>
    /// Validate given Cnpj
    /// </summary>
    /// <param name="cnpjString">Cnpj string representation</param>
    /// <returns> true if the validation was successful; otherwise, false.</returns>
    public static bool ValidateString(string cnpjString) => Validate(cnpjString);

    /// <summary>
    /// Format Cnpj string.
    /// If given <para name="value" /> is smaller than expected, this function will pad the value with left 0.
    /// </summary>
    /// <param name="value">Cnpj string representation</param>
    /// <param name="withMask">if true, returns formatted Cnpj with mask (##.###.###/####-##), otherwise clean (##############).</param>
    /// <returns>Formatted CNPJ string</returns>
    public static string Format(in ReadOnlySpan<char> value, bool withMask = false) =>
        value.FormatToString(DefaultLength, withMask ? Mask : null);

    /// <inheritdoc />
    public static bool operator ==(Cnpj left, CpfCnpj right) => right == left;

    /// <inheritdoc />
    public static bool operator !=(Cnpj left, CpfCnpj right) => right != left;

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

    static Cnpj IParsable<Cnpj>.Parse(string s, IFormatProvider? provider) => Parse(s);

    static bool IParsable<Cnpj>.TryParse(string? s, IFormatProvider? provider, out Cnpj result) =>
        TryParse(s, out result);

    static Cnpj ISpanParsable<Cnpj>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
        Parse(s);

    static bool ISpanParsable<Cnpj>.TryParse(
        ReadOnlySpan<char> s, IFormatProvider? provider, out Cnpj result) =>
        TryParse(s, out result);

    static Cnpj IUtf8SpanParsable<Cnpj>.Parse(
        ReadOnlySpan<byte> utf8Text, IFormatProvider? provider) => Parse(utf8Text);

    static bool IUtf8SpanParsable<Cnpj>.TryParse(ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider, out Cnpj result) => TryParse(utf8Text, out result);

    static int IStringValue.ValueSize => DefaultLength;
#endif
}
