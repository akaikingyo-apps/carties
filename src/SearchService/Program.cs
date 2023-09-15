
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // for auto mapping
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());

builder.Services.AddMassTransit(x => {
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false)); // prefix with search but ignore the namespace
    x.UsingRabbitMq((context, config) => {
        config.ReceiveEndpoint("search-auction-created", e => { // if consumer exception, retry!
            e.UseMessageRetry(r => r.Interval(5, 5)); // 5 times 5 sec 
            e.ConfigureConsumer<AuctionCreatedConsumer>(context); // configure for auction created
        });
        config.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
 
 

app.UseAuthorization();

app.MapControllers();

 // on app start, run asyncly the following code:
app.Lifetime.ApplicationStarted.Register(async () => {
    try {
    await DbInitializer.InitDb(app);
    } 
    catch (Exception e)
    {
            Console.WriteLine(e);
    }
});


 app.Run();

// the below method defines a policy (base on polly extension) to
// handle http error or if result is 404 then do retry forever every 3 sec

static IAsyncPolicy<HttpResponseMessage> GetPolicy()
=> HttpPolicyExtensions.HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
    .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));

