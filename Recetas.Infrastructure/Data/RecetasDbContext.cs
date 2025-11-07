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
        public DbSet<MeasurementUnit> MeasurementUnits { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship between Recipe and Tag
            modelBuilder.Entity<Recipe>()
                .HasMany(r => r.Tags)
                .WithMany();

            // Replace many-to-many Recipe-Ingredient with explicit join entity RecipeIngredient
            modelBuilder.Entity<RecipeIngredient>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany()
                .HasForeignKey(ri => ri.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.MeasurementUnit)
                .WithMany()
                .HasForeignKey(ri => ri.MeasurementUnitCode)
                .OnDelete(DeleteBehavior.Restrict);

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

            // MeasurementUnit constraints
            modelBuilder.Entity<MeasurementUnit>()
                .HasKey(mu => mu.Code);
            modelBuilder.Entity<MeasurementUnit>()
                .Property(mu => mu.Code)
                .ValueGeneratedNever();
            modelBuilder.Entity<MeasurementUnit>()
                .Property(mu => mu.Name)
                .IsRequired();
            modelBuilder.Entity<MeasurementUnit>()
                .HasIndex(mu => mu.Name)
                .IsUnique();
        }
    }
}