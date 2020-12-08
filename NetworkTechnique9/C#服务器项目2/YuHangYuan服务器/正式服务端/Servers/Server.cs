using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using GameServer.Controller;
using Common;


namespace GameServer.Servers
{
    class Server
    {
        private IPEndPoint ipEndPoint;
        private Socket serverSocket;
        private List<Client> clientList =new List<Client>();//这个List用来管理Client对象
        public Room OnlyRoom;
        ControllerManager controllerManager;//创建一个ControllerManager对象来处理接收请求，server相当于一个中转站，Client收到请求后通过server来访问ControllerManager对象来处理请求

        public Server(string ipStr,int port)//有参构造方法
        {
            controllerManager = new ControllerManager(this);
            SetIpAndPort(ipStr,port);
        }
        public void SetIpAndPort(string ipStr, int port)//为无参构造方法创建的实例添加参数的方法
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ipStr), port);
        }


        public void Start()//这个方法用来绑定并开启监听循环接收请求
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(0);
            Console.WriteLine("开始监听");
            serverSocket.BeginAccept(AcceptCallBack, null);
        }

        private void AcceptCallBack(IAsyncResult ar)//这个方法用来接收请求，并根据客户端的Socket创建一个Client实例来管理客户端的Socket，并添加到SocketList中，然后调用客户端的start方法，最后循环接收请求
        {
            Socket clientSocket = serverSocket.EndAccept(ar);
            Console.WriteLine("接收到一个连接");
            Client client = new Client(clientSocket,this);
            client.Start();
            clientList.Add(client);
            serverSocket.BeginAccept(AcceptCallBack, null);
        }
        public void RemoveClient(Client client)//这个方法用来给Client那个类里面发现连接断开时调用，作用是将这个Client对象移除List
        {
            lock (clientList)
            {
                clientList.Remove(client);
            }
            
        }
        public void SendResponse(Client client, ActionCode ac, string data)//这个方法给controllerManager来调用，server再调用client里面的SendResponse来把做出的回应返回给客户端
        {
            client.SendResponse(ac, data);
        }
        public void HandleRequest(RequestCode rc, ActionCode ac, string data, Client client)//这是个中转站方法，在Client中接受消息，通过Message类处理后把处理后的东西回调给Client然后Client在发送给server，server在调用controllerManager里面的HandleRequest
        {
            controllerManager.HandleRequest(rc, ac, data, client);
        }

        public bool CreateRoom(Client client)
        {
            if (OnlyRoom == null)
            {
                Room room = new Room();//实例化新对象
                room.server = this;
                room.clientRoom.Add(client);//将这个客户端添加到room中维护的集合中
                client.room = room;//同时客户端也持有一下引用
                OnlyRoom = room;
                return true;
            }
            else
            {
                return false;
            }
            
        }

    }
}
