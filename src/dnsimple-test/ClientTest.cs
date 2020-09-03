using dnsimple;
using dnsimple.Services;
using Moq;
using NUnit.Framework;

namespace dnsimple_test
{
    [TestFixture]
    public class ClientTest
    {
        [SetUp]
        public void Setup()
        {
            _client = new Client();
        }

        private IClient _client;

        [Test]
        public void CanSetTheBaseUrl()
        {
            _client.ChangeBaseUrlTo("https://api.sandbox.dnsimple.com");

            Assert.AreEqual("https://api.sandbox.dnsimple.com",
                _client.BaseUrl);
        }

        [Test]
        public void HasHttpService()
        {
            Assert.IsInstanceOf(typeof(HttpService), _client.Http);
        }

        [Test]
        public void HasIdentityService()
        {
            Assert.IsInstanceOf(typeof(IdentityService), _client.Identity);
        }

        [Test]
        public void HasOAuthService()
        {
            Assert.IsInstanceOf(typeof(OAuth2Service), _client.OAuth);
        }

        [Test]
        public void HasVersion()
        {
            Assert.AreEqual("v2", _client.Version);
        }

        [Test]
        public void ReturnsInstanceOfTheIdentityService()
        {
            Assert.IsInstanceOf(typeof(IdentityService), _client.Identity);
        }

        [Test]
        public void TheDefaultBaseUrlIsProduction()
        {
            Assert.AreEqual("https://api.dnsimple.com", _client.BaseUrl);
        }

        [Test]
        public void UsesHttpBasicAuthenticator()
        {
            var restClientWrapper = new Mock<RestClientWrapper>();
            var credentials =
                new BasicHttpCredentials("example-user@example.com",
                    "secret");
#pragma warning disable 618
            IClient client = new Client(restClientWrapper.Object);
#pragma warning restore 618

            client.AddCredentials(credentials);

            restClientWrapper.Verify(
                wrapper => wrapper.AddAuthenticator(credentials), Times.Once);
        }

        [Test]
        public void UsesOauth2Authorization()
        {
            var restClientWrapper = new Mock<RestClientWrapper>();
            var credentials = new OAuth2Credentials("Token");
#pragma warning disable 618
            IClient client = new Client(restClientWrapper.Object);
#pragma warning restore 618

            client.AddCredentials(credentials);

            restClientWrapper.Verify(
                wrapper => wrapper.AddAuthenticator(credentials), Times.Once);
        }

        [Test]
        public void VersionedBaseUrl()
        {
            Assert.AreEqual("https://api.dnsimple.com/v2/",
                _client.VersionedBaseUrl());
        }

        [Test]
        public void CanSetTheUserAgent()
        {
            _client.SetUserAgent("MySuperAPP");

            Assert.AreEqual($"MySuperAPP {Client.DefaultUserAgent}", _client.UserAgent);
        }

        [Test, Description("https://github.com/dnsimple/dnsimple-csharp/issues/21")]
        public void AvoidSettingUserAgentOnChangeBaseUrlTo()
        {
            var restClientWrapper = new RestClientWrapper();
            var client = new Client(restClientWrapper);

            client.SetUserAgent("MySuperAPP");
            var userAgent = restClientWrapper.RestClient.UserAgent;

            client.ChangeBaseUrlTo("https://api.sandbox.dnsimple.com");
            
            Assert.AreEqual(userAgent, restClientWrapper.RestClient.UserAgent);
        }
    }
}