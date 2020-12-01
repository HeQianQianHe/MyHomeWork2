using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using Common;

/// <summary>
/// 这个是用来管理跟服务器端的Socket连接
/// </summary>
public class ClientManager :BaseManager {

    private const string IP = "127.0.0.1";
    private const int PORT = 6688;

    private Socket clientSocket;
    private Message msg = new Message();//帮助解析信息的类

    public ClientManager(GameFacade facade) : base(facade) { }

    public override void OnInit()//当ClientManager被GameFacade创建出来的时候调用这个方法
    {
        base.OnInit();

        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            clientSocket.Connect(IP, PORT);
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallback, null);//开始循环接收消息
        }
        catch (Exception e)
        {
            Debug.LogWarning("无法连接到服务器端，请检查您的网络！！" + e);
        }
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            if (clientSocket == null || clientSocket.Connected == false) return;
            int count = clientSocket.EndReceive(ar);

            msg.ReadMessage(count, OnProcessDataCallback);//需要两个参数，一个是这次读取新信息要增加的startIndex值，第二个是解析完消息调用的回调函数

            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallback, null); //循环接收消息
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    private void OnProcessDataCallback(ActionCode actionCode,string data)//解析完从服务器发回来的消息后，把actionCode和data通过GameFacade传递给RequestManager处理
    {
        facade.HandleReponse(actionCode, data);
    }

    public void SendRequest(RequestCode requestCode, ActionCode actionCode, string data)//发送数据的方法，把需要发送的请求数据传到这里，然后按格式打包发送，需要通过GameFacade中介来调用这个方法
    {
        byte[] bytes = Message.PackData(requestCode, actionCode, data);
        clientSocket.Send(bytes);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        try
        {
            clientSocket.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning("无法关闭跟服务器端的连接！！" + e);
        }
    }
}