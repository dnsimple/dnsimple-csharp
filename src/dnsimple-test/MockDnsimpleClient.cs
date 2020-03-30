using dnsimple;

using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace dnsimple_test
{
    public class MockDnsimpleClient : IClient
    {
        public IdentityService Identity { get; }
        public string Fixture { get; set; }
        private string Version { get; } = "v2";

        public MockDnsimpleClient()
        {
            Identity = new IdentityService(this);
        }
        
        public JToken Get(string path)
        {
            return JObject.Parse(JsonPartFrom(LoadFixture()));
        }

        private static string JsonPartFrom(string fixture)
        {
            return fixture.Split(new[] { "\r\n\r\n" }, 
                StringSplitOptions.RemoveEmptyEntries).Last();
        }

        private string LoadFixture()
        {
            var path = Path.Combine(Environment.CurrentDirectory,
                $"src/dnsimple-test/fixtures/{Version}/api/" + Fixture);
            
            return File.ReadAllText(path);
        }
    }
}
