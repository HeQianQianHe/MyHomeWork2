using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;

public class CreateRoomRequest : BaseRequest
{
    bool ReturnCodeisfail = false;
    bool ReturnCodeissuccess = false;


    public TiShi tishi;
    public GameObject RoomItem;
    public GameObject RoomList;
    public Image player1;
    public Text kaishibuttontext;
    public GameObject kaishibutton;


    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.CreateRoom;
        base.Awake();
    }

    private void Update()
    {
        if (ReturnCodeisfail)
        {
            ReturnCodeisfail = false;

            tishi.ShowXinXi("现阶段只能存在一个房间！");

        }
        if (ReturnCodeissuccess)
        {
            ReturnCodeissuccess = false;

            RoomItem.SetActive(true);
            RoomList.SetActive(false);
            player1.enabled = true;

            kaishibutton.SetActive(true);
            kaishibuttontext.text = "开始游戏";//自己是房主，所以开始游戏按钮
        }
    }

    public void CreatRoom()
    {
        base.SendRequest("r");
    }

    public override void OnResponse(string data)
    {
        string[] strs = data.Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs[0]);
        if (returnCode == ReturnCode.Success)
        {
            GameFacade.Instance.player = int.Parse(strs[1]);

            ReturnCodeissuccess = true;
        }
        else if (returnCode == ReturnCode.Fail)
        {
            ReturnCodeisfail = true;
        }

    }

}
