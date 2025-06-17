using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TransactionService.Configuration;
using TransactionService.Db;
using TransactionService.Models;
using TransactionService.Repository;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMapster();
        TypeAdapterConfig.GlobalSettings.Scan(typeof(MappingConfig).Assembly);
        TypeAdapterConfig.GlobalSettings.Apply(new MappingConfig());

        var connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string"
                + "'DefaultConnection' not found.");

        builder.Services.AddDbContext<TransactionDbContext>(options =>
            options.UseNpgsql(connectionString));


        builder.Services.AddScoped<Transaction.ITransactionService, Transaction.TransactionService>();
        builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
        builder.Services.AddScoped<IClientBalanceRepository, ClientBalanceRepository>();
        builder.Services.AddScoped<IClientRepository, ClientRepository>();
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<CreditRequestValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<DebitRequestValidator>();



        builder.Services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Transaction Service API",
                Version = "v1"
            });
        });

        builder.Services.AddControllers();

        builder.Services.AddProblemDetails();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider
                .GetRequiredService<TransactionDbContext>();
            dbContext.Database.Migrate();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }


        app.UseExceptionHandler();
        app.UseStatusCodePages();

        app.UseRouting();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.Run();
    }
}