using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <inheritdoc cref="DomainsService"/>
    /// <see>https://developer.dnsimple.com/v2/domains/collaborators/</see>
    public partial class DomainsService
    {
        /// <summary>
        /// List collaborators for the domain in the account.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <returns>A list of collaborators wrapped in a response</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/collaborators/#listDomainCollaborators</see>
        public PaginatedResponse<Collaborator> ListCollaborators(long accountId, string domainIdentifier)
        {
            var builder = BuildRequestForPath(CollaboratorsPath(accountId, domainIdentifier));

            return new PaginatedResponse<Collaborator>(Execute(builder.Request));
        }

        /// <summary>
        /// Adds a collaborator for the domain in the account.
        /// 
        /// At the time of the add, a collaborator may or may not have a
        /// DNSimple account.
        ///
        /// In case the collaborator doesn't have a DNSimple account, the system
        /// will invite her/him to register to DNSimple first and then to accept
        /// the collaboration invitation.
        ///
        /// In the other case, she/he is automatically added to the domain as
        /// collaborator. She/he can decide to reject the invitation later.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <param name="email">The email of the collaborator to be added/invited</param>
        /// <returns>The collaborator wrapped in a response.</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/collaborators/#addDomainCollaborator</see>
        public SimpleResponse<Collaborator> AddCollaborator(long accountId, string domainIdentifier, string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email cannot be null or empty");
            
            var builder = BuildRequestForPath(CollaboratorsPath(accountId, domainIdentifier));
            builder.Method(Method.POST);

            var parameters = new Collection<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("email", email)
            };
            builder.AddParameters(parameters);

            return new SimpleResponse<Collaborator>(Execute(builder.Request));
        }

        /// <summary>
        /// Removes a collaborator from the domain in the account.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <param name="collaboratorId">The collaborator id</param>
        /// <see>https://developer.dnsimple.com/v2/domains/collaborators/#removeDomainCollaborator</see>
        public EmptyResponse RemoveCollaborator(long accountId, string domainIdentifier, long collaboratorId)
        {
            var builder = BuildRequestForPath(RemoveCollaboratorPath(accountId, domainIdentifier, collaboratorId));
            builder.Method(Method.DELETE);

            return new EmptyResponse(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents a <c>Collaborator</c>
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/domains/collaborators/</see>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct Collaborator
    {
        public long Id { get; set; }
        public long DomainId { get; set; }
        public string DomainName { get; set; }
        public long? UserId { get; set; }
        public string UserEmail { get; set; }
        public bool? Invitation { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
    }
}
