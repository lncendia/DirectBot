using AutoMapper;
using AutoMapper.EquivalencyExpression;
using AutoMapper.Extensions.ExpressionMapping;
using DirectBot.API.Mapper;
using DirectBot.BLL.HangfireAuthorization;
using DirectBot.BLL.Mapper;
using DirectBot.BLL.Services;
using DirectBot.Core.Configuration;
using DirectBot.Core.Repositories;
using DirectBot.Core.Services;
using DirectBot.DAL.Data;
using DirectBot.DAL.Repositories;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), optionsBuilder =>
    {
        optionsBuilder.EnableRetryOnFailure(1);
        optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
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
builder.Services.AddScoped<IBillService, BillService>();
builder.Services.AddScoped<ISubscribeService, SubscribeService>();
builder.Services.AddScoped<IWorkerService, WorkerService>();
builder.Services.AddScoped<IWorkService, WorkService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddScoped<IInstagramUsersGetterService, InstagramUsersGetterService>();
builder.Services.AddScoped<IMailingService, MailingService>();
builder.Services.AddScoped<IFileDownloader, FileDownloader>();
builder.Services.AddScoped<IWorkNotifier, WorkNotifier>();
builder.Services.AddScoped<IBackgroundJobService, BackgroundJobService>();
builder.Services.AddScoped<ISubscribeDeleter, SubscribeDeleter>();
builder.Services.AddScoped<IWorkDeleter, WorkDeleter>();
builder.Services.AddScoped<IMessageParser, MessageParser>();
builder.Services.AddScoped<IProxyParser, ProxyParser>();


builder.Services.AddOptions<Configuration>().Bind(builder.Configuration.GetSection("BotConfiguration"))
    .ValidateDataAnnotations();
builder.Services.AddScoped(sp => sp.GetService<IOptions<Configuration>>()!.Value);

builder.Services.AddHttpClient("tgwebhook").AddTypedClient<ITelegramBotClient>(httpClient
    => new TelegramBotClient(builder.Configuration["BotConfiguration:Token"], httpClient));

builder.Services.AddAutoMapper((mc, automapper) =>
{
    automapper.AddProfile(new DirectBot.DAL.Mapper.MappingProfile());
    automapper.AddProfile(new MappingProfile());
    automapper.AddProfile(new ToLiteMapper());
    automapper.AddCollectionMappers();
    automapper.UseEntityFrameworkCoreModel<ApplicationDbContext>(mc);
    automapper.AddExpressionMapping();
}, typeof(ApplicationDbContext).Assembly);


builder.Services.AddHangfire((_, configuration) =>
{
    configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("Hangfire2"),
        new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true // Migration to Schema 7 is required
        });

    configuration.UseSerializerSettings(new JsonSerializerSettings
        {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
    RecurringJob.AddOrUpdate("subscribesChecker",
        () => _.CreateScope().ServiceProvider.GetService<ISubscribeDeleter>()!.StartDeleteAsync(),
        Cron.Daily);

    RecurringJob.AddOrUpdate("worksChecker",
        () => _.CreateScope().ServiceProvider.GetService<IWorkDeleter>()!.StartDeleteAsync(),
        Cron.Daily);
});
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.UseHangfireDashboard($"/{builder.Configuration["BotConfiguration:Token"]}/hangfire",
    new DashboardOptions
    {
        AppPath = $"/{builder.Configuration["BotConfiguration:Token"]}",
        Authorization = new[] {new NoAuthorizationFilter()}
    });
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("tgwebhook",
        builder.Configuration["BotConfiguration:Token"],
        new {controller = "Bot", action = "Post"});
    endpoints.MapControllerRoute(
        "default",
        builder.Configuration["BotConfiguration:Token"] + "/{controller=Proxy}/{action=Index}/{id?}");
    endpoints.MapControllers();
});
app.Run();