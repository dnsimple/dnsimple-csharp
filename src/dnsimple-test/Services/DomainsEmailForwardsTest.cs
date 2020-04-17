using System;
using System.Globalization;
using System.Linq;
using dnsimple.Services;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class DomainsEmailForwardsTest
    {
        private JToken _jToken;

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
            _jToken = JObject.Parse(loader.ExtractJsonPayload());
        }

        [Test]
        public void EmailForwardsData()
        {
            var emailForwards = new EmailForwardsData(_jToken).EmailForwards;

            Assert.Multiple(() =>
            {
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
        public void EmailForwardsResponse()
        {
            var response = new EmailForwardsResponse(_jToken);

            Assert.AreEqual(2, response.Data.EmailForwards.Count);
        }

        [Test]
        [TestCase(1010, "100")]
        [TestCase(1010, "example.com")]
        public void ListEmailForwards(long accountId, string domainIdentifier)
        {
            var client = new MockDnsimpleClient(ListEmailForwardsFixture);
            var emailForwards =
                client.Domains.ListEmailForwards(accountId, domainIdentifier);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, emailForwards.Data.EmailForwards.Count);
                Assert.AreEqual(1, emailForwards.Pagination.CurrentPage);
            });
        }

        [Test]
        [TestCase(1010, "228963")]
        [TestCase(1010, "example.com")]
        public void CreateEmailForward(long accountId, string domainIdentifier)
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
            });
        }

        [Test]
        [TestCase(1010, "228963")]
        [TestCase(1010, "example.com")]
        public void GetEmailForward(long accountId, string domainIdentifier)
        {
            var client = new MockDnsimpleClient(GetEmailForwardFixture);
            var emailForward =
                client.Domains.GetEmailForward(accountId, domainIdentifier,
                    17706).Data;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(17706, emailForward.Id);
                Assert.AreEqual(228963, emailForward.DomainId);
            });
        }

        [Test]
        [TestCase(1010, "228963")]
        [TestCase(1010, "example.com")]
        public void DeleteEmailForward(long accountId, string domainIdentifier)
        {
            var client = new MockDnsimpleClient(DeleteEmailForwardFixture);

            Assert.DoesNotThrow(() =>
            {
                client.Domains.DeleteEmailForward(accountId, domainIdentifier,
                    228963);
            });
        }
    }
}