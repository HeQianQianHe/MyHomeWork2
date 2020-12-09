using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
using System.Text;
using System.Linq;
public class Message  {

    private byte[] data = new byte[1024];
    private int startIndex = 0;//我们存取了多少个字节的数据在数组里面

    public byte[] Data
    {
        get { return data; }
    }
    public int StartIndex
    {
        get { return startIndex; }
    }
    public int RemainSize
    {
        get { return data.Length - startIndex; }
    }
    /// <summary>
    /// 解析数据或者叫做读取数据
    /// </summary>
    public void ReadMessage(int newDataAmount, Action<ActionCode, string> processDataCallback)//需要两个参数，一个是这次读取新信息要增加的startIndex值，第二个是解析完消息调用的回调函数
    {
        startIndex += newDataAmount;
        while (true)
        {
            if (startIndex <= 4) return;//若存储的信息没满足最小长度
            int count = BitConverter.ToInt32(data, 0);//解析前四个获取信息长度
            if ((startIndex - 4) >= count)//判断后面的信息是否是完整的一条或者多点
            {
                ActionCode actionCode = (ActionCode)BitConverter.ToInt32(data, 4);
                string s = Encoding.UTF8.GetString(data, 8, count - 4);
                processDataCallback(actionCode, s);//把解析出来的ActionCode和数据传进去
                Array.Copy(data, count + 4, data, 0, startIndex - 4 - count);
                startIndex -= (count + 4);
            }
            else
            {
                break;
            }
        }
    }

    public static byte[] PackData(RequestCode requestData,ActionCode actionCode, string data)//打包数据的方法，打包完就要发送个服务器端，可以看出数据格式是（长度）+（RequestCode）+（ActionCode）+(data)
    {
        byte[] requestCodeBytes = BitConverter.GetBytes((int)requestData);
        byte[] actionCodeBytes = BitConverter.GetBytes((int)actionCode);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        int dataAmount = requestCodeBytes.Length + dataBytes.Length + actionCodeBytes.Length;
        byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);
        return dataAmountBytes.Concat(requestCodeBytes).ToArray<byte>().Concat(actionCodeBytes).ToArray<byte>().Concat(dataBytes).ToArray<byte>();
    }
}
