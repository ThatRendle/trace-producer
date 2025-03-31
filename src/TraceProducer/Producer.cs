using System.Diagnostics;

namespace TraceProducer;

public static class Producer
{
    public static Activity? ProduceApiTrace(string name, ActivityKind kind,
        int errorProbability) =>
        CreateTrace(ActivitySources.Api, name, kind, errorProbability);

    public static Activity? ProduceBackendTrace(string name, ActivityKind kind,
        int errorProbability) =>
        CreateTrace(ActivitySources.Backend, name, kind, errorProbability);

    public static Activity? ProduceDatabaseTrace(string name, ActivityKind kind,
        int errorProbability) =>
        CreateTrace(ActivitySources.Database, name, kind, errorProbability);

    public static async Task PauseAsync(Range timeRangeInMilliseconds, CancellationToken cancellationToken = default)
    {
        var timeInMilliseconds =
            Random.Shared.Next(timeRangeInMilliseconds.Start.Value, timeRangeInMilliseconds.End.Value);
        await Task.Delay(timeInMilliseconds, cancellationToken);
    }

    private static Activity? CreateTrace(ActivitySource activitySource, string name, ActivityKind kind,
        int errorProbability)
    {
        var activity = activitySource.StartActivity(name, kind);
        if (activity is null) return null;

        if (Random.Shared.Next(errorProbability) == 0)
        {
            activity.SetStatus(ActivityStatusCode.Error, "Random error");
        }
        else
        {
            activity.SetStatus(ActivityStatusCode.Ok, "Ok");
        }

        return activity;
    }
}

public static class ActivitySources
{
    public static readonly ActivitySource Api = new ActivitySource("api");
    public static readonly ActivitySource Backend = new ActivitySource("backend");
    public static readonly ActivitySource Database = new ActivitySource("database");
}