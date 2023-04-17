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
            Console.WriteLine("\n\n\tВведите 'токен' бота");
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
                int NumberOfFilm = 0;
                //вывод сообщений пользователя в консоль. Для удобства выведено его имя и ID
                Console.WriteLine($"{message.Chat.FirstName ?? "анон"} {message.Chat.LastName ?? ""}     {message.From.Id}   |   {message.Text}");

                if (message.Text == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Здравствуй, {message.Chat.FirstName}");
                    SQL_Users.RegisterUser((update.Message.From.Id).ToString()); // добавление нового пользователя в БД для статистики 
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"{message.Chat.FirstName}, напиши пожалуйста номер фильма, который ты ищешь(цифрами)\n" +
                        $"Или же можешь использовать команду из предоставленных, нажав 'Меню'");
                }
                else
                 if (message.Text == "/list") //функция позволет вывести колличество фильмов, которое имеется в Базе Данных 
                {
                    string filmStr = string.Empty;   // все фильмы хранятся в отдельной таблице в БД, где указаны их номер, название, постер, ссылка для просмотра
                    foreach (var film in SQL_Films.GetNum())
                    {
                        filmStr += film; // все фильмы хранятся в отдельной таблице в БД, где указаны их номер, название, постер, ссылка для просмотра
                    }
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"На данный момент я знаю {filmStr} фильмов!");
                }
                else
                if (message.Text == "GETUSERSINFO")//отслеживание статистики пользования ботом, не видимой для пользователей
                {
                    string userStr = string.Empty;   // все пользователи хранятся в отдельной таблице в БД, где указаны их ID, дата первого сообщения боту, дата последнего сообщения боту
                    foreach (var user in SQL_Users.GetUsers())
                    {
                        userStr += user + Environment.NewLine;
                    }
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Привет, хозяин!\nНа данный момент ботом уже воспользовались пользователи с ID:\n{userStr}");
                }
                else
                if (message.Text == "/random")// функция позволяет вывести случайный фильм из Базы Данных
                {
                    var randFilm = SQL_Films.GetRandomFilm();           
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Хочешь случайный фильм? Ну что ж...\nТебе достался фильм №{randFilm} \n");

                    await GetFilmInMessage(botClient, update, randFilm);
                }
                else
                if (Int32.TryParse(message.Text, out NumberOfFilm))//главная функция, выводящая всю информацию о фильме из БД, при условии, что номер верный
                {
                    await GetFilmInMessage(botClient, update, NumberOfFilm);
                }
                else //если человеком отправлен текст (НЕ число) или же смайлик(в телеграмме он считается текстом)
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Я уверен, что ты просто потрясающий собеседник!😊\nНо к сожалению я умею только помогать тебе с поиском "+
                        "нужного  фильма.🎬\nНапиши пожалуйста номер фильма, который ты ищешь или же воспользуйся командой '/random'");
            }
            else
                await botClient.SendTextMessageAsync(message.Chat.Id, Exceptions.Message(update));

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
        
        async static Task GetFilmInMessage(ITelegramBotClient botClient, Update update, int numberFilm)
        {

            await botClient.SendPhotoAsync(update.Message.Chat.Id, photo: $"{SQL_Films.GetIMG(numberFilm)}");//картинка из БД отправляется пользователю в сообщение
            string filmStr = string.Empty;
            foreach (var film in SQL_Films.GetFilmInfo(numberFilm))//запрос к БД на показ информации и передача номера фильма
                filmStr += film;// все фильмы хранятся в отдельной таблице в БД, где указаны их номер, название, постер, ссылка для просмотра
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"{filmStr}");
        }
    }
}