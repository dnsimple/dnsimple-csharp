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
        [TestCase(1010, "100")]
        [TestCase(1010, "example.com")]
        public void InitiatePush(long accountId, string domainIdentifier)
        {
            var client = new MockDnsimpleClient(InitiatePushFixture);
            var push = client.Domains.InitiatePush(accountId, domainIdentifier,
                "admin@target-account.test");
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, push.Data.Id);
                Assert.AreEqual(100, push.Data.DomainId);
                Assert.IsNull(push.Data.ContactId);
                Assert.AreEqual(2020, push.Data.AccountId);
                Assert.AreEqual(CreatedAt, push.Data.CreatedAt);
                Assert.AreEqual(UpdatedAt, push.Data.UpdatedAt);
            });
        }

        [Test]
        [TestCase(1010)]
        [TestCase(1010)]
        public void ListPushes(long accountId)
        {
            var client = new MockDnsimpleClient(ListPushesFixture);
            var pushes = client.Domains.ListPushes(accountId);
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, pushes.Data.Pushes.Count);
                Assert.AreEqual(1, pushes.Pagination.CurrentPage);
            });
        }

        [Test]
        [TestCase(1010, 208, 3)]
        public void AcceptPush(long accountId, long pushId, long contactId)
        {
            var client = new MockDnsimpleClient(AcceptPushFixture);
            
            Assert.DoesNotThrow(() =>
            {
                client.Domains.AcceptPush(accountId, pushId, contactId);
            });
        }

        [Test]
        public void RejectPush()
        {
            var client = new MockDnsimpleClient(RejectPushFixture);
            
            Assert.DoesNotThrow(() =>
            {
                client.Domains.RejectPush(1010, 2);
            });
        }
    }
}