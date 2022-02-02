using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Services;
using Hangfire;

namespace DirectBot.BLL.Services;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly IInstagramUsersGetterService _getterService;
    private readonly IMailingService _mailingService;
    private readonly IWorkService _workService;
    private readonly IWorkNotifier _workNotifier;
    private readonly IMessageParser _messageParser;

    public BackgroundJobService(IInstagramUsersGetterService getterService, IWorkService workService,
        IMailingService mailingService, IWorkNotifier workNotifier, IMessageParser messageParser)
    {
        _getterService = getterService;
        _workService = workService;
        _mailingService = mailingService;
        _workNotifier = workNotifier;
        _messageParser = messageParser;
    }

    [AutomaticRetry(Attempts = 1, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task RunAsync(int workId, CancellationToken token)
    {
        var work = await _workService.GetAsync(workId);
        if (work == null) return;
        if (work.CountErrors == 0 && work.CountSuccess == 0)
            await _workNotifier.NotifyStartAsync(work);
        if (!work.Instagram!.IsActive)
        {
            work.Message = "Инстаграм не активен.";
            await _workService.UpdateWithoutStatusAsync(work);
            return;
        }

        IResult<List<InstaUser>> users;
        try
        {
            users = await _getterService.GetUsersAsync(work, token);
        }
        catch (OperationCanceledException)
        {
            if (await _workService.IsCancelled(work)) return;
            throw;
        }

        if (!users.Succeeded)
        {
            work.ErrorMessage = $"Не удалось получить пользователей: {users.ErrorMessage}";
            await _workService.UpdateWithoutStatusAsync(work);
            return;
        }

        await ProcessingAsync(work, users.Value!.Skip(work.CountSuccess + work.CountErrors), token);
    }

    private async Task ProcessingAsync(WorkDto work, IEnumerable<InstaUser> users, CancellationToken token)
    {
        var text = _messageParser.Split(work.Message!);
        var vocabularies = _messageParser.GetVocabularies(work.Message!);


        var countErrors = 0;
        var rnd = new Random();
        foreach (var user in users)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(rnd.Next(work.LowerInterval, work.UpperInterval)), token);
            }
            catch (OperationCanceledException)
            {
                if (await _workService.IsCancelled(work)) break;
                throw;
            }

            var result =
                await _mailingService.SendMessageAsync(work.Instagram!, _messageParser.Generate(text, vocabularies),
                    user);
            if (!result.Succeeded)
            {
                work.CountErrors++;
                countErrors++;
                work.ErrorMessage = result.ErrorMessage!;
            }
            else
            {
                countErrors = 0;
                work.CountSuccess++;
            }

            await _workService.UpdateWithoutStatusAsync(work);
            if (countErrors == 10) break;
        }
    }

    [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SaveAfterContinuedAsync(int id)
    {
        var work = await _workService.GetAsync(id);
        if (work == null) return;
        work.IsCompleted = true;
        work.IsSucceeded = string.IsNullOrEmpty(work.ErrorMessage) || work.CountSuccess != 0;
        await Task.WhenAll(_workNotifier.NotifyEndAsync(work), _workService.UpdateAsync(work));
    }
}