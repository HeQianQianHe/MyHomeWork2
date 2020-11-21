using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsMove : MonoBehaviour
{
    Rigidbody myRigid;
    public bool canMove = false;
    public Transform targetObject;
    public float followForce = 10;
    public float explosionForce = 100;
    public bool isReverseForceDiretion = false;
    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        Invoke("CancleGravity",3);
    }

    
    void Update()
    {
        if (canMove)
        {
            Vector3 tpos = targetObject.position - transform.position;
            if (isReverseForceDiretion)
            {
                
            }
            else
            {
                myRigid.AddForce(tpos * followForce, ForceMode.Force);
            }
            
            
        }
    }

    public void ExplosionBegin()
    {
        canMove = false;
        myRigid.AddExplosionForce(explosionForce, targetObject.position, 10);
    }

    void CancleGravity()
    {
        myRigid.useGravity = false;
        canMove = true;
    }
}
