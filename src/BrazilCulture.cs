using System;
using System.Globalization;

namespace BrazilModels;

/// <summary>
/// Lightweight brazilian culture helpers
/// </summary>
public static class BrazilCulture
{
    /// <summary>
    /// NumberFormatInfo using ',' for decimal separators and '.' for group separators
    /// </summary>
    /// {}
    public static readonly NumberFormatInfo NumberFormat =
        new()
        {
            CurrencyDecimalDigits = 2,
            CurrencyDecimalSeparator = ",",
            CurrencyGroupSeparator = ".",
            CurrencyNegativePattern = 9,
            CurrencyPositivePattern = 2,
            CurrencySymbol = "R$",
            NaNSymbol = "NaN",
            NegativeInfinitySymbol = "-\u221E",
            NegativeSign = "-",
            NumberDecimalDigits = 3,
            NumberDecimalSeparator = ",",
            NumberGroupSeparator = ".",
            NumberNegativePattern = 1,
            PerMilleSymbol = "\u2030",
            PercentDecimalDigits = 3,
            PercentDecimalSeparator = ",",
            PercentGroupSeparator = ".",
            PercentNegativePattern = 1,
            PercentPositivePattern = 1,
            PercentSymbol = "%",
            PositiveInfinitySymbol = "\u221E",
            PositiveSign = "\u002B",
            NativeDigits = new[]
            {
                "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
            },
            DigitSubstitution = DigitShapes.None,
            CurrencyGroupSizes = new[]
            {
                3,
            },
            NumberGroupSizes = new[]
            {
                3,
            },
            PercentGroupSizes = new[]
            {
                3,
            },
        };

    /// <summary>
    ///  Format Dates in pt-BR
    /// </summary>
    public static readonly DateTimeFormatInfo DateTimeFormat =
        new()
        {
            AMDesignator = "AM",
            PMDesignator = "PM",
            DateSeparator = "/",
            TimeSeparator = ":",
            Calendar = new GregorianCalendar(),
            CalendarWeekRule = CalendarWeekRule.FirstDay,
            FirstDayOfWeek = DayOfWeek.Sunday,
            FullDateTimePattern = "dddd, d 'de' MMMM 'de' yyyy HH:mm:ss",
            MonthDayPattern = "d 'de' MMMM",
            YearMonthPattern = "MMMM 'de' yyyy",
            LongDatePattern = "dddd, d 'de' MMMM 'de' yyyy",
            LongTimePattern = "HH:mm:ss",
            ShortDatePattern = "dd/MM/yyyy",
            ShortTimePattern = "HH:mm",
            AbbreviatedDayNames = new[]
            {
                "dom.", "seg.", "ter.", "qua.", "qui.", "sex.", "sáb.",
            },
            AbbreviatedMonthGenitiveNames = new[]
            {
                "jan.",
                "fev.",
                "mar.",
                "abr.",
                "mai.",
                "jun.",
                "jul.",
                "ago.",
                "set.",
                "out.",
                "nov.",
                "dez.",
                "",
            },
            AbbreviatedMonthNames = new[]
            {
                "jan.",
                "fev.",
                "mar.",
                "abr.",
                "mai.",
                "jun.",
                "jul.",
                "ago.",
                "set.",
                "out.",
                "nov.",
                "dez.",
                "",
            },
            DayNames = new[]
            {
                "domingo",
                "segunda-feira",
                "terça-feira",
                "quarta-feira",
                "quinta-feira",
                "sexta-feira",
                "sábado",
            },
            MonthGenitiveNames = new[]
            {
                "janeiro",
                "fevereiro",
                "março",
                "abril",
                "maio",
                "junho",
                "julho",
                "agosto",
                "setembro",
                "outubro",
                "novembro",
                "dezembro",
                "",
            },
            MonthNames = new[]
            {
                "janeiro",
                "fevereiro",
                "março",
                "abril",
                "maio",
                "junho",
                "julho",
                "agosto",
                "setembro",
                "outubro",
                "novembro",
                "dezembro",
                "",
            },
            ShortestDayNames = new[]
            {
                "D", "S", "T", "Q", "Q", "S", "S",
            },
        };

    /// <summary>
    /// Lightweight brazil culture info
    /// </summary>
    public static readonly CultureInfo CultureInfo = new BrazilCultureInfo();

    sealed class BrazilCultureInfo : CultureInfo
    {
        public BrazilCultureInfo() : base(string.Empty)
        {
            this.NumberFormat = BrazilCulture.NumberFormat;
            this.DateTimeFormat = BrazilCulture.DateTimeFormat;
        }

        public override string Name { get; } = "pt-BR";
        public override string EnglishName { get; } = "Portuguese";
        public override string DisplayName { get; } = "Portuguese (Brazil)";
        public override string NativeName { get; } = "português (Brasil)";
        public override string TwoLetterISOLanguageName { get; } = "pt";
        public override string ThreeLetterISOLanguageName { get; } = "por";
        public override string ThreeLetterWindowsLanguageName { get; } = "PTB";
        public override CultureInfo Parent { get; } = InvariantCulture;
    }
}
