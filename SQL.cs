using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace Telegram_KinoBot
{
    internal class SQL
    {
        public static string CONNECTION_STRING
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

        public static List<string> GetUsers()//вывод списка всех пользователей из БД
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                var users = new List<string>();
                connection.Open();
                var command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select UserName from UserInfo"; // вывод только ID пользователей на экран
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
            using(var connection = new SqlConnection(CONNECTION_STRING)) 
            {
                connection.Open();
                var command = new SqlCommand();
                command.Connection = connection;
                if(IsUserExists(username)) 
                {       // обновления даты последнего общения у уже зарегестрированного пользователя
                    command.CommandText = $"update UserInfo set LastDate = '{DateTime.Now.ToString("dd-MM-yyyy")}' where UserName like '{username}'";
                }
                else
                {           // добавление нового ID в БД и дату первого сообщения
                    command.CommandText = $"insert into UserInfo(UserName,EnterDate,LastDate) values('{username}'" + 
                        $",'{DateTime.Now.ToString("dd-MM-yyyy")}','{DateTime.Now.ToString("dd-MM-yyyy")}')";
                }
                command.ExecuteNonQuery();
            }
        }

        private static bool IsUserExists(string username)// проверка на зарегестрированность пользователя в БД
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                var command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select 1 from UserInfo where UserName like '{username}'";// поиск в БД по ID 
                return command.ExecuteScalar() != null;
            }
        }
    }
}
