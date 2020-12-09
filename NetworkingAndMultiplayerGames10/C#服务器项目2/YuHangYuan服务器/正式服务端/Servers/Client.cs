using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Common;
using MySql.Data.MySqlClient;
using GameServer.Tool;
using GameServer.Model;

namespace GameServer.Servers
{
    class Client
    {
        private Socket clientSocket;
        private Server server;
        private Message msg = new Message();//实例化这个对象用来处理发送数据的解析和打包
        private MySqlConnection conn;//实例化这个对象用来每一个客户端连接的时候创建一个MySqlConnection
        public  Room room;
        public int playertype =-1;

        public MySqlConnection Conn//为字段设置属性
        {
            get { return conn; }
        }
        public Client()//无参构造方法
        {

        }
        public Client(Socket clientSocket, Server server)//有参构造方法
        {
            //conn = ConnHelper.Connect();//用来创建一个和数据库的连接
            this.clientSocket = clientSocket;
            this.server = server;
        }

        public void Start()//这个方法开启循环接收数据
        {
            if (clientSocket == null || clientSocket.Connected == false)
            {
                return;
            }
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, null);
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                int l = clientSocket.EndReceive(ar);
                if (l == 0)//发现连接断开，关闭这个客户端Socket，并在Server类里面将自身这个Client对象移除List
                {
                    if (room != null)
                    {
                        room.Close(this);
                    }
                    Console.WriteLine("断开了一个连接");
                    //ConnHelper.CloseConnection(conn);
                    if (clientSocket != null)
                    {
                        clientSocket.Close();
                        server.RemoveClient(this);
                    }
                }
                else
                {
                    msg.AddCount(l);//每次接收到数据之后改变startIndex
                    msg.ReadMessage(OnProcessMessage);//每次接收到数据之后都调用这个方法来解析数据，同时把解析过的数据通过回调发送给server，server再通过controllerManager里面的HandleRequest方法分发给不同controller做处理
                    clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, null);
                }
                
            }
            catch (Exception e)
            {
                if (room != null)
                {
                    room.Close(this);
                }
                Console.WriteLine("断开了一个连接");
                Console.WriteLine(e);
                //ConnHelper.CloseConnection(conn);
                if (clientSocket != null)
                {
                    clientSocket.Close();
                    server.RemoveClient(this);
                }
            }
        }

        private void OnProcessMessage(RequestCode rc, ActionCode ac, string s)//这个方法是ReadMessage的回调函数，用来把解析过的数据发送给server
        {
            server.HandleRequest(rc, ac, s, this);
        }

        public void SendResponse(ActionCode ac, string data)//这个方法由controllerManager调用server在调用client里面的这个方法，用来把消息返回对应的客户端
        {
            try
            {
                byte[] b = Message.PackData(ac, data);//这个是专门用来打包返回消息的一个方法，可以将要返回的数据打包成（数据长度）+（ActionCode）+（返回的消息）这种格式的字节数组
                clientSocket.Send(b);
            }
            catch (Exception e)
            {
                Console.WriteLine("无法发送消息"+e);
            }
        }

        public bool IsHouseOwner()//判断这个客户端是不是房主
        {
            return room.IsHouseOwner(this);
        }
    }
}
