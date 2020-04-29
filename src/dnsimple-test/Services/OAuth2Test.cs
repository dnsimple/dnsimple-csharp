using System.Collections.Generic;
using dnsimple;
using dnsimple.Services;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class OAuth2Test
    {
        private Dictionary<OAuthParams, string> _authArguments;

        private static void SetupMockHttpService(Mock<HttpService> http,
            IMock<RestResponse> mockRestResponse)
        {
            var response = new RestResponse();
            response.Content = mockRestResponse.Object.Content;
            http.Setup(mock =>
                    mock.RequestBuilder(It.IsAny<string>()))
                .Returns(new RequestBuilder(""));

            http.Setup(mock =>
                mock.Execute(
                    It.IsAny<RestRequest>()
                )
            ).Returns(response);
        }

        [Test]
        public void ExchangeAuthorizationForToken()
        {
            var http = new Mock<HttpService>(new RestClient(), new RequestBuilder());
            var mockRestResponse = new Mock<RestResponse>();
            var oauthService = new OAuth2Service(http.Object);

            mockRestResponse.Object.Content =
                new FixtureLoader("v2", "oauthAccessToken/success.http")
                    .ExtractJsonPayload();

            SetupMockHttpService(http, mockRestResponse);

            _authArguments = new Dictionary<OAuthParams, string>
            {
                {OAuthParams.ClientId, "id"},
                {OAuthParams.ClientSecret, "secret"},
                {OAuthParams.Code, "code"},
                {OAuthParams.State, "state"},
                {OAuthParams.RedirectUri, "/redirectUri"}
            };
            var accessTokenData =
                oauthService.ExchangeAuthorizationForToken(_authArguments);
            Assert.AreEqual("zKQ7OLqF5N1gylcJweA9WodA000BUNJD",
                accessTokenData.AccessToken);
        }
    }
}