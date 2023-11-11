using Npgsql;
using System;
using System.Collections.Generic;

namespace Telegram_KinoBot
{
    internal class SQL_Users
    {       
        public static List<string> GetUsers()
        {
            //вывод списка всех пользователей из БД

            NpgsqlConnection connection = TelegramBot.CONNECTION_STRING();
            var users = new List<string>();
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = $"select user_id, user_name, user_first_name, user_second_name, first_entry_date, last_entry_date from kino.users_info";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                //вывод информации о пользователях в сообщение
                users.Add("\n" + reader.GetInt64(0).ToString()+ "  " + reader.GetString(1) + "  " + reader.GetString(2) + "  " + reader.GetString(3) + "  '" + reader.GetDateTime(4).ToString() + "'   '" + reader.GetDateTime(5).ToString() + "'\n");
            }
            return users;
        }

        public static void RegisterUser(string user_id, string user_name, string user_first_name, string user_second_name)
        {

            // регистрация нового пользователя в БД или обновление последнего сообщения у уже имеющегося пользователя

            if (IsUserExists(user_id))
            {       // обновления даты последнего общения у уже зарегестрированного пользователя
                UpdateUserLastDate(user_id);
            }
            else
            {           // добавление нового пользоватея в БД и дату сообщения
                NpgsqlConnection connection = TelegramBot.CONNECTION_STRING();
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;
                command.CommandText = $"insert into kino.users_info(user_id, user_name, user_first_name, user_second_name, first_entry_date, last_entry_date) values({user_id}" +
                    $",'{user_name}','{user_first_name}', '{user_second_name}'" + 
                    $",$${DateTime.Now}$$,$${DateTime.Now}$$)";
                command.ExecuteNonQuery();
                Console.WriteLine($"Пользователь {user_first_name} был зарегестрирован!\n");
            }
            
        }

        private static bool IsUserExists(string user_id)
        {
            // проверка на зарегестрированность пользователя в БД

            NpgsqlConnection connection = TelegramBot.CONNECTION_STRING();
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = $"select 1 from kino.users_info where user_id = {user_id}";// поиск в БД по ID 
            return command.ExecuteScalar() != null;
        }

        public static void UpdateUserLastDate(string user_id)
        {
            // обновления даты последнего общения у уже зарегестрированного пользователя

            if (IsUserExists(user_id))
            {
                NpgsqlConnection connection = TelegramBot.CONNECTION_STRING();
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;

                command.CommandText = $"update kino.users_info set last_entry_date = $${DateTime.Now}$$ where user_id = {user_id}";

                command.ExecuteNonQuery();
            }
            else
            {
                Console.WriteLine($"Error: Пользователь не зарегестрирован!!! Невозможно обновить дату последнего сообщения!\n");
            }
           
        }
    }
}
