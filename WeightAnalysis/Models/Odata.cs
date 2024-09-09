using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Text.Json;

namespace WeightAnalysis.Models
{
    public class Odata
    {
        public static async Task<string> GetValidAccessToken(User user, IDbContextFactory<WeightContext> dbContextFactory)
        {
            if (user.AccessTokenExpiresAt <= DateTime.UtcNow)
            {
                if (user.RefreshTokenExpiresAt > DateTime.UtcNow)
                {
                    var (accessToken, expiresIn, refreshToken) = await getNewAccessTokenFromRefreshToken(user);
                    var updatedUser = user.UpdateWithNewTokens(
                        dbContextFactory, 
                        accessToken,
                        expiresIn, 
                        refreshToken
                        );

                    return updatedUser.AccessToken;
                }
                else
                {
                    throw new Exception("RefreshToken is no longer valid!");
                }
            }

            return user.AccessToken;

        }

        public static async Task<(string accessToken, int expiresIn, string refreshToken)> getNewAccessTokenFromRefreshToken(User user)
        {
            var uri = new Uri("https://wbsapi.withings.net/v2/oauth2");

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "action", "requesttoken" },
                { "client_id", "6254ee5ff962ff5df7205878ebf19b15f5e54ef84a60d3ad47e913b61d2465cd" },
                { "client_secret", "c0abb7e8249a8bc3604b782996e7aa0442394481ef3fb4be95e5944057b2e85e" },
                { "grant_type", "authorization_code" },
                { "refresh_token", user.RefreshToken },
            };

            var client = new HttpClient();
            var body = new FormUrlEncodedContent(data);
            var response = await client.PostAsync(uri, body);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            RequestTokenResponseBody? responseData = JsonSerializer.Deserialize<RequestTokenResponse>(content).Body;

            return (responseData.accessToken, responseData.expiresIn, responseData.refreshToken);
        }
    }
}
