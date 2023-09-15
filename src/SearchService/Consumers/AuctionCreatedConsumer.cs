using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated> // event + consumer by convention => masstransit
{
    private readonly IMapper _mapper;

    public AuctionCreatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }
    
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    { 
        Console.WriteLine($"********* consuming auction created: {context.Message.Id}");
        var item = _mapper.Map<Item>(context.Message);

        // simulate an error
        if (item.Model == "Foo") throw new Exception("cannot sell car model foo");

        await item.SaveAsync(); // mongo entity
    }
}
