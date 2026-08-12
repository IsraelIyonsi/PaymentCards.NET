using System.Diagnostics.CodeAnalysis;

namespace PaymentCards;

/// <summary>
/// A parsed primary account number (PAN) together with its detected scheme and the results of
/// validating it against that scheme's rules.
/// </summary>
public sealed class PaymentCard
{
    private PaymentCard(string normalizedNumber)
    {
        Number = normalizedNumber;
        Scheme = CardSchemeDetector.Detect(normalizedNumber);
        IsLuhnValid = LuhnChecksum.IsValid(normalizedNumber);
        HasValidLength = CardSchemeRules.IsValidLength(Scheme, normalizedNumber.Length);
    }

    /// <summary>
    /// Gets the PAN with all separators removed, containing only ASCII digits.
    /// </summary>
    public string Number { get; }

    /// <summary>
    /// Gets the scheme detected from <see cref="Number"/>'s issuer identification number, or
    /// <see cref="CardScheme.Unknown"/> if no known scheme matches.
    /// </summary>
    public CardScheme Scheme { get; }

    /// <summary>
    /// Gets a value indicating whether <see cref="Number"/> satisfies the Luhn checksum.
    /// </summary>
    public bool IsLuhnValid { get; }

    /// <summary>
    /// Gets a value indicating whether <see cref="Number"/>'s digit count is one that
    /// <see cref="Scheme"/> is known to issue.
    /// </summary>
    public bool HasValidLength { get; }

    /// <summary>
    /// Gets a value indicating whether this card passes the Luhn checksum, has a length valid
    /// for its scheme, and belongs to a recognized scheme.
    /// </summary>
    public bool IsValid => IsLuhnValid && HasValidLength && Scheme != CardScheme.Unknown;

    /// <summary>
    /// Gets <see cref="Number"/> grouped into <see cref="Scheme"/>'s display blocks with every
    /// digit except the last four replaced by a mask character. Safe to log or display.
    /// </summary>
    public string MaskedNumber => CardNumberFormatter.Mask(Number, Scheme);

    /// <summary>
    /// Gets <see cref="Number"/> grouped into <see cref="Scheme"/>'s display blocks, with every
    /// digit visible.
    /// </summary>
    public string FormattedNumber => CardNumberFormatter.Format(Number, Scheme);

    /// <summary>
    /// Parses a PAN, detecting its scheme and validating it in the same pass.
    /// </summary>
    /// <param name="cardNumber">
    /// The PAN to parse. Spaces and hyphens are ignored; every other character must be an ASCII
    /// digit.
    /// </param>
    /// <returns>The parsed <see cref="PaymentCard"/>. Check <see cref="IsValid"/> for the result.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="cardNumber"/> does not normalize to a non-empty digit string.
    /// </exception>
    public static PaymentCard Parse(string? cardNumber)
    {
        if (!TryParse(cardNumber, out var card))
        {
            throw new ArgumentException(
                "Card number must contain at least one digit and no characters other than digits, spaces, or hyphens.",
                nameof(cardNumber));
        }

        return card;
    }

    /// <summary>
    /// Attempts to parse a PAN, detecting its scheme and validating it in the same pass.
    /// </summary>
    /// <param name="cardNumber">
    /// The PAN to parse. Spaces and hyphens are ignored; every other character must be an ASCII
    /// digit.
    /// </param>
    /// <param name="card">
    /// When this method returns <see langword="true"/>, the parsed <see cref="PaymentCard"/>.
    /// When it returns <see langword="false"/>, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="cardNumber"/> normalized to a non-empty digit
    /// string; otherwise <see langword="false"/>. This reflects whether the input was
    /// structurally parseable, not whether the card is valid; check <see cref="IsValid"/> on the
    /// returned card for that.
    /// </returns>
    public static bool TryParse(string? cardNumber, [NotNullWhen(true)] out PaymentCard? card)
    {
        if (!CardNumberNormalizer.TryNormalize(cardNumber, out var digits))
        {
            card = null;
            return false;
        }

        card = new PaymentCard(digits);
        return true;
    }
}
