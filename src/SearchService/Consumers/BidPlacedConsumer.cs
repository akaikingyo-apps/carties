using System.Net.Mime;
using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
namespace SearchService;

public class BidPlacedConsumer: IConsumer<BidPlaced> // event + consumer by convention => masstransit
{ 
    public async Task Consume(ConsumeContext<BidPlaced> context)
    { 
        Console.WriteLine($"********* consuming bid placed: {context.Message.AuctionId}");

        var auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId);
        if (context.Message.BidStatus.Contains("Accepted") &&  context.Message.Amount > auction.CurrentHighBid)
        {
            auction.CurrentHighBid =   context.Message.Amount ;
            await auction.SaveAsync();
        } 
    }
}