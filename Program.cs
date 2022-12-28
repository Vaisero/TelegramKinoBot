using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
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
                Console.WriteLine($"{message.Chat.FirstName ?? "анон"}   |   {message.Text}");
                if (message.Text.ToLower().Contains("привет"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Здарова");
                    return;
                }
            }
            if (message.Photo != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Фото очень красивое! Но я умею читать только текст)");
                return;
            }
            if (message.Document != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Файл просто потрясающий! Но я умею читать только текст)");
                return;
            }
        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}
