using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using System.Text;
using TestoBot.Services;

namespace TestoBot.Controllers
{
    public class TextMessageController
    {
        private readonly ITelegramBotClient _telegramClient;
        private readonly IStorage _memoryStorage;

        public TextMessageController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(Message message, CancellationToken ct)
        {
            switch (message.Text)
            {
                case "/start":

                    // Объект, представляющий кноки
                    var buttons = new List<InlineKeyboardButton[]>();
                    buttons.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData($" 🖊️ Писать" , $"write"),
                        InlineKeyboardButton.WithCallbackData($" 🔢 Считать" , $"calculate")
                    });

                    // передаем кнопки вместе с сообщением (параметр ReplyMarkup)
                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"<b> Наш бот считает символы или сумму чисел.\nВыберите, что хотите делать.</b> {Environment.NewLine}", cancellationToken: ct, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));
                    break;
                default:
                    string actionType = _memoryStorage.GetSession(message.Chat.Id).ActionType; // Здесь получим что делать будем
                    switch (actionType)
                    {
                        case "write":
                            {
                                await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Длина сообщения: {message.Text.Length} знаков", cancellationToken: ct);
                            }
                            break;
                        case "calculate":
                            {
                                try
                                {
                                    int sum = 0;
                                    StringBuilder snumber = new StringBuilder(); // временная переменная под число
                                    foreach (var s in message.Text.Trim()) // перебираем посимвольно строку, убрав пробелы слева и справа
                                    {
                                        // проверяем на конец сформированного числа (при этом, чтобы временная переменная под число не была пустой - может быть, когда между числами >1 пробела)
                                        if (s == ' ' && snumber.ToString().Trim() != "")
                                        {
                                            sum += Convert.ToInt32(snumber.ToString()); // добавляем в сумму
                                            snumber.Clear(); // очищаем временную переменную под число
                                        }
                                        else
                                        {
                                            snumber.Append(s); // добавляем символ в формируемое число
                                        }
                                    }
                                    sum += Convert.ToInt32(snumber.ToString()); // добавляем последнее число из строки                   
                                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Сумма чисел: {sum}", cancellationToken: ct);
                                }
                                catch
                                {
                                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Мы умеем складывать только числа!", cancellationToken: ct);
                                }
                            }
                            break;
                        default:
                            await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Сначала выберите в меню, что вы хотите делать", cancellationToken: ct);
                            break;
                    }
                    break;
            }
        }
    }
}