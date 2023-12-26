using System;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Castle.Core.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

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

        private string JsonPartFrom(string fixture)
        {
            Fixture = fixture;
            LoadFixture();
            var lastLine = GetLines(true).Last();
            return IsValidJson(lastLine) ? lastLine : "";
        }

        private static bool IsValidJson(string lastLine)
        {
            try
            {
                JToken.Parse(lastLine);
                return true;
            }
            catch (JsonReaderException ex)
            {
                Trace.WriteLine(ex);
                return false;
            }
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
                $"fixtures/{Version}/api/" + Fixture);

            return File.ReadAllText(path);
        }

        public List<Parameter> ExtractHeaders()
        {
            var headers = new List<Parameter>();

            foreach (var line in GetLines())
            {
                if (String.IsNullOrEmpty(line))
                    break;
                if (line.Contains(':'))
                {
                    var header = line.Split(':');
                    headers.Add(new Parameter(header[0], header[1], ParameterType.HttpHeader));
                }
            }

            return headers;
        }

        public HttpStatusCode ExtractStatusCode()
        {
            return (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode),
                GetLines().First().Split(' ')[1]);
        }
    }
}
