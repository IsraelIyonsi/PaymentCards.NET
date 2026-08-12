# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2026-08-12

### Added

- `LuhnChecksum.IsValid(string)`: Luhn checksum validation (ISO/IEC 7812-1), ignoring space and hyphen separators.
- `CardSchemeDetector.Detect(string)`: scheme detection from published IIN ranges for Visa, Mastercard (including the 2221-2720 second range), American Express, Discover, Diners Club, JCB, UnionPay, Maestro, and Verve (5060, 5061, 5078, 6500).
- `CardSchemeRules`: `GetValidLengths`, `IsValidLength`, `GetCvvLength`, and `GetDisplayName` per scheme.
- `CardExpiry`: parses "MM/YY", "MM/YYYY", "MM-YY", and "MM-YYYY", exposes `LastValidDate`, and `IsExpiredAsOf` / `IsExpired` for a past-due check.
- `CardNumberFormatter`: `Format` groups a PAN into its scheme's display blocks (4-6-5 for American Express, 4-6-4 for Diners Club, 4-4-4-4 for everything else); `Mask` applies the same grouping while replacing every digit except the last four.
- `PaymentCard`: `Parse` / `TryParse` combine detection, Luhn checking, and length checking into `IsLuhnValid`, `HasValidLength`, `IsValid`, `MaskedNumber`, and `FormattedNumber`.
- Test suite embeds the standard test card numbers published in Stripe's, Paystack's, and Interswitch's sandbox documentation as correctness fixtures, asserts single-digit corruption always fails Luhn, and asserts every IIN range boundary (including the Verve-versus-Mastercard and Verve-versus-Discover boundaries) resolves to the correct scheme.
- Zero runtime dependencies; net8.0, Native AOT friendly.
- SourceLink (GitHub), deterministic CI builds and `.snupkg` symbol packages.
