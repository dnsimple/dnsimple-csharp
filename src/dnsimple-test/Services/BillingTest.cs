using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using dnsimple;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using NUnit.Framework;
using RestSharp;
using Pagination = dnsimple.Services.ListOptions.Pagination;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class BillingTest
    {

        private MockResponse _response;

        private const string ListChargesFixture =
            "listCharges/success.http";

        private const string ListChargesBadFilterFixture =
            "listCharges/fail-400-bad-filter.http";

        private const string ListChargesUnauthorizedFixture =
            "listCharges/fail-403.http";

        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", ListChargesFixture);
            _response = new MockResponse(loader);
        }

        [Test]
        public void ListCharges()
        {
            var charges =
                new PaginatedResponse<Charge>(_response).Data;

            Assert.Multiple(() =>
            {
                Assert.That(charges.Count, Is.EqualTo(3));
                Assert.That(charges.First().TotalAmount, Is.EqualTo(14.50m));
                Assert.That(charges.First().BalanceAmount, Is.EqualTo(0.00m));
                Assert.That(charges.First().Reference, Is.EqualTo("1-2"));
                Assert.That(charges.First().State, Is.EqualTo("collected"));
                Assert.That(charges.First().Items.First().Description, Is.EqualTo("Register bubble-registered.com"));
                Assert.That(charges.First().Items.First().Amount, Is.EqualTo(14.50m));
                Assert.That(charges.First().Items.First().ProductId, Is.EqualTo(1));
                Assert.That(charges.First().Items.First().ProductType, Is.EqualTo("domain-registration"));
            });
        }

        [Test]
        [TestCase(
            "https://api.sandbox.dnsimple.com/v2/1010/domains?sort=invoiced:asc&start_date=2023-01-01&end_date=2023-08-31")]
        public void ListChargesWithOptions(string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListChargesFixture);
            var listOptions = new ListChargesOptions();
            listOptions.FilterByStartDate("2023-01-01");
            listOptions.FilterByEndDate("2023-08-31");
            listOptions.SortByInvoiceDate(Order.asc);

            client.Domains.ListDomains(1010, listOptions);

            Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
        }

        [Test]
        public void ListChargesOptions()
        {
            var filters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("start_date", "2023-01-01"),
                new KeyValuePair<string, string>("end_date", "2023-08-31")
            };
            var sorting = new KeyValuePair<string, string>("sort",
                "invoiced:asc");
            var pagination = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("per_page", "30"),
                new KeyValuePair<string, string>("page", "1")
            };

            var options = new ListChargesOptions
            {
                Pagination = new Pagination
                {
                    PerPage = 30,
                    Page = 1
                }
            }.FilterByStartDate("2023-01-01")
                .FilterByEndDate("2023-08-31")
                .SortByInvoiceDate(Order.asc);


            Assert.Multiple(() =>
            {
                Assert.That(options.UnpackFilters(), Is.EqualTo(filters));
                Assert.That(options.UnpackPagination(), Is.EqualTo(pagination));
                Assert.That(options.UnpackSorting(), Is.EqualTo(sorting));
            });
        }

        [Test]
        [TestCase(1010)]
        public void ListChargesBadFilter(long accountId)
        {
            var client = new MockDnsimpleClient(ListChargesBadFilterFixture);
            client.StatusCode(HttpStatusCode.BadRequest);

            Assert.Throws(
                Is.TypeOf<DnsimpleValidationException>().And.Message
                    .EqualTo("Invalid date format must be ISO8601 (YYYY-MM-DD)"),
                delegate { client.Billing.ListCharges(accountId); });
        }

        [Test]
        [TestCase(1010)]
        public void ListChargesUnauthorized(long accountId)
        {
            var client = new MockDnsimpleClient(ListChargesUnauthorizedFixture);
            client.StatusCode(HttpStatusCode.BadRequest);

            Assert.Throws(
                Is.TypeOf<DnsimpleValidationException>().And.Message
                    .EqualTo("Permission Denied. Required Scope: billing:*:read"),
                delegate { client.Billing.ListCharges(accountId); });
        }
    }
}
