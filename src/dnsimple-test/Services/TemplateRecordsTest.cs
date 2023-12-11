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
    public class TemplateRecordsTest
    {
        private MockResponse _response;

        private const string ListTemplateRecordsFixture =
            "listTemplateRecords/success.http";

        private const string CreateTemplateRecordFixture =
            "createTemplateRecord/created.http";

        private const string GetTemplateRecordFixture =
            "getTemplateRecord/success.http";

        private const string DeleteTemplateRecordFixture =
            "deleteTemplateRecord/success.http";

        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2016-04-26T08:23:54Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2016-04-26T08:23:54Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", ListTemplateRecordsFixture);
            _response = new MockResponse(loader);
        }

        [Test]
        public void ListTemplateRecordsResponse()
        {
            var records =
                new PaginatedResponse<TemplateRecord>(_response);
            var record = records.Data.First();

            Assert.Multiple(() =>
            {
                Assert.That(record.Id, Is.EqualTo(296));
                Assert.That(record.TemplateId, Is.EqualTo(268));
                Assert.That(record.Name, Is.Empty);
                Assert.That(record.Content, Is.EqualTo("192.168.1.1"));
                Assert.That(record.Ttl, Is.EqualTo(3600));
                Assert.That(record.Priority, Is.Null);
                Assert.That(record.Type, Is.EqualTo("A"));
                Assert.That(record.CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(record.UpdatedAt, Is.EqualTo(UpdatedAt));
            });
        }

        [Test]
        [TestCase(1010, "268",
            "https://api.sandbox.dnsimple.com/v2/1010/templates/268/records")]
        public void ListTemplateRecords(long accountId, string template,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListTemplateRecordsFixture);

            var records =
                client.Templates.ListTemplateRecords(accountId, template);

            Assert.Multiple(() =>
            {
                Assert.That(records.Data.Count, Is.EqualTo(records.Pagination.TotalEntries));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "268",
            "https://api.sandbox.dnsimple.com/v2/1010/templates/268/records?sort=id:asc,name:desc,content:asc,type:desc&per_page=42&page=7")]
        public void ListTemplateRecordsSorted(long accountId, string template,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListTemplateRecordsFixture);
            var options = new ListTemplateRecordsOptions
            {
                Pagination = new Pagination
                {
                    PerPage = 42,
                    Page = 7
                }
            }.SortById(Order.asc).SortByName(Order.desc)
                .SortByContent(Order.asc).SortByType(Order.desc);

            client.Templates.ListTemplateRecords(accountId, template, options);

            Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
        }

        [Test]
        [TestCase(1010, "268",
            "https://api.sandbox.dnsimple.com/v2/1010/templates/268/records")]
        public void CreateTemplateRecord(long accountId, string template, string expectedUrl)
        {
            var client = new MockDnsimpleClient(CreateTemplateRecordFixture);
            var templateRecord = new TemplateRecord
            {
                Name = "",
                Type = "MX",
                Content = "mx.example.com",
                Ttl = 600,
                Priority = 10
            };

            var record =
                client.Templates.CreateTemplateRecord(accountId, template,
                    templateRecord).Data;

            Assert.Multiple(() =>
            {
                Assert.That(record.Id, Is.EqualTo(300));
                Assert.That(record.TemplateId, Is.EqualTo(268));
                Assert.That(record.Name, Is.EqualTo(templateRecord.Name));
                Assert.That(record.Type, Is.EqualTo(templateRecord.Type));
                Assert.That(record.Content, Is.EqualTo(templateRecord.Content));
                Assert.That(record.Ttl, Is.EqualTo(templateRecord.Ttl));
                Assert.That(record.Priority, Is.EqualTo(templateRecord.Priority));

                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.POST));
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "268", 301,
            "https://api.sandbox.dnsimple.com/v2/1010/templates/268/records/301")]
        public void GetTemplateRecord(long accountId, string template, long recordId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetTemplateRecordFixture);
            var record =
                client.Templates.GetTemplateRecord(accountId, template,
                    recordId).Data;

            Assert.Multiple(() =>
            {
                Assert.That(record.Id, Is.EqualTo(recordId));
                Assert.That(record.TemplateId, Is.EqualTo(268));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "268", 301,
            "https://api.sandbox.dnsimple.com/v2/1010/templates/268/records/301")]
        public void DeleteTemplateRecord(long accountId, string template, long recordId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(DeleteTemplateRecordFixture);

            client.Templates.DeleteTemplateRecord(accountId, template,
                recordId);

            Assert.Multiple(() =>
            {
                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.DELETE));
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }
    }
}
