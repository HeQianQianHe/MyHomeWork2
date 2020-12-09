using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace GameServer.Tool
{
    class ConnHelper
    {
        public static MySqlConnection Connect()//用来创建一个和数据库的连接
        {
            MySqlConnection conn = new MySqlConnection("Database=game-jungle-war;datasource=127.0.0.1;port=3306;userid=root;password=12345;");
            try
            {
                conn.Open();
                return conn;
            }
            catch (Exception e)
            {
                Console.WriteLine("连接数据库的时候异常："+e);
                return null;
            }
        }
        public static void CloseConnection(MySqlConnection conn)//关闭和数据库的连接
        {
            if (conn != null)
            {
                conn.Close();
            }
            else
            {
                Console.WriteLine("现在MySqlConnection是空的");
            }
        }

    }
}
