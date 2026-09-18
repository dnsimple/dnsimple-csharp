using System.Linq;
using dnsimple.Services.ListOptions;
using NUnit.Framework;
using RestSharp;
using Pagination = dnsimple.Services.ListOptions.Pagination;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class DnsAnalyticsTest
    {
        private const string QueryDnsAnalyticsFixture = "dnsAnalytics/success.http";

        [Test]
        [TestCase(1010, "https://api.sandbox.dnsimple.com/v2/1010/dns_analytics")]
        public void Query(long accountId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(QueryDnsAnalyticsFixture);
            var response = client.DnsAnalytics.Query(accountId);

            Assert.Multiple(() =>
            {
                Assert.That(response.Data.Count, Is.EqualTo(12));
                Assert.That(response.Data.First().ZoneName, Is.EqualTo("bar.com"));
                Assert.That(response.Data.First().Date, Is.EqualTo("2023-12-08"));
                Assert.That(response.Data.First().Volume, Is.EqualTo(1200));
                Assert.That(response.Data.Last().ZoneName, Is.EqualTo("foo.com"));
                Assert.That(response.Data.Last().Date, Is.EqualTo("2024-01-08"));
                Assert.That(response.Data.Last().Volume, Is.EqualTo(1200));

                Assert.That(response.Pagination.CurrentPage, Is.EqualTo(0));
                Assert.That(response.Pagination.PerPage, Is.EqualTo(100));
                Assert.That(response.Pagination.TotalEntries, Is.EqualTo(93));
                Assert.That(response.Pagination.TotalPages, Is.EqualTo(1));

                Assert.That(response.Query.AccountId, Is.EqualTo(1));
                Assert.That(response.Query.StartDate, Is.EqualTo("2023-12-08"));
                Assert.That(response.Query.EndDate, Is.EqualTo("2024-01-08"));
                Assert.That(response.Query.Sort, Is.EqualTo("zone_name:asc,date:asc"));
                Assert.That(response.Query.Page, Is.EqualTo(0));
                Assert.That(response.Query.PerPage, Is.EqualTo(100));
                Assert.That(response.Query.Groupings, Is.EqualTo("zone_name,date"));

                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.Get));
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "https://api.sandbox.dnsimple.com/v2/1010/dns_analytics?sort=volume:desc,zone_name:asc&start_date=2023-12-08&end_date=2024-01-08&groupings=zone_name,date&per_page=100&page=2")]
        public void QueryWithOptions(long accountId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(QueryDnsAnalyticsFixture);
            var options = new DnsAnalyticsOptions
            {
                Pagination = new Pagination
                {
                    PerPage = 100,
                    Page = 2
                }
            }.FilterByStartDate("2023-12-08")
                .FilterByEndDate("2024-01-08")
                .GroupByZoneName()
                .GroupByDate()
                .SortByVolume(Order.desc)
                .SortByZoneName(Order.asc);

            client.DnsAnalytics.Query(accountId, options);

            Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
        }
    }
}
