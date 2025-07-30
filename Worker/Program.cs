using Application.Extensions;
using Infrastructure.Extensions;
using Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<EmailProcessorWorker>();

var host = builder.Build();
host.Run();
