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
                       "Password = JBkQeUObCt;" +
                       "TrustServerCertificate=True;" +
                       "Encrypt=True;" +
                       "Trusted_Connection=True;";
                    //"Data Source=WIN-KHDP309B3KQ\\SQLEXPRESS;Initial Catalog=Kino;Integrated Security=True";
            }            
        }

        public static List<string> GetUsers()
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                var users = new List<string>();
                connection.Open();
                var command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select UserName from UserInfo";
                var reader = command.ExecuteReader();
                while (reader.Read()) 
                {
                    users.Add(reader.GetString(0));
                }
                return users;
            }
        }

        public static void RegisterUser(string username)
        {
            using(var connection = new SqlConnection(CONNECTION_STRING)) 
            {
                connection.Open();
                var command = new SqlCommand();
                command.Connection = connection;
                if(IsUserExists(username)) 
                {
                    command.CommandText = $"update UserInfo set LastDate = '{DateTime.Now.ToString("dd-MM-yyyy")}' where UserName like '{username}'";
                }
                else
                {
                    command.CommandText = $"insert into UserInfo(UserName,EnterDate,LastDate) values('{username}'" + 
                        $",'{DateTime.Now.ToString("dd-MM-yyyy")}','{DateTime.Now.ToString("dd-MM-yyyy")}')";
                }
                command.ExecuteNonQuery();
            }
        }

        private static bool IsUserExists(string username)
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                var command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select 1 from UserInfo where UserName like '{username}'";
                return command.ExecuteScalar() != null;
            }
        }
    }
}
