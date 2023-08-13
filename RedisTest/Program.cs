// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using RedisTest.Library;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serviceName = configuration["Redis:ServiceName"]!;
var host = configuration["Redis:Host"]!;
var port = int.Parse(configuration["Redis:Port"]!);
var password = configuration["Redis:Password"]!;
using var redisClient = new RedisClient(serviceName, host, port, password);

var httpClient = new HttpClient();

Console.WriteLine("Testing cache key eviction...");

foreach (var j in Enumerable.Range(0, 3))
{
    Console.WriteLine($"Iteration #{j}");

    for (int i = 0; i < 100; i++)
    {
        await GetWithExpiry(Urls.Countries5_26Mb + "?ttl=24&count=100", redisClient, httpClient, TimeSpan.FromHours(24));
    }

    await Task.Delay(1000);

    for (int i = 0; i < 70; i++)
    {
        await Get(Urls.Countries5_26Mb + "?count=70", redisClient, httpClient);
    }

    await Task.Delay(1000);

    for (int i = 0; i < 50; i++)
    {
        await GetWithExpiry(Urls.Countries5_26Mb + "?count=50&ttl=1", redisClient, httpClient, TimeSpan.FromHours(1));
    }

    await Task.Delay(1000);

    for (int i = 0; i < 10; i++)
    {
        await Get(Urls.Countries5_26Mb + "?count=10", redisClient, httpClient);
    }

    await Task.Delay(1000);

    // This should force eviction.
    await GetWithExpiry(Urls.Countries5_26Mb + "?count=1&ttl=12", redisClient, httpClient, TimeSpan.FromHours(12));

    await Task.Delay(1000); // ensure replication is finished.
}


Task<string> Get(string url, RedisClient redis, HttpClient http)
{
    return redis.Get(url, () => http.GetStringAsync(url));
}

Task<string> GetWithExpiry(string url, RedisClient redis, HttpClient http, TimeSpan expiry)
{
    return redis.Get(url, () => http.GetStringAsync(url), expiry);
}