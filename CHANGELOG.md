# CHANGELOG

This project uses [Semantic Versioning 2.0.0](http://semver.org/).

## main

### Removed

- Removed deprecated `GetWhoisPrivacy` (dnsimple/dnsimple-developer#919)
- Removed deprecated `RenewWhoisPrivacy` (dnsimple/dnsimple-developer#919)

## 0.20.0 - 2025-08-20

### Changed

- Remove from and to from EmailForward model (#185)
- Add active attribute to EmailForward model (#183)
- Remove domain collaborators (#182)

### Housekeeping

- Bump Microsoft.NET.Test.Sdk from 17.13.0 to 17.14.1 (#174)
- Bump NUnit.Analyzers from 4.7.0 to 4.10.0 (#175)
- Bump NUnit from 4.3.2 to 4.4.0 (#176)
- Bump NUnit3TestAdapter from 5.0.0 to 5.1.0 (#172)
- Sync test fixtures as of 2025-08-19 03:48:16 (#184)

## 0.19.1 - 2025-04-30

### Housekeeping

- Bump NUnit3TestAdapter from 4.5.0 to 5.0.0 (#165)
- Bump Microsoft.NET.Test.Sdk and Newtonsoft.Json (#164)
- Bump NUnit.Analyzers from 4.4.0 to 4.6.0 (#163)
- Bump nosborn/github-action-markdown-cli from 3.3.0 to 3.4.0 (#162)
- Bump Moq from 4.20.70 to 4.20.72 (#160)
- Bump nunit from 4.1.0 to 4.3.2 (#156)

## 0.19.0 - 2024-12-06

### Added

- Added `AliasEmail` to `EmailForward`

### Changed

- Bumped `dotnet` to `9.0`

### Deprecated

- Deprecated `From` and `To` fields in `EmailForward`
- `DomainCollaborators` have been deprecated and will be removed in the next major version. Please use our Domain Access Control feature.

### Housekeeping

- Bump Microsoft.NET.Test.Sdk from 17.9.0 to 17.10.0 (#152)
- Bump NUnit.Analyzers from 4.0.1 to 4.4.0 (#153)

## 0.18.1 - 2024-03-12

### Housekeeping

- Bump nunit from 4.0.1 to 4.1.0 (#147)
- Bump NUnit.Analyzers from 3.10.0 to 4.0.1 (#146)
- Bump Microsoft.NET.Test.Sdk from 17.8.0 to 17.9.0 (#145)

## 0.18.0 - 2024-01-16

### Added

- Added `Dnsimple.Registrar.CheckRegistrantChange` to retrieves the requirements of a registrant change. (#140)
- Added `Dnsimple.Registrar.GetRegistrantChange` to retrieves the details of an existing registrant change. (#140)
- Added `Dnsimple.Registrar.CreateRegistrantChange` to start registrant change. (#140)
- Added `Dnsimple.Registrar.ListRegistrantChanges` to lists the registrant changes for a domain. (#140)
- Added `Dnsimple.Registrar.DeleteRegistrantChange` to cancel an ongoing registrant change from the account. (#140)
- Added `Dnsimple.Registrar.EnableDomainTransferLock` to enable the domain transfer lock for a domain. (#142)
- Added `Dnsimple.Registrar.DisableDomainTransferLock` to disable the domain transfer lock for a domain. (#142)
- Added `Dnsimple.Registrar.GetDomainTransferLock` to get the domain transfer lock status for a domain. (#142)

## 0.17.0 - 2023-12-12

### Added

- Added `Secondary`, `LastTransferredAt`, `Active` to `Zone` (#138)

## 0.16.0 - 2023-12-06

### Added

- Added `Dnsimple.Billing.ListCharges` to list billing charges for an account. (#133)

## 0.15.0 - 2023-08-10

### Added

- Added `Dnsimple.Zones.ActivateDns` to activate DNS services (resolution) for a zone. (#128)
- Added `Dnsimple.Zones.DeactivateDns` to deactivate DNS services (resolution) for a zone. (#128)

### Deprecated

- `EmailForward` `From` is deprecated. Please use `AliasName` instead for creating email forwards. (#128)
- `EmailForward` `To` is deprecated. Please use `DestinationEmail` instead for creating email forwards. (#128)

## 0.14.0 - 2023-03-03

### Added

- Adds getDomainRenewal and getDomainRegistration API endpoints (#113)
- Support for issuing RSA certificates via the `signing_algorithm` param (dnsimple/dnsimple-developer#459)

## 0.13.6 - 2022-09-20

### Deprecated

- Deprecate Certificate's `contact_id` (#85)

### Changed

- Add getter for attribute errors in `DnsimpleValidationException` (#96)

## 0.13.5 - 2022-02-15

### Fixed

- `Registrar.ChangeDomainDelegation` not serialising delegation list properly

## 0.13.1 - 2022-02-10

### Changed

- Bumped up dependency versions.

## 0.13.0 - 2021-11-04

### Changed

- Added support for DS record key-data interface (#58)

## 0.12.0 - 2021-05-19

### Removed

- Removed deprecated `Registrar.GetDomainPremiumPrices`

## 0.11.0 - 2021-04-22

### Added

- Added `Registrar.GetDomainPrices` to retrieve whether a domain is premium and the prices to register, transfer, and renew. (#46)

## 0.10.0 - 2021-04-01

### Fixed

- Adds missing CDNSKEY & CDS record types (#32). (#32)
- Avoids setting `UserAgent` on `ChangeBaseUrlTo` if Client is provided. (#22)

### Changed

- Moves / Migrates project to .NET standard (#23)
- `Contact.Fax` is not required anymore. (#16)
- `Certificate.ExpiresOn` has been replaced by `Certificate.ExpiresAt`. (#14)
- `Domain.ExpiresOn` has been replaced by `Domain.ExpiresAt`. (#11)

## 0.9.0 - 2020-06-18

Initial non-development release that supports API v2.
