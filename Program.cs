using ArticoliWebService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMvc();
// Inversion Of Control: a livello di configurazione si specifica quale classe dobbiamo implementare per un'interfaccia
builder.Services.AddScoped<IArticoliRepository, ArticoliRepository>();

var app = builder.Build();

app.UseCors(options =>
    options
        .WithOrigins("http://localhost:4200")
        .WithMethods("GET", "POST", "PUT", "DELETE")
        .AllowAnyHeader()
);

app.MapControllers();
app.Run();
