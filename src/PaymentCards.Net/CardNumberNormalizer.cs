namespace PaymentCards;

/// <summary>
/// Strips the whitespace and hyphen separators that PANs are commonly displayed with, leaving
/// only the digits used for validation, scheme detection, and formatting.
/// </summary>
internal static class CardNumberNormalizer
{
    private const char HyphenSeparator = '-';

    /// <summary>
    /// Attempts to reduce <paramref name="cardNumber"/> to its digits only.
    /// </summary>
    /// <param name="cardNumber">The raw PAN, which may contain spaces or hyphens.</param>
    /// <param name="normalized">
    /// When this method returns <see langword="true"/>, the digit-only PAN. When it returns
    /// <see langword="false"/>, an empty string.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="cardNumber"/> is non-empty and, once separators
    /// are removed, contains only ASCII digits; otherwise <see langword="false"/>.
    /// </returns>
    internal static bool TryNormalize(string? cardNumber, out string normalized)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            normalized = string.Empty;
            return false;
        }

        Span<char> buffer = stackalloc char[cardNumber.Length];
        var digitCount = 0;

        foreach (var character in cardNumber)
        {
            if (char.IsWhiteSpace(character) || character == HyphenSeparator)
            {
                continue;
            }

            if (!char.IsAsciiDigit(character))
            {
                normalized = string.Empty;
                return false;
            }

            buffer[digitCount] = character;
            digitCount++;
        }

        if (digitCount == 0)
        {
            normalized = string.Empty;
            return false;
        }

        normalized = new string(buffer[..digitCount]);
        return true;
    }
}
