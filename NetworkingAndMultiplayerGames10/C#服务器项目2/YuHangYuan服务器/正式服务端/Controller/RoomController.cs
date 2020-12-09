using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;


namespace GameServer.Controller
{
    class RoomController:BaseController
    {
        public RoomController()
        {
            requestCode = RequestCode.Room;
        }

        public string CreateRoom(string data, Client client, Server server)//创建房间成功，返回ReturnCode.Success+0给客户端,0是角色类型
        {

            if (server.CreateRoom(client))
            {
                Console.WriteLine("客户端创建房间成功");

                client.playertype = 0;//设置下这个客户端的角色类型
                return ((int)ReturnCode.Success).ToString() + "," + "0";
            }
            else
            {
                Console.WriteLine("客户端创建房间失败");

                return ((int)ReturnCode.Fail).ToString();
            }
            
        }

        public string ListRoom(string data,Client client, Server server)//用于列出现在有的所有房间
        {
            if (server.OnlyRoom != null)
            {
                Console.WriteLine("客户端刷新了一次房间，显示出一个房间，状态为：" + server.OnlyRoom.state.ToString());

                string state = "";
                if (server.OnlyRoom.state == RoomState.WaitingJoin)//把房间状态也返回
                {
                    state = "WaitingJoin";
                }
                if (server.OnlyRoom.state == RoomState.WaitingBattle)
                {
                    state = "WaitingBattle";
                }
                if (server.OnlyRoom.state == RoomState.Battle)
                {
                    state = "Battle";
                }

                return ((int)ReturnCode.Success).ToString()+","+state;
            }else
            {
                Console.WriteLine("客户端刷新了一次房间，现在并没有房间");

                return ((int)ReturnCode.Fail).ToString();
            }
        }

        public string JoinRoom(string data, Client client, Server server)//处理加入房间的请求
        {
            if (server.OnlyRoom != null&&server.OnlyRoom.state==RoomState.WaitingJoin)
            {
                server.OnlyRoom.clientRoom.Add(client);
                client.room = server.OnlyRoom;
                client.playertype = 1;//分配角色类型

                server.OnlyRoom.state = RoomState.WaitingBattle;//把房间改为等待战斗
                client.room.BroadcastMessage(client, ActionCode.UpdateRoom, "1");

                return ((int)ReturnCode.Success).ToString() + "," + "1";
            }
            else
            {
                return ((int)ReturnCode.Fail).ToString();
            }
        }

        public string QuitRoom(string data, Client client, Server server)//处理退出房间的操作，分两种情况，是房主退出和不是房主退出
        {
            bool ishouseowner = client.IsHouseOwner();
            if (ishouseowner)//是房主退出
            {
                Console.WriteLine("房主退出房间");
                client.playertype = -1;
                client.room.BroadcastMessage(client, ActionCode.QuitRoom,((int)ReturnCode.Success).ToString());//让另一个玩家也退出
                client.room.Close();
                return ((int)ReturnCode.Success).ToString();
            }
            else//不是房主退出
            {
                Console.WriteLine("不是房主退出房间");
                client.playertype = -1;
                client.room.clientRoom.Remove(client);//需要先清空这个client所在房间中房间列表里的自己
                client.room.BroadcastMessage(client, ActionCode.UpdateRoom,"0");
                server.OnlyRoom.state = RoomState.WaitingJoin;
                client.room = null;//然后把这个客户端client里面的room也清空
                return ((int)ReturnCode.Success).ToString();//返回退出成功的ReturnCode
            }
        }
    }
}
