namespace WeightAnalysis.Models
{
    using Microsoft.EntityFrameworkCore;

    public class WeightContext:DbContext
    {

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=localhost:5432;Database=withings;Username=my_user;Password=my_pw");

    }
}
