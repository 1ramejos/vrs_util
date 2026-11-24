#nullable enable

using System;

namespace vrsutils;

public sealed class CryptoException : Exception
{
    public CryptoException()
    {
    }

    public CryptoException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
