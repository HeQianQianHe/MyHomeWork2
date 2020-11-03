using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraShake;

public class Attack : MonoBehaviour
{
    // Parameters of the shake to tweak in the inspector.
    public BounceShake.Params shakeParams;

    public GameObject vfxPrefab;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    float attacktime = 0;
    bool begin = false;
    // Update is called once per frame
    void Update()
    {
        if (begin)
        {
            attacktime += Time.deltaTime;
            if (attacktime>0.4f)
            {
                attacktime = 0;
                begin = false;
                attack = false;
            }
        }
        if (Input.GetMouseButton(0) )
        {
            attack = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            begin = true;
            
        }
    }

    bool attack =false;

    // 碰撞开始
     void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.CompareTag("Rock")&& attack)
        {
            CameraShaker.Shake(new BounceShake(shakeParams));
            GameObject tempHole = Instantiate(vfxPrefab, collider.ClosestPoint(transform.position), Quaternion.identity);
            
        }
    }

    
}
