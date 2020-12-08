using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;


namespace 客户端
{
    class Program
    {
        class Message
        {
            
        }


        static void Main(string[] args)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse("192.168.1.105"), 8888));

            byte[] data = new byte[1024];
            int l = clientSocket.Receive(data);
            Console.WriteLine(Encoding.UTF8.GetString(data,0,l));

            //byte[] test = new byte[1024];
            //clientSocket.Send(test);

            while (true)
            {
                string message=Console.ReadLine();
                if (message == "c")//这里用来模拟用户正常关闭
                {
                    clientSocket.Close();
                    return;
                }
                clientSocket.Send(GetGoodBytes(message));
            }

        }

        static byte[] GetGoodBytes(string data)//这个方法用来把字符串转化成前四个字节代表长度的byte数组
        {
            byte[] databytes = Encoding.UTF8.GetBytes(data);
            int l = databytes.Length;
            Console.WriteLine(l);
            byte[] lengthbytes = BitConverter.GetBytes(l);//这里编码字符串信息和编码长度信息的编码格式不一样
            byte[] goodbytes = lengthbytes.Concat(databytes).ToArray();
            return goodbytes;
        }



    }
}
