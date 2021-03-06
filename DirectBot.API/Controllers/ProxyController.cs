using AutoMapper;
using DirectBot.API.ViewModels.Proxy;
using DirectBot.Core.DTO;
using DirectBot.Core.Models;
using DirectBot.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace DirectBot.API.Controllers;

public class ProxyController : Controller
{
    private readonly IProxyService _proxyService;
    private readonly IProxyParser _proxyParser;
    private readonly IMapper _mapper;

    public ProxyController(IProxyService proxyService, IProxyParser proxyParser)
    {
        _proxyService = proxyService;
        _proxyParser = proxyParser;
        _mapper = GetMapper();
    }

    [HttpGet]
    public async Task<IActionResult> Index(ProxyListViewModel model, string? message = null)
    {
        if (!string.IsNullOrEmpty(message)) ViewData["Alert"] = message;
        model.ProxySearchViewModel ??= new ProxySearchViewModel();
        if (!ModelState.IsValid) return View(model);
        if (model.ProxySearchViewModel.Page < 1) return BadRequest();
        var query = _mapper.Map<ProxySearchQuery>(model.ProxySearchViewModel);

        model.Proxies = _mapper.Map<List<ProxyViewModel>>(await _proxyService.GetProxiesAsync(query));
        model.Count = await _proxyService.GetProxiesCountAsync(query);
        if (model.ProxySearchViewModel.Page != 1 && !model.Proxies.Any()) return RedirectToAction("Index");
        return View(model);
    }


    [HttpGet]
    public IActionResult Add()
    {
        var model = new AddProxyViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddProxyViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var proxies = _proxyParser.GetProxies(model.ProxyList);
        if (!proxies.Succeeded)
        {
            ModelState.AddModelError("", proxies.ErrorMessage!);
            return View(model);
        }

        foreach (var proxyDto in proxies.Value!)
        {
            var result = await _proxyService.AddAsync(proxyDto);
            if (result.Succeeded) continue;
            ModelState.AddModelError("", result.ErrorMessage!);
            return View(model);
        }

        return RedirectToAction("Index",
            new
            {
                message = "???????????? ?????????????? ??????????????????."
            });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var proxy = await _proxyService.GetAsync(id);
        if (proxy == null)
            return RedirectToAction("Index",
                new
                {
                    message = "???????????? ???? ??????????????."
                });

        var result = await _proxyService.DeleteAsync(proxy);
        if (result.Succeeded)
        {
            return RedirectToAction("Index",
                new
                {
                    message = "???????????? ?????????????? ??????????????."
                });
        }

        return RedirectToAction("Index",
            new
            {
                message = $"???????????? ?????? ???????????????? ????????????: {result.ErrorMessage}."
            });
    }

    private IMapper GetMapper()
    {
        return new Mapper(new MapperConfiguration(expr =>
        {
            expr.CreateMap<ProxyDto, ProxyViewModel>();
            expr.CreateMap<ProxySearchViewModel, ProxySearchQuery>();
        }));
    }
}