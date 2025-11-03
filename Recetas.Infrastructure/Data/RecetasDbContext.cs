using Microsoft.EntityFrameworkCore;
using Recetas.Core.Entities;

namespace Recetas.Infrastructure.Data
{
    public class RecetasDbContext : DbContext
    {
        public RecetasDbContext(DbContextOptions<RecetasDbContext> options)
            : base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<RecipeImage> RecipeImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship between Recipe and Tag
            modelBuilder.Entity<Recipe>()
                .HasMany(r => r.Tags)
                .WithMany();

            // Configure many-to-many relationship between Recipe and Ingredient
            modelBuilder.Entity<Recipe>()
                .HasMany(r => r.Ingredients)
                .WithMany();

            // Configure one-to-many relationship between Recipe and Step
            modelBuilder.Entity<Step>()
                .HasOne(s => s.Recipe)
                .WithMany(r => r.Steps)
                .HasForeignKey(s => s.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Step order
            modelBuilder.Entity<Step>()
                .Property(s => s.Order)
                .IsRequired();
        }
    }
}