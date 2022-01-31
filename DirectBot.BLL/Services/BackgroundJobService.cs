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

    [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task RunAsync(int workId, CancellationToken token)
    {
        var work = await _workService.GetAsync(workId);
        if (work == null) return;
        await _workNotifier.NotifyStartAsync(work);
        work.IsCompleted = true;
        if (!work.Instagram!.IsActive)
        {
            work.Message = "Инстаграм не активен.";
            await _workService.UpdateAsync(work);
            return;
        }

        IResult<List<InstaUser>> users;
        try
        {
            users = await _getterService.GetUsersAsync(work, token);
        }
        catch (OperationCanceledException)
        {
            work.IsSucceeded = true;
            await _workService.UpdateAsync(work);
            return;
        }

        if (!users.Succeeded)
        {
            work.ErrorMessage = $"Не удалось получить пользователей: {users.ErrorMessage}";
            await _workService.UpdateAsync(work);
            return;
        }

        await ProcessingAsync(work, users.Value!, token);
        await Task.WhenAll(_workNotifier.NotifyEndAsync(work), _workService.UpdateAsync(work));
    }

    private async Task ProcessingAsync(WorkDto work, List<InstaUser> users, CancellationToken token)
    {
        var text = _messageParser.Split(work.Message!);
        var vocabularies = _messageParser.GetVocabularies(work.Message!);


        List<string> errors = new List<string>();
        int countErrors = 0;
        foreach (var user in users)
        {
            if (token.IsCancellationRequested) break;
            var result = await _mailingService.SendMessageAsync(work.Instagram!,
                new Range(work.LowerInterval, work.UpperInterval), _messageParser.Generate(text, vocabularies), user);
            if (!result.Succeeded)
            {
                countErrors++;
                errors.Add(result.ErrorMessage!);
                if (countErrors == 10) break;
            }
            else
            {
                countErrors = 0;
                work.CountSuccess++;
                await _workService.UpdateAsync(work);
            }
        }

        if (errors.Any())
        {
            work.ErrorMessage = errors.GroupBy(d => d).OrderByDescending(e => e.Count()).First().Key;
            work.CountErrors = errors.Count;
            work.IsSucceeded = errors.Count != users.Count;
        }
        else
            work.IsSucceeded = true;
    }

    [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SaveAfterContinuedAsync(int id)
    {
        var work = await _workService.GetAsync(id);
        if (work == null) return;
        work.IsCompleted = true;
        await _workService.UpdateAsync(work);
    }
}