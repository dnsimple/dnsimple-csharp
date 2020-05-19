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
            
            Assert.AreEqual(100, response.Data.First().Id);
            Assert.AreEqual(1,
                response.Data.First().DomainId);
            Assert.AreEqual("example.com",
                response.Data.First().DomainName);
            Assert.AreEqual(999,
                response.Data.First().UserId);
            Assert.AreEqual("existing-user@example.com",
                response.Data.First().UserEmail);
            Assert.IsFalse(response.Data.First().Invitation);

            Assert.AreEqual(2, response.Data.Count);
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
                Assert.AreEqual(100,
                    collaborators.Data.First().Id);
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                Assert.AreEqual(999, collaborator.UserId);
                Assert.AreEqual("existing-user@example.com",
                    collaborator.UserEmail);
                Assert.IsFalse(collaborator.Invitation);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                delegate { 
                    client.Domains.AddCollaborator(AccountId, "ruby.codes",collaborator);
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
                Assert.AreEqual("invited-user@example.com",
                    collaborator.Data.UserEmail);
                Assert.IsNull(collaborator.Data.UserId);
                Assert.IsTrue(collaborator.Data.Invitation);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase("example.com", 100,"https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/collaborators/100")]
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
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
            
        }
    }
}