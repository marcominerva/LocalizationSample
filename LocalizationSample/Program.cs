using System.Diagnostics;
using FluentValidation.AspNetCore;
using LocalizationSample.Resources;
using LocalizationSample.Swagger;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
            .Where(e => e.Value?.Errors.Any() ?? false)
            .Select(e => new ValidationError(e.Key, e.Value!.Errors.First().ErrorMessage));

        var httpContext = actionContext.HttpContext;
        var statusCode = StatusCodes.Status400BadRequest;
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = Messages.ValidationErrorsOccurred,
            Type = $"https://httpstatuses.io/{statusCode}",
            Instance = httpContext.Request.Path
        };
        problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? httpContext.TraceIdentifier);
        problemDetails.Extensions.Add("errors", errors);

        var result = new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };

        return result;
    };
});

var supportedCultures = new[] { "en", "it" };
var localizationOptions = new RequestLocalizationOptions()
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures)
    .SetDefaultCulture(supportedCultures[0]);

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SupportedCultures = localizationOptions.SupportedCultures;
    options.SupportedUICultures = localizationOptions.SupportedUICultures;
    options.DefaultRequestCulture = localizationOptions.DefaultRequestCulture;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<CultureAwareOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRequestLocalization();

app.UseAuthorization();

app.MapControllers();

app.Run();

public record class ValidationError(string Name, string Message);