using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Telegram_KinoBot
{
    internal class SQL_Films
    {
        private static string CONNECTION_STRING
        {
            get
            {
                return "Data Source = WIN-KHDP309B3KQ\\SQLEXPRESS;" +
                       "Initial Catalog = Kino;" +
                       "User ID = devUser;" +
                       "Password = JBkQeUObCt;" + // в БД создан пользователь для удалённого подключения и администрирования. Он является владельцем БД
                       "TrustServerCertificate=True;" +
                       "Encrypt=True;" +
                       "Trusted_Connection=True;";
            }
        }

        public static List<int> GetNum()//вывод колличества фильмов в БД
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                var num = new List<int>();
                connection.Open();
                var command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select MAX(id) as id from kino"; // выводится номер самого последнего фильма в БД, что и является общим колличеством фильмов
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
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                string img= string.Empty;
                connection.Open();
                var command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select MAX(id) as id from kino";//проверка, что данный номер присутствует в базе
                if (num <= int.Parse(command.ExecuteScalar().ToString()) && num > 0)
                {
                    command.CommandText = $"select image from kino where id = {num}";
                    img += command.ExecuteScalar().ToString();
                    return img;
                }
                else return "https://i.ibb.co/m63Hvjv/E8d-C7a-SWEAU-qj-N.jpg";
            }
        }

        public static List<string> GetFilmInfo(int num)//вывод всей бд КРОМЕ картинки
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                var film = new List<string>();
                connection.Open();
                var command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select MAX(id) as id from kino";//проверка, что данный номер присутствует в базе
                if (num <= int.Parse(command.ExecuteScalar().ToString()) && num > 0)
                {
                    command.CommandText = $"select name, link, link2, link3, link4, link5 from kino where id = {num}";
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {//передача имени фильма и ссылок на фильм. Если какая-либо ссылка отсутствует, то поле будет пропущено
                        film.Add("Потрясающий фильм:\n" + reader[0].ToString() + "\n\nРекомендую посмотреть этот шедевр на этих официальных сайтах\n" + reader[1].ToString() + //первая строка всегда из кинопоиска
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
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                Random rnd = new Random();
                int randomID = 1;
                connection.Open();
                var command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select MAX(id) as id from kino";
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
