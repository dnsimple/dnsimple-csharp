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
    public class ContactsService : Service
    {
        /// <inheritdoc cref="Service"/>
        public ContactsService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// Lists contacts in the account.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <returns>The list of contacts in the account</returns>
        /// <see cref="Contact"/>
        /// <see>https://developer.dnsimple.com/v2/contacts/#listContacts</see>
        public PaginatedDnsimpleResponse<Contact> ListContacts(long accountId)
        {
            return new PaginatedDnsimpleResponse<Contact>(
                Client.Http.Execute(Client.Http
                    .RequestBuilder(ContactsPath(accountId)).Request));
        }

        /// <summary>
        /// Lists the contacts in the account according to the options sent.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="options">Options passed to the list (sorting and
        /// pagination).</param>
        /// <returns>The list of contacts in the account.</returns>
        /// <see cref="ContactsListOptions"/>
        /// <see>https://developer.dnsimple.com/v2/contacts/#listContacts</see>
        public PaginatedDnsimpleResponse<Contact> ListContacts(long accountId,
            ContactsListOptions options)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(ContactsPath(accountId));
            requestBuilder.AddParameter(options.UnpackSorting());

            if (!options.Pagination.IsDefault())
            {
                requestBuilder.AddParameters(options.UnpackPagination());
            }

            return new PaginatedDnsimpleResponse<Contact>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Creates a contact.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="contact">The contact to create</param>
        /// <returns>The newly created contact for the account</returns>
        /// <see cref="Contact"/>
        /// <see>https://developer.dnsimple.com/v2/contacts/#createContact</see>
        public SimpleDnsimpleResponse<Contact> CreateContact(long accountId,
            Contact contact)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(ContactsPath(accountId));
            requestBuilder.Method(Method.POST);
            requestBuilder.AddJsonPayload(contact);

            return new SimpleDnsimpleResponse<Contact>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Retrieves a contact of the account
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="contactId">The contact id</param>
        /// <returns>The contact requested</returns>
        /// <see cref="Contact"/>
        /// <see>https://developer.dnsimple.com/v2/contacts/#getContact</see>
        public SimpleDnsimpleResponse<Contact> GetContact(long accountId,
            long contactId)
        {
            return new SimpleDnsimpleResponse<Contact>(
                Client.Http.Execute(Client.Http
                    .RequestBuilder(ContactPath(accountId, contactId))
                    .Request));
        }

        /// <summary>
        /// Updates a contact
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="contactId">The contact id</param>
        /// <param name="contact">The contact data we want to update</param>
        /// <returns>The updated contact</returns>
        /// <see cref="Contact"/>
        /// <see>https://developer.dnsimple.com/v2/contacts/#updateContact</see>
        public SimpleDnsimpleResponse<Contact> UpdateContact(long accountId,
            long contactId, Contact contact)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(ContactPath(accountId,
                    contactId));
            requestBuilder.Method(Method.PATCH);
            requestBuilder.AddJsonPayload(contact);

            return new SimpleDnsimpleResponse<Contact>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Deletes a contact
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="contactId">The contact id</param>
        /// <see>https://developer.dnsimple.com/v2/contacts/#deleteContact</see>
        /// <exception cref="DnSimpleValidationException">If the contact cannot
        /// be deleted because it’s currently used by a domain or a
        /// certificate.</exception>
        public EmptyDnsimpleResponse DeleteContact(long accountId, long contactId)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(ContactPath(accountId,
                    contactId));
            requestBuilder.Method(Method.DELETE);

            return new EmptyDnsimpleResponse(Client.Http.Execute(requestBuilder.Request));
        }
    }

    /// <summary>
    /// Represents a contact.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/openapi.yml</see>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct Contact
    {
        public long? Id { get; set; }
        public long? AccountId { get; set; }
        public string Label { get; set; }

        // [JsonProperty(Required = Required.Always)]
        public string FirstName { get; set; }

        // [JsonProperty(Required = Required.Always)]
        public string LastName { get; set; }

        public string JobTitle { get; set; }
        public string OrganizationName { get; set; }

        // [JsonProperty(Required = Required.Always)]
        public string Email { get; set; }

        // [JsonProperty(Required = Required.Always)]
        public string Phone { get; set; }

        // [JsonProperty(Required = Required.Always)]
        public string Fax { get; set; }

        // [JsonProperty(Required = Required.Always)]
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        // [JsonProperty(Required = Required.Always)]
        public string City { get; set; }

        // [JsonProperty(Required = Required.Always)]
        public string StateProvince { get; set; }

        // [JsonProperty(Required = Required.Always)]
        public string PostalCode { get; set; }

        // [JsonProperty(Required = Required.Always)]
        public string Country { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}