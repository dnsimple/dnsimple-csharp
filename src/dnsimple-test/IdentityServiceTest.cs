using NUnit.Framework;

namespace dnsimple_test
{
    [TestFixture]
    public class IdentityServiceTest
    {
        [Test]
        public void WhoamiSuccessTest()
        {
            var client = new MockDnsimpleClient();

            var response = client.Identity.Whoami();
            var account = response.Data.Account;

            Assert.AreEqual(1,account.Id);
            Assert.AreEqual("example-account@example.com", account.Email);
            Assert.AreEqual("dnsimple-professional", account.PlanIdentifier);
            Assert.AreEqual("2015-09-18T23:04:37Z", account.CreatedAt);
            Assert.AreEqual("2016-06-09T20:03:39Z", account.UpdatedAt);
        }
    }
}