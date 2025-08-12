using TickerQ.DependencyInjection.Hosting;
using Worker.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWorker(builder.Configuration);

var host = builder.Build();

host.UseTickerQ();

host.Run();
