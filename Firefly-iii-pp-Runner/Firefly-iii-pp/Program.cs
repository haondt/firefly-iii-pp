using Firefly_iii_pp_Runner.Extensions;
using Firefly_pp_Runner.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

var builder = WebApplication.CreateBuilder();
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ConfigureFireflyppRunnerSettings();
    });
builder.Services.AddFireflyIIIPPRunnerServices(builder.Configuration);

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.Map("api/{**slug}", context =>
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return Task.CompletedTask;
    });
    app.MapFallbackToFile("index.html");
});

app.Run();
