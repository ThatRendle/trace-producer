# Trace Producer

- `docker compose up` in the `docker` directory
- Set the `OTEL_EXPORTER_OTLP_ENDPOINT` and `OTEL_EXPORTER_OTLP_PROTOCOL` environment variables or in `appsettings`
- Run the `TraceProducer` project
- Counts of `success` vs `error` traces are displayed in logs
