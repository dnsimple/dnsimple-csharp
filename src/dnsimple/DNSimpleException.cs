using System;

namespace dnsimple
{
    /// <summary>
    /// Default <c>Exception</c>
    /// </summary>
    /// <see cref="Exception"/>
    public class DnSimpleException : Exception
    {
        /// <summary>
        /// Constructs and <c>Exception</c> with a message.
        /// </summary>
        /// <param name="message">A human readable message.</param>
        public DnSimpleException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// <c>Exception</c> thrown when the resource is not found.
    /// </summary>
    /// <see cref="DnSimpleException"/>
    class NotFoundException : DnSimpleException
    {
        /// <inheritdoc />
        public NotFoundException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// <c>Exception</c> thrown when the user failed to authenticate. 
    /// </summary>
    /// <see cref="DnSimpleException"/>
    /// <see>https://developer.dnsimple.com/v2/oauth/</see>
    public class AuthenticationException : DnSimpleException
    {
        /// <inheritdoc />
        public AuthenticationException(string message) : base(message)
        {
        }
    }
}