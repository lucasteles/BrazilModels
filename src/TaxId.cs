using System;
using System.ComponentModel;
using System.Diagnostics;

namespace BrazilModels;

/// <summary>
/// Defines a generic TaxId type
/// </summary>
public enum TaxIdType
{
    /// <summary>
    /// Represent an CNPJ TaxId
    /// </summary>
    CNPJ = 1,

    /// <summary>
    /// Represent an CPF TaxId
    /// </summary>
    CPF
}

/// <summary>
/// Brazilian CNPJ or CPF
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(StringSystemTextJsonConverter<TaxId>))]
[TypeConverter(typeof(StringTypeConverter<TaxId>))]
[Swashbuckle.AspNetCore.Annotations.SwaggerSchemaFilter(typeof(StringSchemaFilter))]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public readonly record struct TaxId : IComparable<TaxId>
{
    /// <summary>
    /// Defines if the TaxId is an CPF or CNPJ
    /// </summary>
    public TaxIdType Type { get; }

    /// <summary>
    /// Tax Id string representation
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Empty invalid TaxId
    /// </summary>
    public static readonly TaxId Empty = new(string.Empty, 0);

    /// <summary>
    /// Construct an Empty Tax Id
    /// </summary>
    public TaxId()
    {
        Value = string.Empty;
        Type = 0;
    }

    /// <summary>
    /// Construct new TaxId
    /// </summary>
    /// <param name="value">A valid string TaxId value</param>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid Tax Id.
    /// </exception>
    public TaxId(in string value) : this(value.AsSpan()) { }

    /// <summary>
    /// Construct a new TaxId
    /// </summary>
    /// <param name="value">A valid Tax Id as ReadOnlySpan of char</param>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid Tax Id.
    /// </exception>
    public TaxId(in ReadOnlySpan<char> value)
    {
        var inferred = Validate(value);
        if (inferred is null)
            throw TaxIdException(value);

        Type = inferred.Value;
        Value = Format(value, Type);
    }

    TaxId(in ReadOnlySpan<char> value, TaxIdType type)
    {
        Type = type;
        Value = Format(value, type);
    }

    /// <summary>
    /// Return a Tax Id string representation without special symbols
    /// </summary>
    /// <returns>Tax Id as string</returns>
    public override string ToString() => Value;

    /// <summary>
    /// Return a Tax Id string representation
    /// </summary>
    /// <param name="withMask">If true, returns TaxId string with mask (eg. 00.000.000/0000-00)</param>
    /// <returns>Tax Id as string</returns>
    public string ToString(bool withMask) => Format(Value, Type, withMask);

    static Exception TaxIdException(in ReadOnlySpan<char> value) => new FormatException($"Invalid TaxId: {value}");

    /// <summary>
    /// Convert Tax Id to string representation without mask
    /// </summary>
    /// <param name="taxId">A Tax Id structure</param>
    /// <returns>Tax Id as string</returns>
    public static implicit operator string(in TaxId taxId) => taxId.Value;

    /// <summary>
    /// Convert Tax Id to ReadOnlySpan representation without mask
    /// </summary>
    /// <param name="taxId">A Tax Id structure</param>
    /// <returns>Tax Id as string</returns>
    public static implicit operator ReadOnlySpan<char>(in TaxId taxId) => taxId.Value;

    /// <summary>
    /// Try to parse an string to a valid TaxId structure
    /// </summary>
    /// <param name="value">Tax Id string</param>
    /// <returns>TaxId structure</returns>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid Tax Id.
    /// </exception>
    public static explicit operator TaxId(in string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(value);
    }

    /// <summary>
    /// Parses a string to TaxId
    /// </summary>
    /// <param name="value">Tax Id string</param>
    /// <returns>TaxId structure</returns>
    /// <exception cref="FormatException">
    /// Throws a FormatException if the passed <para name="value" /> is not a valid Tax Id.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Throws a ArgumentNullException if the passed <para name="value" /> is null.
    /// </exception>
    public static TaxId Parse(in string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new TaxId(value);
    }

    /// <summary>
    /// Converts the string representation of a Tax Id to the equivalent TaxId structure.
    /// </summary>
    /// <param name="value">A string containing the Tax Id to convert</param>
    /// <param name="result">A TaxId instance to contain the parsed value. If the method returns true, result
    /// contains a valid TaxId. If the method returns false, result equals Empty.
    /// </param>
    /// <returns> true if the parse operation was successful; otherwise, false.</returns>
    public static bool TryParse(in string value, out TaxId result)
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

    /// <inheritdoc />
    public int CompareTo(TaxId other) => string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase);

#if DEBUG
    string DebuggerDisplay() => Value == Empty ? "WARNING: INVALID TAX ID!" : $"Tax Id{{{Format(Value, Type, true)}}}";
#endif

    /// <summary>
    /// Validate given TaxId
    /// </summary>
    /// <param name="taxId">TaxId string representation</param>
    /// <returns> true if the validation was successful; otherwise, false.</returns>
    public static TaxIdType? Validate(in ReadOnlySpan<char> taxId)
    {
        var cleared = taxId.ClearString();
        return cleared.Length switch
        {
            Cnpj.DefaultLength when Cnpj.Validate(cleared) => TaxIdType.CNPJ,
            Cpf.DefaultLength when Cpf.Validate(cleared) => TaxIdType.CPF,
            _ => null
        };
    }

    /// <summary>
    /// Format TaxId string.
    /// If <para name="value" /> has size smaller then expected, this function will pad the value with left 0.
    /// </summary>
    /// <param name="value">TaxId string representation</param>
    /// <param name="withMask">if true, returns formatted TaxId with mask</param>
    /// <returns>Formatted Tax Id string</returns>
    public static string Format(in ReadOnlySpan<char> value, bool withMask = false) =>
        Format(value, Validate(value), withMask);

    /// <summary>
    /// Format TaxId string.
    /// If <para name="value" /> has size smaller then expected, this function will pad the value with left 0.
    /// </summary>
    /// <param name="value">TaxId string representation</param>
    /// <param name="type">TaxId Type</param>
    /// <param name="withMask">if true, returns formatted TaxId with mask</param>
    /// <returns>Formatted Tax Id string</returns>
    public static string Format(in ReadOnlySpan<char> value, TaxIdType? type, bool withMask = false) =>
        type switch
        {
            TaxIdType.CNPJ => Cnpj.Format(value, withMask),
            TaxIdType.CPF => Cpf.Format(value, withMask),
            _ => string.Empty
        };
}
