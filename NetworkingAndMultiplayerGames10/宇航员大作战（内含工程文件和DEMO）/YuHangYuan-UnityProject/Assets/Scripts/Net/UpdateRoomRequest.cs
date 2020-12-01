using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;

public class UpdateRoomRequest : BaseRequest
{
    public Image player2;

    bool is1 = false;
    bool is0 = false;

    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.UpdateRoom;
        base.Awake();
    }

    private void Update()
    {
        if (is1)
        {
            is1 = false;

            player2.enabled = true;

        }
        if (is0)
        {
            is0 = false;

            player2.enabled = false;

        }
    }



    public override void OnResponse(string data)
    {
        if (data == "1")
        {
            is1 = true;
        }
        else if (data == "0")
        {
            is0 = true;
        }
    
    }
}
