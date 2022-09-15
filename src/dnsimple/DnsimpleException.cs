using System;
using Newtonsoft.Json.Linq;

namespace dnsimple
{
    /// <summary>
    /// Default <c>Exception</c>
    /// </summary>
    /// <see cref="Exception"/>
    public class DnsimpleException : Exception
    {
        /// <summary>
        /// Constructs and <c>Exception</c> with a message.
        /// </summary>
        /// <param name="message">A human readable message.</param>
        public DnsimpleException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// <c>Exception</c> thrown when the resource is not found.
    /// </summary>
    /// <see cref="DnsimpleException"/>
    public class NotFoundException : DnsimpleException
    {
        /// <inheritdoc />
        public NotFoundException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// <c>Exception</c> thrown when the user failed to authenticate. 
    /// </summary>
    /// <see cref="DnsimpleException"/>
    /// <see>https://developer.dnsimple.com/v2/oauth/</see>
    public class AuthenticationException : DnsimpleException
    {
        /// <inheritdoc />
        public AuthenticationException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// <c>Exception</c> thrown when there are validation errors.
    /// </summary>
    public class DnsimpleValidationException : DnsimpleException
    {
        public JObject Validation { get; }

        public DnsimpleValidationException(JToken error) : base(error["message"]?.ToString()) => Validation = (JObject)error["errors"];

        public JObject GetAttributeErrors() => Validation;
    }
}