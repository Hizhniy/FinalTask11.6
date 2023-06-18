using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TestoBot.Services;

namespace TestoBot.Controllers
{
    public class InlineKeyboardController
    {
        private readonly IStorage _memoryStorage;
        private readonly ITelegramBotClient _telegramClient;

        public InlineKeyboardController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(CallbackQuery? callbackQuery, CancellationToken ct)
        {
            if (callbackQuery?.Data == null)
                return;

            // Обновление пользовательской сессии новыми данными
            _memoryStorage.GetSession(callbackQuery.From.Id).ActionType = callbackQuery.Data;

            // Генерим информационное сообщение
            string actionText = callbackQuery.Data switch
            {
                "write" => " 🖊️ Пишите и мы определим длину строки",
                "calculate" => " 🔢 Введите числа через пробел и мы их проссумируем",
                _ => String.Empty
            };

            // Отправляем в ответ уведомление о выборе
            await _telegramClient.SendTextMessageAsync(callbackQuery.From.Id,
                $"<b>{actionText}{Environment.NewLine}</b>", cancellationToken: ct, parseMode: ParseMode.Html);
        }
    }
}