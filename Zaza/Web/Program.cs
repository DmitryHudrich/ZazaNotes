using Microsoft.AspNetCore.Authentication.JwtBearer;
using Zaza.Web;
using Zaza.Web.DataBase;
using Zaza.Web.DataBase.Repository;
using Zaza.Web.StorageInterfaces;
using Zaza.Web.Stuff;
using Zaza.Web.Stuff.InteractLogic;

ArgumentManager.Check();

var builder = WebApplication.CreateBuilder(args);
var tokExp = builder.Configuration["Jwt:Expiry"] ?? "2";
StaticStuff.JwtExpire = double.Parse(tokExp);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(StaticStuff.JwtBearerOptions);

builder.Services.AddLogging(conf => conf.SetMinimumLevel(LoadLogLevel()));
builder.Services.AddScoped<MongoService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<RepositoryContainer>();
builder.Services.AddScoped<AuthInteractions>();
builder.Services.AddCors();

var app = builder.Build();
app.Logger.LogInformation("MongoString: " + Environment.GetEnvironmentVariable(StaticStuff.MongoStringEnvName));
if (State.UseSwagger) {
    app.Logger.LogInformation("Enabling swagger");
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseCors(options => {
    _ = options.AllowAnyHeader();
    _ = options.AllowAnyMethod();
    _ = options.AllowAnyOrigin();
});
app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) => {
    var httpContext = context.Request.HttpContext;
    var logger = httpContext.RequestServices.GetRequiredService<ILogger<WebApplication>>();

    var endpoint = httpContext.GetEndpoint()?.DisplayName;
    var method = context.Request.Method;
    app.Logger.LogDebug($"REQUEST: {endpoint}");
    await next.Invoke();
});

RouteManager.SetEndpoints(app, app.Logger);
app.Run();

LogLevel LoadLogLevel() {
    var level = builder.Configuration["Logging:LogLevel:Default"] ??
        throw new ArgumentException("Value in <Logging -> LogLevel -> Default> not found");
    Console.WriteLine("LogLevel: " + level);
    return level switch {
        "Trace" => LogLevel.Trace,
        "Debug" => LogLevel.Debug,
        "Information" => LogLevel.Information,
        "Warning" => LogLevel.Warning,
        "Error" => LogLevel.Error,
        "Critical" => LogLevel.Critical,
        "None" => LogLevel.None,
        _ => throw new ArgumentException("Unknown log level was set in configuration"),
    };
}

// for tests
public partial class Program { }
