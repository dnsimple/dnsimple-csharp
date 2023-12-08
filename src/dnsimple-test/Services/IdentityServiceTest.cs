using System;
using System.Globalization;
using NUnit.Framework;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class IdentityServiceTest
    {
        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2015-09-18T23:04:37Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2016-06-09T20:03:39Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        [Test]
        [TestCase("https://api.sandbox.dnsimple.com/v2/whoami")]
        public void WhoamiAccountSuccessTest(string expectedUrl)
        {
            var client = new MockDnsimpleClient(
                "whoami/success-account.http");

            var response = client.Identity.Whoami();
            var account = response.Data.Account;

            Assert.Multiple(() =>
            {
                Assert.That(account.Id, Is.EqualTo(1));
                Assert.That(account.Email, Is.EqualTo("example-account@example.com"));
                Assert.That(account.PlanIdentifier, Is.EqualTo("dnsimple-professional"));
                Assert.That(account.CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(account.UpdatedAt, Is.EqualTo(UpdatedAt));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        public void WhoamiUserSuccessTest()
        {
            var client = new MockDnsimpleClient("whoami/success-user.http");

            var response = client.Identity.Whoami();
            var user = response.Data.User;

            Assert.Multiple(() =>
            {
                Assert.That(user.Id, Is.EqualTo(1));
                Assert.That(user.Email, Is.EqualTo("example-user@example.com"));
                Assert.That(user.CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(user.UpdatedAt, Is.EqualTo(UpdatedAt));
            });
        }
    }
}
