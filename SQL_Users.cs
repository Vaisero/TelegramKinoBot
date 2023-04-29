using Npgsql;
using System;
using System.Collections.Generic;

namespace Telegram_KinoBot
{
    internal class SQL_Users
    {
        public static NpgsqlConnection CONNECTION_STRING()
        {
            return new NpgsqlConnection (@"Server=localhost;Port=5432;User Id=devUser;Password=1234;Database=Kino;");
            // в БД создан пользователь для удалённого подключения и администрирования
        }

        public static List<string> GetUsers()//вывод списка всех пользователей из БД
        {
            using (NpgsqlConnection connection = CONNECTION_STRING())
            {
                var users = new List<string>();
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;
                command.CommandText = $"select user_name from kino.user_info"; // вывод только ID пользователей на экран
                var reader = command.ExecuteReader();                   // дата первого сообщения и дата последнего сообщения не отображаются и находятся в БД
                while (reader.Read()) 
                {
                    users.Add(reader.GetString(0));
                }
                return users;
            }
        }

        public static void RegisterUser(string username)// регистрация нового пользователя в БД или обновление последнего сообщения у уже имеющегося пользователя
        {
            using (NpgsqlConnection connection = CONNECTION_STRING())
            {
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;
                if(IsUserExists(username)) 
                {       // обновления даты последнего общения у уже зарегестрированного пользователя
                    command.CommandText = $"update kino.user_info set last_date = '{DateTime.Now.ToString("dd-MM-yyyy")}' where user_name like '{username}'";
                }
                else
                {           // добавление нового ID в БД и дату первого сообщения
                    command.CommandText = $"insert into kino.user_info(user_name,enter_date,last_date) values('{username}'" + 
                        $",'{DateTime.Now.ToString("dd-MM-yyyy")}','{DateTime.Now.ToString("dd-MM-yyyy")}')";
                }
                command.ExecuteNonQuery();
            }
        }

        private static bool IsUserExists(string username)// проверка на зарегестрированность пользователя в БД
        {
            using (NpgsqlConnection connection = CONNECTION_STRING())
            {
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;
                command.CommandText = $"select 1 from kino.user_info where user_name like '{username}'";// поиск в БД по ID 
                return command.ExecuteScalar() != null;
            }
        }
    }
}
