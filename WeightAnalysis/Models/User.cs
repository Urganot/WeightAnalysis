namespace WeightAnalysis.Models
{
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Internal;

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string WithingsUserId { get; set; }
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiresAt { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }


        public static User Upsert(User? user, DbContext dbContext, string accessToken, int accessTokenExpiresIn,
            string refreshToken, string userId, bool saveChanges = true)
        {
            if (user != null)
            {
                user.AccessToken = accessToken;
                user.AccessTokenExpiresAt = DateTime.Now.AddSeconds(accessTokenExpiresIn).ToUniversalTime();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiresAt = DateTime.Now.AddYears(1).ToUniversalTime();

                dbContext.Update(user);
            }
            else
            {
                user = new User
                {
                    Name = "TestName",
                    Email = "TestEmail",
                    AccessToken = accessToken,
                    AccessTokenExpiresAt = DateTime.Now.AddSeconds(accessTokenExpiresIn).ToUniversalTime(),
                    RefreshToken = refreshToken,
                    RefreshTokenExpiresAt = DateTime.Now.AddYears(1).ToUniversalTime(),
                    WithingsUserId = userId
                };
                dbContext.Add(user);
            }

            if (saveChanges)
                dbContext.SaveChanges();

            return user;
        }

        public User UpdateWithNewTokens(IDbContextFactory<WeightContext> dbContextFactory, string newTokensAccessToken, int newTokensExpiresIn, string newTokensRefreshToken)
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                var updatedUser = dbContext.Users.Update(this);
                dbContext.SaveChanges();
                return updatedUser.Entity;
            }
        }
    }
}