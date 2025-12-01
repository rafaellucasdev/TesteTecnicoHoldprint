using ContasAPagar.API.Middleware;
using ContasAPagar.API.Models;
using ContasAPagar.API.Repositories;
using ContasAPagar.API.Services;
using ContasAPagar.API.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Configurar MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Registrar Repositories
builder.Services.AddSingleton<ISupplierRepository, SupplierRepository>();
builder.Services.AddSingleton<IPayableRepository, PayableRepository>();

// Registrar Services
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IPayableService, PayableService>();

// Registrar Validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateSupplierRequestValidator>();

// Adicionar Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Manter PascalCase
    });

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Contas a Pagar API",
        Version = "v1",
        Description = "API para gerenciamento de contas a pagar e fornecedores"
    });
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware de tratamento de exceções
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();

