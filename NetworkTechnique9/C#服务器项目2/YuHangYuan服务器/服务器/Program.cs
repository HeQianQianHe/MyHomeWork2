using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using MySql.Data.MySqlClient;

namespace 服务器
{
    class Program
    {
        class Message
        {
            private byte[] data = new byte[2048];
            private int startIndex = 0;//代表我们已经存储了多少数据

            public byte[] Data//data的属性
            {
                get { return data; }
            }

            public int StartIndex//startIndex属性
            {
                get { return startIndex; }
            }

            public int RemainSize//计算剩余的数组大小
            {
                get { return data.Length - startIndex; }
            }


            public void AddCount(int count)
            {
                startIndex += count;
            }

            public void ReadMessage()
            {
                while (true)
                {
                    if (startIndex <= 4)
                    {
                        return;
                    }
                    int l = BitConverter.ToInt32(data, 0);//读取这个data中前四个字节，看看要传过来的信息的长度是多少（他只会读取前四个字节，因为只有四个字节是一个int32）
                    if ((startIndex - 4) >= l)//表示这个data里面的数据已经是完整的了，或者还多了
                    {
                        string s = Encoding.UTF8.GetString(data, 4, l);//从第四个索引开始
                        Console.WriteLine("解析出来一条数据" + s);
                        Array.Copy(data, l + 4, data, 0, startIndex - l - 4);//将读取过的数据删掉
                        startIndex -= (l + 4);//把startIndex恢复到现在剩余数据的末尾
                    }
                    else
                    {
                        break;
                    }
                }
                
            }
        }

        static Message msg = new Message();//把他定义成静态变量
        static void Main(string[] args)
        {
            StartServerAsync();
            Console.ReadKey();
        }

        static void StartServerAsync()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.105"), 8888));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallBack, serverSocket); //这个方法会异步开始接受连接，他需要一个AsyncCallback委托类型的回调方法作为参数，第二个参数是回调方法的参数，因为在回调方法里面我们要调用endaccept方法，所以要将serverSocket传进去，或者把他定义成静态的变量
                                                                    //AsyncCallback委托类型指向的函数引用必须是一个无返回值，且参数为IAsyncResult ar类型的函数
        }

        //这个回调方法必须有一个IAsyncResult类型的参数
        static void ReceiveCallBack(IAsyncResult ar)
        {
            Socket clientSocket = ar.AsyncState as Socket;
            //这个try catch用来捕捉客户端非正常关闭
            try
            {
                int l = clientSocket.EndReceive(ar);//结束接收数据的方法，返回一个接收了的字节长度,传递一个IAsyncResult类型的参数进去，其实这个方法就是用来得到本来receive方法的返回值
                //这里返回的长度若是0，代表客户端已经关闭了客户端，通过实验发现，只有当客户端关闭的时候，这里才会一直不停的接收到长度为0的信息，若是客户端没关，而是发送空消息，是不会接收到长度为0的信息的(现在我们每个数据头又添加了表示长度的int32，更不会是0了，最小也是4)
                Console.WriteLine(l);
                if (l == 0)//这里用来处理正常关闭的情况
                {
                    clientSocket.Close();
                    return;
                }
                msg.AddCount(l);
                msg.ReadMessage();
                //Console.WriteLine(Encoding.UTF8.GetString(dataBuffer, 0, l)+"本条消息长度："+l);
                clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, clientSocket);//在接收一次数据完成的时候，再重新调用这个方法，循环接收(若第一次装的不够没有解析，下次会接着上次的末尾添加数据)
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if (clientSocket != null)
                {
                    clientSocket.Close();
                }
            }
                    
        }
        
        //当我们接收到一个客户端连接的时候会调用这个方法
        static void AcceptCallBack(IAsyncResult ar)
        {
            Socket serverSocket = ar.AsyncState as Socket;
            Socket clientSocket=serverSocket.EndAccept(ar);

            Console.WriteLine("连接了一个客户端，ip：{0} port：{1}", ((clientSocket.RemoteEndPoint) as IPEndPoint).Address, ((clientSocket.RemoteEndPoint) as IPEndPoint).Port);
            byte[] data = Encoding.UTF8.GetBytes("欢迎连接我们");
            clientSocket.Send(data);

            //dataBuffer是装数据的数组，0是开始装的索引，1024是装的最大长度，ReceiveCallBack是接收到消息手之后的回调方法，clientSocket是向回调函数传递的参数,在回调函数中可以以ar.AsyncState的形式来获取我们传进去的参数
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, clientSocket);//在这里再开始异步接受这个客户端的信息

            serverSocket.BeginAccept(AcceptCallBack, serverSocket);//再次调用这个方法来重复循环的接受客户端的连接
        }
    }
}
