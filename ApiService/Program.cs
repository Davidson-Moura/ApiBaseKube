using Microsoft.AspNetCore.Localization;
using ApiService.Definitions;
using Common.Messages;
using ApiService.Infra.Hubs;

var builder = WebApplication.CreateBuilder(args);

Setup.ConfigServices(builder);

var app = builder.Build();

app.UseExceptionHandler();
app.UseMiddleware<UnitOfWorkMiddleware>();

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("pt-BR"),
    SupportedCultures = SMessage.SupportedCultures,
    SupportedUICultures = SMessage.SupportedCultures
};

app.UseRequestLocalization(localizationOptions);

app.UseSwagger(c =>
{
    c.RouteTemplate = "documentation/{documentName}/swagger.json";
});
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "documentation";
    c.SwaggerEndpoint("/documentation/v1/swagger.json", "Api Service v1");

    c.DocumentTitle = "Api Service - Documentação";
    c.DefaultModelsExpandDepth(-1); // Oculta schemas
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
});

app.UseHttpLogging();
app.UseCors(x =>
{
    x.SetIsOriginAllowed(orgin => true);
    x.AllowAnyHeader();
    x.AllowAnyMethod();
    x.AllowCredentials();
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");
app.MapHub<SessionHub>("/SessionHub");
app.MapHealthChecks("/health").AllowAnonymous();

app.Run();
