using AuctionService;
using AuctionService.Data;
using AuctionService.Entities;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(opt =>  {
   opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var useOutbox = false;

builder.Services.AddMassTransit(x => {
    if (useOutbox) { // not sure why messaging not working after using outbox ..
        x.AddEntityFrameworkOutbox<AuctionDbContext>(options => { // masstransit.entityframeworkcore package
            options.QueryDelay = TimeSpan.FromSeconds(10);
            options.UsePostgres();
            options.UseBusOutbox();
        });
    }
    x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));
    x.UsingRabbitMq((context, config) => {
         
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.Authority = builder.Configuration["IdentityServiceUrl"]; // will talk to server
        options.RequireHttpsMetadata = false; // running on http now
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.NameClaimType = "username";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthentication(); // call this before auth, do the jwt thingly
app.UseAuthorization();


app.MapControllers();

try
{
    DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();
