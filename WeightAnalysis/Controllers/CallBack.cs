using Microsoft.AspNetCore.Mvc;

namespace WeightAnalysis.Controllers
{
    using System.Runtime.InteropServices.JavaScript;
    using Configurations;
    using Microsoft.AspNetCore.Components;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Microsoft.EntityFrameworkCore;
    using WeightAnalysis.Models;
    using static System.Net.WebRequestMethods;
    using System;

    [Microsoft.AspNetCore.Mvc.Route("callback")]
    public class CallBack : Controller
    {
        private WithingsConfiguration Configuration { get; }
        private WeightContext DbContext { get; }


        public CallBack(WithingsConfiguration config, WeightContext dbContext)
        {
            Configuration = config;
            this.DbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string code, string state)
        {
            var uri = new Uri("https://wbsapi.withings.net/v2/oauth2");

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "action", "requesttoken" },
                { "client_id", "6254ee5ff962ff5df7205878ebf19b15f5e54ef84a60d3ad47e913b61d2465cd" },
                { "client_secret", "c0abb7e8249a8bc3604b782996e7aa0442394481ef3fb4be95e5944057b2e85e" },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", Configuration.RedirectUrl }
            };
            var requestContent = new FormUrlEncodedContent(data);

            var client = new HttpClient();
            var response = await client.PostAsync(uri, requestContent);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            RequestTokenResponseBody? responseData = JsonSerializer.Deserialize<RequestTokenResponse>(content)?.Body;

            var user = DbContext.Users.SingleOrDefault(u => u.WithingsUserId == responseData.userid);

            Models.User.Upsert(user, DbContext, responseData.accessToken, responseData.expiresIn, responseData.refreshToken, responseData.userid);

            return Redirect("/analyzer");
        }

    }
}
