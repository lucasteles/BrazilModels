using System.ComponentModel;
using System.Globalization;

namespace BrazilModels.Json;

sealed class StringTypeConverter<T> : TypeConverter where T : struct
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture,
        object value)
    {
        if (value is string)
            return Activator.CreateInstance(typeof(T), value);

        return base.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) =>
        destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture,
        object? value,
        Type destinationType) =>
        value is T myValue && (destinationType == typeof(string))
            ? myValue.ToString()
            : base.ConvertTo(context, culture, value, destinationType);
}
