using System;
using System.Globalization;
using System.Net;
using dnsimple;
using NUnit.Framework;
using RestSharp;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class RegistrarWhoisPrivacyTest
    {
        private const string GetWhoisPrivacyFixture =
            "getWhoisPrivacy/success.http";

        private const string EnableWhoisPrivacyFixture =
            "enableWhoisPrivacy/success.http";

        private const string PurchaseAndEnableWhoisPrivacyFixture =
            "enableWhoisPrivacy/created.http";

        private const string DisableWhoisPrivacyFixture =
            "disableWhoisPrivacy/success.http";

        private const string RenewWhoisPrivacyFixture =
            "renewWhoisPrivacy/success.http";
                                      
        private const string WhoisPrivacyDuplicatedOrderFixture =
            "renewWhoisPrivacy/whois-privacy-duplicated-order.http";
        
        private const string WhoisPrivacyNotFoundFixture =
            "renewWhoisPrivacy/whois-privacy-not-found.http";

        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2016-02-13T14:34:50Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2016-02-13T14:34:52Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);
        
        private DateTime ExpiresOn { get; } = DateTime.ParseExact(
            "2017-02-13", "yyyy-MM-dd",
            CultureInfo.CurrentCulture);
        
        private DateTime RenewalCreatedAt { get; } = DateTime.ParseExact(
            "2019-01-10T12:12:48Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime RenewalUpdatedAt { get; } = DateTime.ParseExact(
            "2019-01-10T12:12:48Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);
        
        private DateTime RenewalExpiresOn { get; } = DateTime.ParseExact(
            "2020-01-10", "yyyy-MM-dd",
            CultureInfo.CurrentCulture);
        
        [Test]
        [TestCase(1010, "42", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/42/whois_privacy")]
        [TestCase(1010, "ruby.codes", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/whois_privacy")]
        public void GetWhoisPrivacy(long accountId, string domain, string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetWhoisPrivacyFixture);

            var privacy = client.Registrar.GetWhoisPrivacy(accountId, domain).Data;
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, privacy.Id);
                Assert.AreEqual(2, privacy.DomainId);
                Assert.AreEqual(ExpiresOn, privacy.ExpiresOn);
                Assert.IsTrue(privacy.Enabled);
                Assert.AreEqual(CreatedAt, privacy.CreatedAt);
                Assert.AreEqual(UpdatedAt, privacy.UpdatedAt);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "42", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/42/whois_privacy")]
        [TestCase(1010, "ruby.codes", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/whois_privacy")]
        public void EnableWhoisPrivacy(long accountId, string domain, string expectedUrl)
        {
            var client = new MockDnsimpleClient(EnableWhoisPrivacyFixture);

            var privacy =
                client.Registrar.EnableWhoisPrivacy(accountId, domain).Data;
            
            Assert.Multiple(() =>
            {
                Assert.IsTrue(privacy.Enabled);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
                Assert.AreEqual(Method.PUT, client.HttpMethodUsed());
            });
        }
        
        [Test]
        [TestCase(1010, "42", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/42/whois_privacy")]
        [TestCase(1010, "ruby.codes", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/whois_privacy")]
        public void PurchaseAndEnableWhoisPrivacy(long accountId, string domain, string expectedUrl)
        {
            var client = new MockDnsimpleClient(PurchaseAndEnableWhoisPrivacyFixture);

            var privacy =
                client.Registrar.EnableWhoisPrivacy(accountId, domain).Data;
            
            Assert.Multiple(() =>
            {
                Assert.IsNull(privacy.Enabled);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
                Assert.AreEqual(Method.PUT, client.HttpMethodUsed());
            });
        }

        [Test]
        [TestCase(1010, "42", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/42/whois_privacy")]
        [TestCase(1010, "ruby.codes", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/whois_privacy")]
        public void DisableWhoisPrivacy(long accountId, string domain, string expectedUrl)
        {
            var client = new MockDnsimpleClient(DisableWhoisPrivacyFixture);

            var privacy =
                client.Registrar.DisableWhoisPrivacy(accountId, domain).Data;
            
            Assert.Multiple(() =>
            {
                Assert.IsFalse(privacy.Enabled);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
                Assert.AreEqual(Method.DELETE, client.HttpMethodUsed());
            });
        }

        [Test]
        [TestCase(1010, "100", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/100/whois_privacy/renewals")]
        [TestCase(1010, "ruby.codes", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/whois_privacy/renewals")]
        public void RenewWhoisPrivacy(long accountId, string domain, string expectedUrl)
        {
            var client = new MockDnsimpleClient(RenewWhoisPrivacyFixture);
            
            var renewedDomain =
                client.Registrar.RenewWhoisPrivacy(accountId, domain).Data;
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, renewedDomain.Id);
                Assert.AreEqual(100, renewedDomain.DomainId);
                Assert.AreEqual(999, renewedDomain.WhoisPrivacyId);
                Assert.AreEqual("new", renewedDomain.State);
                Assert.AreEqual(RenewalExpiresOn, renewedDomain.ExpiresOn);
                Assert.IsTrue(renewedDomain.Enabled);
                Assert.AreEqual(RenewalCreatedAt, renewedDomain.CreatedAt);
                Assert.AreEqual(RenewalUpdatedAt, renewedDomain.UpdatedAt);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
                Assert.AreEqual(Method.POST, client.HttpMethodUsed());
            });
        }

        [Test]
        [TestCase(1010, "100",
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/100/whois_privacy/renewals")]
        [TestCase(1010, "ruby.codes",
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/whois_privacy/renewals")]
        public void RenewWhoisPrivacyDuplicatedOrder(long accountId,
            string domain, string expectedUrl)
        {
            var client = new MockDnsimpleClient(WhoisPrivacyDuplicatedOrderFixture);
            client.StatusCode(HttpStatusCode.BadRequest);
            
            Assert.Multiple(() =>
            {
                Assert.Throws(
                    Is.TypeOf<DnsimpleValidationException>().And.Message.EqualTo("The whois privacy for example.com has just been renewed, a new renewal cannot be started at this time"),
                    delegate
                    {
                        client.Registrar.RenewWhoisPrivacy(accountId, domain);
                    });
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
                Assert.AreEqual(Method.POST, client.HttpMethodUsed());
            });
        }
        
        [Test]
        [TestCase(1010, "100")]
        [TestCase(1010, "ruby.codes")]
        public void RenewWhoisPrivacyDuplicatedOrder(long accountId,
            string domain)
        {
            var client = new MockDnsimpleClient(WhoisPrivacyNotFoundFixture);
            client.StatusCode(HttpStatusCode.BadRequest);
            
            Assert.Multiple(() =>
            {
                Assert.Throws(
                    Is.TypeOf<DnsimpleValidationException>().And.Message.EqualTo("WHOIS privacy not found for example.com"),
                    delegate
                    {
                        client.Registrar.RenewWhoisPrivacy(accountId, domain);
                    });
                
                Assert.AreEqual(Method.POST, client.HttpMethodUsed());
            });
        }
    }
}