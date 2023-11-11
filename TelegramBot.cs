using Npgsql;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Telegram_KinoBot
{
    internal class TelegramBot
    {
        public static NpgsqlConnection CONNECTION_STRING()
        {
            return new NpgsqlConnection(@"Server=localhost;Port=5432;User Id=devUser;Password=1234;Database=Kino;");
            // в БД создан пользователь для удалённого подключения и администрирования
        }

        static void Main(string[] args)
        {
            Console.WriteLine("\n\n\tВведите 'токен' бота");
            string token;
            token = Console.ReadLine(); //ввод токена с консоли для безопасности, либо для запуска нескольких ботов одновременно 
            Console.Clear();
            var client = new TelegramBotClient(token);
            BotCommands.SetCommands(client);
            client.StartReceiving(Update, Error);
            Console.ReadLine(); //работа бота продолжается до нажатия Enter в консоли 
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            if (update.Message.Text != null)
            {              
                //вывод сообщений пользователя в консоль. Для удобства выведено его имя и ID
                Console.WriteLine($"{update.Message.Chat.FirstName ?? "анон"} {update.Message.Chat.LastName ?? ""} {update.Message.Chat.Username ?? ""}   " +
                    $"   {update.Message.From.Id}   |   {update.Message.Text}");

                //команды Telegram бота
                BotCommands.Commands(botClient, update, token);

            }
            else
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, Exceptions.Message(update));

        }

        private static Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            //Обработка ошибок. Самая частая ошибка - не верный токен

            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error: \n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
                _=> exception.ToString()
            };
            Console.WriteLine($"Error: {errorMessage}"); // при появлении ошибок идёт вывод в консоль с номером ошибки
            return Task.CompletedTask;
        }
           
    }
}