using System.Linq;
using dnsimple.Services;
using NUnit.Framework;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class AccountsTest
    {
        private MockResponse _response;

        [SetUp]
        public void Initialize()
        {
            var loader =
                new FixtureLoader("v2", "accounts/success-user.http");

            _response = new MockResponse(loader);
        }

        [Test]
        public void AccountsData()
        {
            var accountsData = new ListResponse<Account>(_response);

            Assert.Multiple(() =>
            {
                Assert.That(accountsData.Data.First().Id, Is.EqualTo(123));
                Assert.That(accountsData.Data.First().Email, Is.EqualTo("john@example.com"));
                Assert.That(accountsData.Data.First().PlanIdentifier, Is.EqualTo("dnsimple-personal"));
            });
        }

        [Test]
        public void AccountsResponse()
        {
            var accountsResponse = new ListResponse<Account>(_response);

            Assert.That(accountsResponse.Data.Count, Is.EqualTo(2));
        }

        [Test]
        [TestCase("https://api.sandbox.dnsimple.com/v2/accounts")]
        public void ReturnsAListOfAccountsForUser(string expectedUrl)
        {
            var client = new MockDnsimpleClient("accounts/success-user.http");

            var accounts = client.Accounts.List();

            var lastAccount = accounts.Data.Last();

            Assert.Multiple(() =>
            {
                Assert.That(lastAccount.Id, Is.EqualTo(456));
                Assert.That(lastAccount.Email, Is.EqualTo("ops@company.com"));
                Assert.That(lastAccount.PlanIdentifier, Is.EqualTo("dnsimple-professional"));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }
    }
}
