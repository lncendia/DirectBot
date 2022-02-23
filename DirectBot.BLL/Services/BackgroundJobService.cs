using DirectBot.Core.Enums;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Services;

namespace DirectBot.BLL.Services;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly IInstagramUsersGetterService _getterService;
    private readonly IInstagramService _instagramService;
    private readonly IMailingService _mailingService;
    private readonly IWorkService _workService;
    private readonly IWorkNotifier _workNotifier;
    private readonly IMessageParser _messageParser;

    public BackgroundJobService(IInstagramUsersGetterService getterService, IWorkService workService,
        IMailingService mailingService, IWorkNotifier workNotifier, IMessageParser messageParser,
        IInstagramService instagramService)
    {
        _getterService = getterService;
        _workService = workService;
        _mailingService = mailingService;
        _workNotifier = workNotifier;
        _messageParser = messageParser;
        _instagramService = instagramService;
    }

    public async Task ProcessingMailingAsync(WorkDto work, CancellationToken token)
    {
        if (!work.Instagrams.All(dto => dto.IsActive) || !work.Instagrams.Any())
        {
            work.ErrorMessage = "Инстаграмы не активны.";
            await _workService.UpdateWithoutStatusAsync(work);
            return;
        }

        if (work.CountSuccess + work.CountErrors == 0) await _workNotifier.NotifyStartAsync(work);

        if (!work.InstagramPks.Any())
        {
            var result = await _getterService.GetUsersAsync(work, token);
            if (result.Succeeded)
            {
                work.InstagramPks = result.Value!;
                await _workService.UpdateWithoutStatusAsync(work);
            }
            else
            {
                work.ErrorMessage = result.ErrorMessage;
                await _workService.UpdateWithoutStatusAsync(work);
                return;
            }
        }

        await ProcessingAsync(work, token);
    }

    public async Task<List<WorkDto>> ProcessingDivideWorkAsync(WorkDto work, CancellationToken token)
    {
        if (!work.InstagramPks.Any())
        {
            await _workNotifier.NotifyStartAsync(work);
            var result = await _getterService.GetUsersAsync(work, token);
            if (result.Succeeded)
            {
                work.InstagramPks = result.Value!;
                await _workService.UpdateWithoutStatusAsync(work);
            }
            else
            {
                work.ErrorMessage = result.ErrorMessage;
                await _workService.UpdateWithoutStatusAsync(work);
                return new List<WorkDto>();
            }
        }

        var count = work.InstagramPks.Count / work.CountPerDivision;
        var countRest = work.InstagramPks.Count % work.CountPerDivision;
        var workDtos = new List<WorkDto>();
        for (var i = 0; i < count; i++)
        {
            var workDto = new WorkDto
            {
                InstagramPks = work.InstagramPks.Skip(i * work.CountPerDivision).Take(work.CountPerDivision).ToList()
            };

            var result = await CopyAndSaveAsync(work, workDto);
            if (!result.Succeeded)
            {
                work.ErrorMessage = result.ErrorMessage;
                await _workService.UpdateWithoutStatusAsync(work);
                continue;
            }

            workDtos.Add(workDto);
        }

        if (countRest != 0)
        {
            var workDto = new WorkDto
            {
                InstagramPks = work.InstagramPks.Skip(count * work.CountPerDivision).Take(countRest).ToList()
            };

            var result = await CopyAndSaveAsync(work, workDto);
            if (!result.Succeeded)
            {
                work.ErrorMessage = result.ErrorMessage;
                await _workService.UpdateWithoutStatusAsync(work);
            }
            else
                workDtos.Add(workDto);
        }

        return workDtos;
    }

    private async Task<IOperationResult> CopyAndSaveAsync(WorkDto source, WorkDto destination)
    {
        destination.Message = source.Message;
        destination.LowerInterval = source.LowerInterval;
        destination.UpperInterval = source.UpperInterval;
        destination.UsersType = source.UsersType;
        destination.Hashtag = source.Hashtag;
        destination.FileIdentifier = source.FileIdentifier;
        destination.Type = WorkType.Simple;
        destination.CountUsers = source.CountPerDivision;
        destination.Instagrams = source.Instagrams.ToList();
        var result = await _workService.AddAsync(destination);
        return !result.Succeeded ? result : OperationResult.Ok();
    }

    public async Task SaveAfterContinuedAsync(WorkDto work)
    {
        work.IsCompleted = true;
        await _workNotifier.NotifyEndAsync(work);
        await _workService.UpdateAsync(work);
    }


    private async Task ProcessingAsync(WorkDto work, CancellationToken token)
    {
        var text = _messageParser.Split(work.Message!);
        var vocabularies = _messageParser.GetVocabularies(work.Message!);
        var instagrams = (await GetInstagramsAsync(work.Instagrams)).Select(dto => (dto, 0)).ToList();


        var rnd = new Random();
        int count = work.CountSuccess + work.CountErrors;
        while (count < work.InstagramPks.Count)
        {
            await Task.Delay(TimeSpan.FromSeconds(rnd.Next(work.LowerInterval, work.UpperInterval)), token);

            if (instagrams.Count == 0)
            {
                work.ErrorMessage = "Из-за большого количества ошибок работа остановлена.";
                await _workService.UpdateWithoutStatusAsync(work);
                break;
            }

            var pks = work.InstagramPks.Skip(count).Take(instagrams.Count).ToList();
            count += pks.Count;
            for (int i = 0; i < pks.Count; i++)
            {
                var result = await _mailingService.SendMessageAsync(instagrams[i].dto,
                    _messageParser.Generate(text, vocabularies), pks[i]);
                HandleResult(result, work, instagrams[i]);
                await _workService.UpdateWithoutStatusAsync(work);
            }

            instagrams.RemoveAll(inst => inst.Item2 == 5);
        }
    }

    private async Task<List<InstagramDto>> GetInstagramsAsync(List<InstagramLiteDto> instagramLiteDtos)
    {
        var list = new List<InstagramDto>();
        foreach (var instagram in instagramLiteDtos)
        {
            var instagramDto = await _instagramService.GetAsync(instagram.Id);
            if (instagramDto is {IsActive: true}) list.Add(instagramDto);
        }

        return list;
    }

    private static void HandleResult(IOperationResult result, WorkDto work, (InstagramDto, int) inst)
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
}