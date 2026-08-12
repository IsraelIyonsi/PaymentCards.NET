namespace PaymentCards.Tests;

public class CardNumberFormatterTests
{
    [Theory]
    [InlineData("4242424242424242", CardScheme.Visa, "4242 4242 4242 4242")]
    [InlineData("5555555555554444", CardScheme.Mastercard, "5555 5555 5555 4444")]
    [InlineData("378282246310005", CardScheme.AmericanExpress, "3782 822463 10005")]
    [InlineData("3056930009020004", CardScheme.DinersClub, "3056 930009 0200 04")]
    [InlineData("36227206271667", CardScheme.DinersClub, "3622 720627 1667")]
    [InlineData("6205500000000000004", CardScheme.UnionPay, "6205 5000 0000 0000 004")]
    [InlineData("507850785078507812", CardScheme.Verve, "5078 5078 5078 5078 12")]
    public void Format_groups_digits_using_the_schemes_pattern(
        string cardNumber, CardScheme scheme, string expected)
    {
        Assert.Equal(expected, CardNumberFormatter.Format(cardNumber, scheme));
    }

    [Fact]
    public void Format_uses_the_default_4_digit_grouping_for_unknown_scheme()
    {
        Assert.Equal("1234 5678 9012 3456", CardNumberFormatter.Format("1234567890123456", CardScheme.Unknown));
    }

    [Theory]
    [InlineData("4242424242424242", CardScheme.Visa, "**** **** **** 4242")]
    [InlineData("378282246310005", CardScheme.AmericanExpress, "**** ****** *0005")]
    [InlineData("3056930009020004", CardScheme.DinersClub, "**** ****** **00 04")]
    public void Mask_hides_every_digit_except_the_last_four(
        string cardNumber, CardScheme scheme, string expected)
    {
        Assert.Equal(expected, CardNumberFormatter.Mask(cardNumber, scheme));
    }

    [Fact]
    public void Mask_accepts_a_custom_mask_character()
    {
        Assert.Equal("XXXX XXXX XXXX 4242", CardNumberFormatter.Mask("4242424242424242", CardScheme.Visa, 'X'));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("42")]
    [InlineData("4242")]
    public void Mask_leaves_short_numbers_fully_visible(string shortNumber)
    {
        Assert.Equal(shortNumber, CardNumberFormatter.Mask(shortNumber, CardScheme.Unknown));
    }

    [Theory]
    [InlineData("4242 4242 4242 4242")]
    [InlineData("4242-4242-4242-4242")]
    public void Format_ignores_existing_separators_in_the_input(string formattedInput)
    {
        Assert.Equal("4242 4242 4242 4242", CardNumberFormatter.Format(formattedInput, CardScheme.Visa));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12ab34")]
    public void Format_throws_for_input_that_does_not_normalize_to_digits(string? cardNumber)
    {
        Assert.Throws<ArgumentException>(() => CardNumberFormatter.Format(cardNumber!, CardScheme.Visa));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12ab34")]
    public void Mask_throws_for_input_that_does_not_normalize_to_digits(string? cardNumber)
    {
        Assert.Throws<ArgumentException>(() => CardNumberFormatter.Mask(cardNumber!, CardScheme.Visa));
    }
}
