using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Threading;

namespace GameServer.Servers
{
    enum RoomState//房间的几种状态
    {
        WaitingJoin,
        WaitingBattle,
        Battle,
    }

    class Room//这是个房间的类
    {
        public  List<Client> clientRoom = new List<Client>();//这个列表用来存储房主和加入房间者的client

        public RoomState state = RoomState.WaitingJoin;//默认刚创建出来的房间是等待状态

        public Server server;



        public void BroadcastMessage(Client excludeClient, ActionCode ac, string data)//这个方法用来广播消息给别的客户端，第一个参数是再要广播的客户端中排除自己，后面两个参数不用说了
        {
            foreach (Client client in clientRoom)
            {
                if (client != excludeClient)
                {
                    server.SendResponse(client, ac, data);
                }
            }
        }

        public bool IsHouseOwner(Client client)
        {
            return client == clientRoom[0];
        }

        public void Close(Client client)
        {
            if (server.OnlyRoom != null)
            {
                if (client != clientRoom[0])
                {
                    BroadcastMessage(client, ActionCode.UpdateRoom, "0");
                    clientRoom.Remove(client);
                    client.room = null;
                }
                else
                {
                    Close();
                }

            }
        }

        public void Close()
        {
            foreach (Client client in clientRoom)
            {
                client.room = null;
            }
            server.OnlyRoom = null;
        }

    }
}
