namespace WeightAnalysis.Components.Pages
{
    using Configurations;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.WebUtilities;
    using Models;
    using System;
    using Microsoft.EntityFrameworkCore;

    public partial class Analyzer
    {
        [Inject] private WithingsConfiguration? Configuration { get; set; }
        [Inject] private IDbContextFactory<WeightContext>? ContextFactory { get; set; }
        [Inject] private NavigationManager? NavigationManager { get; set; }
        private string? Url { get; set; }
        private string? DataCsv { get; set; }

        protected override Task OnInitializedAsync()
        {
            HandleTokenValidation();

            Url = BuildWithingsAccountUrl();

            return base.OnInitializedAsync();
        }

        private void GetWeightData()
        {
            if (ContextFactory != null)
                DataCsv = Models.GetWeightData.getWeightData(ContextFactory);
        }

        private bool IsUserTokenValid(User user)
        {
            return user.AccessTokenExpiresAt <= DateTime.UtcNow;
        }

        private void HandleTokenValidation()
        {
            using (var dbContext = ContextFactory?.CreateDbContext())
            {
                var user = dbContext?.Users.FirstOrDefault();

                if (user == null || IsUserTokenValid(user))
                {
                    if (Url != null)
                        NavigationManager?.NavigateTo(Url);
                }
            }
        }

        private string BuildWithingsAccountUrl()
        {
            var baseUrl = "https://account.withings.com/oauth2_user/authorize2";

            var query = new Dictionary<string, string?>
            {
                { "response_type", "code" },
                { "client_id", Configuration?.CLientId },
                { "scope", "user.info,user.metrics,user.activity" },
                { "redirect_uri", Configuration?.RedirectUrl },
            };

            return QueryHelpers.AddQueryString(baseUrl, query);
        }
    }

    public class MeasurementData
    {
        public MeasurementData()
        {

        }

        public MeasurementData(DateTime timestamp, double value)
        {
            Value = value;
            Timestamp = timestamp;
        }

        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
    }
}
