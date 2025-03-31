# Trace Producer

- For local testing, run `docker compose up` in the `docker` directory to start the Aspire dashboard
- Set the `OTEL_EXPORTER_OTLP_ENDPOINT` and `OTEL_EXPORTER_OTLP_PROTOCOL` environment variables or in `appsettings`
- Run the `TraceProducer` project
- Counts of `success` vs `error` traces are displayed in logs
