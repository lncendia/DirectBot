using AutoMapper;
using DirectBot.BLL.CallbackQueryCommands;
using DirectBot.BLL.Interfaces;
using DirectBot.BLL.TextCommands;
using DirectBot.Core.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.Services;

public class UpdateHandler : IUpdateHandler<Update>
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandler> _logger;
    private readonly ServiceContainer _serviceContainer;

    public UpdateHandler(IUserService userService, Core.Configuration.Configuration configuration,
        ITelegramBotClient botClient, ILogger<UpdateHandler> logger, IWorkerService workerService,
        ISubscribeService subscribeService, IProxyService proxyService, IBillService billService,
        IWorkService workService, IInstagramService instagramService,
        IInstagramLoginService instagramLoginService, IPaymentService paymentService, IMapper mapper)
    {
        _botClient = botClient;
        _logger = logger;

        _serviceContainer = new ServiceContainer(userService, workerService, subscribeService, proxyService,
            billService, instagramLoginService, instagramService, workService, paymentService, mapper, configuration);
    }

    private static readonly List<ITextCommand> TextCommands = new()
    {
        new StartCommand(),
        new BanCommand(),
        new SendKeyboardCommand(),
        new EnterChallengeRequireCodeCommand(),
        new EnterCountSubscribesCommand(),
        new EnterInstagramDataCommand(),
        new EnterEditInstagramDataCommand(),
        new EnterMessageToMailingCommand(),
        new EnterSubscribeDataCommand(),
        new EnterPhoneNumberCommand(),
        new EnterTwoFactorCommand(),
        new EnterOffsetCommand(),
        new EnterDateCommand(),
        new EnterMessageCommand(),
        new EnterFileCommand(),
        new EnterHashtagCommand(),
        new EnterCountCommand(),
        new AdminMailingCommand(),
        new AdminSubscribesCommand(),

        //Do not depend on the state
        new HelpCommand(),
        new InstructionCommand(),
        new PaymentCommand(),
        new WorkCommand(),
        new AccountsCommand(),
    };

    private static readonly List<ICallbackQueryCommand> CallbackQueryCommands = new()
    {
        new ActiveInstagramQueryCommand(),
        new BillQueryCommand(),
        new ChallengeEmailQueryCommand(),
        new ChallengePhoneQueryCommand(),
        new StartEnterAccountDataQueryCommand(),
        new ExitQueryCommand(),
        new ReLogInQueryCommand(),
        new ContinueSelectQueryCommand(),
        new MainMenuQueryCommand(),
        new SelectAllAccountsQueryCommand(),
        new SelectAccountQueryCommand(),
        new StartLaterQueryCommand(),
        new StartNowQueryCommand(),
        new StartWorkingQueryCommand(),
        new SelectTypeQueryCommand(),
        new MyWorksQueryCommand(),
        new MyInstagramsQueryCommand(),
        new MyPaymentsQueryCommand(),
        new MySubscribesQueryCommand(),
        new BuySubscribeQueryCommand(),
        new RestartWorkQueryCommand(),
        new EditInstagramQueryCommand(),
        new StopWorkQueryCommand()
    };

    public async Task HandleAsync(Update update)
    {
        var handler = update.Type switch
        {
            // UpdateType.Unknown:
            // UpdateType.ChannelPost:
            // UpdateType.EditedChannelPost:
            // UpdateType.ShippingQuery:
            // UpdateType.PreCheckoutQuery:
            // UpdateType.Poll:
            UpdateType.Message => BotOnMessageReceived(update.Message!),
            UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery!),
            _ => UnknownUpdateHandlerAsync(update)
        };

        try
        {
            await handler;
        }
        catch (Exception exception)
        {
            HandleErrorAsync(update, exception);
        }
    }

    private void HandleErrorAsync(Update update, Exception ex)
    {
        _logger.LogError(ex, "Update id: {Id}", update.Id);
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        return Task.CompletedTask;
    }

    private async Task BotOnCallbackQueryReceived(CallbackQuery updateCallbackQuery)
    {
        var user = await _serviceContainer.UserService.GetAsync(updateCallbackQuery.From.Id);

        var command = CallbackQueryCommands.FirstOrDefault(command => command.Compare(updateCallbackQuery, user));
        if (command != null)
            await command.Execute(_botClient, user, updateCallbackQuery, _serviceContainer);
    }

    private async Task BotOnMessageReceived(Message updateMessage)
    {
        var user = await _serviceContainer.UserService.GetAsync(updateMessage.From!.Id);
        var command = TextCommands.FirstOrDefault(command => command.Compare(updateMessage, user));
        if (command != null)
            await command.Execute(_botClient, user, updateMessage, _serviceContainer);
    }
}