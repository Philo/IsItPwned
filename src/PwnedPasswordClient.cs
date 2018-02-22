namespace IsItPwned
{
    using System;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class PwnedPasswordClient : IPwnedPasswordClient
    {
        private HttpClient httpClient;

        public PwnedPasswordClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public PwnedPasswordClient(int timeoutInMilliseconds = 2000) : this(new HttpClient() {Timeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds)})
        {
        }

        private byte[] ComputeSha1Hash(string input)
        {
            var sha1HashBytes = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
            return sha1HashBytes;
        }

        private string ToHashedString(byte[] hash)
        {
            var sha1Hash = BitConverter.ToString(hash).Replace("-", "");
            return sha1Hash;
        }

        public async Task<int> IsPwnedAsync(string password)
        {
            var bytes = this.ComputeSha1Hash(password);
            return await this.IsPwnedAsync(bytes).ConfigureAwait(false);
        }

        public async Task<int> IsPwnedAsync(byte[] sha1HashBytes)
        {
            var hashString = this.ToHashedString(sha1HashBytes);
            var hashPrefix = hashString.Substring(0, 5);
            var hashSuffix = hashString.Substring(6);
            var stringResult = await this.httpClient.GetStringAsync($"https://api.pwnedpasswords.com/range/{hashPrefix}");
            return this.ParsePwnedRangeResult(hashSuffix, stringResult);
        }

        private int ParsePwnedRangeResult(string hashSuffix, string stringResult)
        {
            var regexPattern = $"(?<hash>{hashSuffix}.*):(?<count>\\d+)";
            var parseRangeRegex = new Regex(regexPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var match = parseRangeRegex.Match(stringResult);
            return match.Success && int.TryParse(match.Groups["count"].Value, out var count) ? count : 0;
        }

        public void Dispose()
        {
            this.httpClient.Dispose();
        }
    }
}