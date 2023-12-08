using System;
using System.Globalization;
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
    public class ContactsTest
    {
        private MockResponse _response;

        private const string ListContactsFixture = "listContacts/success.http";

        private const string CreateContactFixture =
            "createContact/created.http";

        private const string GetContactFixture = "getContact/success.http";

        private const string UpdateContactFixture =
            "updateContact/success.http";

        private const string DeleteContactFixture =
            "deleteContact/success.http";

        private const string DeleteContactFailedFixture =
            "deleteContact/error-contact-in-use.http";

        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2013-11-08T17:23:15Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2015-01-08T21:30:50Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", ListContactsFixture);
            _response = new MockResponse(loader);
        }

        [Test]
        public void ContactsResponse()
        {
            var contacts = new PaginatedResponse<Contact>(_response).Data;
            var contact = contacts.First();

            Assert.Multiple(() =>
            {
                Assert.That(contact.Id, Is.EqualTo(1));
                Assert.That(contact.AccountId, Is.EqualTo(1010));
                Assert.That(contact.Label, Is.EqualTo("Default"));
                Assert.That(contact.FirstName, Is.EqualTo("First"));
                Assert.That(contact.LastName, Is.EqualTo("User"));
                Assert.That(contact.JobTitle, Is.EqualTo("CEO"));
                Assert.That(contact.OrganizationName, Is.EqualTo("Awesome Company"));
                Assert.That(contact.Email, Is.EqualTo("first@example.com"));
                Assert.That(contact.Phone, Is.EqualTo("+18001234567"));
                Assert.That(contact.Fax, Is.EqualTo("+18011234567"));
                Assert.That(contact.Address1, Is.EqualTo("Italian Street, 10"));
                Assert.IsEmpty(contact.Address2);
                Assert.That(contact.City, Is.EqualTo("Roma"));
                Assert.That(contact.StateProvince, Is.EqualTo("RM"));
                Assert.That(contact.PostalCode, Is.EqualTo("00100"));
                Assert.That(contact.Country, Is.EqualTo("IT"));
                Assert.That(contact.CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(contact.UpdatedAt, Is.EqualTo(UpdatedAt));
            });
        }

        [Test]
        [TestCase(1010, "https://api.sandbox.dnsimple.com/v2/1010/contacts")]
        public void ListContacts(long accountId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListContactsFixture);
            var response = client.Contacts.ListContacts(accountId);

            Assert.Multiple(() =>
            {
                Assert.That(response.Data.Count, Is.EqualTo(2));
                Assert.That(response.Pagination.TotalEntries, Is.EqualTo(2));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010,
            "https://api.sandbox.dnsimple.com/v2/1010/contacts?sort=id:asc,label:desc,email:asc&per_page=42&page=7")]
        public void ListContactsWitSorting(long accountId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListContactsFixture);
            var options = new ContactsListOptions
            {
                Pagination = new Pagination
                {
                    PerPage = 42,
                    Page = 7
                }
            }.SortById(Order.asc).SortByLabel(Order.desc)
                .SortByEmail(Order.asc);

            client.Contacts.ListContacts(accountId, options);

            Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
        }

        [Test]
        [TestCase(1010,
            "https://api.sandbox.dnsimple.com/v2/1010/contacts")]
        public void CreateContact(long accountId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(CreateContactFixture);
            var contact = new Contact
            {
                FirstName = "First",
                LastName = "User",
                Address1 = "Italian Street, 10",
                City = "Roma",
                StateProvince = "RM",
                PostalCode = "00100",
                Country = "IT",
                Email = "first@example.com",
                Phone = "+18001234567",
                Fax = "+18011234567"
            };

            var created =
                client.Contacts.CreateContact(accountId, contact).Data;

            Assert.Multiple(() =>
            {
                Assert.That(created.Id, Is.EqualTo(1));
                Assert.That(created.Email, Is.EqualTo(contact.Email));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.POST));
            });
        }

        [Test]
        [TestCase(null, "User", "first@example.com", "+18001234567", "+18011234567", "Italian Street, 10", "Roma", "RM", "00100", "IT")]
        [TestCase("First", null, "first@example.com", "+18001234567", "+18011234567", "Italian Street, 10", "Roma", "RM", "00100", "IT")]
        [TestCase("First", "User", null, "+18001234567", "+18011234567", "Italian Street, 10", "Roma", "RM", "00100", "IT")]
        [TestCase("First", "User", "first@example.com", null, "+18011234567", "Italian Street, 10", "Roma", "RM", "00100", "IT")]
        [TestCase("First", "User", "first@example.com", "+18001234567", "+18011234567", null, "Roma", "RM", "00100", "IT")]
        [TestCase("First", "User", "first@example.com", "+18001234567", "+18011234567", "Italian Street, 10", null, "RM", "00100", "IT")]
        [TestCase("First", "User", "first@example.com", "+18001234567", "+18011234567", "Italian Street, 10", "Roma", null, "00100", "IT")]
        [TestCase("First", "User", "first@example.com", "+18001234567", "+18011234567", "Italian Street, 10", "Roma", "RM", null, "IT")]
        [TestCase("First", "User", "first@example.com", "+18001234567", "+18011234567", "Italian Street, 10", "Roma", "RM", "00100", null)]
        public void DoesNotCreateIfRequiredFieldsAreMissing(string firstName,
            string lastName, string email, string phone, string fax,
            string address1, string city, string stateProvince,
            string postalCode, string country)
        {
            var client = new MockDnsimpleClient(CreateContactFixture);
            var contact = new Contact
            {
                FirstName = firstName,
                LastName = lastName,
                Address1 = address1,
                City = city,
                StateProvince = stateProvince,
                PostalCode = postalCode,
                Country = country,
                Email = email,
                Phone = phone,
                Fax = fax
            };

            Assert.Throws<Newtonsoft.Json.JsonSerializationException>(delegate
            {
                client.Contacts.CreateContact(1010, contact);
            });
        }

        [Test]
        [TestCase(1010, 1, "https://api.sandbox.dnsimple.com/v2/1010/contacts/1")]
        public void GetContact(long accountId, long contactId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetContactFixture);
            var contact = client.Contacts.GetContact(accountId, contactId).Data;


            Assert.Multiple(() =>
            {
                Assert.That(contact.OrganizationName, Is.EqualTo("Awesome Company"));
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, 1, "https://api.sandbox.dnsimple.com/v2/1010/contacts/1")]
        public void UpdateContact(long accountId, long contactId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(UpdateContactFixture);
            var contact = new Contact
            {
                Email = "changed@example.com"
            };

            var updated = client.Contacts.UpdateContact(accountId, contactId, contact).Data;

            Assert.Multiple(() =>
            {
                Assert.That(updated.Id, Is.EqualTo(contactId));
                Assert.That(updated.AccountId, Is.EqualTo(accountId));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.PATCH));
            });
        }

        [Test]
        [TestCase(1010, 1, "https://api.sandbox.dnsimple.com/v2/1010/contacts/1")]
        public void DeleteContact(long accountId, long contactId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(DeleteContactFixture);

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(delegate
                {
                    client.Contacts.DeleteContact(accountId, contactId);
                });

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.DELETE));
            });
        }

        [Test]
        [TestCase(1010, 1)]
        public void DeleteContactFails(long accountId, long contactId)
        {
            var client = new MockDnsimpleClient(DeleteContactFailedFixture);
            client.StatusCode(HttpStatusCode.BadRequest);

            Assert.Throws(
                Is.TypeOf<DnsimpleValidationException>().And.Message
                    .EqualTo("The contact cannot be deleted because it's currently in use"),
                delegate { client.Contacts.DeleteContact(accountId, contactId); });
        }
    }
}
