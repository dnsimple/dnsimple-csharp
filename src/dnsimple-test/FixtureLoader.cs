using System;
using System.IO;
using System.Linq;

namespace dnsimple_test
{
    public class FixtureLoader
    {
        public FixtureLoader(string version)
        {
            Version = version;
        }

        private string Version { get; }

        public string JsonPartFrom(string fixture)
        {
            return LoadFixture(fixture).Split(new[] {"\r\n\r\n"},
                StringSplitOptions.RemoveEmptyEntries).Last();
        }

        private string LoadFixture(string fixture)
        {
            var path = Path.Combine(Environment.CurrentDirectory,
                $"src/dnsimple-test/fixtures/{Version}/api/" + fixture);

            return File.ReadAllText(path);
        }
    }
}