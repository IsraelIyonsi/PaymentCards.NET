using PaymentCards;

namespace PaymentCards.Tests.Fixtures;

/// <summary>
/// A test card number published by a payment gateway's sandbox documentation, used as a
/// correctness oracle for Luhn validity and scheme detection.
/// </summary>
/// <param name="Source">Where this number is published.</param>
/// <param name="Number">The test PAN, exactly as published (digits only).</param>
/// <param name="ExpectedScheme">The scheme this PAN's IIN identifies.</param>
internal readonly record struct GatewayTestCard(string Source, string Number, CardScheme ExpectedScheme);

/// <summary>
/// The standard published test card numbers used to validate Luhn checking and scheme detection
/// against real gateway sandboxes rather than hand-built vectors alone.
/// </summary>
/// <remarks>
/// Sources:
/// Stripe testing documentation (https://docs.stripe.com/testing) for Visa, Mastercard, American
/// Express, Discover, Diners Club, JCB, and UnionPay.
/// Paystack test card documentation (https://github.com/PaystackHQ/documentation) and Interswitch
/// test card documentation (https://docs.interswitchgroup.com/docs/test-cards) for Verve, the
/// Nigerian domestic scheme neither Stripe nor older card libraries support.
/// A widely published Maestro sandbox test number (6759 6498 2643 8453) for Maestro.
/// </remarks>
internal static class GatewayTestCardFixtures
{
    internal static readonly IReadOnlyList<GatewayTestCard> All =
    [
        new("Stripe", "4242424242424242", CardScheme.Visa),
        new("Stripe", "4000056655665556", CardScheme.Visa),

        new("Stripe", "5555555555554444", CardScheme.Mastercard),
        new("Stripe", "2223003122003222", CardScheme.Mastercard),
        new("Stripe", "5200828282828210", CardScheme.Mastercard),

        new("Stripe", "378282246310005", CardScheme.AmericanExpress),
        new("Stripe", "371449635398431", CardScheme.AmericanExpress),

        new("Stripe", "6011111111111117", CardScheme.Discover),
        new("Stripe", "6011000990139424", CardScheme.Discover),

        new("Stripe", "3056930009020004", CardScheme.DinersClub),
        new("Stripe", "36227206271667", CardScheme.DinersClub),

        new("Stripe", "3566002020360505", CardScheme.Jcb),

        new("Stripe", "6200000000000005", CardScheme.UnionPay),
        new("Stripe", "6205500000000000004", CardScheme.UnionPay),

        new("Paystack", "5060666666666666666", CardScheme.Verve),
        new("Paystack", "507850785078507812", CardScheme.Verve),
        new("Interswitch", "5061830100001895", CardScheme.Verve),
        new("Interswitch", "5061050254756707864", CardScheme.Verve),
        new("Interswitch", "5060990580000217499", CardScheme.Verve),

        new("Common Maestro sandbox test number", "6759649826438453", CardScheme.Maestro)
    ];
}
