using Firefly_iii_pp_Runner.Extensions;
using Firefly_pp_Runner.Extensions;
using FireflyIIIpp.FireflyIII.Extensions;
using FireflyIIIpp.NodeRed.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Haondt.Web.Extensions;
using System.Reflection;

const string CORS_POLICY = "_fireflyIIIPPPolicy";

var builder = WebApplication.CreateBuilder();
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ConfigureFireflyppRunnerSettings();
    });

builder.Configuration.AddEnvironmentVariables();

builder.Services
    .AddCoreServices(builder.Configuration)
    .AddFireflyIIIServices(builder.Configuration)
    .AddNodeRedServices(builder.Configuration)
    .AddFireflyIIIPPRunnerServices(builder.Configuration)
    .AddFilePersistenceServices();

builder.Services.AddMvc()
    .AddApplicationPart(Assembly.GetAssembly(typeof(Haondt.Web.Extensions.ServiceCollectionExtensions)) ?? throw new NullReferenceException());

var app = builder.Build();

app.UseStaticFiles();
//app.UseRouting();
app.UseCors(CORS_POLICY);
app.MapControllers();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//    endpoints.Map("api/{**slug}", context =>
//    {
//        context.Response.StatusCode = StatusCodes.Status404NotFound;
//        return Task.CompletedTask;
//    });
//    app.MapFallbackToFile("index.html");
//});

app.Run();
