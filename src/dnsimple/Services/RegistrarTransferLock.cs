using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
  /// <inheritdoc cref="RegistrarService"/>
  /// <see>https://developer.dnsimple.com/v2/registrar/transfer-lock</see>
  public partial class RegistrarService
  {
    /// <summary>
    /// Enables the transfer lock for a domain.
    /// </summary>
    /// <param name="accountId">The account ID</param>
    /// <param name="domain">The domain name</param>
    /// <returns>The status of the transfer lock wrapped in a response</returns>
    /// <see>https://developer.dnsimple.com/v2/registrar/transfer-lock/#enableDomainTransferLock</see>
    public SimpleResponse<TransferLockStatus> EnableDomainTransferLock(long accountId, string domain)
    {
      var builder = BuildRequestForPath(TransferLockPath(accountId, domain));
      builder.Method(Method.POST);

      return new SimpleResponse<TransferLockStatus>(Execute(builder.Request));
    }

    /// <summary>
    /// Disables the transfer lock for a domain.
    /// </summary>
    /// <param name="accountId">The account ID</param>
    /// <param name="domain">The domain name</param>
    /// <returns>The status of the transfer lock wrapped in a response</returns>
    /// <see>https://developer.dnsimple.com/v2/registrar/transfer-lock/#disableDomainTransferLock</see>
    public SimpleResponse<TransferLockStatus> DisableDomainTransferLock(long accountId, string domain)
    {
      var builder = BuildRequestForPath(TransferLockPath(accountId, domain));
      builder.Method(Method.DELETE);

      return new SimpleResponse<TransferLockStatus>(Execute(builder.Request));
    }

    /// <summary>
    /// Gets the transfer lock status for a domain.
    /// </summary>
    /// <param name="accountId">The account ID</param>
    /// <param name="domain">The domain name</param>
    /// <returns>The status of the transfer lock wrapped in a response</returns>
    /// <see>https://developer.dnsimple.com/v2/registrar/transfer-lock/#getDomainTransferLock</see>
    public SimpleResponse<TransferLockStatus> GetDomainTransferLock(long accountId, string domain)
    {
      var builder = BuildRequestForPath(TransferLockPath(accountId, domain));

      return new SimpleResponse<TransferLockStatus>(Execute(builder.Request));
    }
  }

  /// <summary>
  /// Represents the Transfer Lock status for a domain
  /// </summary>
  /// <see>https://developer.dnsimple.com/v2/registrar/transfer-lock</see>
  [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
  public struct TransferLockStatus
  {
    public bool Enabled { get; set; }
  }
}
