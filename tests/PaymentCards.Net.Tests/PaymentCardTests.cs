using PaymentCards.Tests.Fixtures;

namespace PaymentCards.Tests;

public class PaymentCardTests
{
    public static IEnumerable<object[]> GatewayCards()
    {
        foreach (var card in GatewayTestCardFixtures.All)
        {
            yield return [card.Number, card.ExpectedScheme];
        }
    }

    [Theory]
    [MemberData(nameof(GatewayCards))]
    public void Parse_produces_a_fully_valid_card_for_every_gateway_test_card(
        string cardNumber, CardScheme expectedScheme)
    {
        var card = PaymentCard.Parse(cardNumber);

        Assert.Equal(expectedScheme, card.Scheme);
        Assert.True(card.IsLuhnValid);
        Assert.True(card.HasValidLength);
        Assert.True(card.IsValid);
        Assert.Equal(cardNumber, card.Number);
    }

    [Fact]
    public void Parse_normalizes_separators_out_of_the_number()
    {
        var card = PaymentCard.Parse("4242 4242-4242 4242");
        Assert.Equal("4242424242424242", card.Number);
    }

    [Fact]
    public void Parse_marks_an_unknown_scheme_as_invalid_even_with_correct_luhn()
    {
        var card = PaymentCard.Parse("8888888888888888");

        Assert.Equal(CardScheme.Unknown, card.Scheme);
        Assert.True(card.IsLuhnValid);
        Assert.False(card.IsValid);
    }

    [Fact]
    public void Parse_marks_a_known_scheme_with_wrong_length_as_invalid()
    {
        var card = PaymentCard.Parse("42424242424242");

        Assert.Equal(CardScheme.Visa, card.Scheme);
        Assert.False(card.HasValidLength);
        Assert.False(card.IsValid);
    }

    [Fact]
    public void Parse_marks_a_luhn_failure_as_invalid_even_with_a_correct_scheme_and_length()
    {
        var card = PaymentCard.Parse("4242424242424241");

        Assert.Equal(CardScheme.Visa, card.Scheme);
        Assert.True(card.HasValidLength);
        Assert.False(card.IsLuhnValid);
        Assert.False(card.IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-number")]
    public void Parse_throws_for_input_that_does_not_normalize_to_digits(string? cardNumber)
    {
        Assert.Throws<ArgumentException>(() => PaymentCard.Parse(cardNumber));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-number")]
    public void TryParse_returns_false_for_input_that_does_not_normalize_to_digits(string? cardNumber)
    {
        Assert.False(PaymentCard.TryParse(cardNumber, out var card));
        Assert.Null(card);
    }

    [Fact]
    public void TryParse_returns_true_for_structurally_parseable_input_regardless_of_validity()
    {
        Assert.True(PaymentCard.TryParse("0000000000000000", out var card));
        Assert.NotNull(card);
        Assert.False(card.IsValid);
    }

    [Fact]
    public void MaskedNumber_uses_the_detected_schemes_grouping_and_hides_all_but_last_four()
    {
        var card = PaymentCard.Parse("4242424242424242");
        Assert.Equal("**** **** **** 4242", card.MaskedNumber);
    }

    [Fact]
    public void FormattedNumber_uses_the_detected_schemes_grouping_without_masking()
    {
        var card = PaymentCard.Parse("378282246310005");
        Assert.Equal("3782 822463 10005", card.FormattedNumber);
    }
}
