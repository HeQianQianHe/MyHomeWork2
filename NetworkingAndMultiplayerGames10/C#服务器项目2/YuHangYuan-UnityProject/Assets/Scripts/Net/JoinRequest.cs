using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;

public class JoinRequest : BaseRequest
{
    bool ReturnCodeisfail = false;
    bool ReturnCodeissuccess = false;

    public GameObject RoomList;
    public GameObject RoomItem;
    public Image player1;
    public Image player2;
    public TiShi tishi;
    public GameObject kaishibutton;

    private string datastrq_playertype;

    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.JoinRoom;
        base.Awake();
    }

    private void Update()
    {
        if (ReturnCodeisfail)
        {
            ReturnCodeisfail = false;

            tishi.ShowXinXi("无法加入房间！");

        }
        if (ReturnCodeissuccess)
        {
            ReturnCodeissuccess = false;

            GameFacade.Instance.player = int.Parse(datastrq_playertype);
            RoomItem.SetActive(true);
            RoomList.SetActive(false);
            player1.enabled = true;
            player2.enabled = true;
            kaishibutton.SetActive(false);
        }
    }

    public void JoinRoom()
    {
        base.SendRequest("r");
    }

    public override void OnResponse(string data)
    {
        string[] strs = data.Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs[0]);
        datastrq_playertype = strs[1];
        if (returnCode == ReturnCode.Success)
        {
            ReturnCodeissuccess = true;
        }
        else if (returnCode == ReturnCode.Fail)
        {
            ReturnCodeisfail = true;
        }
    }
}
