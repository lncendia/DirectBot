﻿using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.BLL.Services;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class EnterPhoneNumberCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDTO? user, Message message, ServiceContainer serviceContainer)
    {
        if (user!.CurrentInstagram == null)
        {
            await client.SendTextMessageAsync(message.From!.Id,
                "Ошибка. Попробуйте войти ещё раз.");
            user.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            return;
        }

        var result =
            await serviceContainer.InstagramLoginService.SubmitPhoneNumberAsync(user!.CurrentInstagram, message.Text!);
        if (result.Succeeded)
        {
            user.State = State.ChallengeRequiredAccept;
            await serviceContainer.UserService.UpdateAsync(user);
            await client.SendTextMessageAsync(message.From!.Id,
                "Код отправлен. Введите код из сообщения.", replyMarkup: MainKeyboard.Main);
        }
        else
        {
            await client.SendTextMessageAsync(message.From!.Id,
                "Ошибка. Возможно введён неверный номер. Попробуйте ещё раз.", replyMarkup: MainKeyboard.Main);
        }
    }

    public bool Compare(Message message, UserDTO? user)
    {
        return message.Type == MessageType.Text && user!.State == State.ChallengeRequiredPhoneCall;
    }
}