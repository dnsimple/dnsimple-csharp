using System;
using System.Globalization;
using NUnit.Framework;
using RestSharp;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class RegistrarWhoisPrivacyTest
    {
        private const string EnableWhoisPrivacyFixture =
            "enableWhoisPrivacy/success.http";

        private const string PurchaseAndEnableWhoisPrivacyFixture =
            "enableWhoisPrivacy/created.http";

        private const string DisableWhoisPrivacyFixture =
            "disableWhoisPrivacy/success.http";

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
                Assert.That(privacy.Enabled, Is.True);

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.PUT));
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
                Assert.That(privacy.Enabled, Is.Null);

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.PUT));
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
                Assert.That(privacy.Enabled, Is.False);

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.DELETE));
            });
        }
    }
}
