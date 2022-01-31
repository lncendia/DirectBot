using AutoMapper;
using DirectBot.API.ViewModels.User;
using DirectBot.Core.DTO;
using DirectBot.Core.Models;
using DirectBot.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace DirectBot.API.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserController(IMapper mapper, IUserService userService)
    {
        _mapper = mapper;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(UserListViewModel model, string? message = null)
    {
        if (!string.IsNullOrEmpty(message)) ViewData["Alert"] = message;
        model.UserSearchViewModel ??= new UserSearchViewModel();
        if (!ModelState.IsValid) return View(model);
        if (model.UserSearchViewModel.Page < 1) return BadRequest();

        var query = _mapper.Map<UserSearchQuery>(model.UserSearchViewModel);

        model.Users = _mapper.Map<List<UserViewModel>>(await _userService.GetUsersAsync(query));
        if (model.UserSearchViewModel.Page != 1 && !model.Users.Any()) return RedirectToAction("Index");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var user = await _userService.GetAsync(id);
        if (user == null)
            return RedirectToAction("Index",
                new
                {
                    message = "Пользователь не найден."
                });

        return View(_mapper.Map<UserViewModel>(user));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserViewModel userViewModel)
    {
        if (!ModelState.IsValid) return View(userViewModel);
        var user = _mapper.Map<UserDto>(userViewModel);
        var result = await _userService.UpdateAsync(user);

        if (result.Succeeded)
        {
            return RedirectToAction("Index",
                new
                {
                    message = "Пользователь успешно изменен."
                });
        }

        return RedirectToAction("Index",
            new
            {
                message = $"Ошибка при изменении пользователя: {result.ErrorMessage}."
            });
    }

    public async Task<IActionResult> Delete(long id)
    {
        var user = await _userService.GetAsync(id);
        if (user == null)
            return RedirectToAction("Index",
                new
                {
                    message = "Пользователь не найден."
                });

        var result = await _userService.DeleteAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction("Index",
                new
                {
                    message = "Пользователь успешно удален."
                });
        }

        return RedirectToAction("Index",
            new
            {
                message = $"Ошибка при удалении пользователя: {result.ErrorMessage}."
            });
    }
}