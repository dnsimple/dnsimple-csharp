using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using dnsimple;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using NUnit.Framework;
using Pagination = dnsimple.Services.ListOptions.Pagination;

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
            "2020-06-04T19:47:05Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2020-06-04T19:47:05Z", "yyyy-MM-ddTHH:mm:ssZ",
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
            var domains = new PaginatedResponse<Domain>(_response).Data;


            Assert.Multiple(() =>
            {
                Assert.That(domains.Count, Is.EqualTo(2));
                Assert.That(domains.First().Id, Is.EqualTo(181984));
                Assert.That(domains.First().AccountId, Is.EqualTo(1385));
                Assert.That(domains.First().RegistrantId, Is.EqualTo(2715));
                Assert.That(domains.First().Name, Is.EqualTo("example-alpha.com"));
                Assert.That(domains.First().UnicodeName, Is.EqualTo("example-alpha.com"));
                Assert.That(domains.First().State, Is.EqualTo("registered"));
                Assert.IsFalse(domains.First().AutoRenew);
                Assert.IsFalse(domains.First().PrivateWhois);
                Assert.That(domains.First().ExpiresAt, Is.EqualTo(Convert.ToDateTime("2021-06-05T02:15:00Z")));
                Assert.That(domains.First().CreatedAt, Is.EqualTo(Convert.ToDateTime("2020-06-04T19:15:14Z")));
                Assert.That(domains.First().UpdatedAt, Is.EqualTo(Convert.ToDateTime("2020-06-04T19:15:21Z")));
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
                Assert.That(domains.Data.Count, Is.EqualTo(2));
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(
            "https://api.sandbox.dnsimple.com/v2/1010/domains?sort=id:asc&name_like=example.com")]
        public void ListDomainsWithOptions(string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListDomainsFixture);
            var listOptions = new DomainListOptions();
            listOptions.FilterByName("example.com");
            listOptions.SortById(Order.asc);
            client.Domains.ListDomains(1010, listOptions);

            Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
        }

        [Test]
        [TestCase("181984", "https://api.sandbox.dnsimple.com/v2/1385/domains/181984")]
        [TestCase("example-alpha.com",
            "https://api.sandbox.dnsimple.com/v2/1385/domains/example-alpha.com")]
        public void GetDomain(string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetDomainFixture);
            var domain = client.Domains.GetDomain(1385, domainIdentifier).Data;

            Assert.Multiple(() =>
            {
                Assert.That(domain.Id, Is.EqualTo(181984));
                Assert.That(domain.AccountId, Is.EqualTo(1385));
                Assert.That(domain.RegistrantId, Is.EqualTo(2715));
                Assert.That(domain.Name, Is.EqualTo("example-alpha.com"));
                Assert.That(domain.UnicodeName, Is.EqualTo("example-alpha.com"));
                Assert.That(domain.State, Is.EqualTo("registered"));
                Assert.IsFalse(domain.AutoRenew);
                Assert.IsFalse(domain.PrivateWhois);
                Assert.That(domain.ExpiresAt, Is.EqualTo(Convert.ToDateTime("2021-06-05T02:15:00Z")));
                Assert.That(domain.CreatedAt, Is.EqualTo(Convert.ToDateTime("2020-06-04T19:15:14Z")));
                Assert.That(domain.UpdatedAt, Is.EqualTo(Convert.ToDateTime("2020-06-04T19:15:21Z")));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
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
        [TestCase("https://api.sandbox.dnsimple.com/v2/1385/domains")]
        public void CreateDomain(string expectedUrl)
        {
            var client = new MockDnsimpleClient(CreateDomainFixture);
            var domain = client.Domains.CreateDomain(1385, new Domain { Name = "example-beta.com" }).Data;

            Assert.Multiple(() =>
            {
                Assert.That(domain.Id, Is.EqualTo(181985));
                Assert.That(domain.AccountId, Is.EqualTo(1385));
                Assert.IsNull(domain.RegistrantId);
                Assert.That(domain.Name, Is.EqualTo("example-beta.com"));
                Assert.That(domain.UnicodeName, Is.EqualTo("example-beta.com"));
                Assert.That(domain.State, Is.EqualTo("hosted"));
                Assert.IsFalse(domain.AutoRenew);
                Assert.IsFalse(domain.PrivateWhois);
                Assert.IsNull(domain.ExpiresAt);
                Assert.That(domain.CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(domain.UpdatedAt, Is.EqualTo(UpdatedAt));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "1",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/1")]
        [TestCase(1010, "example-alpha.com",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/example-alpha.com")]
        public void DeleteDomain(long accountId, string domainIdentifier,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(DeleteDomainFixture);

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(delegate
                {
                    client.Domains.DeleteDomain(accountId,
                        domainIdentifier);
                });

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
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
            var sorting = new KeyValuePair<string, string>("sort",
                "id:asc,name:asc,expiration:desc");
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
                .SortByExpiration(Order.desc);


            Assert.Multiple(() =>
            {
                Assert.That(options.UnpackFilters(), Is.EqualTo(filters));
                Assert.That(options.UnpackSorting(), Is.EqualTo(sorting));
                Assert.That(options.UnpackPagination(), Is.EqualTo(pagination));
            });
        }
    }
}
