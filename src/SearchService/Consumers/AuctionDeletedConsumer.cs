using System;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
{
    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {
        Console.WriteLine("--> Consuming AuctionDeleted event");

        var result = await DB.DeleteAsync<Item>(context.Message.Id);

        if (!result.IsAcknowledged)
        {
            Console.WriteLine("--> Failed to delete item in MongoDB");
            throw new MessageException(typeof(AuctionDeleted), "Failed to delete item in MongoDB");
        }
        else
        {
            Console.WriteLine("--> Successfully deleted item in MongoDB");
        }
    }
}
