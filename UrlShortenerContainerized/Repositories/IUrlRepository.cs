namespace UrlShortenerContainerized.Repositories;

public interface IUrlRepository
{
    void Add(string key, string value);
    string? Get(string key);
}