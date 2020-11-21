using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
public class HandControl : MonoBehaviour
{
    LeapProvider provider;
    public GameObject controlObj;
    public GameObject yaoKongQi;
    float dis;
    bool isGrap = false;
    public Scene5Control s5c;
    bool isInExplosion = false;
    void Start()
    {
        provider = FindObjectOfType<LeapProvider>() as LeapProvider;
    }


    void Update()
    {
       
    }

    void ExplosionEnd()
    {
        isInExplosion = false;
    }

    void FixedUpdate()
    {
        Frame frame = provider.CurrentFrame;

        foreach (Hand hand in frame.Hands)
        {
            if (hand.IsLeft == false)//右手抓球
            {
                dis = Vector3.Distance(hand.PalmPosition.ToVector3(), controlObj.transform.position);
                if (hand.GrabStrength > 0.3f && dis < 1)
                {
                    controlObj.transform.position = hand.PalmPosition.ToVector3() + hand.PalmNormal.ToVector3() * 0.35f + hand.Direction.ToVector3() * 0.25f;
                }

            }
            else//左手遥控器
            {
                Vector3 leapHandAngle = hand.Basis.rotation.ToQuaternion().eulerAngles;
                if (leapHandAngle.z > 125 && leapHandAngle.z < 215)
                {
                    yaoKongQi.transform.rotation = hand.Basis.rotation.ToQuaternion();
                    yaoKongQi.transform.position = hand.PalmPosition.ToVector3() + hand.PalmNormal.ToVector3() * 0.4f + hand.Direction.ToVector3() * 0.1f;
                    yaoKongQi.SetActive(true);
                }
                else
                {
                    yaoKongQi.SetActive(false);
                }
                //Debug.Log(hand.GrabStrength);

                if (hand.GrabStrength > 0.7f && isInExplosion == false)
                {
                    /*
                    if (s5c._nv != null)
                    {
                        PhysicsMove[] pm = s5c._nv.GetComponentsInChildren<PhysicsMove>();
                        foreach (PhysicsMove item in pm)
                        {
                            item.ExplosionBegin();
                        }
                    }
                    if (s5c._chengxu != null)
                    {
                        PhysicsMove[] pm = s5c._chengxu.GetComponentsInChildren<PhysicsMove>();
                        foreach (PhysicsMove item in pm)
                        {
                            item.ExplosionBegin();
                        }
                    }
                    if (s5c._yishu != null)
                    {
                        PhysicsMove[] pm = s5c._yishu.GetComponentsInChildren<PhysicsMove>();
                        foreach (PhysicsMove item in pm)
                        {
                            item.ExplosionBegin();
                        }
                    }
                    isInExplosion = true;
                    Invoke("ExplosionEnd",4);
                    */
                }
                else
                {

                }
            }
        }
    }
}
