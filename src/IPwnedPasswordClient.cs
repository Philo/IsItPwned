namespace IsItPwned
{
    using System;
    using System.Threading.Tasks;

    public interface IPwnedPasswordClient : IDisposable
    {
        Task<int> IsPwnedAsync(string password);
        Task<int> IsPwnedAsync(byte[] sha1HashBytes);
    }
}