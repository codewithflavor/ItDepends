using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using FluentValidation;
using ItDepends.API.Common;
using ItDepends.API.Features.Boolean;
using ItDepends.API.Features.EightBall;
using ItDepends.API.Features.SmartBoolean;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Host.UseSerilog((ctx, services, cfg) =>
{
       cfg.ReadFrom.Configuration(ctx.Configuration)
          .ReadFrom.Services(services)
          .Enrich.FromLogContext();
});

builder.Logging.ClearProviders();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddScoped<ISmartBooleanService, SmartBooleanService>();

builder.AddOpenAIClient("openai")
       .AddChatClient("gpt-5-nano");
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapBooleanEndpoints();
app.MapEightBallEndpoints();
app.MapSmartBooleanEndpoints();

app.UseSerilogRequestLogging(options =>
{
       options.EnrichDiagnosticContext = (ctx, http) =>
       {
              ctx.Set("CorrelationId", http.TraceIdentifier);
              ctx.Set("RequestHost", http.Request.Host.Value);
              ctx.Set("RequestScheme", http.Request.Scheme);
              ctx.Set("ClientIP", http.Connection.RemoteIpAddress?.ToString());
              ctx.Set("UserAgent", http.Request.Headers.UserAgent.ToString());
       };
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.Run();