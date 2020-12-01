using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace 数据库测试
{
    class Program
    {
        static void Main(string[] args)
        {
            string str="Database=db1;datasource=127.0.0.1;port=3306;userid=root;password=12345;";//127.0.0.1是本机地址，在哪个机器上使用就是那个机子本机ip（无所谓大小写和空格）
            MySqlConnection conn = new MySqlConnection(str);
            
            conn.Open();//建立连接，打开数据库


            //查询
            MySqlCommand cmd = new MySqlCommand("select*from student", conn);//第一个参数是查询语句，第二个参数是要和哪个数据库连接进行通讯
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())//每调用一次Read（）方法，他就会像指针一样指向下一行，执行循环中的代码前必须先调用一下Read（）方法，当读取到最后一行再向后的时候返回false
            {
                //Console.WriteLine(reader[0].ToString() + reader[1].ToString());//这三种方法都可以用来读取数据
                //Console.WriteLine(reader.GetInt32(0) + reader.GetString(1));//根据列的索引来获取这一行的值，哪一行由Read（）方法决定
                Console.WriteLine(reader.GetInt32("s_id") + reader.GetString("s_name"));//根据列的名字
            }
            

            //插入
            //int tempinput = 10;string tempinput2 = "heqian2";//这里为用户输入的数据
            //MySqlCommand cmd2 = new MySqlCommand("insert into student(s_id,s_name) values(@id,@name)", conn);//通过@的形式来防止sql注入
            //cmd2.Parameters.AddWithValue("id", tempinput);//给刚刚的@设置参数
            //cmd2.Parameters.AddWithValue("name", tempinput2);
            //int result = cmd2.ExecuteNonQuery();//返回值为执行后数据库中受影响的数据行数//ExecuteNonQuery()适用于更新，插入，删除类型的语句，其他语句将返回-1
            //Console.WriteLine("执行成功，影响了{0}行数据", result);


            //更新
            //int tempinput = 10; string tempinput2 = "yunxuan";
            //MySqlCommand cmd3 = new MySqlCommand("update student set s_name=@newname where s_id=@updateid;", conn);
            //cmd3.Parameters.AddWithValue("newname", tempinput2);//给刚刚的@设置参数
            //cmd3.Parameters.AddWithValue("updateid", tempinput);
            //int result = cmd3.ExecuteNonQuery();
            //Console.WriteLine("执行成功，影响了{0}行数据", result);
            

            ////删除
            //MySqlCommand cmd4 = new MySqlCommand("delete from student where s_id = 9 ", conn);
            //int result = cmd4.ExecuteNonQuery();   
            //Console.WriteLine("执行成功，影响了{0}行数据", result);

            

            reader.Close();//读完了记得关闭这两个，记得关闭顺序
            conn.Close();
            Console.ReadKey();
        }
    }
}
