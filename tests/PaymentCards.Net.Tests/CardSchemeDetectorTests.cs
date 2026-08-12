using PaymentCards.Tests.Fixtures;

namespace PaymentCards.Tests;

public class CardSchemeDetectorTests
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
    public void Detect_identifies_the_scheme_of_published_gateway_test_cards(
        string cardNumber, CardScheme expectedScheme)
    {
        Assert.Equal(expectedScheme, CardSchemeDetector.Detect(cardNumber));
    }

    [Theory]
    [MemberData(nameof(GatewayCards))]
    public void Detect_resolves_every_gateway_test_card_to_its_own_scheme_only(
        string cardNumber, CardScheme expectedScheme)
    {
        var detected = CardSchemeDetector.Detect(cardNumber);
        Assert.Equal(expectedScheme, detected);
        Assert.All(
            Enum.GetValues<CardScheme>().Where(scheme => scheme != expectedScheme),
            otherScheme => Assert.NotEqual(otherScheme, detected));
    }

    [Theory]
    [InlineData("5060666666666666666")]
    [InlineData("507850785078507812")]
    [InlineData("5061830100001895")]
    public void Detect_resolves_verve_ranges_to_verve_and_not_mastercard(string verveCardNumber)
    {
        var detected = CardSchemeDetector.Detect(verveCardNumber);
        Assert.Equal(CardScheme.Verve, detected);
        Assert.NotEqual(CardScheme.Mastercard, detected);
    }

    [Theory]
    [InlineData("6500123456789017")]
    [InlineData("6500123456789012340")]
    public void Detect_resolves_the_6500_prefix_to_verve(string cardNumber)
    {
        Assert.Equal(CardScheme.Verve, CardSchemeDetector.Detect(cardNumber));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-card-number")]
    [InlineData("1234567890123456")]
    [InlineData("9999999999999999")]
    public void Detect_returns_unknown_for_unrecognized_or_malformed_input(string? cardNumber)
    {
        Assert.Equal(CardScheme.Unknown, CardSchemeDetector.Detect(cardNumber));
    }

    [Theory]
    [InlineData("2221000000000009", CardScheme.Mastercard)]
    [InlineData("2720000000000005", CardScheme.Mastercard)]
    [InlineData("2220000000000000", CardScheme.Unknown)]
    [InlineData("2721000000000004", CardScheme.Unknown)]
    public void Detect_respects_the_mastercard_2_series_boundaries(string cardNumber, CardScheme expectedScheme)
    {
        Assert.Equal(expectedScheme, CardSchemeDetector.Detect(cardNumber));
    }

    [Theory]
    [InlineData("30000000000004", CardScheme.DinersClub)]
    [InlineData("30500000000003", CardScheme.DinersClub)]
    [InlineData("30600000000001", CardScheme.Unknown)]
    public void Detect_respects_the_diners_club_300_to_305_boundary(string cardNumber, CardScheme expectedScheme)
    {
        Assert.Equal(expectedScheme, CardSchemeDetector.Detect(cardNumber));
    }

    [Theory]
    [InlineData("6221260000000000", CardScheme.Discover)]
    [InlineData("6229250000000003", CardScheme.Discover)]
    [InlineData("6221250000000001", CardScheme.UnionPay)]
    [InlineData("6229260000000002", CardScheme.UnionPay)]
    public void Detect_respects_the_discover_unionpay_carveout_boundary(string cardNumber, CardScheme expectedScheme)
    {
        Assert.Equal(expectedScheme, CardSchemeDetector.Detect(cardNumber));
    }

    [Theory]
    [InlineData("6440000000000005", CardScheme.Discover)]
    [InlineData("6490000000000004", CardScheme.Discover)]
    [InlineData("6430000000000007", CardScheme.Unknown)]
    public void Detect_respects_the_discover_644_to_649_boundary(string cardNumber, CardScheme expectedScheme)
    {
        Assert.Equal(expectedScheme, CardSchemeDetector.Detect(cardNumber));
    }

    [Theory]
    [InlineData("3528000000000007", CardScheme.Jcb)]
    [InlineData("3589000000000003", CardScheme.Jcb)]
    [InlineData("3527000000000008", CardScheme.Unknown)]
    [InlineData("3590000000000000", CardScheme.Unknown)]
    public void Detect_respects_the_jcb_3528_to_3589_boundary(string cardNumber, CardScheme expectedScheme)
    {
        Assert.Equal(expectedScheme, CardSchemeDetector.Detect(cardNumber));
    }

    [Theory]
    [InlineData("6700000000000000", CardScheme.Maestro)]
    [InlineData("6799000000000002", CardScheme.Maestro)]
    [InlineData("6304000000000000", CardScheme.Maestro)]
    [InlineData("6390000000000005", CardScheme.Maestro)]
    [InlineData("6699000000000003", CardScheme.Unknown)]
    [InlineData("6800000000000009", CardScheme.Unknown)]
    public void Detect_respects_the_maestro_ranges(string cardNumber, CardScheme expectedScheme)
    {
        Assert.Equal(expectedScheme, CardSchemeDetector.Detect(cardNumber));
    }

    [Theory]
    [InlineData("5059000000000009", CardScheme.Maestro)]
    [InlineData("5062000000000004", CardScheme.Maestro)]
    [InlineData("5079000000000005", CardScheme.Maestro)]
    [InlineData("6501000000000001", CardScheme.Discover)]
    public void Detect_does_not_widen_verves_narrow_prefixes_to_neighboring_numbers(
        string cardNumber, CardScheme expectedScheme)
    {
        var detected = CardSchemeDetector.Detect(cardNumber);
        Assert.Equal(expectedScheme, detected);
        Assert.NotEqual(CardScheme.Verve, detected);
    }
}
