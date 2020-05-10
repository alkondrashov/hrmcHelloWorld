using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using hrmcHelloWorld.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace hrmcHelloWorld.Services
{
    public class HrmcTokenService : ITokenService
    {
        private HrmcToken token;
        private readonly IOptions<HrmcSettings> hrmcSettings;
        
        public HrmcTokenService(IOptions<HrmcSettings> hrmcSettings)
        {
            this.hrmcSettings = hrmcSettings;
        }

        private async Task<HrmcToken> GetNewAccessToken()
        {
            var form = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
                {"client_id", this.hrmcSettings.Value.ClientId},
                {"client_secret", this.hrmcSettings.Value.ClientSecret},
            };

            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(this.hrmcSettings.Value.TokenUrl),
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/vnd.hmrc.1.0+json") }}

            };
            HttpResponseMessage tokenResponse =
                await client.PostAsync("", new FormUrlEncodedContent(form));
            
            if (!tokenResponse.IsSuccessStatusCode)
            {
                throw new ApplicationException("Unable to retrieve access token from Hrmc");
            }

            var hrmcTokenRaw = await tokenResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<HrmcToken>(hrmcTokenRaw);
        }
        
        public async Task<string> GetToken()
        {
            if (this.token == null)
            {
                this.token = await this.GetNewAccessToken();
            }
            return token.AccessToken;
        }
    }
}