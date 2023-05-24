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
[Swashbuckle.AspNetCore.Annotations.SwaggerSchemaFilter(typeof(StringSchemaFilter))]
public readonly record struct PhoneNumber : IComparable<PhoneNumber>, IFormattable
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
        this.Value = Format(phoneNumber.ToLowerInvariant());
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <summary>
    /// Get phoneNumber instance of an Value string
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
    /// Try parse an Value string to an phoneNumber instance
    /// </summary>
    public static bool TryParse(string? value, out PhoneNumber phoneNumber)
    {
        phoneNumber = default;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        phoneNumber = new(value);
        return true;
    }

    /// <summary>
    /// Parse an Value string to an phoneNumber instance
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
        if (value.IsEmptyOrWhiteSpace())
            return string.Empty;

        var clean = value.RemoveNonDigits();
        if (value.StartsWith("+"))
            return "+" + clean.ToString();

        return clean.ToString();
    }
}
