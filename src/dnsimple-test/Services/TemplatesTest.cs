using System;
using System.Globalization;
using System.Linq;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using NUnit.Framework;
using RestSharp;
using Pagination = dnsimple.Services.ListOptions.Pagination;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class TemplatesTest
    {
        private MockResponse _response;

        private const string ListTemplatesFixture =
            "listTemplates/success.http";

        private const string CreateTemplateFixture =
            "createTemplate/created.http";

        private const string GetTemplateFixture = "getTemplate/success.http";

        private const string UpdateTemplateFixture =
            "updateTemplate/success.http";

        private const string DeleteTemplateFixture =
            "deleteTemplate/success.http";

        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2016-03-22T11:08:58Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2016-03-22T11:08:58Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", ListTemplatesFixture);
            _response = new MockResponse(loader);
        }

        [Test]
        public void ListTemplatesResponse()
        {
            var templates = new PaginatedResponse<Template>(_response);
            var template = templates.Data.First();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, template.Id);
                Assert.AreEqual(1010, template.AccountId);
                Assert.AreEqual("Alpha", template.Name);
                Assert.AreEqual("alpha", template.Sid);
                Assert.AreEqual("An alpha template.", template.Description);
                Assert.AreEqual(CreatedAt, template.CreatedAt);
                Assert.AreEqual(UpdatedAt, template.UpdatedAt);

            });
        }

        [Test]
        [TestCase(1010, "https://api.sandbox.dnsimple.com/v2/1010/templates")]
        public void ListTemplates(long accountId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListTemplatesFixture);
            var templates = client.Templates.ListTemplates(accountId);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(templates.Pagination.TotalEntries,
                    templates.Data.Count);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "https://api.sandbox.dnsimple.com/v2/1010/templates?sort=id:asc,name:desc,sid:asc&per_page=42&page=7")]
        public void ListTemplatesSorted(long accountId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListTemplatesFixture);
            var options = new ListTemplatesOptions
                {
                    Pagination = new Pagination
                    {
                        PerPage = 42,
                        Page = 7
                    }

                }.SortById(Order.asc)
                .SortByName(Order.desc)
                .SortBySid(Order.asc);

            client.Templates.ListTemplates(accountId, options);

            Assert.AreEqual(expectedUrl, client.RequestSentTo());
        }

        [Test]
        [TestCase(1010, "https://api.sandbox.dnsimple.com/v2/1010/templates")]
        public void CreateTemplate(long accountId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(CreateTemplateFixture);
            var templateData = new Template
            {
                Name = "Beta",
                Sid = "beta",
                Description = "A beta template."
            };

            var template =
                client.Templates.CreateTemplate(accountId, templateData).Data;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, template.Id);
                Assert.AreEqual(accountId, template.AccountId);
                Assert.AreEqual(templateData.Name, template.Name);
                Assert.AreEqual(templateData.Sid, template.Sid);
                Assert.AreEqual(templateData.Description, template.Description);

                Assert.AreEqual(Method.POST, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "1", "https://api.sandbox.dnsimple.com/v2/1010/templates/1")]
        [TestCase(1010, "alpha", "https://api.sandbox.dnsimple.com/v2/1010/templates/alpha")]
        public void GetTemplate(long accountId, string templateIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetTemplateFixture);
            var template = client.Templates
                .GetTemplate(accountId, templateIdentifier).Data;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, template.Id);
                Assert.AreEqual(accountId, template.AccountId);
                Assert.AreEqual("Alpha", template.Name);
                Assert.AreEqual("alpha", template.Sid);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "1", "https://api.sandbox.dnsimple.com/v2/1010/templates/1")]
        [TestCase(1010, "alpha", "https://api.sandbox.dnsimple.com/v2/1010/templates/alpha")]
        public void UpdateTemplate(long accountId, string template, string expectedUrl)
        {
            var client = new MockDnsimpleClient(UpdateTemplateFixture);
            var update = new Template
            {
                Description = "An alpha template"
            };

            client.Templates.UpdateTemplate(accountId, template, update);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(Method.PATCH, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "1", "https://api.sandbox.dnsimple.com/v2/1010/templates/1")]
        [TestCase(1010, "alpha", "https://api.sandbox.dnsimple.com/v2/1010/templates/alpha")]
        public void DeleteTemplate(long accountId, string template, string expectedUrl)
        {
            var client = new MockDnsimpleClient(DeleteTemplateFixture);

            client.Templates.DeleteTemplate(accountId, template);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(Method.DELETE, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
    }
}
