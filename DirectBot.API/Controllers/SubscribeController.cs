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
    private readonly IUserService _userService;
    private readonly ISubscribeDeleter _subscribeDeleter;
    private readonly IMapper _mapper;

    public SubscribeController(ISubscribeService subscribeService, ISubscribeDeleter subscribeDeleter,
        IUserService userService)
    {
        _mapper = GetMapper();
        _subscribeService = subscribeService;
        _subscribeDeleter = subscribeDeleter;
        _userService = userService;
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
        model.Count = await _subscribeService.GetSubscribesCountAsync(query);
        if (model.SubscribeSearchViewModel.Page != 1 && !model.Subscribes.Any()) return RedirectToAction("Index");
        return View(model);
    }

    [HttpGet]
    public IActionResult TriggerCheck()
    {
        _subscribeDeleter.Trigger();
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
        var user = await _userService.GetAsync(model.UserId);
        if (user == null)
            ModelState.AddModelError("", "Пользователь не найден");

        var subscribe = new SubscribeDto
        {
            EndSubscribe = model.EndSubscribe,
            User = _mapper.Map<UserLiteDto>(user)
        };
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

    private IMapper GetMapper()
    {
        return new Mapper(new MapperConfiguration(expr =>
        {
            expr.CreateMap<SubscribeDto, SubscribeViewModel>();
            expr.CreateMap<UserDto, UserLiteDto>();
            expr.CreateMap<SubscribeSearchViewModel, SubscribeSearchQuery>();
        }));
    }
}