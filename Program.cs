using MovieWatchlistAPI.Database;
using Microsoft.EntityFrameworkCore;
using MovieWatchlistAPI.Services;
using Scalar.AspNetCore;
using OpenAI;
using MovieWatchlistAPI.Services.Interfaces;
using MovieWatchlistAPI.Validators;
using FluentValidation;

namespace MovieWatchlistAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Add DbContext with SQL Server provider
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add the MovieService to the dependency injection container
            builder.Services.AddHttpClient();

            // Add OpenAI client to the dependency injection container
            builder.Services.AddSingleton<OpenAIClient>(sp =>
            {
                var apiKey = builder
                    .Configuration
                    .GetValue<string>("OpenAI:ApiKey") 
                    ?? throw new InvalidOperationException("OpenAI API key is not configured.");

                return new OpenAIClient(apiKey);
            });

            // Register the IMovieService interface with its implementation MovieService
            builder.Services.AddScoped<IMovieService, MovieService>();
            // Register the IAiService interface with its implementation OpenAiService
            builder.Services.AddScoped<IAiService, OpenAiService>();

            // FluentValidation 
            builder.Services.AddValidatorsFromAssemblyContaining<AddMovieRequestValidator>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
