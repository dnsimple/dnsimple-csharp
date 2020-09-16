# CHANGELOG

This project uses [Semantic Versioning 2.0.0](http://semver.org/).

## master

- FIXED: Avoids setting `UserAgent` on `ChangeBaseUrlTo` if Client is provided. (dnsimple/dnsimple-csharp#22)
- CHANGED: `Contact.Fax` is not required anymore. (dnsimple/dnsimple-csharp#16)
- CHANGED: `Certificate.ExpiresOn` has been replaced by `Certificate.ExpiresAt`. (dnsimple/dnsimple-csharp#14)
- CHANGED: `Domain.ExpiresOn` has been replaced by `Domain.ExpiresAt`. (dnsimple/dnsimple-csharp#11)

## 0.9.0

Initial non-development release that supports API v2.
