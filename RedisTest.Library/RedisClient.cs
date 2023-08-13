using StackExchange.Redis;

namespace RedisTest.Library;

public class RedisClient : IDisposable
{
    private readonly ConnectionMultiplexer _connection;
    private readonly Random _random = new();

    public RedisClient(string sentinelServiceName, string sentinelHost, int sentinelPort, string password)
    {
        Console.WriteLine("Creating RedisClient");

        var config = new ConfigurationOptions
        {
            ServiceName = sentinelServiceName,
            EndPoints = { $"{sentinelHost}:{sentinelPort}" },
            AllowAdmin = true,
            ConnectTimeout = 5000,
            Password = password,
            SyncTimeout = 5000
        };

        _connection = ConnectionMultiplexer.Connect(config);
    }

    public async Task<string> Get(string key, Func<Task<string>> compute, TimeSpan? expiry = null)
    {
        var database = _connection.GetDatabase();
        var value = await database.StringGetAsync(key);

        if (value.HasValue)
        {
            return value.ToString();
        }

        Console.WriteLine($"Cache MISS for key {key}. Computing and persisting value");
        var computed = await compute();
        bool isSet = await database.StringSetAsync(key, computed, expiry);
        var message = isSet
            ? $"Computed and persisted value for {key}."
            : $"Failed to persist value for {key}.";
        Console.WriteLine(message);
        
        return computed;
    }

    public async Task<string> ProbabilisticGet(string key, Func<Task<string>> compute, TimeSpan expiry)
    {
        var database = _connection.GetDatabase();

        var shouldRecompute = await ShouldRecompute(key, expiry, database);
        var isComputing = shouldRecompute && await IsComputing(key, database);
        if (!shouldRecompute || isComputing)
        {
            var value = await database.StringGetAsync(key);
            if (value.HasValue)
            {
                return value.ToString();
            }

            Console.WriteLine($"Cache MISS for key {key}. Computing and persisting value");
        }
        else
        {
            Console.WriteLine($"Refreshing key {key}");
        }

        await database.StringSetAsync(GetComputingKey(key), "True", TimeSpan.FromSeconds(5));

        var computed = await compute();
        bool isSet = await database.StringSetAsync(key, computed, expiry);
        await database.StringGetDeleteAsync(GetComputingKey(key));
        var message = isSet
            ? $"Computed and persisted value for {key}."
            : $"Failed to persist value for {key}.";
        Console.WriteLine(message);

        return computed;
    }

    private async Task<bool> IsComputing(string key, IDatabase database)
    {
        var isComputingKey = await database.StringGetAsync(GetComputingKey(key));
        return isComputingKey.HasValue;
    }

    private async Task<bool> ShouldRecompute(string key, TimeSpan expiry, IDatabase database)
    {
        var remainingTtl = await database.KeyTimeToLiveAsync(key);

        var ttlPct = remainingTtl / expiry;

        var shouldRecompute = !ttlPct.HasValue || ttlPct.Value < 0.2 && ttlPct.Value * _random.NextDouble() < 0.0001;

        if (!shouldRecompute)
        {
            return false;
        }
        
        ////if (remainingTtl.HasValue)
        ////{
        ////    Console.WriteLine(
        ////        $"Preemptively refreshing key {key} as it has only {remainingTtl.Value.TotalSeconds} seconds remaining. Ratio is {ttlPct}");
        ////}
        ////else
        ////{
        ////    Console.WriteLine($"Cache MISS for key {key}. Computing and persisting value");
        ////}

        return true;
    }

    public void Dispose()
    {
        _connection.Close();
    }

    private static string GetComputingKey(string key) => $"{key}_COMPUTING";
}
