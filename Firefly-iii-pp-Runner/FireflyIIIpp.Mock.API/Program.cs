using Firefly_iii_pp_Runner.Extensions;
using Firefly_pp_Runner.Extensions;
using FireflyIIIpp.Mock.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var CORS_POLICY = "_ppRunnerPolicy";

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ConfigureFireflyppRunnerSettings();
    });

builder.Services.AddCors(o =>
{
    o.AddPolicy(name: CORS_POLICY,
        policy =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyHeader();
        });
});
builder.Services
    .AddFireflyIIIPPRunnerServices(builder.Configuration)
    .AddMockFireflyIIIPPServices(builder.Configuration)
    .AddMemoryPersistenceServices();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(CORS_POLICY);

app.UseAuthorization();

app.MapControllers();

app.Run();
