namespace PaymentCards;

/// <summary>
/// Exposes the PAN length, CVV length, and display name rules that <see cref="CardSchemeCatalog"/>
/// defines for each <see cref="CardScheme"/>.
/// </summary>
public static class CardSchemeRules
{
    /// <summary>
    /// Gets the PAN lengths that <paramref name="scheme"/> is known to issue.
    /// </summary>
    /// <param name="scheme">The scheme to look up.</param>
    /// <returns>
    /// The set of valid lengths for <paramref name="scheme"/>, or an empty list for
    /// <see cref="CardScheme.Unknown"/>.
    /// </returns>
    public static IReadOnlyList<int> GetValidLengths(CardScheme scheme)
    {
        return scheme == CardScheme.Unknown
            ? []
            : CardSchemeCatalog.GetDefinition(scheme).ValidLengths;
    }

    /// <summary>
    /// Determines whether <paramref name="length"/> is a PAN length that <paramref name="scheme"/>
    /// is known to issue.
    /// </summary>
    /// <param name="scheme">The scheme to check against.</param>
    /// <param name="length">The candidate PAN digit count.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="scheme"/> is not <see cref="CardScheme.Unknown"/>
    /// and issues PANs of <paramref name="length"/>; otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsValidLength(CardScheme scheme, int length)
    {
        var validLengths = GetValidLengths(scheme);

        foreach (var validLength in validLengths)
        {
            if (validLength == length)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the number of digits in the card verification value (CVV/CVC/CID) that
    /// <paramref name="scheme"/> prints on its cards.
    /// </summary>
    /// <param name="scheme">The scheme to look up.</param>
    /// <returns>
    /// The CVV digit count for <paramref name="scheme"/>. American Express uses 4; every other
    /// known scheme, and <see cref="CardScheme.Unknown"/>, uses the industry-standard 3.
    /// </returns>
    public static int GetCvvLength(CardScheme scheme)
    {
        return scheme == CardScheme.Unknown
            ? CardSchemeCatalog.UnknownSchemeCvvLength
            : CardSchemeCatalog.GetDefinition(scheme).CvvLength;
    }

    /// <summary>
    /// Gets a human-readable display name for <paramref name="scheme"/>.
    /// </summary>
    /// <param name="scheme">The scheme to look up.</param>
    /// <returns>The display name, or <c>"Unknown"</c> for <see cref="CardScheme.Unknown"/>.</returns>
    public static string GetDisplayName(CardScheme scheme)
    {
        return scheme == CardScheme.Unknown
            ? nameof(CardScheme.Unknown)
            : CardSchemeCatalog.GetDefinition(scheme).DisplayName;
    }
}
