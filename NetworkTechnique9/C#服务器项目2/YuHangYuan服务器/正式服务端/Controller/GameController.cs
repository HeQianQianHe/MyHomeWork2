using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    class GameController:BaseController
    {
        public GameController()
        {
            requestCode = RequestCode.Game;
        }
        public string StartGame(string data,Client client,Server server)//开始游戏的方法
        {
            if (client.IsHouseOwner()&&client.room.clientRoom.Count>1)//如果是房主的话并且人数大于一，首先给自己返回ReturnCode.Success，这样Success会传到客户端的StartGameRequest那处理，然后我们还需要把开始游戏的消息广播给其他客户端
            {
                Console.WriteLine("房主开始了游戏");

                server.OnlyRoom.state = RoomState.Battle;//更改房间为开始战斗状态
                client.room.BroadcastMessage(client,ActionCode.StartGame,((int)ReturnCode.Success).ToString());//广播给别的客户端也开始游戏
                return ((int)ReturnCode.Success).ToString();
            }
            else
            {
                Console.WriteLine("开启游戏失败");

                return ((int)ReturnCode.Fail).ToString();
            }
        }

        public string Move(string data, Client client, Server server)//这个方法是用来同步别的客户端的数据的，把发送过来的位置信息转发给别的客户端
        {
            if (server.OnlyRoom != null)
            {
                server.OnlyRoom.BroadcastMessage(client, ActionCode.Move, data);//将数据原封不动转发即可
            }
            return null;//因为是转发给别的客户端，所以自己不用接收自己的数据
        }

        public string Shoot(string data, Client client, Server server)
        {
            if (client.room != null)
            {
                client.room.BroadcastMessage(client, ActionCode.Move, data);
            }
            return null;
        }

        public string NowWeapon(string data, Client client, Server server)
        {
            if (server.OnlyRoom != null)
            {
                server.OnlyRoom.BroadcastMessage(client, ActionCode.NowWeapon, data);//将数据原封不动转发即可
            }
            return null;
        }

        //public string DestroyWeapon(string data, Client client, Server server)
        //{

        //}

        //public string InstantiateWeapon(string data, Client client, Server server)
        //{

        //}

        //public string QuitGame(string data, Client client, Server server)
        //{

        //}
    }
}
