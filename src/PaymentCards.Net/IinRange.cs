namespace PaymentCards;

/// <summary>
/// A closed numeric range of issuer identification number (IIN) prefixes, compared against a
/// fixed number of leading digits of a normalized PAN.
/// </summary>
/// <param name="PrefixLength">The number of leading PAN digits this range examines.</param>
/// <param name="Min">The smallest prefix value, inclusive, that falls within the range.</param>
/// <param name="Max">The largest prefix value, inclusive, that falls within the range.</param>
internal readonly record struct IinRange(int PrefixLength, int Min, int Max)
{
    /// <summary>
    /// Determines whether the leading digits of <paramref name="normalizedPan"/> fall within
    /// this range.
    /// </summary>
    /// <param name="normalizedPan">A PAN containing only ASCII digits.</param>
    /// <returns><see langword="true"/> if the range matches; otherwise <see langword="false"/>.</returns>
    internal bool Matches(ReadOnlySpan<char> normalizedPan)
    {
        if (normalizedPan.Length < PrefixLength)
        {
            return false;
        }

        var prefix = int.Parse(normalizedPan[..PrefixLength]);
        return prefix >= Min && prefix <= Max;
    }
}
