using System;
using System.Globalization;
using System.Linq;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using NUnit.Framework;
using Pagination = dnsimple.Services.ListOptions.Pagination;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class ServicesTest
    {
        private MockResponse _response;
        private const string ListServicesFixture = "listServices/success.http";
        private const string GetServiceFixture = "getService/success.http";

        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2014-02-14T19:15:19Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2016-03-04T09:23:27Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", ListServicesFixture);
            _response = new MockResponse(loader);
        }

        [Test]
        public void ListServicesResponse()
        {
            var services = new PaginatedResponse<Service>(_response).Data;
            var service = services.First();
            var serviceSetting = services.Last().Settings.First();

            Assert.Multiple(() =>
            {
                Assert.That(service.Id, Is.EqualTo(1));
                Assert.That(service.Name, Is.EqualTo("Service 1"));
                Assert.That(service.Sid, Is.EqualTo("service1"));
                Assert.That(service.Description, Is.EqualTo("First service example."));
                Assert.That(service.SetupDescription, Is.Null);
                Assert.That(service.RequiresSetup, Is.False);
                Assert.That(service.DefaultSubdomain, Is.Null);
                Assert.That(service.CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(service.UpdatedAt, Is.EqualTo(UpdatedAt));
                Assert.That(service.Settings, Is.Empty);


                Assert.That(services.Last().Settings.Count, Is.EqualTo(1));
                Assert.That(serviceSetting.Name, Is.EqualTo("username"));
                Assert.That(serviceSetting.Label, Is.EqualTo("Service 2 Account Username"));
                Assert.That(serviceSetting.Append, Is.EqualTo(".service2.com"));
                Assert.That(serviceSetting.Description, Is.EqualTo("Your Service2 username is used to connect services to your account."));
                Assert.That(serviceSetting.Example, Is.EqualTo("username"));
                Assert.That(serviceSetting.Password, Is.False);
            });
        }

        [Test]
        [TestCase("https://api.sandbox.dnsimple.com/v2/services")]
        public void ListServices(string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListServicesFixture);
            var services = client.Services.ListServices().Data;

            Assert.Multiple(() =>
            {
                Assert.That(services.Count, Is.EqualTo(2));
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase("https://api.sandbox.dnsimple.com/v2/services?sort=id:asc,sid:desc&per_page=42&page=7")]
        public void ListServicesWithSorting(string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListServicesFixture);
            var options = new ListServicesOptions
            {
                Pagination = new Pagination
                {
                    PerPage = 42,
                    Page = 7
                }

            }.SortById(Order.asc)
                .SortBySid(Order.desc);

            client.Services.ListServices(options);

            Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
        }

        [Test]
        [TestCase("1", "https://api.sandbox.dnsimple.com/v2/services/1")]
        public void GetService(string serviceId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetServiceFixture);
            var service = client.Services.GetService(serviceId).Data;
            var setting = service.Settings.First();

            Assert.Multiple(() =>
            {
                Assert.That(service.Id, Is.EqualTo(1));
                Assert.That(service.Name, Is.EqualTo("Service 1"));
                Assert.That(service.Sid, Is.EqualTo("service1"));
                Assert.That(service.Description, Is.EqualTo("First service example."));
                Assert.That(service.SetupDescription, Is.Null);
                Assert.That(service.RequiresSetup, Is.True);
                Assert.That(service.DefaultSubdomain, Is.Null);
                Assert.That(service.CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(service.UpdatedAt, Is.EqualTo(UpdatedAt));

                Assert.That(setting.Name, Is.EqualTo("username"));
                Assert.That(setting.Label, Is.EqualTo("Service 1 Account Username"));
                Assert.That(setting.Append, Is.EqualTo(".service1.com"));
                Assert.That(setting.Description, Is.EqualTo("Your Service 1 username is used to connect services to your account."));
                Assert.That(setting.Example, Is.EqualTo("username"));
                Assert.That(setting.Password, Is.False);
            });
        }
    }
}
