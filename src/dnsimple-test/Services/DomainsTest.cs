using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using dnsimple;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using NUnit.Framework;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class DomainsTest
    {
        private MockResponse _response;
        private const string ListDomainsFixture = "listDomains/success.http";
        private const string GetDomainFixture = "getDomain/success.http";
        private const string CreateDomainFixture = "createDomain/created.http";
        private const string DeleteDomainFixture = "deleteDomain/success.http";
        private const string GetDomainNotFoundFixture = "notfound-domain.http";

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
            _response = new MockResponse(loader);
        }

        [Test]
        public void DomainsResponse()
        {
            var domains = new PaginatedDnsimpleResponse<Domain>(_response).Data;

            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, domains.Count);
                Assert.AreEqual(1, domains.First().Id);
                Assert.AreEqual(1010, domains.First().AccountId);
                Assert.IsNull(domains.First().RegistrantId);
                Assert.AreEqual("example-alpha.com", domains.First().Name);
                Assert.AreEqual("example-alpha.com",
                    domains.First().UnicodeName);
                Assert.AreEqual("hosted", domains.First().State);
                Assert.IsFalse(domains.First().AutoRenew);
                Assert.IsFalse(domains.First().PrivateWhois);
                Assert.IsNull(domains.First().ExpiresOn);
                Assert.AreEqual(CreatedAt, domains.First().CreatedAt);
                Assert.AreEqual(UpdatedAt, domains.First().UpdatedAt);
            });
        }

        [Test]
        [TestCase("https://api.sandbox.dnsimple.com/v2/1010/domains")]
        public void ListDomains(string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListDomainsFixture);
            var domains = client.Domains.ListDomains(1010);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, domains.Data.Count);
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase("https://api.sandbox.dnsimple.com/v2/1010/domains?sort=id:asc&name_like=example.com")]
        public void ListDomainsWithOptions(string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListDomainsFixture);
            var listOptions = new DomainListOptions();
            listOptions.FilterByName("example.com");
            listOptions.SortById(Order.asc);
            client.Domains.ListDomains(1010, listOptions);

            Assert.AreEqual(expectedUrl, client.RequestSentTo());
        }

        [Test]
        [TestCase("1", "https://api.sandbox.dnsimple.com/v2/1010/domains/1")]
        [TestCase("example-alpha.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example-alpha.com")]
        public void GetDomain(string domainIdentifier, string expectedUrl)
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
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase("0")]
        public void GetDomainNotFound(string domainIdentifier)
        {
            var client = new MockDnsimpleClient(GetDomainNotFoundFixture);
            client.StatusCode(HttpStatusCode.NotFound);

            Assert.Throws(
                Is.TypeOf<NotFoundException>().And.Message
                    .EqualTo("Domain `0` not found"),
                delegate { client.Domains.GetDomain(1010, domainIdentifier); });
        }

        [Test]
        [TestCase("https://api.sandbox.dnsimple.com/v2/1010/domains")]
        public void CreateDomain(string expectedUrl)
        {
            var client = new MockDnsimpleClient(CreateDomainFixture);
            var domain = client.Domains.CreateDomain(1010, "example-alpha.com")
                .Data;

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
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010,"1", "https://api.sandbox.dnsimple.com/v2/1010/domains/1")]
        [TestCase(1010,"example-alpha.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example-alpha.com")]
        public void DeleteDomain(long accountId, string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(DeleteDomainFixture);

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(delegate
                {
                    client.Domains.DeleteDomain(accountId, domainIdentifier);
                });
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
            
        }

        [Test]
        public void DomainListOptions()
        {
            var filters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("name_like", "example.com"),
                new KeyValuePair<string, string>("registrant_id", "89")
            };
            var sorting = new KeyValuePair<string, string>("sort", "id:asc,name:asc,expires_on:desc");
            var pagination = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("per_page", "42"),
                new KeyValuePair<string, string>("page", "7")
            };

            var options = new DomainListOptions
                {
                    Pagination = new Pagination
                    {
                        PerPage = 42,
                        Page = 7
                    }
                }.FilterByName("example.com")
                .FilterByRegistrantId(89)
                .SortById(Order.asc)
                .SortByName(Order.asc)
                .SortByExpiresOn(Order.desc);


            Assert.Multiple(() =>
            {
                Assert.AreEqual(filters, options.UnpackFilters());
                Assert.AreEqual(sorting, options.UnpackSorting());
                Assert.AreEqual(pagination, options.UnpackPagination());
            });
        }
    }
}