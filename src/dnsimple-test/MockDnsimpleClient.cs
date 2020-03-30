using dnsimple;

using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace dnsimple_test
{
    //TODO: Define a way to load the fixture file we actually want
    public class MockDnsimpleClient : IClient
    {
        public IdentityService Identity { get; }

        public MockDnsimpleClient()
        {
            Identity = new IdentityService(this);
        }

        //TODO: Cleanup
        public JToken Get(string path)
        {
            var split = path.Split('/');
            
            var rawResponse = LoadFixture(split.Last(), "success.http" );
            
            var data = rawResponse.Split(new[] { "\r\n\r\n" },
                StringSplitOptions.RemoveEmptyEntries);
            
            return JObject.Parse(data.Last());
        }

        private static string LoadFixture(string pathSuffix, string fileName)
        {
            var path =
                Path.Combine(Environment.CurrentDirectory, @"src/dnsimple-test/fixtures/v2/api/" + pathSuffix, fileName);
            
            return File.ReadAllText(path);
        }
    }
}
