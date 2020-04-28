using System;
using System.Globalization;
using System.Linq;
using System.Net;
using dnsimple;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class DomainsDelegationSignerRecordsTest
    {
        private JToken _jToken;
        private const string GetRecordFixture = "getDelegationSignerRecord/success.http";
        private const string ListRecordsFixture = "listDelegationSignerRecords/success.http";
        private const string CreateRecordsFixture = "createDelegationSignerRecord/created.http";
        private const string FailToCreateRecordsFixture = "createDelegationSignerRecord/validation-error.http";
        private const string DeleteRecordFixture = "deleteDelegationSignerRecord/success.http";
        
        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2017-03-03T13:49:58Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2017-03-03T13:49:58Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private readonly DelegationSignerRecord _record = new DelegationSignerRecord
        {
            Algorithm = "13",
            Digest =
                "684a1f049d7d082b7f98691657da5a65764913df7f065f6f8c36edf62d66ca03",
            DigestType = "2",
            Keytag = "2371"
        };
        
        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", ListRecordsFixture);
            _jToken = JObject.Parse(loader.ExtractJsonPayload());
        }
        
        [Test]
        public void DelegationSignerRecordsData()
        {
            var records = new PaginatedDnsimpleResponse<DelegationSignerRecord>(_jToken).Data;
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, records.Count);
                Assert.AreEqual(24, records.First().Id);
                Assert.AreEqual(1010, records.First().DomainId);
                Assert.AreEqual("8", records.First().Algorithm);
                Assert.AreEqual("C1F6E04A5A61FBF65BF9DC8294C363CF11C89E802D926BDAB79C55D27BEFA94F", records.First().Digest);
                Assert.AreEqual("2", records.First().DigestType);
                Assert.AreEqual("44620", records.First().Keytag);
                Assert.AreEqual(CreatedAt, records.First().CreatedAt);
                Assert.AreEqual(UpdatedAt, records.First().UpdatedAt);
            });
        }

        [Test]
        [TestCase(1010, "100", "https://api.sandbox.dnsimple.com/v2/1010/domains/100/ds_records")]
        [TestCase(1010, "example.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/ds_records")]
        public void ListDelegationSignerRecords(long accountId, string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListRecordsFixture);
            var records =
                client.Domains.ListDelegationSignerRecords(accountId,
                    domainIdentifier);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, records.Data.Count);
                Assert.AreEqual(1, records.PaginationData.CurrentPage);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
        
        [Test]
        [TestCase(1010, "100", "https://api.sandbox.dnsimple.com/v2/1010/domains/100/ds_records?sort=id:asc%2ccreated_at:desc&per_page=5&page=3")]
        [TestCase(1010, "example.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/ds_records?sort=id:asc%2ccreated_at:desc&per_page=5&page=3")]
        public void ListDelegationSignerRecordsWithOptions(long accountId, string domainIdentifier, string expectedUrl)
        {
            var options = new ListDomainDelegationSignerRecordsOptions
            {
                Pagination = new Pagination {Page = 3, PerPage = 5}
            };
            options.SortById(Order.asc).SortByCreatedAt(Order.desc);

            var client = new MockDnsimpleClient(ListRecordsFixture);
            var records =
                client.Domains.ListDelegationSignerRecords(accountId,
                    domainIdentifier, options);
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, records.Data.Count);
                Assert.AreEqual(1, records.PaginationData.CurrentPage);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "100", "https://api.sandbox.dnsimple.com/v2/1010/domains/100/ds_records")]
        [TestCase(1010, "example.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/ds_records")]
        public void CreateDelegationSignerRecord(long accountId, string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(CreateRecordsFixture);
            
            var created =
                client.Domains.CreateDelegationSignerRecord(accountId,
                    domainIdentifier, _record);
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, created.Data.Id);
                Assert.AreEqual(1010, created.Data.DomainId);
                Assert.AreEqual("13", created.Data.Algorithm);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "100")]
        [TestCase(1010, "example.com")]
        public void FailToCreateDelegationSignerRecord(long accountId, string domainIdentifier)
        {
            var client = new MockDnsimpleClient(FailToCreateRecordsFixture);
            client.StatusCode(HttpStatusCode.BadRequest);
            
            try
            {
                client.Domains.CreateDelegationSignerRecord(accountId,
                    domainIdentifier, _record);
            }
            catch (DnSimpleValidationException exception)
            {
                Assert.Multiple(() =>
                {
                    Assert.AreEqual("can't be blank",
                        exception.Validation["algorithm"]?.First?.ToString());
                    Assert.AreEqual("can't be blank",
                        exception.Validation["digest"]?.First?.ToString());
                    Assert.AreEqual("can't be blank",
                        exception.Validation["digest_type"]?.First?.ToString());
                    Assert.AreEqual("can't be blank",
                        exception.Validation["keytag"]?.First?.ToString());
                });
            }
        }

        [Test]
        [TestCase(1010, "1010", "https://api.sandbox.dnsimple.com/v2/1010/domains/1010/ds_records/24")]
        [TestCase(1010, "example.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/ds_records/24")]
        public void GetDelegationSignerRecord(long accountId, string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetRecordFixture);
            var record =
                client.Domains.GetDelegationSignerRecord(accountId,
                    domainIdentifier, 24).Data;
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(24, record.Id);
                Assert.AreEqual(1010, record.DomainId);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "1010", "https://api.sandbox.dnsimple.com/v2/1010/domains/1010/ds_records/24")]
        [TestCase(1010, "example.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/ds_records/24")]
        public void DeleteDelegationSignerRecord(long accountId, string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(DeleteRecordFixture);
            
            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() =>
                {
                    client.Domains.DeleteDelegationSignerRecord(accountId,
                        domainIdentifier, 24);
                });
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
    }
}