namespace PaymentCards.Tests;

public class CardExpiryTests
{
    [Theory]
    [InlineData("01/26", 1, 2026)]
    [InlineData("12/26", 12, 2026)]
    [InlineData("01/2026", 1, 2026)]
    [InlineData("12/2026", 12, 2026)]
    [InlineData("01-26", 1, 2026)]
    [InlineData("01-2026", 1, 2026)]
    public void TryParse_accepts_standard_expiry_formats(string value, int expectedMonth, int expectedYear)
    {
        Assert.True(CardExpiry.TryParse(value, out var expiry));
        Assert.Equal(expectedMonth, expiry.Month);
        Assert.Equal(expectedYear, expiry.Year);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("13/26")]
    [InlineData("00/26")]
    [InlineData("1/26")]
    [InlineData("01/6")]
    [InlineData("01/20266")]
    [InlineData("ab/cd")]
    [InlineData("0126")]
    public void TryParse_rejects_malformed_input(string? value)
    {
        Assert.False(CardExpiry.TryParse(value, out _));
    }

    [Fact]
    public void Parse_throws_format_exception_for_malformed_input()
    {
        Assert.Throws<FormatException>(() => CardExpiry.Parse("13/26"));
    }

    [Fact]
    public void Parse_returns_the_parsed_expiry_for_valid_input()
    {
        var expiry = CardExpiry.Parse("07/27");
        Assert.Equal(7, expiry.Month);
        Assert.Equal(2027, expiry.Year);
    }

    [Theory]
    [InlineData(0, 2026)]
    [InlineData(13, 2026)]
    [InlineData(-1, 2026)]
    public void Constructor_throws_for_out_of_range_month(int month, int year)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CardExpiry(month, year));
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    [InlineData(1, 10000)]
    public void Constructor_throws_eagerly_for_out_of_range_year(int month, int year)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CardExpiry(month, year));
    }

    [Fact]
    public void Constructor_accepts_the_minimum_and_maximum_representable_years()
    {
        Assert.Equal(1, new CardExpiry(1, 1).Year);
        Assert.Equal(9999, new CardExpiry(1, 9999).Year);
    }

    [Fact]
    public void TryParse_rejects_a_four_digit_year_below_the_minimum_instead_of_throwing()
    {
        Assert.False(CardExpiry.TryParse("01/0000", out _));
    }

    [Theory]
    [InlineData(2, 2024, 29)]
    [InlineData(2, 2026, 28)]
    [InlineData(4, 2026, 30)]
    [InlineData(1, 2026, 31)]
    public void LastValidDate_is_the_final_calendar_day_of_the_expiry_month(int month, int year, int expectedDay)
    {
        var expiry = new CardExpiry(month, year);
        Assert.Equal(new DateOnly(year, month, expectedDay), expiry.LastValidDate);
    }

    [Theory]
    [InlineData(6, 2026, "2026-06-30", false)]
    [InlineData(6, 2026, "2026-07-01", true)]
    [InlineData(6, 2026, "2026-06-29", false)]
    [InlineData(6, 2026, "2027-01-01", true)]
    public void IsExpiredAsOf_treats_a_card_as_valid_through_the_last_day_of_its_month(
        int month, int year, string referenceDateText, bool expectedExpired)
    {
        var expiry = new CardExpiry(month, year);
        var referenceDate = DateOnly.Parse(referenceDateText);
        Assert.Equal(expectedExpired, expiry.IsExpiredAsOf(referenceDate));
    }

    [Fact]
    public void IsExpired_uses_the_current_utc_date_as_the_reference()
    {
        var pastExpiry = new CardExpiry(1, 2000);
        var farFutureExpiry = new CardExpiry(12, 9998);

        Assert.True(pastExpiry.IsExpired());
        Assert.False(farFutureExpiry.IsExpired());
    }

    [Theory]
    [InlineData(1, 2026, "01/26")]
    [InlineData(12, 2026, "12/26")]
    [InlineData(9, 2099, "09/99")]
    [InlineData(1, 2100, "01/00")]
    [InlineData(1, 1999, "01/99")]
    public void ToString_formats_as_two_digit_month_slash_two_digit_year(int month, int year, string expected)
    {
        Assert.Equal(expected, new CardExpiry(month, year).ToString());
    }

    [Fact]
    public void Equality_is_value_based()
    {
        var first = new CardExpiry(6, 2026);
        var second = new CardExpiry(6, 2026);
        var different = new CardExpiry(7, 2026);

        Assert.Equal(first, second);
        Assert.NotEqual(first, different);
        Assert.True(first == second);
        Assert.True(first != different);
    }
}
