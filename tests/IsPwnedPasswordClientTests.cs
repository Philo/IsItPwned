using System.Threading.Tasks;

namespace IsItPwned.Tests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using Xunit;
    using IsItPwned;

    public class IsPwnedPasswordClientTests
    {
        private const string sampleContent
            = "5EF2001A95320ECE9BC2182446F6A620A71:55\r\n" +
              "003D68EB55068C33ACE09247EE4C639306B:3\r\n" +
              "012C192B2F16F82EA0EB9EF18D9D539B0DD:1\r\n" +
              "01330C689E5D64F660D6947A93AD634EF8F:1\r\n" +
              "0198748F3315F40B1A102BF18EEA0194CD9:1\r\n" +
              "01F9033B3C00C65DBFD6D1DC4D22918F5E9:2\r\n" +
              "0424DB98C7A0846D2C6C75E697092A0CC3E:5\r\n" +
              "047F229A81EE2747253F9897DA38946E241:1\r\n" +
              "04A37A676E312CC7C4D236C93FBD992AA3C:4\r\n" +
              "04AE045B134BDC43043B216AEF66100EE00:2\r\n" +
              "0502EA98ED7A1000D932B10F7707D37FFB4:5\r\n" +
              "0539F86F519AACC7030B728CD47803E5B22:5\r\n" +
              "054A0BD53E2BC83A87EFDC236E2D0498C08:3\r\n" +
              "1E4C9B93F3F0682250B6CF8331B7EE68FD8:3303003\r\n" +
              "34E90DD5D5C0293f86b9947A8D6F280D84F1C1BE:999\r\n" +
              "05AA835DC9423327DAEC1CBD38FA99B8834:1";

        public class FixedResponseHttpClientHandler : HttpClientHandler
        {
            private readonly string textContent;

            public FixedResponseHttpClientHandler(string textContent = null)
            {
                this.textContent = textContent;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var r = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(textContent ?? string.Empty)
                };
                return Task.FromResult(r);
            }
        }

        private async Task IsPwnedTest(HttpClient httpClient, string password, Predicate<int> outcomePredicate)
        {
            #if !NETCOREAPP2_0
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            #endif
            using (var client = new PwnedPasswordClient(httpClient))
            {
                var result = await client.IsPwnedAsync(password);

                Assert.True(outcomePredicate(result));
            }
        }

        [Theory]
        [InlineData(sampleContent, "password", 3303003)]
        [InlineData(sampleContent, "ThisIsNotAPasswordThatIsBreached#1234567890", 0)]
        [InlineData(sampleContent, "Password12345", 999)]
        [InlineData(sampleContent, "AnotherPasswordString", 55)]
        [InlineData(null, "AnotherPasswordString", 0)]
        public async Task IsPwned(string responseText, string password, int expectedCount) => await IsPwnedTest(new HttpClient(new FixedResponseHttpClientHandler(responseText)), password, count => count == expectedCount);

        [Theory]
        [InlineData("password")]
        [InlineData("Password12345")]
        public async Task ShouldBeReportedAsPwned_E2E(string password) => await IsPwnedTest(new HttpClient(), password, count => count > 0);
        
        [Theory]
        [InlineData("Super Strong Passphrase With Lots Of Characters and #@[]")]
        public async Task ShouldNotBeReportedAsPwned_E2E(string password) => await IsPwnedTest(new HttpClient(), password, count => count == 0);
    }
}
