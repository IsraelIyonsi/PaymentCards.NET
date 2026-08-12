namespace PaymentCards.Tests;

public class CardSchemeRulesTests
{
    public static IEnumerable<object[]> ValidLengthsPerScheme()
    {
        yield return [CardScheme.Visa, new[] { 13, 16, 19 }];
        yield return [CardScheme.Mastercard, new[] { 16 }];
        yield return [CardScheme.AmericanExpress, new[] { 15 }];
        yield return [CardScheme.Discover, new[] { 16, 19 }];
        yield return [CardScheme.DinersClub, new[] { 14, 16 }];
        yield return [CardScheme.Jcb, new[] { 16 }];
        yield return [CardScheme.UnionPay, new[] { 16, 17, 18, 19 }];
        yield return [CardScheme.Maestro, new[] { 12, 13, 14, 15, 16, 17, 18, 19 }];
        yield return [CardScheme.Verve, new[] { 16, 18, 19 }];
    }

    [Theory]
    [MemberData(nameof(ValidLengthsPerScheme))]
    public void GetValidLengths_returns_the_documented_lengths_for_each_scheme(
        CardScheme scheme, int[] expectedLengths)
    {
        Assert.Equal(expectedLengths, CardSchemeRules.GetValidLengths(scheme));
    }

    [Fact]
    public void GetValidLengths_returns_empty_for_unknown_scheme()
    {
        Assert.Empty(CardSchemeRules.GetValidLengths(CardScheme.Unknown));
    }

    [Theory]
    [InlineData(CardScheme.Visa, 16, true)]
    [InlineData(CardScheme.Visa, 15, false)]
    [InlineData(CardScheme.AmericanExpress, 15, true)]
    [InlineData(CardScheme.AmericanExpress, 16, false)]
    [InlineData(CardScheme.Unknown, 16, false)]
    public void IsValidLength_matches_the_scheme_length_table(CardScheme scheme, int length, bool expected)
    {
        Assert.Equal(expected, CardSchemeRules.IsValidLength(scheme, length));
    }

    [Theory]
    [InlineData(CardScheme.AmericanExpress, 4)]
    [InlineData(CardScheme.Visa, 3)]
    [InlineData(CardScheme.Mastercard, 3)]
    [InlineData(CardScheme.Discover, 3)]
    [InlineData(CardScheme.DinersClub, 3)]
    [InlineData(CardScheme.Jcb, 3)]
    [InlineData(CardScheme.UnionPay, 3)]
    [InlineData(CardScheme.Maestro, 3)]
    [InlineData(CardScheme.Verve, 3)]
    [InlineData(CardScheme.Unknown, 3)]
    public void GetCvvLength_returns_four_for_amex_and_three_for_every_other_scheme(
        CardScheme scheme, int expectedCvvLength)
    {
        Assert.Equal(expectedCvvLength, CardSchemeRules.GetCvvLength(scheme));
    }

    [Theory]
    [InlineData(CardScheme.Visa, "Visa")]
    [InlineData(CardScheme.Mastercard, "Mastercard")]
    [InlineData(CardScheme.AmericanExpress, "American Express")]
    [InlineData(CardScheme.Discover, "Discover")]
    [InlineData(CardScheme.DinersClub, "Diners Club")]
    [InlineData(CardScheme.Jcb, "JCB")]
    [InlineData(CardScheme.UnionPay, "UnionPay")]
    [InlineData(CardScheme.Maestro, "Maestro")]
    [InlineData(CardScheme.Verve, "Verve")]
    [InlineData(CardScheme.Unknown, "Unknown")]
    public void GetDisplayName_returns_the_expected_human_readable_name(CardScheme scheme, string expectedName)
    {
        Assert.Equal(expectedName, CardSchemeRules.GetDisplayName(scheme));
    }
}
