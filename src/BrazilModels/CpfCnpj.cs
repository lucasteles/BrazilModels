using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using BrazilModels.Json;

namespace BrazilModels;

/// <summary>
/// Brazilian CNPJ or CPF
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(StringSystemTextJsonConverter<CpfCnpj>))]
[TypeConverter(typeof(StringTypeConverter<CpfCnpj>))]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public readonly record struct CpfCnpj : IComparable<CpfCnpj>, IFormattable
{
    /// <summary>
    /// Defines if the Document is an CPF or CNPJ
    /// </summary>
    public DocumentType Type { get; }

    /// <summary>
    /// CPF/CNPJ string representation
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Empty invalid CpfCnpj
    /// </summary>
    public static readonly CpfCnpj Empty = new(string.Empty, 0);

    /// <summary>
    /// Construct an Empty CPF/CNPJ
    /// </summary>
    public CpfCnpj()
    {
        Value = string.Empty;
        Type = 0;
    }

    /// <summary>
    /// Construct new CpfCnpj
    /// </summary>
    /// <param name="value">A valid string CpfCnpj value</param>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CPF/CNPJ.
    /// </exception>
    public CpfCnpj(in string value) : this(value.AsSpan()) { }

    /// <summary>
    /// Construct new CpfCnpj
    /// </summary>
    /// <param name="value">A valid string CpfCnpj numeric value</param>
    /// <param name="type">Document type</param>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CPF/CNPJ.
    /// </exception>
    public CpfCnpj(in long value, DocumentType type) : this(
        value.ToString(CultureInfo.InvariantCulture).PadLeft(type.GetSize(), '0'))
    {
    }

    /// <summary>
    /// Construct a new CpfCnpj
    /// </summary>
    /// <param name="value">A valid CPF/CNPJ as ReadOnlySpan of char</param>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CPF/CNPJ.
    /// </exception>
    public CpfCnpj(in ReadOnlySpan<char> value)
    {
        var inferred = Validate(value);
        if (inferred is null)
            throw CpfCnpjException(value);

        Type = inferred.Value;
        Value = Format(value, Type);
    }

    CpfCnpj(in ReadOnlySpan<char> value, DocumentType type)
    {
        Type = type;
        Value = Format(value, type);
    }

    /// <summary>
    /// Return a CPF/CNPJ string representation without special symbols
    /// </summary>
    /// <returns>CPF/CNPJ as string</returns>
    public override string ToString() => Value;

    /// <summary>
    /// Return a CPF/CNPJ string representation
    /// </summary>
    /// <param name="withMask">If true, returns CpfCnpj string with mask (eg. 00.000.000/0000-00)</param>
    /// <returns>CPF/CNPJ as string</returns>
    public string ToString(bool withMask) => Format(Value, Type, withMask);

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

    static FormatException CpfCnpjException(in ReadOnlySpan<char> value) =>
        new($"Invalid CpfCnpj: {value}");

    /// <summary>
    /// Convert document number to string representation without mask
    /// </summary>
    /// <param name="cpfCnpj">A CPF/CNPJ structure</param>
    /// <returns>The CPF/CNPJ as string</returns>
    public static implicit operator string(in CpfCnpj cpfCnpj) => cpfCnpj.Value;

    /// <summary>
    /// Convert Cnpj to CpfCnpj representation without mask
    /// </summary>
    /// <param name="cnpj">A CPF/CNPJ structure</param>
    /// <returns>The CPF/CNPJ as string</returns>
    public static implicit operator CpfCnpj(in Cnpj cnpj) => new(cnpj.Value);


    /// <summary>
    /// Convert Cpf to CpfCnpj representation without mask
    /// </summary>
    /// <param name="cpf">A CPF/CNPJ structure</param>
    /// <returns>CPF/CNPJ as string</returns>
    public static implicit operator CpfCnpj(in Cpf cpf) => new(cpf.Value);


    /// <summary>
    /// Convert CPF/CNPJ to ReadOnlySpan representation without mask
    /// </summary>
    /// <param name="cpfCnpj">A CPF/CNPJ structure</param>
    /// <returns>CPF/CNPJ as string</returns>
    public static implicit operator ReadOnlySpan<char>(in CpfCnpj cpfCnpj) => cpfCnpj.Value;

    /// <summary>
    /// Try to parse an string to a valid CpfCnpj structure
    /// </summary>
    /// <param name="value">CPF/CNPJ string</param>
    /// <returns>CpfCnpj structure</returns>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CPF/CNPJ.
    /// </exception>
    public static explicit operator CpfCnpj(in string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(value);
    }

    /// <summary>
    /// Convert CpfCnpj a numeric representation
    /// </summary>
    /// <param name="value">A CpfCnpj structure</param>
    /// <returns>CpfCnpj as <see cref="long"/></returns>
    public static explicit operator long(in CpfCnpj value) => value.ToNumber();

    /// <summary>
    /// Parses a string to CpfCnpj
    /// </summary>
    /// <param name="value">CPF/CNPJ string</param>
    /// <returns>CpfCnpj structure</returns>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid CPF/CNPJ.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Throws a ArgumentNullException if the passed <para name="value" /> is null.
    /// </exception>
    public static CpfCnpj Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return !TryParse(value, out var cpfCnpj)
            ? throw CpfCnpjException(value)
            : cpfCnpj;
    }

    /// <summary>
    /// Converts the string representation of a brazilian document to the equivalent CpfCnpj structure.
    /// </summary>
    /// <param name="value">A string containing the CPF/CNPJ to convert</param>
    /// <param name="result">A CpfCnpj instance to contain the parsed value. If the method returns true, result
    /// contains a valid CpfCnpj. If the method returns false, result equals Empty.
    /// </param>
    /// <returns> true if the parse operation was successful; otherwise, false.</returns>
    public static bool TryParse(ReadOnlySpan<char> value, out CpfCnpj result)
    {
        var type = Validate(value);
        if (type is null)
        {
            result = Empty;
            return false;
        }

        result = new(value, type.Value);
        return true;
    }

    /// <summary>
    /// Converts the string representation of a brazilian document to the equivalent CpfCnpj structure.
    /// </summary>
    /// <param name="value">A string containing the CPF/CNPJ to convert</param>
    /// <param name="result">A CpfCnpj instance to contain the parsed value. If the method returns true, result
    /// contains a valid CpfCnpj. If the method returns false, result equals Empty.
    /// </param>
    /// <returns> true if the parse operation was successful; otherwise, false.</returns>
    public static bool TryParse(string? value, out CpfCnpj result)
    {
        if (!string.IsNullOrWhiteSpace(value))
            return TryParse(value.AsSpan(), out result);

        result = Empty;
        return false;
    }

    /// <inheritdoc />
    public int CompareTo(CpfCnpj other) =>
        string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    string DebuggerDisplay() => Value == Empty
        ? "WARNING: INVALID CPF/CNPJ!"
        : $"[{Type.ToString().ToUpperInvariant()}]{{{Format(Value, Type, true)}}}";

    /// <summary>
    /// Validate given document number
    /// </summary>
    /// <param name="cpfOrCnpj">Cpf/Cnpj string representation</param>
    /// <returns> true if the validation was successful; otherwise, false.</returns>
    public static DocumentType? Validate(in ReadOnlySpan<char> cpfOrCnpj)
    {
        Span<char> cleared = stackalloc char[cpfOrCnpj.Length];
        cpfOrCnpj.RemoveNonDigits(cleared, out var clearedSize);
        cleared = cleared[..clearedSize];

        return cleared.Length switch
        {
            Cnpj.DefaultLength when Cnpj.Validate(cleared) => DocumentType.CNPJ,
            Cpf.DefaultLength when Cpf.Validate(cleared) => DocumentType.CPF,
            _ => null,
        };
    }

    /// <summary>
    /// Validate given document number
    /// </summary>
    /// <param name="cpfOrCnpj">CPF/CNPJ string representation</param>
    /// <returns> true if the validation was successful; otherwise, false.</returns>
    public static DocumentType? ValidateString(string cpfOrCnpj) => Validate(cpfOrCnpj);

    /// <summary>
    /// Format document string.
    /// If <para name="value" /> has size smaller then expected, this function will pad the value with left 0.
    /// </summary>
    /// <param name="value">Document string representation of CPF/CNPJ</param>
    /// <param name="withMask">if true, returns formatted document with mask</param>
    /// <returns>Formatted CPF/CNPJ string</returns>
    public static string Format(in ReadOnlySpan<char> value, bool withMask = false) =>
        Format(value, Validate(value), withMask);

    /// <summary>
    /// Format document string.
    /// If <para name="value" /> has size smaller then expected, this function will pad the value with left 0.
    /// </summary>
    /// <param name="value">Document string representation of CPF/CNPJ</param>
    /// <param name="type">Document Type</param>
    /// <param name="withMask">if true, returns formatted document number with mask</param>
    /// <returns>Formatted CPF/CNPJ string</returns>
    public static string Format(in ReadOnlySpan<char> value, DocumentType? type,
        bool withMask = false) =>
        type switch
        {
            DocumentType.CNPJ => Cnpj.Format(value, withMask),
            DocumentType.CPF => Cpf.Format(value, withMask),
            _ => string.Empty,
        };
}
