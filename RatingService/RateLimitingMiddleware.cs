namespace RatingService;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly int _maxRequests;
    private readonly TimeSpan _interval;

    public RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache, int maxRequests, TimeSpan interval)
    {
        _next = next;
        _cache = cache;
        _maxRequests = maxRequests;
        _interval = interval;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Get the IP address of the remote client
        var remoteIpAddress = context.Connection.RemoteIpAddress.ToString();

        //if some reason remoteIpAddress can't read from context go next middleware
        if (remoteIpAddress == null) await _next(context);
        
        // Generate a cache key for the IP address and interval
        var cacheKey = $"{remoteIpAddress}-{_interval.TotalSeconds}";

        // Get the current number of requests for this IP address and interval
        var requestCount = _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.SetAbsoluteExpiration(_interval);
            return 0;
        });

        // Check if the request count is over the limit
        if (requestCount >= _maxRequests)
        {
            context.Response.StatusCode = 429; // Too Many Requests
            await context.Response.WriteAsync("You have exceeded the maximum number of requests. After 1 minute, Please try again later.");
            return;
        }

        // Increment the request count and update the cache
        _cache.Set(cacheKey, requestCount + 1, _interval);

        // If the request count is under the limit, call the next middleware component in the pipeline
        await _next(context);
    }
}