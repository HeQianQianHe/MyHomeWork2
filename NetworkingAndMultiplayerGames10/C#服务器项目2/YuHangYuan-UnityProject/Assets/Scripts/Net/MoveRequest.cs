using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Common;

public class MoveRequest : BaseRequest
{
    private Transform localPlayerTransform;
    private Transform remotePlayerTransform;

    private int setRate = 24;
    private Vector3 pos;
    private Vector3 rotation;

    private bool issetRemotePlayer = false;

    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Move;
        base.Awake();
    }

    private void FixedUpdate()
    {
        if (issetRemotePlayer)
        {
            setRemotePlayer();
            issetRemotePlayer = false;
        }
    }

    private void OnEnable()
    {
        if (GameFacade.Instance.player == 0)
        {
            Debug.Log("角色0");
            localPlayerTransform = GameObject.Find("Player0").GetComponent<Transform>();
            remotePlayerTransform = GameObject.Find("Player1").GetComponent<Transform>();
        }
        else if (GameFacade.Instance.player == 1)
        {
            Debug.Log("角色1");
            localPlayerTransform = GameObject.Find("Player1").GetComponent<Transform>();
            remotePlayerTransform = GameObject.Find("Player0").GetComponent<Transform>();
        }
        else if (GameFacade.Instance.player == -1)
        {
            Debug.Log("MoveRequest中角色未指定");
        }

        InvokeRepeating("setLocalPlayerToRemote", 1f, 1f / setRate);
    }


    void setLocalPlayerToRemote()
    {
        float x = localPlayerTransform.position.x;
        float y = localPlayerTransform.position.y;
        float z = localPlayerTransform.position.z;

        float rotationX = localPlayerTransform.eulerAngles.x;
        float rotationY = localPlayerTransform.eulerAngles.y;
        float rotationZ = localPlayerTransform.eulerAngles.z;

        string data = string.Format("{0},{1},{2}|{3},{4},{5}", x, y, z, rotationX, rotationY, rotationZ);
        base.SendRequest(data);
    }

    void setRemotePlayer()
    {
        remotePlayerTransform.position = pos;
        remotePlayerTransform.eulerAngles = rotation;
    }


    public override void OnResponse(string data)
    {
        
        string[] strs = data.Split('|');
        pos = UnityTools.ParseVector3(strs[0]);
        rotation = UnityTools.ParseVector3(strs[1]);

        issetRemotePlayer = true;
    }
}
