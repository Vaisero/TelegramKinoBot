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
            Console.ReadLine(); //ввод токена с консоли для запуска нескольких ботов одновременно 
            Console.Clear();
            var client = new TelegramBotClient(token);
            client.StartReceiving(Update, Error);
            Console.ReadLine(); //работа бота продолжается до нажатия Enter в консоли 
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message.Text != null)
            {
                //вывод сообщений пользователя в консоль. Для удобства выведено его имя и ID
                Console.WriteLine($"{message.Chat.FirstName ?? "анон"} {message.Chat.LastName ?? ""}     {message.From.Id}   |   {message.Text}");

                if (message.Text == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Здравствуй, {message.Chat.FirstName}");
                    SQL_Users.RegisterUser((update.Message.From.Id).ToString()); // добавление нового пользователя в БД для статистики 



                    return;
                }

                if (message.Text == "/list") //функция позволет вывести колличество фильмов, которое имеется в Базе Данных 
                {
                    string filmStr = string.Empty;   // все фильмы хранятся в отдельной таблице в БД, где указаны их номер, название, постер, ссылка для просмотра
                    foreach (var film in SQL_Films.GetNum())
                    {
                        filmStr += film;
                    }
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"На данный момент я знаю {filmStr} фильмов!");
                }

                if (message.Text == "/random")// функция позволяет вывести случайный фильм из Базы Данных
                {
                    var filmStr = SQL_Films.GetRandomFilm();
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Хочешь случайный фильм? Ну что ж...\nТебе достался фильм №{filmStr} \n");
                    //SQL_Films.GetFilm();                  
                }

                if (message.Text == "GETUSERSINFO")//отслеживание статистики пользования ботом, не видимой для пользователей
                {
                    string userStr = string.Empty;   // все пользователи хранятся в отдельной таблице в БД, где указаны их ID, дата первого сообщения боту, дата последнего сообщения боту
                    foreach (var user in SQL_Users.GetUsers())
                    {
                        userStr += user + Environment.NewLine;
                    }
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Привет, хозяин!\nНа данный момент ботом уже воспользовались пользователи с ID:\n{userStr}");
                }
            }

            // далее по коду идёт проверка на введёные данные, ведь бот работает ТОЛЬКО с текстом

            if (message.Photo != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Фото очень красивое! Но я умею читать только текст\nНапиши /start и 'код' фильма, который тебя интересует))");
                return;
            } // бот не будет работать с фото и пешет об этом пользователю
            if (message.Document != null) 
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Файл просто потрясающий! Но я умею читать только текст\nНапиши /start и 'код' фильма, который тебя интересует))");
                return;
            } // бот не будет работать с файлами и пешет об этом пользователю
            if (message.Sticker != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Стикер очень красивый! Но я умею читать только текст\nНапиши /start и 'код' фильма, который тебя интересует))");
                return;
            } // бот не будет работать со стикерами и пешет об этом пользователю
            if (message.Audio != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Песня очень красивая! Но я умею читать только текст\nНапиши /start и 'код' фильма, который тебя интересует))");
                return;
            } // бот не будет работать со звуками и пешет об этом пользователю
            if (message.Voice != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "У тебя очень красивый голос! Но я умею читать только текст\nНапиши /start и 'код' фильма, который тебя интересует))");
                return;
            } // бот не будет работать с голосовыми сообщениями и пешет об этом пользователю
            if (message.Video != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Видео очень красивое! Но я умею читать только текст\nНапиши /start и 'код' фильма, который тебя интересует))");
                return;
            } // бот не будет работать с видео и пешет об этом пользователю
            if (message.VideoNote != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Кружочек очень красивый! Но я умею читать только текст\nНапиши /start и 'код' фильма, который тебя интересует))");
                return;
            } // бот не будет работать с видео-кружками и пешет об этом пользователю
            if (message.Animation != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Гифка очень красивая! Но я умею читать только текст\nНапиши /start и 'код' фильма, который тебя интересует))");
                return;
            } // бот не будет работать с GIF анимацией и пешет об этом пользователю
            if (message.Poll != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Опрос очень увлекательный! Но я умею читать только текст\nНапиши /start и 'код' фильма, который тебя интересует))");
                return;
            } // бот не будет работать с опросами и пешет об этом пользователю
            if (message.Contact != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Это очень интересный человек, но мне больше нравится общаться с тобой! Отправь лучше ему МОИ контакты и я ему помогу найти фильм, как и тебе))");
                return;
            } // бот не будет работать с контактами и пешет об этом пользователю
            if (message.Venue != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Это просто потрясающее место, супер геолакация! Но я умею читать только текст\nНапиши /start и 'код' фильма, который тебя интересует))");
                return;
            } // бот не будет работать с геолакацией и пешет об этом пользователю
            if (message.Location != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Это просто потрясающее место, супер геолакация! Но я умею читать только текст\nНапиши /start и 'код' фильма, который тебя интересует))");
                return;
            } // бот не будет работать с геолакацией и пешет об этом пользователю

        }

        private static Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken token) //самая частая ошибка - не верный токен
        {
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
