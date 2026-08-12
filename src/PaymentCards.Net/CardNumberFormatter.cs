using System.Text;

namespace PaymentCards;

/// <summary>
/// Formats a PAN for safe display: grouped into scheme-appropriate digit blocks, and with all but
/// the last four digits masked so the formatted value is safe to log or show on screen.
/// </summary>
public static class CardNumberFormatter
{
    private const char GroupSeparator = ' ';
    private const char DefaultMaskCharacter = '*';
    private const int VisibleTrailingDigitCount = 4;

    /// <summary>
    /// Groups a PAN's digits into the blocks its scheme is printed with, without masking any
    /// digit. For example, a 16-digit Visa PAN is grouped 4-4-4-4 and a 15-digit American
    /// Express PAN is grouped 4-6-5.
    /// </summary>
    /// <param name="cardNumber">
    /// The PAN to format. Spaces and hyphens are ignored; every other character must be an ASCII
    /// digit.
    /// </param>
    /// <param name="scheme">The scheme whose grouping pattern to apply.</param>
    /// <returns>The grouped PAN, with groups separated by a single space.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="cardNumber"/> does not normalize to a non-empty digit string.
    /// </exception>
    public static string Format(string cardNumber, CardScheme scheme)
    {
        var digits = NormalizeOrThrow(cardNumber);
        var groupingPattern = GetGroupingPattern(scheme);
        return Group(digits, groupingPattern);
    }

    /// <summary>
    /// Groups a PAN's digits the same way as <see cref="Format(string, CardScheme)"/>, but
    /// replaces every digit except the last four with <paramref name="maskCharacter"/>.
    /// </summary>
    /// <param name="cardNumber">
    /// The PAN to mask. Spaces and hyphens are ignored; every other character must be an ASCII
    /// digit.
    /// </param>
    /// <param name="scheme">The scheme whose grouping pattern to apply.</param>
    /// <param name="maskCharacter">The character to substitute for each hidden digit.</param>
    /// <returns>
    /// The grouped, masked PAN. A card number shorter than or equal to four digits is returned
    /// with every digit visible, since there are no leading digits to mask.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="cardNumber"/> does not normalize to a non-empty digit string.
    /// </exception>
    public static string Mask(string cardNumber, CardScheme scheme, char maskCharacter = DefaultMaskCharacter)
    {
        var digits = NormalizeOrThrow(cardNumber);
        var maskedDigits = MaskAllButLastFour(digits, maskCharacter);
        var groupingPattern = GetGroupingPattern(scheme);
        return Group(maskedDigits, groupingPattern);
    }

    private static string NormalizeOrThrow(string cardNumber)
    {
        if (!CardNumberNormalizer.TryNormalize(cardNumber, out var digits))
        {
            throw new ArgumentException(
                "Card number must contain at least one digit and no characters other than digits, spaces, or hyphens.",
                nameof(cardNumber));
        }

        return digits;
    }

    private static IReadOnlyList<int> GetGroupingPattern(CardScheme scheme)
    {
        return scheme == CardScheme.Unknown
            ? CardSchemeCatalog.UnknownSchemeGroupingPattern
            : CardSchemeCatalog.GetDefinition(scheme).GroupingPattern;
    }

    private static string MaskAllButLastFour(string digits, char maskCharacter)
    {
        if (digits.Length <= VisibleTrailingDigitCount)
        {
            return digits;
        }

        var maskedLength = digits.Length - VisibleTrailingDigitCount;
        var masked = new StringBuilder(digits.Length);
        masked.Append(maskCharacter, maskedLength);
        masked.Append(digits, maskedLength, VisibleTrailingDigitCount);
        return masked.ToString();
    }

    private static string Group(string digits, IReadOnlyList<int> groupingPattern)
    {
        var result = new StringBuilder(digits.Length + digits.Length / 2);
        var position = 0;
        var patternIndex = 0;

        while (position < digits.Length)
        {
            var groupSize = groupingPattern[Math.Min(patternIndex, groupingPattern.Count - 1)];
            var remaining = digits.Length - position;
            var actualGroupSize = Math.Min(groupSize, remaining);

            if (position > 0)
            {
                result.Append(GroupSeparator);
            }

            result.Append(digits, position, actualGroupSize);
            position += actualGroupSize;
            patternIndex++;
        }

        return result.ToString();
    }
}
