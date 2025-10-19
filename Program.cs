using ArticoliWebService.Services;
using Microsoft.EntityFrameworkCore; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Per l'accesso al backend dall'eseterno
builder.Services.AddCors();
builder.Services.AddDbContext<AlphaShopDbContext>();
// Inversion Of Control: a livello di configurazione si specifica quale classe dobbiamo implementare per un'interfaccia
builder.Services.AddScoped<IArticoliRepository, ArticoliRepository>();
// Tutti gli assembly della nostra applicazione sono soggetti ad automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseCors(options =>
    options
        .WithOrigins("http://localhost:4200")
        .WithMethods("GET", "POST", "PUT", "DELETE")
        .AllowAnyHeader()
);

app.MapControllers();
app.Run();