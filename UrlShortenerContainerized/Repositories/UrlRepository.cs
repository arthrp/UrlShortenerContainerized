using StackExchange.Redis;

namespace UrlShortenerContainerized.Repositories;

public class UrlRepository : IUrlRepository
{
    private readonly IDatabase _conn;
    public UrlRepository(IConfiguration configuration)
    {
        var host = configuration["Redis:Host"]!;
        var port = int.Parse(configuration["Redis:Port"]!);
        
        var muxer = ConnectionMultiplexer.Connect($"{host}:{port}");
        _conn = muxer.GetDatabase();
    }

    public void Add(string key, string value)
    {
        _conn.StringSet(key, value);
    }

    public string? Get(string key)
    {
        return _conn.StringGet(key);
    }
}