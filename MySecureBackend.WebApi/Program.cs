using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;
using MySecureBackend.WebApi.Repositories;
using MySecureBackend.WebApi.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Retrieve the SQL connection string from configuration.
var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString");
var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);

// Toggle between In-memory and SQL repositories
bool useInMemory = true;

if (useInMemory)
{
    builder.Services.AddSingleton<IUserAvatarRepository, MemoryUserAvatarRepository>();
    builder.Services.AddSingleton<IHighscoreRepository, MemoryHighscoreRepository>();
}
else
{
    if (!sqlConnectionStringFound)
        throw new Exception("SqlConnectionString not found for SQL repositories.");

    builder.Services.AddSingleton<IUserAvatarRepository>(new SqlUserAvatarRepository(sqlConnectionString!));
    builder.Services.AddSingleton<IHighscoreRepository>(new SqlHighscoreRepository(sqlConnectionString!));
}

// ExampleObject repository (kept from original)
builder.Services.AddTransient<IExampleObjectRepository, MemoryExampleObjectRepository>();
// To use SQL-backed ExampleObject:
// builder.Services.AddTransient<IExampleObjectRepository, SqlExampleObjectRepository>(o => new SqlExampleObjectRepository(sqlConnectionString!));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MySecureBackend API",
        Version = "v1",
    });
});

builder.Services.Configure<RouteOptions>(o => o.LowercaseUrls = true);

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()
.AddDapperStores(options =>
{
    options.ConnectionString = sqlConnectionString;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAuthenticationService, AspNetIdentityAuthenticationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MySecureBackend API v1");
        options.RoutePrefix = "swagger";
        options.CacheLifetime = TimeSpan.Zero;

        if (!sqlConnectionStringFound)
            options.HeadContent = "<h1 align=\"center\">❌ SqlConnectionString not found ❌</h1>";
    });
}
else
{
    var buildTimeStamp = File.GetCreationTime(Assembly.GetExecutingAssembly().Location);
    string currentHealthMessage = $"The API is up 🚀 | Connection string found: {(sqlConnectionStringFound ? "✅" : "❌")} | Build timestamp: {buildTimeStamp}";
    app.MapGet("/", () => currentHealthMessage);
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapGroup("/account").MapIdentityApi<IdentityUser>().WithTags("Account");

app.MapControllers();

app.MapGet("/", () => "MySecureBackend API is running!");

app.Run();