using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;
using MySql.Data.MySqlClient;

namespace GameServer.DAO
{
    class ResultDAO
    {
        public Result GetResultByUserid(MySqlConnection conn, int userid)//登陆的时候来获取用户的战绩之类的信息
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from result where userid=@userid", conn);
                cmd.Parameters.AddWithValue("userid", userid);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {

                    Result res = new Result(reader.GetInt32("id"), userid, reader.GetInt32("totalcount"), reader.GetInt32("wincount"));
                    return res;
                }
                else
                {
                    Result res = new Result(-1, userid,0, 0);
                    return res; ;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在GetResultByUserid时候出现异常" + e);

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

    }
}
