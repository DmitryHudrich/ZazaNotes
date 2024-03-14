using Microsoft.AspNetCore.Authentication.JwtBearer;
using Zaza.Web;
using Zaza.Web.DataBase.Repository;

var builder = WebApplication.CreateBuilder();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(StaticStuff.JwtBearerOptions);

builder.Services.AddLogging(conf => conf.SetMinimumLevel(LoadLogLevel()));

builder.Services.AddScoped<MongoService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddCors();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(options => {
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
});
app.UseAuthentication();   // добавление middleware аутентификации 
app.UseAuthorization();   // добавление middleware авторизации 

app.Use(async (context, next) => {
    var httpContext = context.Request.HttpContext;
    var logger = httpContext.RequestServices.GetRequiredService<ILogger<WebApplication>>();

    var endpoint = httpContext.GetEndpoint()?.DisplayName;
    var method = context.Request.Method;
    app.Logger.LogDebug($"REQUEST: {endpoint}");
    await next.Invoke();
});

RouteManager.SetEndpoints(app);

app.Run();

LogLevel LoadLogLevel() {
    var level = builder.Configuration["Logging:LogLevel:Default"];
    if (level == null) {
        throw new Exception("Value in <Logging -> LogLevel -> Default> not found");
    }
    System.Console.WriteLine("LogLevel: " + level);
    switch (level) {
        case "Trace":
            return LogLevel.Trace;
        case "Debug":
            return LogLevel.Debug;
        case "Information":
            return LogLevel.Information;
        case "Warning":
            return LogLevel.Warning;
        case "Error":
            return LogLevel.Error;
        case "Critical":
            return LogLevel.Critical;
        case "None":
            return LogLevel.None;
        default:
            throw new ArgumentException("Unknown log level was set in configuration");
    }
}

/*
 * TODO:
 * 1. Нормальные логи сделать
 * 2. бдшку подрубить
 */
