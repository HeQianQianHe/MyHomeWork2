using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
public class RequestManager : BaseManager
{
    public RequestManager(GameFacade facade) : base(facade) { }

    private Dictionary<ActionCode, BaseRequest> requestDict = new Dictionary<ActionCode, BaseRequest>();

    public void AddRequest(ActionCode actionCode,BaseRequest request)//向字典中添加request对象
    {
        
        requestDict.Add(actionCode, request);
    }
    public void RemoveRequest(ActionCode actionCode)//移除字典里的request对象
    {
        requestDict.Remove(actionCode);
    }
    public void HandleReponse(ActionCode actionCode, string data)//这里处理从ClientManager接收完的返回消息中间经过了中介facade
    {
        BaseRequest request;
        requestDict.TryGetValue(actionCode,out request);//通过ActionCode来把消息传送给对应的Response对象里面的OnResponse方法来处理
        if (request == null)
        {
            Debug.LogWarning("无法得到ActionCode[" + actionCode + "]对应的Request类");return;
        }
        request.OnResponse(data);
    }
}
