# Changelog

## master

- REMOVED: Removed PremiumPrice attribute from registrar order responses. Please do not rely on that attribute, as it returned an incorrect value. The attribute is going to be removed, and the API now returns a null value. The TransferDomain now accepts a DomainTransferInput object instead of a DomainTransfer. (dnsimple/dnsimple-csharp#7)
