# CHANGELOG

This project uses [Semantic Versioning 2.0.0](http://semver.org/).

## main

## 0.19.1

- HOUSEKEEPING: Bump NUnit3TestAdapter from 4.5.0 to 5.0.0 (#165)
- HOUSEKEEPING: Bump Microsoft.NET.Test.Sdk and Newtonsoft.Json (#164)
- HOUSEKEEPING: Bump NUnit.Analyzers from 4.4.0 to 4.6.0 (#163)
- HOUSEKEEPING: Bump nosborn/github-action-markdown-cli from 3.3.0 to 3.4.0 (#162)
- HOUSEKEEPING: Bump Moq from 4.20.70 to 4.20.72 (#160)
- HOUSEKEEPING: Bump nunit from 4.1.0 to 4.3.2 (#156)

## 0.19.0

- CHANGED: Bumped `dotnet` to `9.0`
- NEW: Added `AliasEmail` to `EmailForward`
- CHANGED: Deprecated `From` and `To` fields in `EmailForward`
- CHANGED: `DomainCollaborators` have been deprecated and will be removed in the next major version. Please use our Domain Access Control feature.
- HOUSEKEEPING: Bump Microsoft.NET.Test.Sdk from 17.9.0 to 17.10.0 (#152)
- HOUSEKEEPING: Bump NUnit.Analyzers from 4.0.1 to 4.4.0 (#153)

## 0.18.1

- HOUSEKEEPING: Bump nunit from 4.0.1 to 4.1.0 (#147)
- HOUSEKEEPING: Bump NUnit.Analyzers from 3.10.0 to 4.0.1 (#146)
- HOUSEKEEPING: Bump Microsoft.NET.Test.Sdk from 17.8.0 to 17.9.0 (#145)

## 0.18.0

FEATURES:

- NEW: Added `Dnsimple.Registrar.CheckRegistrantChange` to retrieves the requirements of a registrant change. (#140)
- NEW: Added `Dnsimple.Registrar.GetRegistrantChange` to retrieves the details of an existing registrant change. (#140)
- NEW: Added `Dnsimple.Registrar.CreateRegistrantChange` to start registrant change. (#140)
- NEW: Added `Dnsimple.Registrar.ListRegistrantChanges` to lists the registrant changes for a domain. (#140)
- NEW: Added `Dnsimple.Registrar.DeleteRegistrantChange` to cancel an ongoing registrant change from the account. (#140)

- NEW: Added `Dnsimple.Registrar.EnableDomainTransferLock` to enable the domain transfer lock for a domain. (#142)
- NEW: Added `Dnsimple.Registrar.DisableDomainTransferLock` to disable the domain transfer lock for a domain. (#142)
- NEW: Added `Dnsimple.Registrar.GetDomainTransferLock` to get the domain transfer lock status for a domain. (#142)

## 0.17.0

ENHANCEMENTS:

- NEW: Added `Secondary`, `LastTransferredAt`, `Active` to `Zone` (dnsimple/dnsimple-csharp#138)

## 0.16.0

FEATURES:

- NEW: Added `Dnsimple.Billing.ListCharges` to list billing charges for an account. (dnsimple/dnsimple-csharp#133)

## 0.15.0

FEATURES:

- NEW: Added `Dnsimple.Zones.ActivateDns` to activate DNS services (resolution) for a zone. (dnsimple/dnsimple-csharp#128)
- NEW: Added `Dnsimple.Zones.DeactivateDns` to deactivate DNS services (resolution) for a zone. (dnsimple/dnsimple-csharp#128)

IMPROVEMENTS:

- `EmailForward` `From` is deprecated. Please use `AliasName` instead for creating email forwards. (dnsimple/dnsimple-csharp#128)
- `EmailForward` `To` is deprecated. Please use `DestinationEmail` instead for creating email forwards. (dnsimple/dnsimple-csharp#128)

## 0.14.0

- NEW: Adds getDomainRenewal and getDomainRegistration API endpoints (dnsimple/dnsimple-csharp#113)
- NEW: Support for issuing RSA certificates via the `signing_algorithm` param (dnsimple/dnsimple-developer#459)

## 0.13.6

- CHANGED: Deprecate Certificate's `contact_id` (dnsimple/dnsimple-csharp#85)
- CHANGED: Add getter for attribute errors in `DnsimpleValidationException` (dnsimple/dnsimple-csharp#96)

## 0.13.5

- FIXED: `Registrar.ChangeDomainDelegation` not serialising delegation list properly

## 0.13.1

- CHANGED: Bumped up dependency versions.

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
