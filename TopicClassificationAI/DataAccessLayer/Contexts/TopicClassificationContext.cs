using DataAccessLayer.Models;
using Microsoft.Data.Entity;

namespace DataAccessLayer.Contexts
{
	public class TopicClassificationContext : DbContext
	{
		public DbSet<Word> Words { get; set; }

		public DbSet<Article> Articles { get; set; }

		public DbSet<WordOccurence> WordOccurences { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite("Filename=TopicClassifications.db");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Word>()
				.Property(e => e.Id).IsRequired();

			modelBuilder.Entity<Word>()
				.Property(e => e.Text).IsRequired();

			modelBuilder.Entity<WordOccurence>()
				.Property(e => e.Id).IsRequired();

			modelBuilder.Entity<WordOccurence>()
				.Property(e => e.ArticleId).IsRequired();

			modelBuilder.Entity<WordOccurence>()
				.Property(e => e.WordId).IsRequired();

			modelBuilder.Entity<Article>()
				.Property(e => e.Id).IsRequired();

			modelBuilder.Entity<Article>()
				.Property(e => e.Text).IsRequired();
		}
	}
}
