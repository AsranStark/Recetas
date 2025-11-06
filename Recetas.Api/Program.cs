using Microsoft.EntityFrameworkCore;
using Recetas.Application.Interfaces;
using Recetas.Application.Services;
using Recetas.Core.Interfaces;
using Recetas.Infrastructure.Data;
using Recetas.Infrastructure.Repositories;
using Recetas.Infrastructure.Data.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext
builder.Services.AddDbContext<RecetasDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Dependencies
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IStepService, StepService>();
builder.Services.AddScoped<IRecipeImageRepository, RecipeImageRepository>();
builder.Services.AddScoped<IRecipeImageService, RecipeImageService>();

// Configure AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Seed de datos local desde JSON (solo en Development)
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<RecetasDbContext>();
    var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("Seeder");
    await JsonSeeder.SeedAsync(db, app.Environment.ContentRootPath, logger);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
