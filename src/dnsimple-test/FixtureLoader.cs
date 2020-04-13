using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dnsimple_test
{
    public class FixtureLoader
    {
        private string Fixture { get; set; }
        private string Version { get; }


        public FixtureLoader(string version, string fixture)
        {
            Version = version;
            Fixture = fixture;
        }

        public string JsonPartFrom(string fixture)
        {
            Fixture = fixture;
            LoadFixture(); 
            return GetLines(true).Last();
        }

        public string ExtractJsonPayload()
        {
            return JsonPartFrom(Fixture);
        }

        private IEnumerable<string> GetLines(bool removeEmptyLines = false)
        {
            return LoadFixture().Split(new[] { "\r\n", "\r", "\n" },
                removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }
        

        private string LoadFixture()
        {
            var path = Path.Combine(Environment.CurrentDirectory,
                $"src/dnsimple-test/fixtures/{Version}/api/" + Fixture);

            return File.ReadAllText(path);
        }
    }
}