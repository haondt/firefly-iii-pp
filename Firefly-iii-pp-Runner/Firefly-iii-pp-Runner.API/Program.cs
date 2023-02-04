using Firefly_iii_pp_Runner.API.Converters;
using Firefly_iii_pp_Runner.API.Extensions;
using Newtonsoft.Json.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var CORS_POLICY = "_ppRunnerPolicy";

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.Converters.Add(new ItemConverter());
        options.SerializerSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error;
        options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
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
builder.Services.AddFireflyIIIPPRunnerServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(CORS_POLICY);

app.UseAuthorization();

app.MapControllers();

app.Run();
