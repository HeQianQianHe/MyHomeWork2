using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;

public class NowWeaponRequest : BaseRequest
{
    private Weapon localPlayerWeapon;
    private Weapon remotePlayerWeapon;

    private int setRate = 15;

    bool timetosetweapon = false;

    private int remotenowweapon=-1;

    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.NowWeapon;
        base.Awake();
    }

    private void OnEnable()
    {
        if (GameFacade.Instance.player == 0)
        {
            Debug.Log("角色0");
            localPlayerWeapon = GameObject.Find("Player0").GetComponent<Weapon>();
            remotePlayerWeapon = GameObject.Find("Player1").GetComponent<Weapon>();
        }
        else if (GameFacade.Instance.player == 1)
        {
            Debug.Log("角色1");
            localPlayerWeapon = GameObject.Find("Player1").GetComponent<Weapon>();
            remotePlayerWeapon = GameObject.Find("Player0").GetComponent<Weapon>();
        }
        else if (GameFacade.Instance.player == -1)
        {
            Debug.Log("MoveRequest中角色未指定");
        }

        InvokeRepeating("setLocalPlayerToRemote", 1f, 1f / setRate);
    }

    private void Update()
    {
        if (timetosetweapon)
        {
            timetosetweapon = false;

            remotePlayerWeapon.JianWuQi(remotenowweapon);
        }
    }

    void setLocalPlayerToRemote()
    {
        base.SendRequest(localPlayerWeapon.nowWeapon.ToString());
    }


    public override void OnResponse(string data)
    {

        remotenowweapon = int.Parse(data);

        timetosetweapon = true;
    }


}
