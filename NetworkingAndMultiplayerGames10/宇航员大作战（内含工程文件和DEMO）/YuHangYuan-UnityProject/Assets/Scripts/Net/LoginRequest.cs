using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;
using UnityEngine.SceneManagement;

public class LoginRequest : BaseRequest
{
    bool islogin = false;
    bool ReturnCodeisfail = false;

    public Text nametext;
    public Text passwordtext;

    public TiShi tishi;

    public GameObject LoginPlane;
    public GameObject RoomListPlane;
    public GameObject RoomItem;

    public override void Awake()
    {
        requestCode = RequestCode.User;//这个RequestCode对应着服务器那边的UserController
        actionCode = ActionCode.Login;//这个ActionCode对应着UserController里面的Login方法
        base.Awake();//base里面的语句：facade.AddRequest(actionCode, this) 将自己添加进字典里，所以上面三条语句要在这之前执行
        RoomListPlane.SetActive (false);
        RoomItem.SetActive(false);
    }

    private void Update()
    {
        if (islogin)
        {
            islogin = false;

            RoomListPlane.SetActive(true);//显示房间列表
            RoomListPlane.GetComponent<RoomListRequest>().FlashRoomList();//刷新房间列表
            LoginPlane.SetActive(false);//隐藏自己
        }

        if (ReturnCodeisfail)
        {
            ReturnCodeisfail = false;

            tishi.ShowXinXi("登录失败，请检查密码用户名和网络");
        }
    }

    public void ClickToSendRequest()
    {
        string username = nametext.text;
        string password = passwordtext.text;
        string msg = "";
        if (string.IsNullOrEmpty(username))
        {
            msg += "用户名不能为空 ";
        }
        if (string.IsNullOrEmpty(password))
        {
            msg += "密码不能为空 ";
        }
        if (msg != "")
        {
            tishi.ShowXinXi(msg); return;
        }
        string data = username + "," + password;//把用户名密码按一定规则组合，发送给服务器
        base.SendRequest(data);
        passwordtext.text = "";
        nametext.text = "";
    }

    public override void OnResponse(string data)//这里来处理返回的信息，消息来源是：ReadMessage方法的回调函数OnProcessDataCallback——>GameFacade里的HandleReponse——>RequestManager里的HandleReponse将消息分发给Request对象
    {
        ReturnCode returnCode = (ReturnCode)int.Parse(data);

        if (returnCode == ReturnCode.Success)
        {
            islogin = true;
        }
        else if(returnCode == ReturnCode.Fail)
        {
            ReturnCodeisfail = true;
        }
    }




}
