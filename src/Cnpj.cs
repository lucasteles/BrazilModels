using System;
using System.ComponentModel;
using System.Diagnostics;

namespace BrazilModels;

/// <summary>
/// Brazilian CNPJ number
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(StringSystemTextJsonConverter<Cnpj>))]
[TypeConverter(typeof(StringTypeConverter<Cnpj>))]
[Swashbuckle.AspNetCore.Annotations.SwaggerSchemaFilter(typeof(StringSchemaFilter))]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public readonly record struct Cnpj : IComparable<Cnpj>
{
    const ushort CnpjSize = 14;
    static readonly ushort[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
    static readonly ushort[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

    /// <summary>
    /// Empty invalid CPNJ
    /// </summary>
    public static readonly Cnpj Empty = new();

    /// <summary>
    /// CNPJ string representation
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Construct an Empty CNPJ
    /// </summary>
    public Cnpj() => Value = Value = new string('0', CnpjSize);

    /// <summary>
    /// Construct new CPNJ
    /// </summary>
    /// <param name="value">A valid string CNPJ value</param>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CNPJ.
    /// </exception>
    public Cnpj(in string value) : this(value.AsSpan()) { }

    /// <summary>
    /// Construct a new CPNJ
    /// </summary>
    /// <param name="value">A valid CNPJ as ReadOnlySpan of char</param>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CNPJ.
    /// </exception>
    public Cnpj(in ReadOnlySpan<char> value) : this(value, true) { }

    Cnpj(in ReadOnlySpan<char> value, bool validate)
    {
        Value = Format(value);

        if (validate && !Validate(value))
            throw CnpjException(value);
    }

    /// <summary>
    /// Return a CNPJ string representation without special symbols
    /// </summary>
    /// <returns>CNPJ as string</returns>
    public override string ToString() => Value;

    /// <summary>
    /// Return a CNPJ string representation
    /// </summary>
    /// <param name="withMask">If true, returns CPNJ string with mask (eg. 00.000.000/0000-00)</param>
    /// <returns>CNPJ as string</returns>
    public string ToString(bool withMask) => Format(Value, withMask);

    static Exception CnpjException(in ReadOnlySpan<char> value) => new FormatException($"Invalid CNPJ: {value}");

    /// <summary>
    /// Convert CNPJ to string representation without mask
    /// </summary>
    /// <param name="cnpj">A CNPJ structure</param>
    /// <returns>CNPJ as string</returns>
    public static implicit operator string(in Cnpj cnpj) => cnpj.Value;

    /// <summary>
    /// Convert CNPJ to ReadOnlySpan representation without mask
    /// </summary>
    /// <param name="cnpj">A CNPJ structure</param>
    /// <returns>CNPJ as string</returns>
    public static implicit operator ReadOnlySpan<char>(in Cnpj cnpj) => cnpj.Value;

    /// <summary>
    /// Try to parse an string to a valid Cnpj structure
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
    /// Parses a string to Cnpj
    /// </summary>
    /// <param name="value">CNPJ string</param>
    /// <returns>Cnpj structure</returns>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CNPJ.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Throws a ArgumentNullException if the passed <para name="value" /> is null.
    /// </exception>
    public static Cnpj Parse(in string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return !TryParse(value, out var cnpj)
            ? throw CnpjException(value)
            : cnpj;
    }

    /// <summary>
    /// Converts the string representation of a CNPJ to the equivalent Cnpj structure.
    /// </summary>
    /// <param name="value">A string containing the CNPJ to convert</param>
    /// <param name="result">A Cnpj instance to contain the parsed value. If the method returns true, result
    /// contains a valid Cnpj. If the method returns false, result equals Empty.
    /// </param>
    /// <returns> true if the parse operation was successful; otherwise, false.</returns>
    public static bool TryParse(in string value, out Cnpj result)
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

    /// <inheritdoc />
    public int CompareTo(Cnpj other) => string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase);

#if DEBUG
    string DebuggerDisplay() => Value == Empty ? "WARNING: INVALID CNPJ!" : $"CNPJ{{{Format(Value, true)}}}";
#endif

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

        return position is CnpjSize && !identicalDigits;
    }

    /// <summary>
    /// Format Cpnj string.
    /// If <para name="value" /> has size smaller then expected, this function will pad the value with left 0.
    /// </summary>
    /// <param name="value">Cnpj string representation</param>
    /// <param name="withMask">if true, returns formatted Cnpj with mask (##.###.###/####-##), otherwise clean (##############).</param>
    /// <returns>Formatted CNPJ string</returns>
    public static string Format(in ReadOnlySpan<char> value, bool withMask = false) =>
        withMask
            ? value.FormatMask(CnpjSize, "##.###.###/####-##").ToString()
            : value.FormatClean(CnpjSize).ToString();
}
