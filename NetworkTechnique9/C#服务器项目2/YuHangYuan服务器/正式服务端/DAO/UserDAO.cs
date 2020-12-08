using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using GameServer.Model;

namespace GameServer.DAO
{
    class UserDAO
    {
        public User VerifyUser(MySqlConnection conn, string name, string password)//用来校验用户名和密码
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from user where username=@name and password = @password;", conn);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("password", password);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    User user = new User(id, name, password);
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在VerifyUser时候出现异常" + e);            
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return null;
        }

        public bool GetUserByUsername(MySqlConnection conn, string name)//注册的时候用来判断是否重名
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from user where username=@name;", conn);
                cmd.Parameters.AddWithValue("name", name);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在GetUserByUsername时候出现异常" + e);

            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return false;
        }

        public void AddUser(MySqlConnection conn, string name, string password)//注册用户名和密码
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("insert into user set username=@name , password = @password;", conn);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("password", password);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("在AddUser时候出现异常" + e);
            }   
        }
    }
}
