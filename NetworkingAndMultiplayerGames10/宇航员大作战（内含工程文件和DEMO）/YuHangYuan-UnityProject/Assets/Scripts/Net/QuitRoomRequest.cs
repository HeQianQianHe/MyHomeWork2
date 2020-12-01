using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;

public class QuitRoomRequest : BaseRequest
{
    bool istuichu = false;

    public GameObject RoomItem;
    public GameObject RoomList;
    public Image player1;
    public Image player2;


    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.QuitRoom;
        base.Awake();
    }

    private void Update()
    {
        if (istuichu)
        {
            istuichu = false;

            player1.enabled = false;
            player2.enabled = false;
            RoomList.SetActive(true);
            RoomList.GetComponent<RoomListRequest>().FlashRoomList();//刷新房间列表
            RoomItem.SetActive(false);
        }
    }

    public void TuiChu()
    {
        base.SendRequest("q");
    }

    public override void OnResponse(string data)
    {
        string[] strs = data.Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs[0]);
        if(returnCode == ReturnCode.Success)
        {
            istuichu = true;
        }
    }
}
