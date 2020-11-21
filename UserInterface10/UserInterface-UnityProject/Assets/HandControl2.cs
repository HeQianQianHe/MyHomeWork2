using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
public class HandControl2 : MonoBehaviour
{
    LeapProvider provider;
    public GameObject controlObj;
    public GameObject yaoKongQi;
    float dis;
    bool isGrap = false;
    public Vector3 offectVector;
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
            if (hand.IsLeft == true)
            {
               

            }
            else//左手遥控器
            {
                Vector3 leapHandAngle = hand.Basis.rotation.ToQuaternion().eulerAngles;
                if (leapHandAngle.z > 125 && leapHandAngle.z < 215)
                {
                    yaoKongQi.transform.rotation = hand.Basis.rotation.ToQuaternion();
                    yaoKongQi.transform.position = hand.PalmPosition.ToVector3() + hand.PalmNormal.ToVector3() * 0.4f + hand.Direction.ToVector3() * 0.1f + offectVector;
                    yaoKongQi.SetActive(true);
                }
                else
                {
                    yaoKongQi.SetActive(false);
                }
                //Debug.Log(hand.GrabStrength);


            }
        }
    }
}
