using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_KinoBot
{
    internal class BotCommands
    {
        private const string start = "/start";
        private const string random = "/random";
        private const string total = "/total";



        async public static void SetCommands(TelegramBotClient botClient)
        {
            await botClient.SetMyCommandsAsync(new[] { 
                new BotCommand { Command = start, Description = "Поздороваться с ботом!" },
                new BotCommand{ Command = random, Description = "Посмотреть случайный фильм" },
                new BotCommand { Command = total, Description = "Узнать количество фильмов в базе" } });
        }

        public async static void Commands(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            long NumberOfFilm = 0;
            SQL_Users.UpdateUserLastDate((message.From.Id).ToString());

            if (message.Text.ToLower() == start)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Здравствуй, {message.Chat.FirstName} {message.Chat.LastName ?? ""}");

                SQL_Users.RegisterUser((message.From.Id).ToString(), message.Chat.Username == null ? "" : message.Chat.Username.ToString(), 
                    message.Chat.FirstName.ToString(), message.Chat.LastName == null ? "" : message.Chat.LastName.ToString());//добавление нового пользователя в БД для статистики 

                await botClient.SendTextMessageAsync(message.Chat.Id, $"{message.Chat.FirstName}, напиши, пожалуйста, номер фильма, который ты ищешь(цифрами)\n" +
                $"Или же можешь использовать команду из предоставленных, нажав 'Меню'");
            }
            else
            if (message.Text.ToLower() == total)
            {
                //функция позволет вывести колличество фильмов, которое имеется в Базе Данных 

                string filmStr = string.Empty;
                foreach (var film in SQL_Films.GetTotalNumberOfFilms())
                {
                    filmStr += film;
                }
                await botClient.SendTextMessageAsync(message.Chat.Id, $"На данный момент я знаю {filmStr} фильмов!");
            }
            else
                if (message.Text.ToLower() == "/list" && message.Chat.Id == 5010164097)//только хозяин бота имеет возможность выполнить данную команду
            {
                //вывод информации о зарегестрированных пользователях

                string userStr = string.Empty;
                foreach (var user in SQL_Users.GetUsers())
                {
                    userStr += user + Environment.NewLine;
                }
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Привет, хозяин!\nНа данный момент ботом уже воспользовались пользователи:\n{userStr}");
            }
            else
                if (message.Text.ToLower() == random)
            {
                // функция позволяет вывести случайный фильм из Базы Данных

                var randFilm = SQL_Films.GetRandomFilm();//вывод номера случайного фильма

                if (randFilm == -1)//проверка на пустую БД
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Прости, но база фильмов на данный момент пустая(((((((((\n");
                    Console.WriteLine($"Error: БАЗА ДАННЫХ ПУСТАЯ");
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Хочешь случайный фильм? Ну что ж...\nТебе достался фильм №{randFilm} \n");

                    await SQL_Films.GetFilmInMessage(botClient, update, randFilm);//вывод случайного фильма
                }               
            }
            else
                if (Int64.TryParse(message.Text, out NumberOfFilm))
            {
                //главная функция, выводящая всю информацию о фильме из БД, при условии, что номер верный
                await SQL_Films.GetFilmInMessage(botClient, update, NumberOfFilm);
            }
            else
            {
                //если человеком отправлен текст (НЕ число) или же смайлик(в телеграмме он считается текстом)     
                await botClient.SendTextMessageAsync(message.Chat.Id, "Я уверен, что ты просто потрясающий собеседник!😊\nНо к сожалению я умею только помогать тебе с поиском " +
                   "нужного  фильма.🎬\nНапиши пожалуйста номер фильма, который ты ищешь или же воспользуйся командой '/random'");
            }            
        }
    }
}
