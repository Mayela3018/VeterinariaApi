using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using VeterinariaApi.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Cadena de conexión (User Secrets en local, variable de entorno en la nube)
var cadena = builder.Configuration.GetConnectionString("VeterinariaDB")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'VeterinariaDB'.");

builder.Services.AddDbContext<VeterinariaContext>(options =>
    options.UseSqlServer(cadena));

// 2. Controladores (el ciclo JSON se evita con [JsonIgnore] en Dueno.Mascotas)
builder.Services.AddControllers();

// 3. Números estrictos en el documento OpenAPI:
//    evita que .NET 10 marque los ids como "integer | string",
//    que Swagger UI no sabe validar (bug del botón Execute)
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict);

// 4. OpenAPI (documento en /openapi/v1.json)
builder.Services.AddOpenApi();

// 5. CORS con origen concreto (NO AllowAnyOrigin)
var origenes = builder.Configuration.GetSection("Cors:Origenes").Get<string[]>()
    ?? ["http://localhost:5173"];

builder.Services.AddCors(options =>
    options.AddPolicy("Front", policy => policy
        .WithOrigins(origenes)
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

// 6. Swagger visible también en producción (lo pide el lab en la API publicada)
app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "VeterinariaApi v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseCors("Front");      // antes de UseAuthorization y MapControllers
app.UseAuthorization();

app.MapControllers();

// La raíz redirige a Swagger
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.Run();