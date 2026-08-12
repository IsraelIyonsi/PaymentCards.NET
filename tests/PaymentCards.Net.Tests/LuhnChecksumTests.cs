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

    /// <summary>
    /// Every way a single digit of every fixture PAN can be altered to a different digit: each
    /// digit position crossed with each of the 9 possible non-identity replacement values. This
    /// is exhaustive per fixture, not a spot check at one position with one delta, which is what
    /// makes it a valid demonstration that Luhn catches every single-digit substitution rather
    /// than a single example of it doing so.
    /// </summary>
    public static IEnumerable<object[]> GatewayCardsWithSingleDigitCorruption()
    {
        foreach (var card in GatewayTestCardFixtures.All)
        {
            for (var position = 0; position < card.Number.Length; position++)
            {
                var originalDigit = card.Number[position] - '0';
                for (var delta = 1; delta <= 9; delta++)
                {
                    var corruptedDigit = (char)('0' + (originalDigit + delta) % 10);
                    var corrupted = string.Concat(
                        card.Number[..position],
                        corruptedDigit,
                        card.Number[(position + 1)..]);
                    yield return [corrupted];
                }
            }
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
