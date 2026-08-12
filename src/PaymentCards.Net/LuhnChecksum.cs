namespace PaymentCards;

/// <summary>
/// Validates numeric strings against the Luhn checksum algorithm (ISO/IEC 7812-1) used by every
/// major payment card scheme to catch single-digit typos and adjacent-digit transpositions.
/// </summary>
public static class LuhnChecksum
{
    private const int DoubleDigitThreshold = 9;
    private const int DoubleDigitAdjustment = 9;
    private const int LuhnModulus = 10;
    private const int MinimumDigitCount = 2;

    /// <summary>
    /// Determines whether <paramref name="cardNumber"/> satisfies the Luhn checksum.
    /// </summary>
    /// <param name="cardNumber">
    /// The number to check. Spaces and hyphens are ignored; every other character must be an
    /// ASCII digit.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="cardNumber"/> normalizes to at least two digits
    /// and satisfies the Luhn checksum; otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsValid(string? cardNumber)
    {
        if (!CardNumberNormalizer.TryNormalize(cardNumber, out var digits))
        {
            return false;
        }

        if (digits.Length < MinimumDigitCount)
        {
            return false;
        }

        var sum = 0;
        var isSecondDigitFromRight = false;

        for (var i = digits.Length - 1; i >= 0; i--)
        {
            var digit = digits[i] - '0';

            if (isSecondDigitFromRight)
            {
                digit *= 2;
                if (digit > DoubleDigitThreshold)
                {
                    digit -= DoubleDigitAdjustment;
                }
            }

            sum += digit;
            isSecondDigitFromRight = !isSecondDigitFromRight;
        }

        return sum % LuhnModulus == 0;
    }
}
