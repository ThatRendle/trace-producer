using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TraceProducer;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var otelExporterOtlpEndpoint = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? throw new InvalidOperationException());
var otelExporterOtlpProtocol = builder.Configuration["OTEL_EXPORTER_OTLP_PROTOCOL"]?.ToLowerInvariant() switch
{
    "grpc" => OtlpExportProtocol.Grpc,
    "http" or "http/protobuf" => OtlpExportProtocol.HttpProtobuf,
    _ => throw new InvalidOperationException(),
};

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
    {
        resource.Clear()
            .AddService("trace-producer");
    })
    .WithLogging()
    .WithTracing(tracing =>
    {
        tracing.AddSource(ActivitySources.Api.Name, ActivitySources.Backend.Name, ActivitySources.Database.Name);
        tracing.SetSampler<AlwaysOnSampler>();
    })
    .UseOtlpExporter(otelExporterOtlpProtocol, otelExporterOtlpEndpoint);

var host = builder.Build();
host.Run();
