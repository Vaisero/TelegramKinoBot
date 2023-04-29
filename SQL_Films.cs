using Npgsql;
using System;
using System.Collections.Generic;

namespace Telegram_KinoBot
{
    internal class SQL_Films
    {
        public static NpgsqlConnection CONNECTION_STRING()
        {
            return new NpgsqlConnection(@"Server=localhost;Port=5432;User Id=devUser;Password=1234;Database=Kino;");
            // в БД создан пользователь для удалённого подключения и администрирования
        }

        public static List<int> GetNum()//вывод колличества фильмов в БД
        {
            using (NpgsqlConnection connection = CONNECTION_STRING())
            {
                var num = new List<int>();
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;
                command.CommandText = $"select MAX(id) as id from kino.kino"; // выводится номер самого последнего фильма в БД, что и является общим колличеством фильмов
                var reader = command.ExecuteReader();                   
                while (reader.Read())
                {
                    num.Add(reader.GetInt32(0));
                }
                return num;
            }
        }

        public static string GetIMG(int num)//передаётся картинка из БД с помощью нужного ID
        {
            using (NpgsqlConnection connection = CONNECTION_STRING())
            {
                string img= string.Empty;
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
                else return "https://i.ibb.co/m63Hvjv/E8d-C7a-SWEAU-qj-N.jpg";
            }
        }

        public static List<string> GetFilmInfo(int num)//вывод всей бд КРОМЕ картинки
        {
            using (NpgsqlConnection connection = CONNECTION_STRING())
            {
                var film = new List<string>();
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;
                command.CommandText = $"select MAX(id) as id from kino.kino";//проверка, что данный номер присутствует в базе
                if (num <= int.Parse(command.ExecuteScalar().ToString()) && num > 0)
                {
                    command.CommandText = $"select name, link, coalesce(link2, 'Это один из моих любимых фильмов!'), coalesce(link3, 'Фильм просто потрясающий!')," +
                        $" coalesce(link4, 'Уверен, что ты будешь в восторге!'), coalesce(link5, 'Приятного просмотра!') from kino.kino where id = {num}";
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {//передача имени фильма и ссылок на фильм. Если какая-либо ссылка отсутствует, то поле будет пропущено
                        film.Add("Потрясающий фильм:\n" + reader[0].ToString() +
                            "\n\nРекомендую посмотреть этот шедевр на этих официальных сайтах\n" + reader[1].ToString() + //первая строка всегда из кинопоиска
                            "\n\n" + reader[2].ToString() + "\n\n" + reader[3].ToString() + "\n\n" + reader[4].ToString() + "\n\n" + reader[5].ToString() +
                            "\nСпасибо, что воспользовался этим ботом👍❤️");
                    }
                }
                else 
                    film.Add("Прости, но я не могу найти такой фильм.🎬 \nПопробуй ввести другой номер.🔢 \nМожешь узнать колличество всех фильмов, введя '/list'");

                return film;
            }
        }

        public static int GetRandomFilm()//создаётся случайное число из максимального ID с БД
        {
            using (NpgsqlConnection connection = CONNECTION_STRING())
            {
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
}
