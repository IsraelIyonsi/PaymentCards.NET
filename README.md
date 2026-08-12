# PaymentCards.NET

Payment card validation and brand detection for .NET. Luhn check, scheme detection (Visa, Mastercard, American Express, Discover, Diners Club, JCB, UnionPay, Maestro, and Verve), expected PAN length and CVV length per scheme, expiry parsing with a past-due check, and a safe formatter that groups digits and masks everything but the last four. Zero external dependencies.

Every established .NET card-validation library on NuGet has gone stale since 2022, and none of them know about Verve, the Interswitch-operated scheme that a large share of Nigerian bank cards are issued on. If your checkout serves Nigerian customers and you route Verve traffic through a Mastercard code path, you get the brand icon wrong and, if you are keying CVV length off scheme, you get validation wrong too. PaymentCards.NET is a small, dependency-free library that treats Verve as a first-class scheme instead of an afterthought.

## Install

```
dotnet add package PaymentCards.Net
```

## Usage

### Parse and validate a card in one call

```csharp
using PaymentCards;

var card = PaymentCard.Parse("5061 8301 0000 1895");

Console.WriteLine(card.Scheme);        // Verve
Console.WriteLine(card.IsLuhnValid);   // True
Console.WriteLine(card.HasValidLength);// True
Console.WriteLine(card.IsValid);       // True
Console.WriteLine(card.MaskedNumber);  // **** **** **** 1895
```

### Validate scheme, length, and CVV independently

```csharp
using PaymentCards;

var scheme = CardSchemeDetector.Detect(pan);
var cvvIsRightLength = enteredCvv.Length == CardSchemeRules.GetCvvLength(scheme);
var panLengthIsRight = CardSchemeRules.IsValidLength(scheme, pan.Length);
var luhnPasses = LuhnChecksum.IsValid(pan);
```

### Parse an expiry and reject an expired card

```csharp
using PaymentCards;

var expiry = CardExpiry.Parse("07/26");

if (expiry.IsExpired())
{
    return CheckoutError.CardExpired;
}
```

### Show a card number safely in a UI or a log line

```csharp
using PaymentCards;

var card = PaymentCard.Parse(rawPan);
logger.LogInformation("Charging {Scheme} card {Masked}", card.Scheme, card.MaskedNumber);
// Charging AmericanExpress card **** ****** *0005
```

## What "valid" means here

`PaymentCard.IsValid` is `true` only when all three of these hold:

- `IsLuhnValid`: the PAN passes the Luhn checksum (ISO/IEC 7812-1)
- `HasValidLength`: the digit count matches a length the detected scheme actually issues
- the scheme is recognized (not `CardScheme.Unknown`)

`PaymentCard.Parse` and `TryParse` never throw or fail because a card is *invalid* in this sense. They only throw (`Parse`) or return `false` (`TryParse`) when the input is not structurally parseable at all, meaning it has no digits once spaces and hyphens are stripped. This mirrors how `TryParse` works across the rest of .NET: parseability and business validity are different questions, and you check `IsValid` for the second one.

## Scheme coverage and the correctness bar

Detection is driven by a single table of published issuer identification number (IIN) ranges covering Visa, Mastercard (including the 2221-2720 second range), American Express, Discover, Diners Club, JCB, UnionPay, Maestro, and Verve (5060, 5061, 5078, and 6500). Verve's ranges are checked before Discover's and Maestro's broader ranges so a Verve PAN never gets misclassified as Mastercard or Discover just because it starts with a digit those schemes also use.

The test suite embeds the standard test card numbers published in Stripe's, Paystack's, and Interswitch's own sandbox documentation as fixtures and asserts exact scheme and Luhn results against them, not against hand-built numbers alone. It also asserts that corrupting a single digit of any of those numbers always fails the Luhn check, and that every IIN range boundary resolves to the correct neighboring scheme rather than bleeding into it.

## Zero dependencies, AOT-friendly

No runtime NuGet dependencies. The library is plain arithmetic and string handling: no reflection, no `System.Text.Json`, nothing that needs a runtime source generator. It trims and publishes cleanly with Native AOT.

## License

MIT. See [LICENSE](LICENSE).
