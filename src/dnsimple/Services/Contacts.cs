using System;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <summary>
    /// The <c>ContactsService</c> handles the communication with the contacts
    /// related methods of the DNSimple API.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/contacts/</see>
    public class ContactsService : ServiceBase
    {
        /// <inheritdoc cref="ServiceBase"/>
        public ContactsService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// Lists the contacts in the account according to the options sent.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="options">Options passed to the list (sorting and
        /// pagination).</param>
        /// <returns>The list of contacts in the account.</returns>
        /// <see cref="ContactsListOptions"/>
        /// <see>https://developer.dnsimple.com/v2/contacts/#listContacts</see>
        public PaginatedResponse<Contact> ListContacts(long accountId,
            ContactsListOptions options = null)
        {
            var builder = BuildRequestForPath(ContactsPath(accountId));

            AddListOptionsToRequest(options, ref builder);

            return new PaginatedResponse<Contact>(Execute(builder.Request));
        }

        /// <summary>
        /// Creates a contact.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="contact">The contact to create</param>
        /// <returns>The newly created contact for the account</returns>
        /// <see cref="Contact"/>
        /// <see>https://developer.dnsimple.com/v2/contacts/#createContact</see>
        public SimpleResponse<Contact> CreateContact(long accountId,
            Contact contact)
        {
            var builder = BuildRequestForPath(ContactsPath(accountId));
            builder.Method(Method.POST);
            builder.AddJsonPayload(contact);

            return new SimpleResponse<Contact>(Execute(builder.Request));
        }

        /// <summary>
        /// Retrieves a contact of the account
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="contactId">The contact id</param>
        /// <returns>The contact requested</returns>
        /// <see cref="Contact"/>
        /// <see>https://developer.dnsimple.com/v2/contacts/#getContact</see>
        public SimpleResponse<Contact> GetContact(long accountId,
            long contactId)
        {
            return new SimpleResponse<Contact>(
                Execute(BuildRequestForPath(ContactPath(accountId, contactId))
                    .Request));
        }

        /// <summary>
        /// Updates a contact
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="contactId">The contact id</param>
        /// <param name="contact">The contact data we want to update</param>
        /// <returns>The updated contact</returns>
        /// <see cref="Contact"/>
        /// <see>https://developer.dnsimple.com/v2/contacts/#updateContact</see>
        public SimpleResponse<Contact> UpdateContact(long accountId,
            long contactId, Contact contact)
        {
            var builder = BuildRequestForPath(ContactPath(accountId, contactId));
            builder.Method(Method.PATCH);
            builder.AddJsonPayload(new UpdateContact(contact));

            return new SimpleResponse<Contact>(Execute(builder.Request));
        }

        /// <summary>
        /// Deletes a contact
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="contactId">The contact id</param>
        /// <see>https://developer.dnsimple.com/v2/contacts/#deleteContact</see>
        /// <exception cref="DnSimpleValidationException">If the contact cannot
        /// be deleted because it’s currently used by a domain or a
        /// certificate.</exception>
        public EmptyResponse DeleteContact(long accountId, long contactId)
        {
            var builder = BuildRequestForPath(ContactPath(accountId, contactId));
            builder.Method(Method.DELETE);

            return new EmptyResponse(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents a contact.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/openapi.yml</see>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct Contact
    {
        public long? Id { get; set; }
        public long? AccountId { get; set; }
        public string Label { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string FirstName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string LastName { get; set; }

        public string JobTitle { get; set; }
        public string OrganizationName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Email { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Phone { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Fax { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string City { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string StateProvince { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string PostalCode { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Country { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    internal class UpdateContact
    {
        public string Label { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }
        public string OrganizationName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        internal UpdateContact(Contact contact)
        {
            Label = contact.Label;
            FirstName = contact.FirstName;
            LastName = contact.LastName;
            JobTitle = contact.JobTitle;
            OrganizationName = contact.OrganizationName;
            Email = contact.Email;
            Phone = contact.Phone;
            Fax = contact.Fax;
            Address1 = contact.Address1;
            Address2 = contact.Address2;
            City = contact.City;
            StateProvince = contact.StateProvince;
            Country = contact.Country;
        }
    }
}