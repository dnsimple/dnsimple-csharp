using System;
using System.Globalization;
using System.Linq;
using System.Net;
using dnsimple;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using NUnit.Framework;
using Pagination = dnsimple.Services.ListOptions.Pagination;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class DomainsDelegationSignerRecordsTest
    {
        private MockResponse _response;

        private const string GetRecordFixture =
            "getDelegationSignerRecord/success.http";

        private const string ListRecordsFixture =
            "listDelegationSignerRecords/success.http";

        private const string CreateRecordsFixture =
            "createDelegationSignerRecord/created.http";

        private const string FailToCreateRecordsFixture =
            "createDelegationSignerRecord/validation-error.http";

        private const string DeleteRecordFixture =
            "deleteDelegationSignerRecord/success.http";


        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2017-03-03T13:49:58Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2017-03-03T13:49:58Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private readonly DelegationSignerRecord _record =
            new DelegationSignerRecord
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
            _response = new MockResponse(loader);
        }

        [Test]
        public void DelegationSignerRecordsData()
        {
            var records =
                new PaginatedResponse<DelegationSignerRecord>(_response)
                    .Data;

            Assert.Multiple(() =>
            {
                Assert.That(records.Count, Is.EqualTo(1));
                Assert.That(records.First().Id, Is.EqualTo(24));
                Assert.That(records.First().DomainId, Is.EqualTo(1010));
                Assert.That(records.First().Algorithm, Is.EqualTo("8"));
                Assert.That(
                    records.First().Digest, Is.EqualTo("C1F6E04A5A61FBF65BF9DC8294C363CF11C89E802D926BDAB79C55D27BEFA94F"));
                Assert.That(records.First().DigestType, Is.EqualTo("2"));
                Assert.That(records.First().Keytag, Is.EqualTo("44620"));
                Assert.That(records.First().CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(records.First().UpdatedAt, Is.EqualTo(UpdatedAt));
            });
        }

        [Test]
        [TestCase(1010, "100",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/100/ds_records")]
        [TestCase(1010, "example.com",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/ds_records")]
        public void ListDelegationSignerRecords(long accountId,
            string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListRecordsFixture);
            var records =
                client.Domains.ListDelegationSignerRecords(accountId,
                    domainIdentifier);
            Assert.Multiple(() =>
            {
                Assert.That(records.Data.Count, Is.EqualTo(1));
                Assert.That(records.Pagination.CurrentPage, Is.EqualTo(1));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "100",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/100/ds_records?sort=id:asc,created_at:desc&per_page=5&page=3")]
        [TestCase(1010, "example.com",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/ds_records?sort=id:asc,created_at:desc&per_page=5&page=3")]
        public void ListDelegationSignerRecordsWithOptions(long accountId,
            string domainIdentifier, string expectedUrl)
        {
            var options = new ListDomainDelegationSignerRecordsOptions
            {
                Pagination = new Pagination { Page = 3, PerPage = 5 }
            };
            options.SortById(Order.asc).SortByCreatedAt(Order.desc);

            var client = new MockDnsimpleClient(ListRecordsFixture);
            var records =
                client.Domains.ListDelegationSignerRecords(accountId,
                    domainIdentifier, options);

            Assert.Multiple(() =>
            {
                Assert.That(records.Data.Count, Is.EqualTo(1));
                Assert.That(records.Pagination.CurrentPage, Is.EqualTo(1));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "100",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/100/ds_records")]
        [TestCase(1010, "example.com",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/ds_records")]
        public void CreateDelegationSignerRecord(long accountId,
            string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(CreateRecordsFixture);

            var created =
                client.Domains.CreateDelegationSignerRecord(accountId,
                    domainIdentifier, _record);

            Assert.Multiple(() =>
            {
                Assert.That(created.Data.Id, Is.EqualTo(2));
                Assert.That(created.Data.DomainId, Is.EqualTo(1010));
                Assert.That(created.Data.Algorithm, Is.EqualTo("13"));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "100")]
        [TestCase(1010, "example.com")]
        public void FailToCreateDelegationSignerRecord(long accountId,
            string domainIdentifier)
        {
            var client = new MockDnsimpleClient(FailToCreateRecordsFixture);
            client.StatusCode(HttpStatusCode.BadRequest);

            try
            {
                client.Domains.CreateDelegationSignerRecord(accountId,
                    domainIdentifier, _record);
            }
            catch (DnsimpleValidationException exception)
            {
                Assert.That(exception.Validation["algorithm"]?.First?.ToString(), Is.EqualTo("can't be blank"));
            }
        }

        [Test]
        [TestCase("", "digest", "digestType", "keytag")]
        [TestCase(null, "digest", "digestType", "keytag")]
        public void CreateDelegationSignerRecordClientSideValidation(
            string algorithm, string digest, string digestType, string keytag)
        {
            var client = new MockDnsimpleClient(CreateRecordsFixture);

            Assert.Throws(Is.InstanceOf<Exception>(),
                delegate
                {
                    client.Domains.CreateDelegationSignerRecord(1010,
                        "ruby.codes", new DelegationSignerRecord
                        {
                            Algorithm = algorithm,
                            Digest = digest,
                            DigestType = digestType,
                            Keytag = keytag
                        });
                });
        }

        [Test]
        [TestCase(1010, "1010",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/1010/ds_records/24")]
        [TestCase(1010, "example.com",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/ds_records/24")]
        public void GetDelegationSignerRecord(long accountId,
            string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetRecordFixture);
            var record =
                client.Domains.GetDelegationSignerRecord(accountId,
                    domainIdentifier, 24).Data;

            Assert.Multiple(() =>
            {
                Assert.That(record.Id, Is.EqualTo(24));
                Assert.That(record.DomainId, Is.EqualTo(1010));
                Assert.That(record.Algorithm, Is.EqualTo("8"));
                Assert.That(record.Digest, Is.EqualTo("C1F6E04A5A61FBF65BF9DC8294C363CF11C89E802D926BDAB79C55D27BEFA94F"));
                Assert.That(record.DigestType, Is.EqualTo("2"));
                Assert.That(record.Keytag, Is.EqualTo("44620"));
                Assert.That(record.PublicKey, Is.EqualTo(null));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "1010",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/1010/ds_records/24")]
        [TestCase(1010, "example.com",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/ds_records/24")]
        public void DeleteDelegationSignerRecord(long accountId,
            string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(DeleteRecordFixture);

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() =>
                {
                    client.Domains.DeleteDelegationSignerRecord(accountId,
                        domainIdentifier, 24);
                });

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }
    }
}
