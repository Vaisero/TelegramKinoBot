using Npgsql;
using System;
using System.Collections.Generic;

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

        public static string GetImageOfFilm(int num)
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

        public static List<string> GetInformationOfFilm(int num)
        {
            //вывод всей бд КРОМЕ картинки

            NpgsqlConnection connection = TelegramBot.CONNECTION_STRING();
            var film = new List<string>();
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = $"select MAX(id) as id from kino.kino";//проверка, что данный номер присутствует в базе
            if (num <= int.Parse(command.ExecuteScalar().ToString()) && num > 0)
            {
                //при отсутствии дополнительных ссылок (link1,2,3,4) выводится текст, что бы занимать отсутствующее пространство
                command.CommandText = $"select name, kino_link, coalesce(link1, 'Это один из моих любимых фильмов!'), coalesce(link2, 'Фильм просто потрясающий!')," +
                    $" coalesce(link3, 'Уверен, что ты будешь в восторге!'), coalesce(link4, 'Приятного просмотра!') from kino.kino where id = {num}";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //текст сообщения с информацией о фильме
                    film.Add("Потрясающий фильм:\n" + reader[0].ToString() +
                        "\n\nРекомендую посмотреть этот шедевр на этих официальных сайтах\n" + reader[1].ToString() + //первая строка всегда из кинопоиска
                        reader[2].ToString() + reader[3].ToString() + reader[4].ToString() + reader[5].ToString() +
                        "\nСпасибо, что воспользовался этим ботом👍❤️");
                }
            }
            else
                film.Add("Прости, но я не могу найти такой фильм.🎬 \nПопробуй ввести другой номер.🔢 \nЛибо, можешь узнать колличество всех фильмов, введя '/total'");

            return film;
        }

        public static int GetRandomFilm()
        {
            //создаётся случайное число из максимального ID в БД

            NpgsqlConnection connection = TelegramBot.CONNECTION_STRING();
            Random rnd = new Random();
            int randomID = 1;
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = $"select MAX(id) as id from kino.kino";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                randomID += rnd.Next(reader.GetInt32(0));
            }
            return randomID;
        }
    }
}
