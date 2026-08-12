using PaymentCards.Tests.Fixtures;

namespace PaymentCards.Tests;

public class LuhnChecksumTests
{
    public static IEnumerable<object[]> GatewayCards()
    {
        foreach (var card in GatewayTestCardFixtures.All)
        {
            yield return [card.Number];
        }
    }

    public static IEnumerable<object[]> GatewayCardsWithSingleDigitCorruption()
    {
        foreach (var card in GatewayTestCardFixtures.All)
        {
            var corruptedDigit = (char)('0' + (card.Number[2] - '0' + 1) % 10);
            var corrupted = string.Concat(card.Number[..2], corruptedDigit, card.Number[3..]);
            yield return [corrupted];
        }
    }

    [Theory]
    [MemberData(nameof(GatewayCards))]
    public void IsValid_returns_true_for_published_gateway_test_cards(string cardNumber)
    {
        Assert.True(LuhnChecksum.IsValid(cardNumber));
    }

    [Theory]
    [MemberData(nameof(GatewayCardsWithSingleDigitCorruption))]
    public void IsValid_returns_false_when_a_single_digit_is_corrupted(string corruptedCardNumber)
    {
        Assert.False(LuhnChecksum.IsValid(corruptedCardNumber));
    }

    [Theory]
    [InlineData("4242 4242 4242 4242")]
    [InlineData("4242-4242-4242-4242")]
    [InlineData("4242 4242-4242 4242")]
    public void IsValid_ignores_spaces_and_hyphens(string formattedCardNumber)
    {
        Assert.True(LuhnChecksum.IsValid(formattedCardNumber));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("4")]
    [InlineData("1234abcd5678")]
    [InlineData("4242-4242-4242-424X")]
    public void IsValid_returns_false_for_malformed_input(string? cardNumber)
    {
        Assert.False(LuhnChecksum.IsValid(cardNumber));
    }

    [Theory]
    [InlineData("0", false)]
    [InlineData("00", true)]
    [InlineData("18", true)]
    [InlineData("19", false)]
    public void IsValid_handles_short_numeric_strings(string cardNumber, bool expected)
    {
        Assert.Equal(expected, LuhnChecksum.IsValid(cardNumber));
    }
}
