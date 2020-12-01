using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace GameServer.Servers
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

        public void ReadMessage(Action<RequestCode,ActionCode,string> processDataCallBack)//这个方法用来解析数据，解决了粘包分包问题
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
                    RequestCode rc = (RequestCode)BitConverter.ToInt32(data, 4);//这一步是把RequestCode解析出来，枚举类型，4字节长度
                    ActionCode ac = (ActionCode)BitConverter.ToInt32(data, 8);//这一步是把ActionCode解析出来，枚举类型，4字节长度
                    string s = Encoding.UTF8.GetString(data,12, l-8);//从第12个索引开始后面就都是数据了
                    processDataCallBack(rc, ac, s);
                    Array.Copy(data, l + 4, data, 0, startIndex - l - 4);//将读取过的数据删掉
                    startIndex -= (l + 4);//把startIndex恢复到现在剩余数据的末尾
                }
                else
                {
                    break;
                }
            }
        }

        public static byte[] PackData(ActionCode ac, string data)//将要返回的数据打包成（数据长度）+（ActionCode）+（返回的消息）这种格式的字节数组
        {
            byte[] rcBytes = BitConverter.GetBytes((int)ac);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            int l = rcBytes.Length + dataBytes.Length;
            byte[] lBytes = BitConverter.GetBytes(l);
            return (lBytes.Concat(rcBytes).Concat(dataBytes)).ToArray(); 
        }
    }
}
