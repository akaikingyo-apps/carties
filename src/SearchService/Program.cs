
using Polly;
using Polly.Extensions.Http;
using SearchService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());

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

