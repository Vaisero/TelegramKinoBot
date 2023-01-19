using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using Telegram.Bot.Types;

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


        public static int GetRandomFilm()//создаётся случайное число из максимального ID с БД
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                Random rnd = new Random();
                int randomID = 0;
                connection.Open();
                var command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select MAX(id) as id from kino";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    randomID = 1 + rnd.Next(reader.GetInt32(0));
                }
                return randomID;
            }
        }
    }
}
