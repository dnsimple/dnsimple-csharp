using System.Linq;
using dnsimple.Services;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class AccountsTest
    {
        private JToken _jToken;

        [SetUp]
        public void Initialize()
        {
            var loader =
                new FixtureLoader("v2", "accounts/success-user.http");
                
            _jToken = JObject.Parse(loader.ExtractJsonPayload());
        }

        [Test]
        public void AccountsData()
        {
            var accountsData = new AccountsData(_jToken);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(123, accountsData.Accounts.First().Id);
                Assert.AreEqual("john@example.com",
                    accountsData.Accounts.First().Email);
                Assert.AreEqual("dnsimple-personal",
                    accountsData.Accounts.First().PlanIdentifier);
            });
        }

        [Test]
        public void AccountsResponse()
        {
            var accountsResponse = new AccountsResponse(_jToken);

            Assert.AreEqual(2, accountsResponse.Data.Accounts.Count);
        }

        [Test]
        public void ReturnsAListOfAccountsForUser()
        {
            var client = new MockDnsimpleClient("accounts/success-user.http");

            var accounts = client.Accounts.List();
            var lastAccount = accounts.Data.Accounts.Last();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(456, lastAccount.Id);
                Assert.AreEqual("ops@company.com", lastAccount.Email);
                Assert.AreEqual("dnsimple-professional",
                    lastAccount.PlanIdentifier);
            });
        }
    }
}