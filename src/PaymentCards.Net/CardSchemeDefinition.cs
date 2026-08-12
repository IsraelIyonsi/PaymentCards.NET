namespace PaymentCards;

/// <summary>
/// The complete set of rules that describe one <see cref="CardScheme"/>: the IIN ranges that
/// identify it, the PAN lengths it issues, its CVV length, and how it is grouped for display.
/// </summary>
/// <param name="Scheme">The scheme these rules describe.</param>
/// <param name="DisplayName">A human-readable name for the scheme.</param>
/// <param name="IinRanges">
/// The issuer identification number ranges that identify a PAN as belonging to this scheme,
/// evaluated in order against the leading digits of the PAN.
/// </param>
/// <param name="ValidLengths">The PAN lengths this scheme is known to issue.</param>
/// <param name="CvvLength">The number of digits in this scheme's card verification value.</param>
/// <param name="GroupingPattern">
/// The digit group sizes used to format a PAN for display, applied left to right. When a PAN is
/// longer than the sum of the pattern, the final group size repeats for the remaining digits.
/// </param>
internal sealed record CardSchemeDefinition(
    CardScheme Scheme,
    string DisplayName,
    IReadOnlyList<IinRange> IinRanges,
    IReadOnlyList<int> ValidLengths,
    int CvvLength,
    IReadOnlyList<int> GroupingPattern);
