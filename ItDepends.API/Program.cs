using FluentValidation;
using ItDepends.API.Common;
using ItDepends.API.Common.HealthChecks;
using ItDepends.API.Features.Boolean;
using ItDepends.API.Features.EightBall;
using ItDepends.API.Features.SmartBoolean;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Host.ConfigureApiSerilog();
builder.Logging.ClearProviders();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.AddOpenAIClient("openai")
       .AddChatClient("gpt-5-nano");
builder.Services.AddScoped<ISmartBooleanService, SmartBooleanService>();

builder.Services.AddApiHealthChecks();
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapApiHealthChecks();

app.MapBooleanEndpoints();
app.MapEightBallEndpoints();
app.MapSmartBooleanEndpoints();

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.Run();