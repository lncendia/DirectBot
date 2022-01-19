using AutoMapper;
using DirectBot.BLL.Services;
using DirectBot.Core.Configuration;
using DirectBot.Core.Repositories;
using DirectBot.Core.Services;
using DirectBot.DAL.Data;
using DirectBot.DAL.Mapper;
using DirectBot.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 27)),
        optionsBuilder => optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IInstagramRepository, InstagramRepository>();
builder.Services.AddScoped<IProxyRepository, ProxyRepository>();
builder.Services.AddScoped<ISubscribeRepository, SubscribeRepository>();
builder.Services.AddScoped<IWorkRepository, WorkRepository>();


builder.Services.AddScoped<IProxyService, ProxyService>();
builder.Services.AddScoped<IInstagramService, InstagramService>();
builder.Services.AddScoped<IInstagramLoginService, InstagramLoginService>();
builder.Services.AddScoped<IUpdateHandler<Update>, UpdateHandler>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ISubscribeService, SubscribeService>();
builder.Services.AddScoped<IWorkerService, WorkerService>();
builder.Services.AddScoped<IWorkService, WorkService>();


builder.Services.AddOptions<Configuration>().Bind(builder.Configuration.GetSection("BotConfiguration"))
    .ValidateDataAnnotations();
builder.Services.AddScoped(sp => sp.GetService<IOptions<Configuration>>()!.Value);

builder.Services.AddHttpClient("tgwebhook")
    .AddTypedClient<ITelegramBotClient>(httpClient
        => new TelegramBotClient(builder.Configuration["BotConfiguration:Token"], httpClient));


var mapperConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });

var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);


// builder.Services.AddHangfire((_, configuration) =>
// {
//     configuration.UseStorage(new MySqlStorage(builder.Configuration.GetConnectionString("Hangfire"),
//         new MySqlStorageOptions
//         {
//             TransactionIsolationLevel = IsolationLevel.ReadCommitted,
//             QueuePollInterval = TimeSpan.FromSeconds(15),
//             JobExpirationCheckInterval = TimeSpan.FromHours(1),
//             CountersAggregateInterval = TimeSpan.FromMinutes(5),
//             PrepareSchemaIfNecessary = true,
//             DashboardJobListLimit = 50000,
//             TransactionTimeout = TimeSpan.FromMinutes(1)
//         }));
//     configuration.UseSerializerSettings(new JsonSerializerSettings
//         {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
// });
// builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("tgwebhook",
        builder.Configuration["BotConfiguration:Token"],
        new {controller = "Bot", action = "Post"});
    endpoints.MapControllers();
});

app.Run();