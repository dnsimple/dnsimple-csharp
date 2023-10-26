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
                Assert.AreEqual(3, charges.Count);
                Assert.AreEqual(14.5f, charges.First().TotalAmount);
                Assert.AreEqual(0.0f, charges.First().BalanceAmount);
                Assert.AreEqual("1-2", charges.First().Reference);
                Assert.AreEqual("collected", charges.First().State);
                Assert.AreEqual("Register bubble-registered.com", charges.First().Items.First().Description);
                Assert.AreEqual(14.5f, charges.First().Items.First().Amount);
                Assert.AreEqual(1, charges.First().Items.First().ProductId);
                Assert.AreEqual("domain-registration", charges.First().Items.First().ProductType);
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

            Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                Assert.AreEqual(filters, options.UnpackFilters());
                Assert.AreEqual(pagination, options.UnpackPagination());
                Assert.AreEqual(sorting, options.UnpackSorting());
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
