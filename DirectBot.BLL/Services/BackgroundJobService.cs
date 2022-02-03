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

        if (!work.Instagrams.All(dto => dto.IsActive))
        {
            work.Message = "Инстаграмы не активны.";
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

        if (!users.Succeeded || users.Value == null)
        {
            work.ErrorMessage = $"Не удалось получить пользователей: {users.ErrorMessage}";
            await _workService.UpdateWithoutStatusAsync(work);
            return;
        }

        await ProcessingAsync(work, users.Value!.Skip(work.CountSuccess + work.CountErrors).ToList(), token);
    }

    private async Task ProcessingAsync(WorkDto work, IReadOnlyList<InstaUser> users, CancellationToken token)
    {
        var text = _messageParser.Split(work.Message!);
        var vocabularies = _messageParser.GetVocabularies(work.Message!);


        var rnd = new Random();
        var instagrams = work.Instagrams.Select(dto => (dto, 0)).ToList();
        for (var i = 0; i < users.Count; i++)
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

            if (instagrams.Count == 0)
            {
                work.ErrorMessage = $"Не удалось получить пользователей: {users.ErrorMessage}";
                await _workService.UpdateWithoutStatusAsync(work);
            }
            var inst = instagrams[i % work.Instagrams.Count];
            var result =
                await _mailingService.SendMessageAsync(inst.dto, _messageParser.Generate(text, vocabularies), users[i]);
            HandleResult(result, work, inst);
            await _workService.UpdateWithoutStatusAsync(work);
            if (inst.Item2 == 5)
            {
                instagrams.Remove(inst);
            }
        }
    }

    private void HandleResult(IOperationResult result, WorkDto work, (InstagramDto, int) inst)
    {
        if (!result.Succeeded)
        {
            work.CountErrors++;
            inst.Item2++;
            work.ErrorMessage = result.ErrorMessage!;
        }
        else
        {
            inst.Item2 = 0;
            work.CountSuccess++;
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