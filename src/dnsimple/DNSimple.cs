using Newtonsoft.Json.Linq;

namespace dnsimple
{
    public interface IClient
    {
        IdentityService Identity { get; }
        JToken Get(string path);
    }
}