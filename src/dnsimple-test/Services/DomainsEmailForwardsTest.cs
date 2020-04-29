using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class DomainsEmailForwardsTest
    {
        private JToken _jToken;
        private IRestResponse _response;

        private const string ListEmailForwardsFixture =
            "listEmailForwards/success.http";

        private const string CreateEmailForwardFixture =
            "createEmailForward/created.http";

        private const string GetEmailForwardFixture =
            "getEmailForward/success.http";

        private const string DeleteEmailForwardFixture =
            "deleteEmailForward/success.http";

        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2016-02-04T13:59:29Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2016-02-04T13:59:29Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", ListEmailForwardsFixture);
            _response = new RestResponse();
            _response.Content = loader.ExtractJsonPayload();
            _jToken = JObject.Parse(loader.ExtractJsonPayload());
        }

        [Test]
        public void EmailForwardsData()
        {
            var emailForwards = new PaginatedDnsimpleResponse<EmailForward>(_response).Data;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, emailForwards.Count);
                Assert.AreEqual(17702, emailForwards.First().Id);
                Assert.AreEqual(228963, emailForwards.First().DomainId);
                Assert.AreEqual(".*@a-domain.com", emailForwards.First().From);
                Assert.AreEqual("jane.smith@example.com",
                    emailForwards.First().To);
                Assert.AreEqual(CreatedAt, emailForwards.First().CreatedAt);
                Assert.AreEqual(UpdatedAt, emailForwards.First().UpdatedAt);
            });
        }

        [Test]
        [TestCase(1010, "100", "https://api.sandbox.dnsimple.com/v2/1010/domains/100/email_forwards")]
        [TestCase(1010, "example.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/email_forwards")]
        public void ListEmailForwards(long accountId, string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListEmailForwardsFixture);
            var emailForwards =
                client.Domains.ListEmailForwards(accountId, domainIdentifier);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, emailForwards.Data.Count);
                Assert.AreEqual(1, emailForwards.PaginationData.CurrentPage);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
        
        [Test]
        [TestCase(1010, "100", "https://api.sandbox.dnsimple.com/v2/1010/domains/100/email_forwards?sort=from:asc")]
        [TestCase(1010, "example.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/email_forwards?sort=from:asc")]
        public void ListEmailForwardsWithOptions(long accountId, string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListEmailForwardsFixture);
            var options = new DomainEmailForwardsListOptions();
            options.SortByFrom(Order.asc);
            
            var emailForwards =
                client.Domains.ListEmailForwards(accountId, domainIdentifier, options);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, emailForwards.Data.Count);
                Assert.AreEqual(1, emailForwards.PaginationData.CurrentPage);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "228963", "https://api.sandbox.dnsimple.com/v2/1010/domains/228963/email_forwards")]
        [TestCase(1010, "example.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/email_forwards")]
        public void CreateEmailForward(long accountId, string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(CreateEmailForwardFixture);
            var record = new EmailForward
            {
                DomainId = 228963,
                From = "jim@a-domain.com",
                To = "jim@another.com"
            };

            var created =
                client.Domains.CreateEmailForward(accountId, domainIdentifier,
                    record);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(17706, created.Data.Id);
                Assert.AreEqual(record.DomainId, created.Data.DomainId);
                Assert.AreEqual(record.From, created.Data.From);
                Assert.AreEqual(record.To, created.Data.To);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "228963","https://api.sandbox.dnsimple.com/v2/1010/domains/228963/email_forwards/17706")]
        [TestCase(1010, "example.com","https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/email_forwards/17706")]
        public void GetEmailForward(long accountId, string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetEmailForwardFixture);
            var emailForward =
                client.Domains.GetEmailForward(accountId, domainIdentifier,
                    17706).Data;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(17706, emailForward.Id);
                Assert.AreEqual(228963, emailForward.DomainId);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "100", "https://api.sandbox.dnsimple.com/v2/1010/domains/100/email_forwards/228963")]
        [TestCase(1010, "example.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/email_forwards/228963")]
        public void DeleteEmailForward(long accountId, string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(DeleteEmailForwardFixture);

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() =>
                {
                    client.Domains.DeleteEmailForward(accountId, domainIdentifier,
                        228963);
                });
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
            
        }

        [Test]
        public void DomainEmailForwardsListOptions()
        {
            var sorting = new KeyValuePair<string, string>("sort", "id:asc,from:asc,to:desc");
            var pagination = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("per_page", "42"),
                new KeyValuePair<string, string>("page", "7")
            };

            var options = new DomainEmailForwardsListOptions
            {
                Pagination = new Pagination
                {
                    Page = 7,
                    PerPage = 42
                }
            };
            options.SortById(Order.asc).SortByFrom(Order.asc)
                .SortByTo(Order.desc);
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(sorting, options.UnpackSorting());
                Assert.AreEqual(pagination, options.UnpackPagination());
            });
        }
    }
}