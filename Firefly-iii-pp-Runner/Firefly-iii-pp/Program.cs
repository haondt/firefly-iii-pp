using Firefly_iii_pp_Runner.Extensions;
using Firefly_pp_Runner.Extensions;
using FireflyIIIpp.FireflyIII.Extensions;
using FireflyIIIpp.NodeRed.Extensions;
using Haondt.Web.Extensions;
using FireflyIIIpp.Web.Extensions;

const string CORS_POLICY = "_fireflyIIIPPPolicy";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddApplicationPart(typeof(Haondt.Web.Extensions.ServiceCollectionExtensions).Assembly)
    .AddApplicationPart(typeof(FireflyIIIpp.Web.Extensions.ServiceCollectionExtensions).Assembly)
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ConfigureFireflyppRunnerSettings();
    });

builder.Configuration.AddEnvironmentVariables();

builder.Services
    .AddFireflyIIIPPWebServices(builder.Configuration)
    .AddCoreServices(builder.Configuration)
    .AddFireflyIIIServices(builder.Configuration)
    .AddNodeRedServices(builder.Configuration)
    .AddFireflyIIIPPRunnerServices(builder.Configuration)
    .AddFilePersistenceServices();


builder.Services.AddMvc();
builder.Services.AddCors(o => o.AddPolicy(CORS_POLICY, p =>
{
    p.AllowAnyOrigin();
    p.AllowAnyHeader();
}));


var app = builder.Build();

app.UseStaticFiles();
app.UseCors(CORS_POLICY);
app.MapControllers();


app.Run();
