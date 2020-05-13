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
            var services = new PaginatedDnsimpleResponse<Service>(_response).Data;
            var service = services.First();
            var serviceSetting = services.Last().Settings.First();
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, service.Id);
                Assert.AreEqual("Service 1", service.Name);
                Assert.AreEqual("service1", service.Sid);
                Assert.AreEqual("First service example.", service.Description);
                Assert.IsNull(service.SetupDescription);
                Assert.IsFalse(service.RequiresSetup);
                Assert.IsNull(service.DefaultSubdomain);
                Assert.AreEqual(CreatedAt, service.CreatedAt);
                Assert.AreEqual(UpdatedAt, service.UpdatedAt);
                Assert.IsEmpty(service.Settings);
                
                Assert.AreEqual(1,services.Last().Settings.Count);
                Assert.AreEqual("username", serviceSetting.Name);
                Assert.AreEqual("Service 2 Account Username", serviceSetting.Label);
                Assert.AreEqual(".service2.com", serviceSetting.Append);
                Assert.AreEqual("Your Service2 username is used to connect services to your account.", serviceSetting.Description);
                Assert.AreEqual("username", serviceSetting.Example);
                Assert.IsFalse(serviceSetting.Password);
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
                Assert.AreEqual(2, services.Count);
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase("https://api.sandbox.dnsimple.com/v2/services?sort=id:asc%2csid:desc&per_page=42&page=7")]
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
            
            Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                Assert.AreEqual(1, service.Id);
                Assert.AreEqual("Service 1", service.Name);
                Assert.AreEqual("service1", service.Sid);
                Assert.AreEqual("First service example.", service.Description);
                Assert.IsNull(service.SetupDescription);
                Assert.IsTrue(service.RequiresSetup);
                Assert.IsNull(service.DefaultSubdomain);
                Assert.AreEqual(CreatedAt, service.CreatedAt);
                Assert.AreEqual(UpdatedAt, service.UpdatedAt);
                
                Assert.AreEqual("username", setting.Name);
                Assert.AreEqual("Service 1 Account Username", setting.Label);
                Assert.AreEqual(".service1.com", setting.Append);
                Assert.AreEqual("Your Service 1 username is used to connect services to your account.", setting.Description);
                Assert.AreEqual("username", setting.Example);
                Assert.IsFalse(setting.Password);
            });
        }
    }
}