# CHANGELOG

This project uses [Semantic Versioning 2.0.0](http://semver.org/).

## main

## 0.13.0

- CHANGED: Added support for DS record key-data interface (dnsimple/dnsimple-chsarp#58)

## 0.12.0

- CHANGED: Removed deprecated `Registrar.GetDomainPremiumPrices`

## 0.11.0

- NEW: Added `Registrar.GetDomainPrices` to retrieve whether a domain is premium and the prices to register, transfer, and renew. (dnsimple/dnsimple-csharp#46)

## 0.10.0

- FIXED: Adds missing CDNSKEY & CDS record types (#32). (dnsimple/dnsimple-csharp#32)
- FIXED: Avoids setting `UserAgent` on `ChangeBaseUrlTo` if Client is provided. (dnsimple/dnsimple-csharp#22)
- CHANGED: Moves / Migrates project to .NET standard (dnsimple/dnsimple-csharp#23)
- CHANGED: `Contact.Fax` is not required anymore. (dnsimple/dnsimple-csharp#16)
- CHANGED: `Certificate.ExpiresOn` has been replaced by `Certificate.ExpiresAt`. (dnsimple/dnsimple-csharp#14)
- CHANGED: `Domain.ExpiresOn` has been replaced by `Domain.ExpiresAt`. (dnsimple/dnsimple-csharp#11)

## 0.9.0

Initial non-development release that supports API v2.
