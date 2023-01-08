using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Telegram_KinoBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n\n\tВведите 'токен' вашего бота");
            string token = 
            Console.ReadLine();
            Console.Clear();
            var client = new TelegramBotClient(token);
            client.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message.Text != null)
            {
                Console.WriteLine($"{message.Chat.FirstName ?? "анон"} {message.Chat.LastName ?? ""}     {message.From.Id}   |   {message.Text}");
                //if (message.Text.ToLower().Contains("привет"))
                if (message.Text == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Здравствуй, {message.Chat.FirstName}");
                    SQL.RegisterUser((update.Message.From.Id).ToString());



                    return;
                }
                if (message.Text == "/list")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Вот, сколько фильмов я знаю на данный момент:\n");

                    return;
                }
                if (message.Text == "/random")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Хочешь случайный фильм? Ну что ж...\nТебе достался фильм\n");

                    return;
                }
                if (message.Text == "GETUSERSINFO")//отслеживание статистики пользования ботом, не видимой для пользователей
                {
                    string userStr = string.Empty;       
                    foreach (var user in SQL.GetUsers())
                    {
                        userStr += user + Environment.NewLine;
                    }
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Привет, хозяин!\nНа данный момент ботом уже воспользовались пользователи с ID:\n{userStr}");
                }
            }
            if (message.Photo != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Фото очень красивое! Но я умею читать только текст\nНапиши /start и 'код' фильма, который тебя интересует))");
                return;
            }
            if (message.Document != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Файл просто потрясающий! Но я умею читать только текст\nНапиши /start и 'код' фильма, который тебя интересует))");
                return;
            }
        }

        private static Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error: \n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
                _=> exception.ToString()
            };
            Console.WriteLine($"Error: {errorMessage}");
            return Task.CompletedTask;
        }
    }
}
