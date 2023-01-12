using Firefly_iii_pp_Runner.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson();
builder.Services.AddFireflyIIIPPRunnerServices(builder.Configuration);
builder.Services.AddMongoServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
