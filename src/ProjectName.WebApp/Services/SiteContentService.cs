using System.Net.Http.Json;

namespace ProjectName.WebApp;

public class SiteContent
{
    public string? Name { get; set; }
    public Dictionary<string, object>? Business { get; set; }
    public Dictionary<string, object>? Theme { get; set; }
    public Dictionary<string, object>? Analytics { get; set; }
    public Dictionary<string, object>? Hero { get; set; }
    public Dictionary<string, object>? About { get; set; }
    public IEnumerable<Dictionary<string, object>>? Services { get; set; }
    public Dictionary<string, object>? Metrics { get; set; }
}

public class SiteContentService(HttpClient http)
{
    private readonly HttpClient _http = http;
    private SiteContent? _cache;

    public async Task<SiteContent?> GetAsync(bool forceRefresh = false)
    {
        if (!forceRefresh && _cache is not null) return _cache;
        try
        {
            _cache = await _http.GetFromJsonAsync<SiteContent>("data/site.json");
        }
        catch
        {
            _cache = null;
        }
        return _cache;
    }
}
