namespace PaymentCards;

/// <summary>
/// Detects the card scheme (payment network) that issued a PAN, using published issuer
/// identification number (IIN) ranges.
/// </summary>
public static class CardSchemeDetector
{
    /// <summary>
    /// Detects the scheme that a PAN's leading digits identify.
    /// </summary>
    /// <param name="cardNumber">
    /// The PAN to inspect. Spaces and hyphens are ignored; every other character must be an
    /// ASCII digit.
    /// </param>
    /// <returns>
    /// The first <see cref="CardScheme"/> whose IIN ranges match the PAN, or
    /// <see cref="CardScheme.Unknown"/> if <paramref name="cardNumber"/> does not normalize to
    /// digits or matches no known range.
    /// </returns>
    public static CardScheme Detect(string? cardNumber)
    {
        if (!CardNumberNormalizer.TryNormalize(cardNumber, out var digits))
        {
            return CardScheme.Unknown;
        }

        foreach (var definition in CardSchemeCatalog.Definitions)
        {
            foreach (var range in definition.IinRanges)
            {
                if (range.Matches(digits))
                {
                    return definition.Scheme;
                }
            }
        }

        return CardScheme.Unknown;
    }
}
