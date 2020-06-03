using dnsimple;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace dnsimple_test
{
    [TestFixture]
    public class DnsimpleValidationExceptionTest
    {
        private JToken _jToken;

        private const string FailToCreateRecordsFixture =
            "createDelegationSignerRecord/validation-error.http";

        private const string Message = "Validation failed";
        private DnsimpleValidationException _exception;

        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", FailToCreateRecordsFixture);
            _jToken = JObject.Parse(loader.ExtractJsonPayload());
            _exception = new DnsimpleValidationException(_jToken);
        }

        [Test]
        public void ContainsMessage()
        {
            Assert.AreEqual(Message, _exception.Message);
        }

        [Test]
        [TestCase("can't be blank", "algorithm")]
        [TestCase("can't be blank", "digest")]
        [TestCase("can't be blank", "digest_type")]
        [TestCase("can't be blank", "keytag")]
        public void ContainsTheValidationErrors(string expectation,
            string field)
        {
            Assert.AreEqual(expectation,
                _exception.Validation[field]?.First?.ToString());
        }
    }
}