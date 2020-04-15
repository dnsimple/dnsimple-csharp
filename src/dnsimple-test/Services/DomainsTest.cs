using System;
using System.Globalization;
using System.Linq;
using dnsimple.Services;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class DomainsTest
    {
        private JObject _jToken;
        private const string ListDomainsFixture = "listDomains/success.http";
        private const string GetDomainFixture = "getDomain/success.http";
        private const string CreateDomainFixture = "createDomain/created.http";
        private const string DeleteDomainFixture = "deleteDomain/success.http";

        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2014-12-06T15:56:55Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2015-12-09T00:20:56Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", ListDomainsFixture);
            _jToken = JObject.Parse(loader.ExtractJsonPayload());
        }

        [Test]
        public void DomainsData()
        {
            var domains = new DomainsData(_jToken).Domains;
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, domains.First().Id);
                Assert.AreEqual(1010, domains.First().AccountId);
                Assert.IsNull(domains.First().RegistrantId);
                Assert.AreEqual("example-alpha.com", domains.First().Name);
                Assert.AreEqual("example-alpha.com", domains.First().UnicodeName);
                Assert.AreEqual("hosted", domains.First().State);
                Assert.IsFalse(domains.First().AutoRenew);
                Assert.IsFalse(domains.First().PrivateWhois);
                Assert.IsNull(domains.First().ExpiresOn);
                Assert.AreEqual(CreatedAt, domains.First().CreatedAt);
                Assert.AreEqual(UpdatedAt, domains.First().UpdatedAt);
            });
        }
        
        [Test]
        public void DomainsResponse()
        {
            var response = new DomainsResponse(_jToken);
            
            Assert.AreEqual(2, response.Data.Domains.Count);
        }

        [Test]
        public void ListDomains()
        {
            var client = new MockDnsimpleClient(ListDomainsFixture);
            var domains = client.Domains.ListDomains(1010);
            
            var lastDomain = domains.Data.Domains.Last();
            var pagination = domains.Pagination;

            Assert.Multiple(()=>
            {
                Assert.AreEqual(2, lastDomain.Id);
                Assert.AreEqual(1010, lastDomain.AccountId);
                Assert.AreEqual(21, lastDomain.RegistrantId);
                
                Assert.AreEqual(1, pagination.CurrentPage);
                Assert.AreEqual(30, pagination.PerPage);
                Assert.AreEqual(2, pagination.TotalEntries);
                Assert.AreEqual(1, pagination.TotalPages);
            });
        }

        [Test]
        public void ListDomainsWithPagination()
        {
            var client = new MockDnsimpleClient(ListDomainsFixture);
            var domains = client.Domains.ListDomains(1010, 1, 1);
            
            Assert.AreEqual(2, domains.Data.Domains.Count);
        }

        [Test]
        [TestCase("1")]
        [TestCase("example-alpha.com")]
        public void GetDomain(string domainIdentifier)
        {
            var client = new MockDnsimpleClient(GetDomainFixture);
            var domain = client.Domains.GetDomain(1010, domainIdentifier).Data;
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, domain.Id);
                Assert.AreEqual(1010, domain.AccountId);
                Assert.IsNull(domain.RegistrantId);
                Assert.AreEqual("example-alpha.com", domain.Name);
                Assert.AreEqual("example-alpha.com", domain.UnicodeName);
                Assert.AreEqual("hosted", domain.State);
                Assert.IsFalse(domain.AutoRenew);
                Assert.IsFalse(domain.PrivateWhois);
                Assert.IsNull(domain.ExpiresOn);
                Assert.AreEqual(CreatedAt, domain.CreatedAt);
                Assert.AreEqual(UpdatedAt, domain.UpdatedAt);
            });
        }

        [Test]
        public void CreateDomain()
        {
            var client = new MockDnsimpleClient(CreateDomainFixture);
            var domain = client.Domains.CreateDomain(1010, "example-alpha.com").Data;
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, domain.Id);
                Assert.AreEqual(1010, domain.AccountId);
                Assert.IsNull(domain.RegistrantId);
                Assert.AreEqual("example-alpha.com", domain.Name);
                Assert.AreEqual("example-alpha.com", domain.UnicodeName);
                Assert.AreEqual("hosted", domain.State);
                Assert.IsFalse(domain.AutoRenew);
                Assert.IsFalse(domain.PrivateWhois);
                Assert.IsNull(domain.ExpiresOn);
                Assert.AreEqual(CreatedAt, domain.CreatedAt);
                Assert.AreEqual(UpdatedAt, domain.UpdatedAt);
            });
        }

        [Test]
        [TestCase(1010,"example-alpha.com")]
        [TestCase(1010, "1")]
        public void DeleteDomain(long accountId,string domainIdentifier)
        {
            var client = new MockDnsimpleClient(DeleteDomainFixture);
            
            Assert.DoesNotThrow(delegate
            {
                client.Domains.DeleteDomain(accountId, domainIdentifier);
            });
        }
    }
}