using System;
using System.Linq;
using dnsimple.Services;
using NUnit.Framework;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class DomainsCollaboratorsTest
    {
        private MockResponse _response;

        private const string ListCollaboratorsFixture =
            "listCollaborators/success.http";
        private const string AddCollaboratorFixture =
            "addCollaborator/success.http";
        private const string AddCollaboratorInviteFixture =
            "addCollaborator/invite-success.http";
        private const string RemoveCollaboratorFixture =
            "removeCollaborator/success.http";

        private const long AccountId = 1010;

        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", ListCollaboratorsFixture);
            _response = new MockResponse(loader);
        }

        [Test]
        public void CollaboratorsResponse()
        {
            var response = new PaginatedResponse<Collaborator>(_response);

            Assert.That(response.Data.First().Id, Is.EqualTo(100));
            Assert.That(response.Data.First().DomainId, Is.EqualTo(1));
            Assert.That(response.Data.First().DomainName, Is.EqualTo("example.com"));
            Assert.That(response.Data.First().UserId, Is.EqualTo(999));
            Assert.That(response.Data.First().UserEmail, Is.EqualTo("existing-user@example.com"));
            Assert.IsFalse(response.Data.First().Invitation);

            Assert.That(response.Data.Count, Is.EqualTo(2));
        }

        [Test]
        [TestCase(1010, "100", "https://api.sandbox.dnsimple.com/v2/1010/domains/100/collaborators")]
        [TestCase(1010, "example.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/collaborators")]
        public void ListCollaborators(long accountId, string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListCollaboratorsFixture);
            var collaborators =
                client.Domains.ListCollaborators(accountId, domainIdentifier);

            Assert.Multiple(() =>
            {
                Assert.That(collaborators.Data.First().Id, Is.EqualTo(100));
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase("1", "https://api.sandbox.dnsimple.com/v2/1010/domains/1/collaborators")]
        [TestCase("example.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/collaborators")]
        public void AddCollaborator(string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(AddCollaboratorFixture);
            var collaborator =
                client.Domains.AddCollaborator(AccountId, domainIdentifier,
                    "existing-user@example.com").Data;

            Assert.Multiple(() =>
            {
                Assert.That(collaborator.UserId, Is.EqualTo(999));
                Assert.That(collaborator.UserEmail, Is.EqualTo("existing-user@example.com"));
                Assert.IsFalse(collaborator.Invitation);

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void AddCollaboratorCannotBeNullOrEmpty(string collaborator)
        {
            var client = new MockDnsimpleClient(AddCollaboratorFixture);

            Assert.Throws(
                Is.TypeOf<ArgumentException>(),
                delegate
                {
                    client.Domains.AddCollaborator(AccountId, "ruby.codes", collaborator);
                }
            );
        }

        [Test]
        [TestCase("1", "https://api.sandbox.dnsimple.com/v2/1010/domains/1/collaborators")]
        [TestCase("example.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/collaborators")]
        public void InviteCollaborator(string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient(AddCollaboratorInviteFixture);
            var collaborator = client.Domains.AddCollaborator(AccountId,
                domainIdentifier, "invited-user@example.com");

            Assert.Multiple(() =>
            {
                Assert.That(collaborator.Data.UserEmail, Is.EqualTo("invited-user@example.com"));
                Assert.IsNull(collaborator.Data.UserId);
                Assert.IsTrue(collaborator.Data.Invitation);

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase("example.com", 100, "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/collaborators/100")]
        public void RemoveCollaborator(string domainIdentifier, long collaboratorId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(RemoveCollaboratorFixture);

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(delegate
                {
                    client.Domains.RemoveCollaborator(AccountId,
                        domainIdentifier,
                        collaboratorId);
                });

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });

        }
    }
}
