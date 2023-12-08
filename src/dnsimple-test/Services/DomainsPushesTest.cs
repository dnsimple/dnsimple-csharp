using System;
using System.Globalization;
using NUnit.Framework;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class DomainsPushesTest
    {
        private const string InitiatePushFixture = "initiatePush/success.http";
        private const string ListPushesFixture = "listPushes/success.http";
        private const string AcceptPushFixture = "acceptPush/success.http";
        private const string RejectPushFixture = "rejectPush/success.http";

        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2016-08-11T10:16:03Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2016-08-11T10:16:03Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        [Test]
        [TestCase(1010, "100", "https://api.sandbox.dnsimple.com/v2/1010/domains/100/pushes")]
        [TestCase(1010, "example.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/pushes")]
        public void InitiatePush(long accountId, string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(InitiatePushFixture);
            var push = client.Domains.InitiatePush(accountId, domainIdentifier,
                "admin@target-account.test");

            Assert.Multiple(() =>
            {
                Assert.That(push.Data.Id, Is.EqualTo(1));
                Assert.That(push.Data.DomainId, Is.EqualTo(100));
                Assert.IsNull(push.Data.ContactId);
                Assert.That(push.Data.AccountId, Is.EqualTo(2020));
                Assert.That(push.Data.CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(push.Data.UpdatedAt, Is.EqualTo(UpdatedAt));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void InitiatePushValidationFail(string email)
        {
            var client = new MockDnsimpleClient(InitiatePushFixture);

            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Email cannot be null or empty"),
                delegate
                {
                    client.Domains.InitiatePush(1010, "100", email);

                });
        }

        [Test]
        [TestCase(1010, "https://api.sandbox.dnsimple.com/v2/1010/pushes")]
        [TestCase(1010, "https://api.sandbox.dnsimple.com/v2/1010/pushes")]
        public void ListPushes(long accountId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListPushesFixture);
            var pushes = client.Domains.ListPushes(accountId);

            Assert.Multiple(() =>
            {
                Assert.That(pushes.Data.Count, Is.EqualTo(2));
                Assert.That(pushes.Pagination.CurrentPage, Is.EqualTo(1));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, 208, 3, "https://api.sandbox.dnsimple.com/v2/1010/pushes/208")]
        public void AcceptPush(long accountId, long pushId, long contactId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(AcceptPushFixture);

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() =>
                {
                    client.Domains.AcceptPush(accountId, pushId, contactId);
                });

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase("https://api.sandbox.dnsimple.com/v2/1010/pushes/2")]
        public void RejectPush(string expectedUrl)
        {
            var client = new MockDnsimpleClient(RejectPushFixture);

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() =>
                {
                    client.Domains.RejectPush(1010, 2);
                });

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }
    }
}
