﻿using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.Interfaces;

public interface ICallbackQueryCommand
{
    public Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query, ServiceContainer serviceContainer);

    public bool Compare(CallbackQuery query, UserDto? user);
}