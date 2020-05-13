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
                Assert.AreEqual(1, push.Data.Id);
                Assert.AreEqual(100, push.Data.DomainId);
                Assert.IsNull(push.Data.ContactId);
                Assert.AreEqual(2020, push.Data.AccountId);
                Assert.AreEqual(CreatedAt, push.Data.CreatedAt);
                Assert.AreEqual(UpdatedAt, push.Data.UpdatedAt);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                Assert.AreEqual(2, pushes.Data.Count);
                Assert.AreEqual(1, pushes.Pagination.CurrentPage);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
    }
}