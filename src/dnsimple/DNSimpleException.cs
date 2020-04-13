using System;

namespace dnsimple
{
    /// <summary>
    /// Default <c>Exception</c>
    /// </summary>
    /// <see cref="Exception"/>
    public class DNSimpleException : Exception
    {
        /// <summary>
        /// Constructs and <c>Exception</c> with a message.
        /// </summary>
        /// <param name="message">A human readable message.</param>
        public DNSimpleException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// <c>Exception</c> thrown when the resource is not found.
    /// </summary>
    /// <see cref="DNSimpleException"/>
    class NotFoundException : DNSimpleException
    {
        /// <inheritdoc />
        public NotFoundException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// <c>Exception</c> thrown when the user failed to authenticate. 
    /// </summary>
    /// <see cref="DNSimpleException"/>
    /// <see>https://developer.dnsimple.com/v2/oauth/</see>
    public class AuthenticationException : DNSimpleException
    {
        /// <inheritdoc />
        public AuthenticationException(string message) : base(message)
        {
        }
    }
}