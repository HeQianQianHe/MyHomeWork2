using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager//所有管理器类的基类，提取出来的一些通用的成员和方法
{
    protected GameFacade facade;
    public BaseManager(GameFacade facade)//构造方法，在自己被GameFacade创建出来的时候获取他的引用
    {
        this.facade = facade;
    }
    public virtual void OnInit() { }
    public virtual void Update(){}
    public virtual void OnDestroy() { }
}
