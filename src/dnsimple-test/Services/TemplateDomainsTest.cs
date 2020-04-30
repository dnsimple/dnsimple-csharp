using NUnit.Framework;
using RestSharp;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class TemplateDomainsTest
    {
        private const string ApplyTemplateFixture =
            "applyTemplate/success.http";
        
        [Test]
        [TestCase(1010, "ruby.codes", "12", "https://api.sandbox.dnsimple.com/v2/1010/domains/ruby.codes/templates/12")]
        public void ApplyTemplate(long accountId, string domain, string template, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ApplyTemplateFixture);

            client.Templates.ApplyTemplate(accountId, domain, template);
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(Method.POST, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
    }
}