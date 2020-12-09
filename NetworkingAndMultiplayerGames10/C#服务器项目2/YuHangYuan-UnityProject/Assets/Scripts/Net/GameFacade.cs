using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
public class GameFacade : MonoBehaviour//GameFacade挂在一个空游戏对象上，继承自MonoBehaviour，可以实例化
{
    public int player = -1;

    private static GameFacade _instance;//单例模式，作为游戏主控制脚本，需要在别的很多地方都能访问到
    public static GameFacade Instance
    {   get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameFacade").GetComponent<GameFacade>();
            }
            return _instance;
        }
    }

    //持有这些各个模块之间的管理器，管理器们没有挂在游戏物体上，要在这里从初始
    public  RequestManager requestMng;
    public  ClientManager clientMng;

    private bool isEnterPlaying = false;

    void Start ()//初始化所有脚本，即先new出来，在调用他们自己内部的初始化方法
    {
        //这里在每个脚本的初始化方法里把自己传递过去，让每个管理器持有自己的引用
        requestMng = new RequestManager(this);
        clientMng = new ClientManager(this);


        requestMng.OnInit();
        clientMng.OnInit();

        Screen.SetResolution(1366, 768, false);
    }

	void Update ()
    {
        UpdateManager();
        if (isEnterPlaying)
        {

        }
	}
    
    private void OnDestroy()
    {
        requestMng.OnDestroy();
        clientMng.OnDestroy();
    }

    private void UpdateManager()
    {

        requestMng.Update();
        clientMng.Update();
    }

    public void AddRequest(ActionCode actionCode, BaseRequest request)
    {
        requestMng.AddRequest(actionCode, request);
    }

    public void RemoveRequest(ActionCode actionCode)
    {
        requestMng.RemoveRequest(actionCode);
    }

    public void SendRequest(RequestCode requestCode, ActionCode actionCode, string data)//发送数据的方法，把需要发送的请求数据传到这里，在传递给ClientManager然后按格式打包发送
    {
        clientMng.SendRequest(requestCode, actionCode, data);
    }

    public void HandleReponse(ActionCode actionCode, string data)
    {
        requestMng.HandleReponse(actionCode, data);
    }

    /*

    public void ShowMessage(string msg)
    {
        uiMng.ShowMessage(msg);
    }

    public void PlayBgSound(string soundName)
    {
        audioMng.PlayBgSound(soundName);
    }
    public void PlayNormalSound(string soundName)
    {
        audioMng.PlayNormalSound(soundName);
    }
    public void SetUserData(UserData ud)
    {
        playerMng.UserData = ud;
    }
    public UserData GetUserData()
    {
        return playerMng.UserData;
    }
    public void SetCurrentRoleType(RoleType rt)
    {
        playerMng.SetCurrentRoleType(rt);
    }
    public GameObject GetCurrentRoleGameObject()
    {
        return playerMng.GetCurrentRoleGameObject();
    }
    public void EnterPlayingSync()
    {
        isEnterPlaying = true;
    }
    private void EnterPlaying()
    {
        playerMng.SpawnRoles();
        cameraMng.FollowRole();
    }
    public void StartPlaying()
    {
        playerMng.AddControlScript();
        playerMng.CreateSyncRequest();
    }
    public void SendAttack(int damage)
    {
        playerMng.SendAttack(damage);
    }
    public void GameOver()
    {
        cameraMng.WalkthroughScene();
        playerMng.GameOver();
    }
    public void UpdateResult(int totalCount, int winCount)
    {
        playerMng.UpdateResult(totalCount, winCount);
    }
*/
}
