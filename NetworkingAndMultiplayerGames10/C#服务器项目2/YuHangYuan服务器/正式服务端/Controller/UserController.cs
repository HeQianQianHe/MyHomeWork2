using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;
using GameServer.DAO;
using GameServer.Model;


namespace GameServer.Controller
{
    class UserController:BaseController
    {
        private UserDAO userDAO = new UserDAO();
        private ResultDAO resultDAO = new ResultDAO();
        public UserController()//构造方法
        {
            requestCode = RequestCode.User;//父类里面的成员，每个Controller都对应一个RequestCode，里面的方法名对应ActionCode
        }


        public string Login(string data, Client client, Server server)//处理登录请求
        {
            string[] strArray = data.Split(',');

            if (strArray[0] == "heqian" && strArray[1] == "123")
            {
                Console.WriteLine("验证成功，可以登录");
                return ((int)ReturnCode.Success).ToString();
            }
            else
            {
                return ((int)ReturnCode.Fail).ToString();
            }

            //User user= userDAO.VerifyUser(client.Conn, strArray[0], strArray[1]);
            //if (user == null)
            //{
            //    //Enum.GetName(typeof(ReturnCode), ReturnCode.Fail);
            //    return ((int)ReturnCode.Fail).ToString();
            //}
            //else
            //{
            //    Result res =resultDAO.GetResultByUserid(client.Conn, user.Id);
            //    client.Result = res; client.User = user;
            //    Console.WriteLine("验证成功，可以登录");
            //    return ((int)ReturnCode.Success).ToString()+","+user.Username+","+res.TotalCount+","+res.WinCount;
            //}
        }

        public string Register(string data, Client client, Server server)//处理注册请求
        {
            string[] strArray = data.Split(',');//把传来的数据分割
            string name = strArray[0]; string password = strArray[1];
            bool res = userDAO.GetUserByUsername(client.Conn, name);//校验是否用户名重复
            if(res)
            {
                return ((int)ReturnCode.Fail).ToString();//用户名重复
            }
            else
            {
                userDAO.AddUser(client.Conn, name, password);//向数据库内添加信息
                return ((int)ReturnCode.Success).ToString();//注册成功
            }
        }
    }
}
