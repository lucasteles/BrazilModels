using System;
using System.ComponentModel;
using System.Diagnostics;

namespace BrazilModels;

/// <summary>
/// Brazilian CPF number
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(StringSystemTextJsonConverter<Cpf>))]
[TypeConverter(typeof(StringTypeConverter<Cpf>))]
[Swashbuckle.AspNetCore.Annotations.SwaggerSchemaFilter(typeof(StringSchemaFilter))]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public readonly record struct Cpf : IComparable<Cpf>
{
    const int CpfSize = 11;

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
    public Cpf() => Value = new string('0', CpfSize);

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

    static Exception CpfException(in ReadOnlySpan<char> value) => new FormatException($"Invalid CPF: {value}");

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
    public static Cpf Parse(in string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return !TryParse(value, out var cpf)
            ? throw CpfException(value)
            : cpf;
    }

    /// <summary>
    /// Converts the string representation of a CPF to the equivalent Cpf structure.
    /// </summary>
    /// <param name="value">A string containing the CPF to convert</param>
    /// <param name="result">A Cpf instance to contain the parsed value. If the method returns true, result
    /// contains a valid Cpf. If the method returns false, result equals Empty.
    /// </param>
    /// <returns> true if the parse operation was successful; otherwise, false.</returns>
    public static bool TryParse(in string value, out Cpf result)
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
    public int CompareTo(Cpf other) => string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase);

#if DEBUG
    string DebuggerDisplay() => Value == Empty ? "WARNING: EMPTY CPF!" : $"CPF{{{Format(Value, true)}}}";
#endif

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
                    totalDigit1 += digit * (CpfSize - 1 - position);
                    totalDigit2 += digit * (CpfSize - position);
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

        if (position is not CpfSize || identicalDigits)
            return false;

        var digit1 = totalDigit1 % CpfSize;
        digit1 = digit1 < 2 ? 0 : CpfSize - digit1;

        if (dv1 != digit1) return false;

        totalDigit2 += digit1 * 2;
        var digit2 = totalDigit2 % CpfSize;
        digit2 = digit2 < 2 ? 0 : CpfSize - digit2;

        return dv2 == digit2;
    }

    /// <summary>
    /// Format Cpnj string.
    /// If <para name="value" /> has size smaller then expected, this function will pad the value with left 0.
    /// </summary>
    /// <param name="value">Cpf string representation</param>
    /// <param name="withMask">if true, returns formatted Cpf with mask (##.###.###/####-##), otherwise clean (##############).</param>
    /// <returns>Formatted CPF string</returns>
    public static string Format(in ReadOnlySpan<char> value, bool withMask = false) =>
        withMask
            ? value.FormatMask(CpfSize, "###.###.###-##").ToString()
            : value.FormatClean(CpfSize).ToString();
}
