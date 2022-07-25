using System;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace SeedWork.Distributed;

public static class ResilientSyncCommunication
{
    public static IAsyncPolicy<HttpResponseMessage> WaitAndRetry(int retryCount = 5) =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    public static IAsyncPolicy<HttpResponseMessage> Timeout(int seconds = 10) =>
        Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(seconds));

    public static IAsyncPolicy<HttpResponseMessage> CircuitBreaker(int seconds = 30) => HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(seconds));
}
