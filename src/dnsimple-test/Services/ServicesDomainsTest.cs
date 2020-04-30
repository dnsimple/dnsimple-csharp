using NUnit.Framework;
using RestSharp;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class ServicesDomainsTest
    {
        private const string AppliedServicesFixture =
            "appliedServices/success.http";

        private const string ApplyServiceFixture = "applyService/success.http";

        private const string UnapplyServiceFixture =
            "unapplyService/success.http";

        [Test]
        [TestCase(1010, "1",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/1/services")]
        public void AppliedServices(long accountId, string domain,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(AppliedServicesFixture);
            var services = client.Services.AppliedServices(accountId, domain);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, services.Data.Count);
                Assert.AreEqual(30, services.PaginationData.PerPage);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes", "1",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/ruby.codes/services/1")]
        [TestCase(1010, "ruby.codes", "wordpress",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/ruby.codes/services/wordpress")]
        public void ApplyService(long accountId, string domain, string service,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(ApplyServiceFixture);
            client.Services.ApplyService(accountId, domain, service);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(Method.POST, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes", "1",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/ruby.codes/services/1")]
        [TestCase(1010, "ruby.codes", "wordpress",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/ruby.codes/services/wordpress")]
        public void UnapplyService(long accountId, string domain, string service, string expectedUrl)
        {
            var client = new MockDnsimpleClient(UnapplyServiceFixture);
            client.Services.UnapplyService(accountId, domain, service);
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(Method.DELETE, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
    }
}