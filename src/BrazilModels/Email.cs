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
public readonly record struct Email : IComparable<Email>, IFormattable
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

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <inheritdoc />
    string IFormattable.ToString(string? format, IFormatProvider? formatProvider) =>
        Value.ToString(formatProvider);

    /// <summary>
    /// Get Email instance of an Value string
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
    /// Try parse an Value string to an Email instance
    /// </summary>
    public static bool TryParse(string? value, out Email email)
    {
        email = default;
        if (value is null || !IsValid(value))
            return false;

        email = new(value);
        return true;
    }

    /// <summary>
    /// Parse an Value string to an Email instance
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static Email Parse(string value) =>
        TryParse(value, out var valid)
            ? valid
            : throw new InvalidOperationException($"Invalid E-mail {value}");

    /// <summary>
    /// Validate Email string
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsValid(string? value)
    {
        const string at = "@";
        if (value is null)
            return false;

        var index = value.IndexOf(at, StringComparison.OrdinalIgnoreCase);

        return index > 0 &&
               index != value.Length - 1 &&
               index == value.LastIndexOf(at, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public int CompareTo(Email other) =>
        string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    string DebuggerDisplay() => $"EMAIL{{{Value}}}";
}
