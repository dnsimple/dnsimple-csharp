using NUnit.Framework;
using RestSharp;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class RegistrarAutoRenewalTest
    {

        private const string EnableDomainAutoRenewalFixture =
            "enableDomainAutoRenewal/success.http";

        private const string DisableDomainAutoRenewalFixture =
            "disableDomainAutoRenewal/success.http";

        [Test]
        [TestCase(1010, "42", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/42/auto_renewal")]
        [TestCase(1010, "ruby.codes", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/auto_renewal")]
        public void EnableDomainAutoRenewal(long accountId, string domain, string expectedUrl)
        {
            var client = new MockDnsimpleClient(EnableDomainAutoRenewalFixture);

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(delegate
                {
                    client.Registrar.EnableDomainAutoRenewal(accountId, domain);
                });
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.PUT));
            });
        }

        [Test]
        [TestCase(1010, "42", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/42/auto_renewal")]
        [TestCase(1010, "ruby.codes", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/auto_renewal")]
        public void DisableDomainAutoRenewal(long accountId, string domain, string expectedUrl)
        {
            var client = new MockDnsimpleClient(DisableDomainAutoRenewalFixture);

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(delegate
                {
                    client.Registrar.DisableDomainAutoRenewal(accountId, domain);
                });
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.DELETE));
            });
        }
    }
}
