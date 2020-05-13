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
                new PaginatedDnsimpleResponse<TemplateRecord>(_response);
            var record = records.Data.First();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(296, record.Id);
                Assert.AreEqual(268, record.TemplateId);
                Assert.IsEmpty(record.Name);
                Assert.AreEqual("192.168.1.1", record.Content);
                Assert.AreEqual(3600, record.Ttl);
                Assert.IsNull(record.Priority);
                Assert.AreEqual("A", record.Type);
                Assert.AreEqual(CreatedAt, record.CreatedAt);
                Assert.AreEqual(UpdatedAt, record.UpdatedAt);
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
                Assert.AreEqual(records.Pagination.TotalEntries,
                    records.Data.Count);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "268",
            "https://api.sandbox.dnsimple.com/v2/1010/templates/268/records?sort=id:asc%2cname:desc%2ccontent:asc%2ctype:desc&per_page=42&page=7")]
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

            Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                Assert.AreEqual(300, record.Id);
                Assert.AreEqual(268, record.TemplateId);
                Assert.AreEqual(templateRecord.Name, record.Name);
                Assert.AreEqual(templateRecord.Type, record.Type);
                Assert.AreEqual(templateRecord.Content, record.Content);
                Assert.AreEqual(templateRecord.Ttl, record.Ttl);
                Assert.AreEqual(templateRecord.Priority, record.Priority);
                
                Assert.AreEqual(Method.POST, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                Assert.AreEqual(recordId, record.Id);
                Assert.AreEqual(268, record.TemplateId);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                Assert.AreEqual(Method.DELETE, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
    }
}