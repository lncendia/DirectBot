using AutoMapper;
using DirectBot.API.ViewModels.Subscribe;
using DirectBot.Core.DTO;
using DirectBot.Core.Models;
using DirectBot.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace DirectBot.API.Controllers;

public class SubscribeController : Controller
{
    private readonly ISubscribeService _subscribeService;
    private readonly ISubscribeDeleter _SubscribeDeleter;
    private readonly IMapper _mapper;

    public SubscribeController(IMapper mapper, ISubscribeService subscribeService, ISubscribeDeleter SubscribeDeleter)
    {
        _mapper = mapper;
        _subscribeService = subscribeService;
        _SubscribeDeleter = SubscribeDeleter;
    }

    [HttpGet]
    public async Task<IActionResult> Index(SubscribeListViewModel model, string? message = null)
    {
        if (!string.IsNullOrEmpty(message)) ViewData["Alert"] = message;
        model.SubscribeSearchViewModel ??= new SubscribeSearchViewModel();
        if (!ModelState.IsValid) return View(model);
        if (model.SubscribeSearchViewModel.Page < 1) return BadRequest();

        var query = _mapper.Map<SubscribeSearchQuery>(model.SubscribeSearchViewModel);

        model.Subscribes = _mapper.Map<List<SubscribeViewModel>>(await _subscribeService.GetSubscribesAsync(query));
        if (model.SubscribeSearchViewModel.Page != 1 && !model.Subscribes.Any()) return RedirectToAction("Index");
        return View(model);
    }

    [HttpGet]
    public IActionResult TriggerCheck()
    {
        _SubscribeDeleter.Trigger();
        return RedirectToAction("Index",
            new
            {
                message = "Вызвана проверка подписок."
            });
    }

    [HttpGet]
    public IActionResult Add()
    {
        var model = new AddSubscribeViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddSubscribeViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var subscribe = _mapper.Map<SubscribeDto>(model);
        var result = await _subscribeService.AddAsync(subscribe);
        if (result.Succeeded)
            return RedirectToAction("Index",
                new
                {
                    message = "Подписка успешно добавлена."
                });
        ModelState.AddModelError("", result.ErrorMessage!);
        return View(model);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var subscribe = await _subscribeService.GetAsync(id);
        if (subscribe == null)
            return RedirectToAction("Index",
                new
                {
                    message = "Подписка не найдена."
                });

        var result = await _subscribeService.DeleteAsync(subscribe);
        if (result.Succeeded)
        {
            return RedirectToAction("Index",
                new
                {
                    message = "Подписка успешно удалена."
                });
        }

        return RedirectToAction("Index",
            new
            {
                message = $"Ошибка при удалении подписки: {result.ErrorMessage}."
            });
    }
}