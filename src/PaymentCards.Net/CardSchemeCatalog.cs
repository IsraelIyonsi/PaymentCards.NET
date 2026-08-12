namespace PaymentCards;

/// <summary>
/// The single source of truth for every card scheme's IIN ranges, PAN lengths, CVV length, and
/// display grouping. <see cref="CardSchemeDetector"/>, <see cref="CardSchemeRules"/>, and
/// <see cref="CardNumberFormatter"/> all read from this catalog.
/// </summary>
internal static class CardSchemeCatalog
{
    private const int StandardCvvLength = 3;
    private const int AmericanExpressCvvLength = 4;

    private static readonly int[] DefaultGroupingPattern = [4, 4, 4, 4];
    private static readonly int[] AmericanExpressGroupingPattern = [4, 6, 5];
    private static readonly int[] DinersClubGroupingPattern = [4, 6, 4];

    private static readonly int[] VisaValidLengths = [13, 16, 19];
    private static readonly int[] MastercardValidLengths = [16];
    private static readonly int[] AmericanExpressValidLengths = [15];
    private static readonly int[] DiscoverValidLengths = [16, 19];
    private static readonly int[] DinersClubValidLengths = [14, 16];
    private static readonly int[] JcbValidLengths = [16];
    private static readonly int[] UnionPayValidLengths = [16, 17, 18, 19];
    private static readonly int[] MaestroValidLengths = [12, 13, 14, 15, 16, 17, 18, 19];
    private static readonly int[] VerveValidLengths = [16, 18, 19];

    /// <summary>
    /// Every known scheme definition, ordered from most specific to least specific IIN range so
    /// that narrow ranges (for example Verve's 6500 prefix) are matched before the broader
    /// ranges they are carved out of (for example Discover's 65 prefix).
    /// </summary>
    internal static readonly IReadOnlyList<CardSchemeDefinition> Definitions = BuildDefinitions();

    private static IReadOnlyList<CardSchemeDefinition> BuildDefinitions()
    {
        return
        [
            new CardSchemeDefinition(
                Scheme: CardScheme.Verve,
                DisplayName: "Verve",
                IinRanges:
                [
                    new IinRange(PrefixLength: 4, Min: 5060, Max: 5060),
                    new IinRange(PrefixLength: 4, Min: 5061, Max: 5061),
                    new IinRange(PrefixLength: 4, Min: 5078, Max: 5078),
                    new IinRange(PrefixLength: 4, Min: 6500, Max: 6500)
                ],
                ValidLengths: VerveValidLengths,
                CvvLength: StandardCvvLength,
                GroupingPattern: DefaultGroupingPattern),

            new CardSchemeDefinition(
                Scheme: CardScheme.AmericanExpress,
                DisplayName: "American Express",
                IinRanges:
                [
                    new IinRange(PrefixLength: 2, Min: 34, Max: 34),
                    new IinRange(PrefixLength: 2, Min: 37, Max: 37)
                ],
                ValidLengths: AmericanExpressValidLengths,
                CvvLength: AmericanExpressCvvLength,
                GroupingPattern: AmericanExpressGroupingPattern),

            new CardSchemeDefinition(
                Scheme: CardScheme.DinersClub,
                DisplayName: "Diners Club",
                IinRanges:
                [
                    new IinRange(PrefixLength: 3, Min: 300, Max: 305),
                    new IinRange(PrefixLength: 2, Min: 36, Max: 36),
                    new IinRange(PrefixLength: 2, Min: 38, Max: 39)
                ],
                ValidLengths: DinersClubValidLengths,
                CvvLength: StandardCvvLength,
                GroupingPattern: DinersClubGroupingPattern),

            new CardSchemeDefinition(
                Scheme: CardScheme.Jcb,
                DisplayName: "JCB",
                IinRanges:
                [
                    new IinRange(PrefixLength: 4, Min: 3528, Max: 3589)
                ],
                ValidLengths: JcbValidLengths,
                CvvLength: StandardCvvLength,
                GroupingPattern: DefaultGroupingPattern),

            new CardSchemeDefinition(
                Scheme: CardScheme.Discover,
                DisplayName: "Discover",
                IinRanges:
                [
                    new IinRange(PrefixLength: 4, Min: 6011, Max: 6011),
                    new IinRange(PrefixLength: 6, Min: 622126, Max: 622925),
                    new IinRange(PrefixLength: 3, Min: 644, Max: 649),
                    new IinRange(PrefixLength: 2, Min: 65, Max: 65)
                ],
                ValidLengths: DiscoverValidLengths,
                CvvLength: StandardCvvLength,
                GroupingPattern: DefaultGroupingPattern),

            new CardSchemeDefinition(
                Scheme: CardScheme.UnionPay,
                DisplayName: "UnionPay",
                IinRanges:
                [
                    new IinRange(PrefixLength: 2, Min: 62, Max: 62)
                ],
                ValidLengths: UnionPayValidLengths,
                CvvLength: StandardCvvLength,
                GroupingPattern: DefaultGroupingPattern),

            new CardSchemeDefinition(
                Scheme: CardScheme.Mastercard,
                DisplayName: "Mastercard",
                IinRanges:
                [
                    new IinRange(PrefixLength: 2, Min: 51, Max: 55),
                    new IinRange(PrefixLength: 4, Min: 2221, Max: 2720)
                ],
                ValidLengths: MastercardValidLengths,
                CvvLength: StandardCvvLength,
                GroupingPattern: DefaultGroupingPattern),

            new CardSchemeDefinition(
                Scheme: CardScheme.Maestro,
                DisplayName: "Maestro",
                IinRanges:
                [
                    new IinRange(PrefixLength: 2, Min: 50, Max: 50),
                    new IinRange(PrefixLength: 2, Min: 56, Max: 58),
                    new IinRange(PrefixLength: 4, Min: 6304, Max: 6304),
                    new IinRange(PrefixLength: 4, Min: 6390, Max: 6390),
                    new IinRange(PrefixLength: 4, Min: 6700, Max: 6799)
                ],
                ValidLengths: MaestroValidLengths,
                CvvLength: StandardCvvLength,
                GroupingPattern: DefaultGroupingPattern),

            new CardSchemeDefinition(
                Scheme: CardScheme.Visa,
                DisplayName: "Visa",
                IinRanges:
                [
                    new IinRange(PrefixLength: 1, Min: 4, Max: 4)
                ],
                ValidLengths: VisaValidLengths,
                CvvLength: StandardCvvLength,
                GroupingPattern: DefaultGroupingPattern)
        ];
    }

    /// <summary>
    /// The grouping pattern applied to a PAN whose scheme could not be determined.
    /// </summary>
    internal static IReadOnlyList<int> UnknownSchemeGroupingPattern => DefaultGroupingPattern;

    /// <summary>
    /// The CVV length applied to a PAN whose scheme could not be determined.
    /// </summary>
    internal const int UnknownSchemeCvvLength = StandardCvvLength;

    /// <summary>
    /// Looks up the definition for a known scheme.
    /// </summary>
    /// <param name="scheme">The scheme to look up.</param>
    /// <returns>The matching definition.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="scheme"/> is <see cref="CardScheme.Unknown"/> or not a recognized value.
    /// </exception>
    internal static CardSchemeDefinition GetDefinition(CardScheme scheme)
    {
        foreach (var definition in Definitions)
        {
            if (definition.Scheme == scheme)
            {
                return definition;
            }
        }

        throw new ArgumentOutOfRangeException(
            nameof(scheme),
            scheme,
            "No scheme definition exists for the specified scheme.");
    }
}
