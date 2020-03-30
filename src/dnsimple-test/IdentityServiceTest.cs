using System;
using NUnit.Framework;

namespace dnsimple_test
{
    [TestFixture]
    public class IdentityServiceTest
    {
        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2015-09-18T23:04:37Z", "yyyy-MM-ddTHH:mm:ssZ",
            System.Globalization.CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2016-06-09T20:03:39Z", "yyyy-MM-ddTHH:mm:ssZ",
            System.Globalization.CultureInfo.CurrentCulture);

        [Test]
        public void WhoamiAccountSuccessTest()
        {
            var client = new MockDnsimpleClient
            {
                Fixture = "whoami/success-account.http"
            };

            var response = client.Identity.Whoami();
            var account = response.Data.Account;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, account.Id);
                Assert.AreEqual("example-account@example.com", account.Email);
                Assert.AreEqual("dnsimple-professional",
                    account.PlanIdentifier);
                Assert.AreEqual(CreatedAt, account.CreatedAt);
                Assert.AreEqual(UpdatedAt, account.UpdatedAt);
            });
        }

        [Test]
        public void WhoamiUserSuccessTest()
        {
            var client = new MockDnsimpleClient
            {
                Fixture = "whoami/success-user.http"
            };

            var response = client.Identity.Whoami();
            var user = response.Data.User;
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, user.Id);
                Assert.AreEqual("example-user@example.com", user.Email);
                Assert.AreEqual(CreatedAt, user.CreatedAt);
                Assert.AreEqual(UpdatedAt, user.UpdatedAt);
            });
        }
    }
}