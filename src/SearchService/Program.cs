using System.Net;
using MassTransit;
using MongoDB.Entities;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Models;
using SearchService.RequestHelpers;
using SearchService.Services;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetRetryPolicy());
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.AddConsumersFromNamespaceContaining<AuctionUpdatedConsumer>();
    x.AddConsumersFromNamespaceContaining<AuctionDeletedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("search-auction-created", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });

        cfg.ReceiveEndpoint("search-auction-updated", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionUpdatedConsumer>(context);
        });

        cfg.ReceiveEndpoint("search-auction-deleted", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionDeletedConsumer>(context);
        });

        cfg.ConfigureEndpoints(context);
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.MapGet("/api/search", async ([AsParameters] SearchParams searchParams) =>
{
    var query = DB.PagedSearch<Item, Item>();

    if (!string.IsNullOrEmpty(searchParams.SearchTerm))
    {
        query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
    }

    query = searchParams.OrderBy switch
    {
        "make" => query.Sort(a => a.Ascending(a => a.Make)),
        "new" => query.Sort(a => a.Descending(a => a.CreatedAt)),
        _ => query.Sort(a => a.Ascending(a => a.AuctionEnd)),
    };

    query = searchParams.FilterBy switch
    {
        "finished" => query.Match(a => a.AuctionEnd < DateTime.UtcNow),
        "endingSoon" => query.Match(a => a.AuctionEnd < DateTime.UtcNow.AddHours(6)
            && a.AuctionEnd > DateTime.UtcNow),
        _ => query.Match(a => a.AuctionEnd > DateTime.UtcNow),
    };

    if (!string.IsNullOrEmpty(searchParams.Seller))
    {
        query = query.Match(a => a.Seller == searchParams.Seller);
    }

    if (!string.IsNullOrEmpty(searchParams.Winner))
    {
        query = query.Match(a => a.Winner == searchParams.Winner);
    }

    query.PageNumber(searchParams.PageNumber ?? 1).PageSize(searchParams.PageSize ?? 4);

    var result = await query.ExecuteAsync();
    return Results.Ok(new
    {
        results = result.Results,
        pageCount = result.PageCount,
        totalCount = result.TotalCount
    });
});

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DbInitializer.InitDb(app);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
});

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
 => HttpPolicyExtensions
     .HandleTransientHttpError()
     .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
     .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
