namespace dnsimple.Services
{
    public readonly struct Paths
    {
        public static string CollaboratorsPath(long accountId,
            string domainIdentifier)
        {
            return $"{DomainPath(accountId, domainIdentifier)}/collaborators";
        }

        public static string DeleteDomainPath(long accountId,
            string domainIdentifier)
        {
            return $"{DomainsPath(accountId)}/{domainIdentifier}";
        }

        public static string DomainPath(long accountId,
            string domainIdentifier)
        {
            return $"/{accountId}/domains/{domainIdentifier}";
        }

        public static string DomainsPath(long accountId)
        {
            return $"/{accountId}/domains";
        }

        public static string CertificatesPath(long accountId, string domainName)
        {
            return $"{DomainPath(accountId, domainName)}/certificates";
        }

        public static string CertificatePath(long accountId, string domainName,
            long certificateId)
        {
            return
                $"{CertificatesPath(accountId, domainName)}/{certificateId}";
        }

        public static string PemCertificateDownloadPath(long accountId,
            string domainName, long certificateId)
        {
            return
                $"{CertificatePath(accountId, domainName, certificateId)}/download";
        }

        public static string CertificatePrivateKeyPath(long accountId,
            string domainName, long certificateId)
        {
            return
                $"{CertificatePath(accountId, domainName, certificateId)}/private_key";
        }

        public static string PurchaseLetsEncryptCertificatePath(long accountId,
            string domainName)
        {
            return $"{CertificatesPath(accountId, domainName)}/letsencrypt";
        }
        
        public static string IssueLetsEncryptCertificatePath(long accountId,
            string domainName, long certificateId)
        {
            return
                $"{PurchaseLetsEncryptCertificatePath(accountId, domainName)}/{certificateId}/issue";
        }

        public static string LetsEncryptCertificateRenewalPath(long accountId,
            string domainName, long certificateId)
        {
            return
                $"{PurchaseLetsEncryptCertificatePath(accountId, domainName)}/{certificateId}/renewals";
        }

        public static string IssueLetsEncryptCertificateRenewalPath(
            long accountId, string domainName, long certificateId,
            long certificateRenewalId)
        {
            return
                $"{LetsEncryptCertificateRenewalPath(accountId, domainName, certificateId)}/{certificateRenewalId}/issue";
        }
        
        public static string RemoveCollaboratorPath(long accountId,
            string domainIdentifier, long collaboratorId)
        {
            return
                $"{CollaboratorsPath(accountId, domainIdentifier)}/{collaboratorId}";
        }
        
        public static string DsRecordsPath(long accountId,
            string domainIdentifier)
        {
            return $"{DomainPath(accountId, domainIdentifier)}/ds_records";
        }

        public static string DsRecordPath(long accountId,
            string domainIdentifier, long recordId)
        {
            return $"{DsRecordsPath(accountId, domainIdentifier)}/{recordId}";
        }
        
        public static string DnssecPath(long accountId, string domainIdentifier)
        {
            return $"{DomainPath(accountId, domainIdentifier)}/dnssec";
        }
        
        public static string EmailForwardPath(long accountId, string domainIdentifier,
            int emailForwardId)
        {
            return
                $"{EmailForwardsPath(accountId, domainIdentifier)}/{emailForwardId}";
        }

        public static string EmailForwardsPath(long accountId,
            string domainIdentifier)
        {
            return $"{DomainPath(accountId, domainIdentifier)}/email_forwards";
        }
        
        public static string PushPath(long accountId, long pushId)
        {
            return $"{PushPath(accountId)}/{pushId}";
        }

        public static string InitiatePushPath(long accountId,
            string domainIdentifier)
        {
            return $"{DomainPath(accountId, domainIdentifier)}/pushes";
        }

        public static string PushPath(long accountId)
        {
            return $"{accountId}/pushes";
        }
        
        public static string DomainTransferOutPath(long accountId, string domainName)
        {
            return
                $"{RegistrarPath(accountId, domainName)}/authorize_transfer_out";
        }

        public static string DomainRenewalPath(long accountId, string domainName)
        {
            return $"{RegistrarPath(accountId, domainName)}/renewals";
        }

        public static string DomainTransferPath(long accountId, string domainName)
        {
            return $"{RegistrarPath(accountId, domainName)}/transfers";
        }

        public static string DomainRegistrationPath(long accountId,
            string domainName)
        {
            return $"{RegistrarPath(accountId, domainName)}/registrations";
        }

        public static string DomainCheckPath(long accountId, string domainName)
        {
            return $"{RegistrarPath(accountId, domainName)}/check";
        }

        public static string DomainPremiumPricePath(long accountId,
            string domainName)
        {
            return $"{RegistrarPath(accountId, domainName)}/premium_price";
        }

        public static string RegistrarPath(long accountId, string domainName)
        {
            return $"{accountId}/registrar/domains/{domainName}";
        }

        public static string TldsPath()
        {
            return "/tlds";
        }

        public static string GetTldPath(string tld)
        {
            return $"{TldsPath()}/{tld}";
        }
        
        public static string GetTldExtendedAttributesPath(string tld)
        {
            return $"{GetTldPath(tld)}/extended_attributes";
        }

        public static string ContactsPath(long accountId)
        {
            return $"/{accountId}/contacts";
        }

        public static string ContactPath(long accountId, long contactId)
        {
            return $"{ContactsPath(accountId)}/{contactId}";
        }

        public static string ServicesPath()
        {
            return "/services";
        }

        public static string ServicePath(string service)
        {
            return $"{ServicesPath()}/{service}";
        }

        public static string AppliedServicesPath(long accountId, string domain)
        {
            return $"{DomainPath(accountId, domain)}/services";
        }

        public static string ApplyServicePath(long accountId, string domain,
            string service)
        {
            return $"{AppliedServicesPath(accountId, domain)}/{service}";
        }

        public static string TemplatesPath(long accountId)
        {
            return $"/{accountId}/templates";
        }

        public static string TemplatePath(long account, string template)
        {
            return $"{TemplatesPath(account)}/{template}";
        }

        public static string TemplateRecordsPath(long accountId,
            string template)
        {
            return $"{TemplatePath(accountId, template)}/records";
        }

        public static string TemplateRecordPath(long accountId,
            string template, long recordId)
        {
            return $"{TemplateRecordsPath(accountId, template)}/{recordId}";
        }

        public static string TemplateDomainPath(long accountId, string domain,
            string template)
        {
            return $"{DomainPath(accountId, domain)}/templates/{template}";
        }

        public static string VanityNameServersPath(long accountId,
            string domain)
        {
            return $"/{accountId}/vanity/{domain}";
        }
        
        public static string AutoRenewalPath(long accountId, string domain)
        {
            return $"{RegistrarPath(accountId, domain)}/auto_renewal";
        }
        
        public static string VanityDelegationPath(long accountId, string domain)
        {
            return $"{DelegationPath(accountId, domain)}/vanity";
        }

        public static string DelegationPath(long accountId, string domain)
        {
            return $"{RegistrarPath(accountId, domain)}/delegation";
        }
        
        public static string WhoisRenewalPath(long accountId, string domain)
        {
            return $"{WhoisPrivacyPath(accountId, domain)}/renewals";
        }

        public static string WhoisPrivacyPath(long accountId, string domain)
        {
            return $"{RegistrarPath(accountId, domain)}/whois_privacy";
        }
        
        public static string ZoneRecordDistributionPath(long accountId,
            string zoneId, long recordId)
        {
            return
                $"{ZoneRecordPath(accountId, zoneId, recordId)}/distribution";
        }

        public static string ZoneRecordPath(long accountId, string zoneId,
            long recordId)
        {
            return $"{ZoneRecordsPath(accountId, zoneId)}/{recordId}";
        }

        public static string ZoneRecordsPath(long accountId, string zoneId)
        {
            return $"{ZonePath(accountId, zoneId)}/records";
        }
        
        public static string ZoneDistributionPath(long accountId,
            string zoneName)
        {
            return $"{ZonePath(accountId, zoneName)}/distribution";
        }

        public static string ZoneFilePath(long accountId, string zoneName)
        {
            return $"{ZonePath(accountId, zoneName)}/file";
        }

        public static string ZonePath(long accountId, string zoneName)
        {
            return $"{ZonesPath(accountId)}/{zoneName}";
        }

        public static string ZonesPath(long accountId)
        {
            return $"/{accountId}/zones";
        }
    }
}