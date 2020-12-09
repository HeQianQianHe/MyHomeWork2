using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;

public class RoomListRequest : BaseRequest
{
    bool ReturnCodeisfail = false;
    bool ReturnCodeissuccess = false;

    string roomstate;

    public GameObject roomplane;
    public TiShi tishi;
    public Image battle;
    public Image waitingbattle;
    public Image waitingjoin;

    public override void Awake()
    {
        requestCode = RequestCode.Room;//这个RequestCode对应着服务器那边的UserController
        actionCode = ActionCode.ListRoom;//这个ActionCode对应着UserController里面的Login方法
        base.Awake();//base里面的语句：facade.AddRequest(actionCode, this) 将自己添加进字典里，所以上面三条语句要在这之前执行

    }

    private void Update()
    {
        if (ReturnCodeisfail)
        {
            ReturnCodeisfail = false;

            roomplane.SetActive(false);
            tishi.ShowXinXi("没有房间！");
        }
        if (ReturnCodeissuccess)
        {
            ReturnCodeissuccess = false;

            roomplane.SetActive(true);
            if (roomstate == "Battle")
            {
                battle.enabled = true;
                waitingbattle.enabled = false;
                waitingjoin.enabled = false;
            }
            else if (roomstate == "WaitingBattle")
            {
                battle.enabled = false;
                waitingbattle.enabled = true;
                waitingjoin.enabled = false;
            }
            else if (roomstate == "WaitingJoin")
            {
                battle.enabled = false;
                waitingbattle.enabled = false;
                waitingjoin.enabled = true;
            }
        }
    }

    public void FlashRoomList()
    {
        base.SendRequest("Flash");
    }


    public override void OnResponse(string data)
    {
        string[] strs = data.Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs[0]);
        if (returnCode == ReturnCode.Success)
        {
            ReturnCodeissuccess = true;
            if (strs[1] == "Battle")
            {
                roomstate = "Battle";
            }
            else if (strs[1] == "WaitingBattle")
            {
                roomstate = "WaitingBattle";
            }
            else if (strs[1] == "WaitingJoin")
            {
                roomstate = "WaitingJoin";
            }

        }
        else if (returnCode == ReturnCode.Fail)
        {
            ReturnCodeisfail = true;
        }
    }
}
