using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using NUnit.Framework;
using Pagination = dnsimple.Services.ListOptions.Pagination;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class DomainsEmailForwardsTest
    {
        private MockResponse _response;

        private const string ListEmailForwardsFixture =
            "listEmailForwards/success.http";

        private const string CreateEmailForwardFixture =
            "createEmailForward/created.http";

        private const string GetEmailForwardFixture =
            "getEmailForward/success.http";

        private const string DeleteEmailForwardFixture =
            "deleteEmailForward/success.http";

        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2017-05-25T19:23:16Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2017-05-25T19:23:16Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", ListEmailForwardsFixture);
            _response = new MockResponse(loader);
        }

        [Test]
        public void EmailForwardsData()
        {
            var emailForwards =
                new PaginatedResponse<EmailForward>(_response).Data;

            Assert.Multiple(() =>
            {
                Assert.That(emailForwards.Count, Is.EqualTo(1));
                Assert.That(emailForwards.First().Id, Is.EqualTo(24809));
                Assert.That(emailForwards.First().DomainId, Is.EqualTo(235146));
                Assert.That(emailForwards.First().AliasEmail, Is.EqualTo(".*@a-domain.com"));
                Assert.That(emailForwards.First().DestinationEmail, Is.EqualTo("jane.smith@example.com"));
                Assert.That(emailForwards.First().Active, Is.True);
                Assert.That(emailForwards.First().CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(emailForwards.First().UpdatedAt, Is.EqualTo(UpdatedAt));
            });
        }

        [Test]
        [TestCase(1010, "100",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/100/email_forwards")]
        [TestCase(1010, "example.com",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/email_forwards")]
        public void ListEmailForwards(long accountId, string domainIdentifier,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListEmailForwardsFixture);
            var emailForwards =
                client.Domains.ListEmailForwards(accountId, domainIdentifier);

            Assert.Multiple(() =>
            {
                Assert.That(emailForwards.Data.Count, Is.EqualTo(1));
                Assert.That(emailForwards.Pagination.CurrentPage, Is.EqualTo(1));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "100",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/100/email_forwards?sort=from:asc")]
        [TestCase(1010, "example.com",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/email_forwards?sort=from:asc")]
        public void ListEmailForwardsWithOptions(long accountId,
            string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListEmailForwardsFixture);
            var options = new DomainEmailForwardsListOptions();
            options.SortByFrom(Order.asc);

            var emailForwards =
                client.Domains.ListEmailForwards(accountId, domainIdentifier,
                    options);

            Assert.Multiple(() =>
            {
                Assert.That(emailForwards.Data.Count, Is.EqualTo(1));
                Assert.That(emailForwards.Pagination.CurrentPage, Is.EqualTo(1));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "235146",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/235146/email_forwards")]
        [TestCase(1010, "example.com",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/email_forwards")]
        public void CreateEmailForward(long accountId, string domainIdentifier,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(CreateEmailForwardFixture);
            var record = new EmailForward
            {
                AliasName = "example@dnsimple.xyz",
                DestinationEmail = "example@example.com"
            };

            var created =
                client.Domains.CreateEmailForward(accountId, domainIdentifier,
                    record);

            Assert.Multiple(() =>
            {
                Assert.That(created.Data.Id, Is.EqualTo(41872));
                Assert.That(created.Data.DomainId, Is.EqualTo(235146));
                Assert.That(created.Data.AliasEmail, Is.EqualTo("example@dnsimple.xyz"));
                Assert.That(created.Data.DestinationEmail, Is.EqualTo(record.DestinationEmail));
                Assert.That(created.Data.Active, Is.True);
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase("", "jim@another.com")]
        [TestCase("jim@a-domain.com", "")]
        [TestCase(null, "jim@another.com")]
        [TestCase("jim@a-domain.com", null)]
        public void CreateEmailForwardClientValidation(string from, string to)
        {
            var client = new MockDnsimpleClient(CreateEmailForwardFixture);
            var record = new EmailForward
            {
                AliasName = from,
                DestinationEmail = to
            };

            Assert.Throws(Is.InstanceOf<Exception>(),
                delegate
                {
                    client.Domains.CreateEmailForward(1010, "ruby.codes",
                        record);
                });
        }

        [Test]
        [TestCase(1010, "235146",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/235146/email_forwards/41872")]
        [TestCase(1010, "example.com",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/email_forwards/41872")]
        public void GetEmailForward(long accountId, string domainIdentifier,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetEmailForwardFixture);
            var emailForward =
                client.Domains.GetEmailForward(accountId, domainIdentifier,
                    41872).Data;

            Assert.Multiple(() =>
            {
                Assert.That(emailForward.Id, Is.EqualTo(41872));
                Assert.That(emailForward.DomainId, Is.EqualTo(235146));
                Assert.That(emailForward.Active, Is.True);

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "100",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/100/email_forwards/228963")]
        [TestCase(1010, "example.com",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/email_forwards/228963")]
        public void DeleteEmailForward(long accountId, string domainIdentifier,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(DeleteEmailForwardFixture);

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() =>
                {
                    client.Domains.DeleteEmailForward(accountId,
                        domainIdentifier,
                        228963);
                });

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        public void DomainEmailForwardsListOptions()
        {
            var sorting =
                new KeyValuePair<string, string>("sort",
                    "id:asc,from:asc,to:desc");
            var pagination = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("per_page", "42"),
                new KeyValuePair<string, string>("page", "7")
            };

            var options = new DomainEmailForwardsListOptions
            {
                Pagination = new Pagination
                {
                    Page = 7,
                    PerPage = 42
                }
            };
            options.SortById(Order.asc).SortByFrom(Order.asc)
                .SortByTo(Order.desc);

            Assert.Multiple(() =>
            {
                Assert.That(options.UnpackSorting(), Is.EqualTo(sorting));
                Assert.That(options.UnpackPagination(), Is.EqualTo(pagination));
            });
        }
    }
}
