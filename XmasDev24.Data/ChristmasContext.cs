using Microsoft.EntityFrameworkCore;
using XmasDev24.Data.Models;

namespace XmasDev24.Data
{
    public class ChristmasContext(DbContextOptions<ChristmasContext> options) : DbContext(options)
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChristmasLetter>().HasKey(x => x.Id);
        }

        public DbSet<ChristmasLetter> ChristmasLetters { get; set; }
    }
}
