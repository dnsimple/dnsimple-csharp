using System.Collections.Generic;
using System.Collections.ObjectModel;
using dnsimple;
using dnsimple.Services;
using NUnit.Framework;
using RestSharp;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class HttpTest
    {
        [Test]
        public void ReturnsARequestBuilder()
        {
            var http = new HttpService(null, new RequestBuilder());
            Assert.IsInstanceOf(typeof(RequestBuilder),
                http.RequestBuilder(""));
        }
    }

    [TestFixture]
    public class RequestBuilderTest
    {
        [SetUp]
        public void Setup()
        {
            _builder = new RequestBuilder("some/path");
        }

        [TearDown]
        public void TearDown()
        {
            _builder.Reset();
        }

        private RequestBuilder _builder;

        [Test]
        public void BuildsRequestWithCustomHeaders()
        {
            var headers = new Collection<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Accept", "application/json"),
                new KeyValuePair<string, string>("Content-Type",
                    "application/x-www-form-urlencoded")
            };

            _builder.AddHeaders(headers);

            var request = _builder.Request;

            Assert.AreEqual(2, request.Parameters.Count);
        }

        [Test]
        public void BuildsRequestWithParameters()
        {
            var parameters = new Collection<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", "1"),
                new KeyValuePair<string, string>("client_secret", "secret"),
                new KeyValuePair<string, string>("grant_type",
                    "authorization_code"),
                new KeyValuePair<string, string>("code", "some_code")
            };

            _builder.AddParameters(parameters);

            var request = _builder.Request;

            Assert.AreEqual(4, request.Parameters.Count);
        }

        [Test]
        public void BuildsRequestWithPath()
        {
            const string path = "some/other/path";
            _builder.Reset().AddPath(path);

            Assert.AreEqual(path, _builder.Request.Resource);
        }

        [Test]
        public void Resets()
        {
            _builder.Method(Method.HEAD);

            Assert.IsNull(_builder.Reset().Request);
        }

        [Test]
        public void SetsTheMethod()
        {
            _builder.Method(Method.POST);
            var request = _builder.Request;

            Assert.AreEqual(Method.POST, request.Method);
        }
    }
}