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
                Assert.AreEqual(123, accountsData.Data.First().Id);
                Assert.AreEqual("john@example.com",
                    accountsData.Data.First().Email);
                Assert.AreEqual("dnsimple-personal",
                    accountsData.Data.First().PlanIdentifier);
            });
        }

        [Test]
        public void AccountsResponse()
        {
            var accountsResponse = new ListResponse<Account>(_response);

            Assert.AreEqual(2, accountsResponse.Data.Count);
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
                Assert.AreEqual(456, lastAccount.Id);
                Assert.AreEqual("ops@company.com", lastAccount.Email);
                Assert.AreEqual("dnsimple-professional",
                    lastAccount.PlanIdentifier);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
    }
}