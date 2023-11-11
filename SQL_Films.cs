using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace Telegram_KinoBot
{
    internal class SQL_Films
    {

        public static List<int> GetTotalNumberOfFilms()
        {
            //вывод колличества фильмов в БД

            NpgsqlConnection connection = TelegramBot.CONNECTION_STRING();
            var num = new List<int>();
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = $"select count (*) from kino.kino";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                num.Add(reader.GetInt32(0));
            }
            return num;
        }

        public static string GetImageOfFilm(long num)
        {
            //получить постер фильма из БД

            NpgsqlConnection connection = TelegramBot.CONNECTION_STRING();
            string img = string.Empty;
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = $"select MAX(id) as id from kino.kino";//проверка, что данный номер присутствует в базе
            if (num <= int.Parse(command.ExecuteScalar().ToString()) && num > 0)
            {
                command.CommandText = $"select image from kino.kino where id = {num}";
                img += command.ExecuteScalar().ToString();
                return img;
            }
            else 
                return "https://i.ibb.co/m63Hvjv/E8d-C7a-SWEAU-qj-N.jpg";
        }

        public static List<string> GetInformationOfFilm(long num)
        {
            //вывод всей бд КРОМЕ картинки

            NpgsqlConnection connection = TelegramBot.CONNECTION_STRING();
            var film = new List<string>();
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = $"select MAX(id) as id from kino.kino";//проверка, что данный номер присутствует в базе
            var result = command.ExecuteScalar();
            if (result != System.DBNull.Value)
            {
                if (num <= int.Parse(result.ToString()) && num > 0)
                {
                    //при отсутствии дополнительных ссылок (link1,2,3,4) выводится текст, что бы занимать отсутствующее пространство
                    command.CommandText = $"select name, kino_link, coalesce(link1, 'Это один из моих любимых фильмов!'), coalesce(link2, 'Фильм просто потрясающий!')," +
                        $" coalesce(link3, 'Уверен, что ты будешь в восторге!'), coalesce(link4, 'Приятного просмотра!') from kino.kino where id = {num}";
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        //текст сообщения с информацией о фильме
                        film.Add("Потрясающий фильм:\n" + reader[0].ToString() +
                            "\n\nРекомендую посмотреть этот шедевр на этих официальных сайтах\n" + "\n" + reader[1].ToString() + "\n\n" + //первая строка всегда из кинопоиска
                            reader[2].ToString() + "\n\n" + reader[3].ToString() + "\n\n" + reader[4].ToString() + "\n\n" + reader[5].ToString() + "\n" +
                            "\nСпасибо, что воспользовался этим ботом👍❤️");
                    }
                }
                else
                    film.Add("Прости, но я не могу найти такой фильм.🎬 \nПопробуй ввести другой номер.🔢 \nЛибо, можешь узнать колличество всех фильмов, введя '/total'");
            }           
            else
            {
                film.Add($"Прости, но база фильмов на данный момент пустая(((((((((\n");
                Console.WriteLine($"Error: БАЗА ДАННЫХ ПУСТАЯ");
            }
            return film;
        }

        public static int GetRandomFilm()
        {
            //создаётся случайное число из максимального ID в БД

            NpgsqlConnection connection = TelegramBot.CONNECTION_STRING();
            Random rnd = new Random();
            int randomID = -1;
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = $"select MAX(id) as id from kino.kino";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                try 
                {
                    randomID += rnd.Next(reader.GetInt32(0));
                }
                catch
                {
                    break;
                }
            }
            return randomID;
        }

        public async static Task GetFilmInMessage(ITelegramBotClient botClient, Update update, long numberFilm)
        {
            //вывод фильма (постера и текста) в сообщении

            try
            {
                //картинка из БД отправляется пользователю в отдельном сообщении
                await botClient.SendPhotoAsync(update.Message.Chat.Id, photo: $"{SQL_Films.GetImageOfFilm(numberFilm)}");
            }
            catch
            {
                await botClient.SendPhotoAsync(update.Message.Chat.Id, photo: $"https://i.postimg.cc/BZRzVxWY/fghkhgasdasdgfdgh.jpg");
            }

            string filmStr = string.Empty;
            foreach (var film in SQL_Films.GetInformationOfFilm(numberFilm))//запрос к БД на показ информации и передача номера фильма
                filmStr += film;
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"{filmStr}");
        }
    }
}
