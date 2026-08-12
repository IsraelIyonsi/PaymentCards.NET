using System.Globalization;

namespace PaymentCards;

/// <summary>
/// The month and year a payment card expires, as printed on the card in "MM/YY" form.
/// </summary>
public readonly record struct CardExpiry
{
    private const char SlashSeparator = '/';
    private const char DashSeparator = '-';
    private const int MinimumMonth = 1;
    private const int MaximumMonth = 12;
    private const int TwoDigitYearLength = 2;
    private const int FourDigitYearLength = 4;
    private const int TwoDigitYearCentury = 2000;
    private const int RequiredMonthLength = 2;

    /// <summary>
    /// Initializes a new <see cref="CardExpiry"/>.
    /// </summary>
    /// <param name="month">The expiry month, from 1 (January) to 12 (December).</param>
    /// <param name="year">The expiry year, as a four-digit calendar year.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="month"/> is not between 1 and 12.
    /// </exception>
    public CardExpiry(int month, int year)
    {
        if (month is < MinimumMonth or > MaximumMonth)
        {
            throw new ArgumentOutOfRangeException(
                nameof(month),
                month,
                $"Month must be between {MinimumMonth} and {MaximumMonth}.");
        }

        Month = month;
        Year = year;
    }

    /// <summary>
    /// Gets the expiry month, from 1 (January) to 12 (December).
    /// </summary>
    public int Month { get; }

    /// <summary>
    /// Gets the expiry year, as a four-digit calendar year.
    /// </summary>
    public int Year { get; }

    /// <summary>
    /// Gets the last calendar date on which a card with this expiry is valid: the final day of
    /// <see cref="Month"/> in <see cref="Year"/>.
    /// </summary>
    public DateOnly LastValidDate => new(Year, Month, DateTime.DaysInMonth(Year, Month));

    /// <summary>
    /// Parses an expiry in "MM/YY", "MM/YYYY", "MM-YY", or "MM-YYYY" form.
    /// </summary>
    /// <param name="value">The text to parse.</param>
    /// <returns>The parsed <see cref="CardExpiry"/>.</returns>
    /// <exception cref="FormatException">
    /// <paramref name="value"/> is not a recognized expiry format.
    /// </exception>
    public static CardExpiry Parse(string value)
    {
        if (!TryParse(value, out var expiry))
        {
            throw new FormatException(
                $"'{value}' is not a valid card expiry. Expected format is MM/YY or MM/YYYY.");
        }

        return expiry;
    }

    /// <summary>
    /// Attempts to parse an expiry in "MM/YY", "MM/YYYY", "MM-YY", or "MM-YYYY" form.
    /// </summary>
    /// <param name="value">The text to parse.</param>
    /// <param name="expiry">
    /// When this method returns <see langword="true"/>, the parsed <see cref="CardExpiry"/>.
    /// When it returns <see langword="false"/>, the default value.
    /// </param>
    /// <returns><see langword="true"/> if parsing succeeded; otherwise <see langword="false"/>.</returns>
    public static bool TryParse(string? value, out CardExpiry expiry)
    {
        expiry = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var separatorIndex = value.IndexOf(SlashSeparator);
        if (separatorIndex < 0)
        {
            separatorIndex = value.IndexOf(DashSeparator);
        }

        if (separatorIndex < 0)
        {
            return false;
        }

        var monthPart = value[..separatorIndex];
        var yearPart = value[(separatorIndex + 1)..];

        if (monthPart.Length != RequiredMonthLength)
        {
            return false;
        }

        if (!int.TryParse(monthPart, NumberStyles.None, CultureInfo.InvariantCulture, out var month))
        {
            return false;
        }

        if (month is < MinimumMonth or > MaximumMonth)
        {
            return false;
        }

        if (!int.TryParse(yearPart, NumberStyles.None, CultureInfo.InvariantCulture, out var year))
        {
            return false;
        }

        year = yearPart.Length switch
        {
            TwoDigitYearLength => TwoDigitYearCentury + year,
            FourDigitYearLength => year,
            _ => -1
        };

        if (year < 0)
        {
            return false;
        }

        expiry = new CardExpiry(month, year);
        return true;
    }

    /// <summary>
    /// Determines whether this expiry has passed as of <paramref name="referenceDate"/>.
    /// </summary>
    /// <param name="referenceDate">The date to compare against.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="referenceDate"/> is after
    /// <see cref="LastValidDate"/>; otherwise <see langword="false"/>.
    /// </returns>
    public bool IsExpiredAsOf(DateOnly referenceDate)
    {
        return referenceDate > LastValidDate;
    }

    /// <summary>
    /// Determines whether this expiry has passed as of the current UTC date.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if today, in UTC, is after <see cref="LastValidDate"/>; otherwise
    /// <see langword="false"/>.
    /// </returns>
    public bool IsExpired()
    {
        return IsExpiredAsOf(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    /// <summary>
    /// Formats this expiry as "MM/YY", matching how it is printed on a physical card.
    /// </summary>
    /// <returns>The formatted expiry.</returns>
    public override string ToString()
    {
        var twoDigitYear = Year % TwoDigitYearCentury;
        return string.Create(
            CultureInfo.InvariantCulture,
            $"{Month:D2}{SlashSeparator}{twoDigitYear:D2}");
    }
}
