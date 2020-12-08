using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;

public class StartGameRequest : BaseRequest
{
    public GameObject roomitem;
    public TiShi tishi;

    bool ReturnCodeisfail = false;
    bool ReturnCodeissuccess = false;
    public GameObject camera0;
    public GameObject camera1;
    public GameObject nothingcamera;

    public GameObject gamecontrol;
    private Rigidbody remotePlayerrig;
    public AudioSource UiYinYyue;
    public AudioSource GameYinYue;

    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.StartGame;
        base.Awake();
    }

    private void Update()
    {
        if (ReturnCodeisfail)
        {
            ReturnCodeisfail = false;

            tishi.ShowXinXi("无法开始游戏，人数不足");
        }
        if (ReturnCodeissuccess)
        {
            ReturnCodeissuccess = false;

            nothingcamera.SetActive(false);
            roomitem.SetActive(false);
            gamecontrol.AddComponent<TestPlayerControl>();//添加控制移动的脚本
            gamecontrol.AddComponent<MoveRequest>();//添加同步移动的脚本
            gamecontrol.AddComponent<NowWeaponRequest>();//添加同步武器的脚本
            UiYinYyue.Stop();//结束ui音乐的播放
            GameYinYue.Play();//播放游戏音乐

            if (GameFacade.Instance.player == 0)//激活摄像机
            {
                remotePlayerrig = GameObject.Find("Player1").GetComponent<Rigidbody>();
                Destroy(remotePlayerrig);
                camera0.SetActive(true);
            }else if(GameFacade.Instance.player == 1)
            {
                remotePlayerrig = GameObject.Find("Player0").GetComponent<Rigidbody>();
                Destroy(remotePlayerrig);
                camera1.SetActive(true);
            }
        }
    }

    public void ClickStartGame()
    {
        base.SendRequest("r");
    }

    public override void OnResponse(string data)
    {
        ReturnCode returnCode = (ReturnCode)int.Parse(data);
        if (returnCode == ReturnCode.Success)
        {
            ReturnCodeissuccess = true;
        }
        else if(returnCode == ReturnCode.Fail)
        {
            ReturnCodeisfail = true;
        }
    }
}
