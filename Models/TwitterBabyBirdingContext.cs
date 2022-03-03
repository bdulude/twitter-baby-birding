using Microsoft.EntityFrameworkCore;


namespace twitter_baby_birding.Models
{
    public class TwitterBabyBirdingContext : DbContext
    {
        public TwitterBabyBirdingContext(DbContextOptions options):base(options){}

        public DbSet<Tweet> Tweets {get; set;}
        public DbSet<User> Users {get; set;}
    }
}