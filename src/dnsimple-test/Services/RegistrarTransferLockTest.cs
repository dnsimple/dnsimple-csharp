using NUnit.Framework;
using RestSharp;

namespace dnsimple_test.Services
{
  [TestFixture]
  public class RegistrarTransferLockTest
  {

    private const string EnableDomainTransferLockFixture =
        "enableDomainTransferLock/success.http";

    private const string DisableDomainTransferLockFixture =
        "disableDomainTransferLock/success.http";

    private const string GetDomainTransferLockFixture =
        "getDomainTransferLock/success.http";

    [Test]
    [TestCase(1010, "42", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/42/transfer_lock")]
    [TestCase(1010, "ruby.codes", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/transfer_lock")]
    public void EnableDomainTransferLock(long accountId, string domain, string expectedUrl)
    {
      var client = new MockDnsimpleClient(EnableDomainTransferLockFixture);
      var transferLockStatus = client.Registrar.EnableDomainTransferLock(accountId, domain).Data;

      Assert.Multiple(() =>
      {
        Assert.That(transferLockStatus.Enabled, Is.True);

        Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
        Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.POST));
      });
    }

    [Test]
    [TestCase(1010, "42", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/42/transfer_lock")]
    [TestCase(1010, "ruby.codes", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/transfer_lock")]
    public void DisableDomainTransferLock(long accountId, string domain, string expectedUrl)
    {
      var client = new MockDnsimpleClient(DisableDomainTransferLockFixture);
      var transferLockStatus = client.Registrar.DisableDomainTransferLock(accountId, domain).Data;

      Assert.Multiple(() =>
      {
        Assert.That(transferLockStatus.Enabled, Is.False);

        Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
        Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.DELETE));
      });
    }

    [Test]
    [TestCase(1010, "ruby.codes", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/transfer_lock")]
    public void GetDomainTransferLock(long accountId, string domain, string expectedUrl)
    {
      var client = new MockDnsimpleClient(GetDomainTransferLockFixture);
      var transferLockStatus = client.Registrar.GetDomainTransferLock(accountId, domain).Data;

      Assert.Multiple(() =>
      {
        Assert.That(transferLockStatus.Enabled, Is.True);

        Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
        Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.GET));
      });
    }
  }
}
