using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
public class BaseRequest : MonoBehaviour//所有请求类的基类
{
    protected RequestCode requestCode = RequestCode.None;
    protected ActionCode actionCode = ActionCode.None;
    protected GameFacade _facade;
    private bool isinit = false;


    protected GameFacade facade
    {
        get
        {
            if (_facade == null)
                _facade = GameFacade.Instance;
            return _facade;
        }
    }

    private void LateUpdate()
    {
        if (isinit == false)
        {

            AddSelf();
        }
    }
   

    public virtual void Awake ()//当自己这个Request对象被unity创建出来的时候通过facade间接的把自己添加到RequestManager中的字典中进行管理
    {
        AddSelf();
    }

    protected void SendRequest(string data)//每个请求类中都有这样一个方法，将自己的RequestCode，ActionCode和要发送的数据请求给中介facade再由ClientManager发送给服务器
    {
        
        facade.SendRequest(requestCode, actionCode, data);
    }

    public virtual void SendRequest() { }
    public virtual void OnResponse(string data) { }

    public virtual void OnDestroy()//当自己这个Request对象被销毁的时候通过facade间接的把自己从RequestManager中的字典中删除
    {
        if(facade != null)
            facade.RemoveRequest(actionCode);
    }

    void AddSelf()
    {
        if (facade.requestMng != null)
        {
            facade.AddRequest(actionCode, this);
            isinit = true;
        }
    }
}
