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
        private static void SetupMockHttpService(Mock<HttpService> http,
            IMock<RestResponse> mockRestResponse)
        {
            http.Setup(mock =>
                    mock.RequestBuilder(It.IsAny<string>()))
                .Returns(new RequestBuilder(""));

            http.Setup(mock =>
                mock.Execute(
                    It.IsAny<RestRequest>()
                )
            ).Returns(JObject.Parse(mockRestResponse.Object.Content));
        }

        [Test]
        public void ExchangeAuthorizationForToken()
        {
            var http = new Mock<HttpService>(null, new RequestBuilder());
            var mockRestResponse = new Mock<RestResponse>();
            var oauthService = new OAuth2Service(http.Object);

            mockRestResponse.Object.Content =
                new FixtureLoader("v2").JsonPartFrom(
                    "oauthAccessToken/success.http");

            SetupMockHttpService(http, mockRestResponse);

            var authArguments = new Dictionary<OAuthParams, string>
            {
                {OAuthParams.ClientId, "id"},
                {OAuthParams.ClientSecret, "secret"},
                {OAuthParams.Code, "code"},
                {OAuthParams.State, "state"},
                {OAuthParams.RedirectUri, "/redirectUri"}
            };
            var accessTokenData =
                oauthService.ExchangeAuthorizationForToken(authArguments);
            Assert.AreEqual("zKQ7OLqF5N1gylcJweA9WodA000BUNJD",
                accessTokenData.AccessToken);
        }

        // [Test]
        // [TestCase("810910769e5c42e5", "fSreUjaffm07A5nPlVpXuMtYgRbIbSO6", "sEu5zlQ550Fz4Qu1Fg98QQ0V9JdyAw7A", "covid19", "/callback")]
        // public void RealExchange(string id, string secret, string code, string state, string redirectUri)
        // {
        //     var client = new Client("https://api.sandbox.dnsimple.com");
        //     var authArguments = new Dictionary<OAuthParams, string>
        //     {
        //         {OAuthParams.ClientId, id},
        //         {OAuthParams.ClientSecret, secret},
        //         {OAuthParams.Code, code},
        //         {OAuthParams.State, state},
        //         {OAuthParams.RedirectUri, redirectUri}
        //     };
        //     var accessTokenData =
        //         client.OAuth.ExchangeAuthorizationForToken(authArguments);
        // }
    }
}