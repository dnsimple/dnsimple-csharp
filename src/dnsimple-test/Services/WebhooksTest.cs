using System.Linq;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using NUnit.Framework;
using RestSharp;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class WebhooksTest
    {
        private MockResponse _response;
        
        private const string ListWebhooksFixture = "listWebhooks/success.http";

        private const string CreateWebhookFixture =
            "createWebhook/created.http";

        private const string GetWebhookFixture = "getWebhook/success.http";

        private const string DeleteWebhookFixture =
            "deleteWebhook/success.http";

        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", ListWebhooksFixture);
            _response = new MockResponse(loader);
        }
        
        [Test]
        public void WebhooksResponse()
        {
            var webhooks = new ListDnsimpleResponse<Webhook>(_response);
            var webhook = webhooks.Data.First();
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, webhook.Id);
                Assert.AreEqual("https://webhook.test", webhook.Url);
            });
        }
        
        [Test]
        [TestCase(1010, "https://api.sandbox.dnsimple.com/v2/1010/webhooks")]
        public void ListWebhooks(long accountId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListWebhooksFixture);
            var webhooks = client.Webhooks.ListWebhooks(accountId).Data;
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, webhooks.Count);
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
        
        [Test]
        [TestCase(1010, "https://api.sandbox.dnsimple.com/v2/1010/webhooks?sort=id:asc&per_page=42&page=7")]
        public void ListWebhooksSorted(long accountId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListWebhooksFixture);
            var options = new ListWebhooksOptions
            {
                Pagination = new Pagination
                {
                    PerPage = 42,
                    Page = 7
                }
            }.SortById(Order.asc);
            
            client.Webhooks.ListWebhooks(accountId, options);
            
            Assert.AreEqual(expectedUrl, client.RequestSentTo());
        }

        [Test]
        [TestCase(1010, "https://api.sandbox.dnsimple.com/v2/1010/webhooks")]
        public void CreateWebhook(long accountId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(CreateWebhookFixture);
            var hook = new Webhook {Url = "https://webhook.test"};
            var created = client.Webhooks.CreateWebhook(accountId, hook).Data;
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(hook.Url, created.Url);
                
                Assert.AreEqual(Method.POST, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, 12,"https://api.sandbox.dnsimple.com/v2/1010/webhooks/12")]
        public void GetWebhook(long accountId, long webhookId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetWebhookFixture);
            var webhook = client.Webhooks.GetWebhook(accountId, webhookId).Data;
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, webhook.Id);
                Assert.AreEqual("https://webhook.test", webhook.Url);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, 12,"https://api.sandbox.dnsimple.com/v2/1010/webhooks/12")]
        public void DeleteWebhook(long accountId, long webhookId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(DeleteWebhookFixture);
            client.Webhooks.DeleteWebhook(accountId, webhookId);
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(Method.DELETE, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
    }
}