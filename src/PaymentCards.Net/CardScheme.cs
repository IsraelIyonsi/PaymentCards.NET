namespace PaymentCards;

/// <summary>
/// Identifies the payment network (card scheme) that issued a primary account number (PAN).
/// </summary>
public enum CardScheme
{
    /// <summary>
    /// The scheme could not be determined from the PAN's issuer identification number (IIN).
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Visa, identified by PANs beginning with the digit 4.
    /// </summary>
    Visa,

    /// <summary>
    /// Mastercard, identified by PANs in the 51-55 range or the 2221-2720 range.
    /// </summary>
    Mastercard,

    /// <summary>
    /// American Express, identified by PANs beginning with 34 or 37.
    /// </summary>
    AmericanExpress,

    /// <summary>
    /// Discover, identified by PANs beginning with 6011, 644-649, 65, or 622126-622925.
    /// </summary>
    Discover,

    /// <summary>
    /// Diners Club, identified by PANs beginning with 300-305, 36, 38, or 39.
    /// </summary>
    DinersClub,

    /// <summary>
    /// JCB, identified by PANs in the 3528-3589 range.
    /// </summary>
    Jcb,

    /// <summary>
    /// UnionPay, identified by PANs beginning with 62.
    /// </summary>
    UnionPay,

    /// <summary>
    /// Maestro, identified by PANs beginning with 50, 56-58, 6304, 6390, or 6700-6799.
    /// </summary>
    Maestro,

    /// <summary>
    /// Verve, the Nigerian domestic scheme operated by Interswitch, identified by PANs
    /// beginning with 5060, 5061, 5078, or 6500.
    /// </summary>
    Verve
}
