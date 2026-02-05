using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true);

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

app.UseSwaggerForOcelotUI(opt => { opt.PathToSwaggerGenerator = "/swagger/docs"; });
await app.UseOcelot();
await app.RunAsync();
