using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherSphere : MonoBehaviour
{
    Vector3 prePos;
    public float friction;
    public float mass;
    public float radius;

    [NonSerialized]
    public Vector3 currentV;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //摩擦力
        Vector3 frictionDeltaV = -Time.deltaTime * friction * currentV.normalized;
        //防止摩擦力反向运动
        Vector3 finalV = currentV + frictionDeltaV;
        if (finalV.x * currentV.x <= 0)
            frictionDeltaV.x = -currentV.x;
        if (finalV.y * currentV.y <= 0)
            frictionDeltaV.y = -currentV.y;
        if (finalV.z * currentV.z <= 0)
            frictionDeltaV.z = -currentV.z;

        //应用加速度
        Vector3 curV = currentV + frictionDeltaV;
        prePos = transform.position;
        transform.Translate((curV + currentV) * Time.deltaTime / 2);
        currentV = curV;

        Vector3 pos = transform.position;
        if (pos.x > 0.95f)
        {
            Bounce(new Vector3(-1, 0, 0));
        }
        if (pos.x < -0.95f)
        {
            Bounce(new Vector3(1, 0, 0));
        }
        if (pos.z > 0.95f)
        {
            Bounce(new Vector3(0, 0, -1));
        }
        if (pos.z < -0.95f)
        {
            Bounce(new Vector3(0, 0, 1));
        }

    }


    void Bounce(Vector3 normal)
    {
        Vector3 reflect = currentV - 2*(Vector3.Dot(currentV,normal))*normal;

        currentV = reflect;

        //如果有碰撞，位置回退，防止穿透
        transform.position = prePos;
    }
}
