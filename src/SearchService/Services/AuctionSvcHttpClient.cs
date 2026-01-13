using System;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionSvcHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AuctionSvcHttpClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<List<Item>> GetItemsForSearchDb()
    {
        var lastUpdated = await DB.Find<Item, string>()
            .Sort(i => i.Descending(i => i.UpdatedAt))
            .Project(a => a.UpdatedAt.ToString())
            .ExecuteSingleAsync();

        return await _httpClient.GetFromJsonAsync<List<Item>>(
            $"{_configuration["AuctionServiceUrl"]}/api/auctions?date={Uri.EscapeDataString(lastUpdated ?? string.Empty)}"
        ) ?? new List<Item>();
    }
}
