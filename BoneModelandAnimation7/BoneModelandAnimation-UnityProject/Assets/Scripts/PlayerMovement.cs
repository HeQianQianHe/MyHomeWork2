using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 8.0f;      
	public float jumpSpeed;
	public float gravity;
	float h,v;
	float cooldownTimer, evadeTimer;

	public float evadeTime;
	public float evadeDistance; 

	[HideInInspector] public Transform target;
	[SerializeField] public Transform pivotTransform;

	[HideInInspector] public Vector3 targetPos;
	Vector3 movement;

	Vector3 forward, right, evadeDirection;

	Quaternion newRotation, lockOnRotation;

	Animator anim;


	public bool orbit, backwards, evading, walking;
	public bool attacking;

	CharacterController controller;

    void Awake () {

		anim = GetComponent<Animator> ();
		controller = GetComponent<CharacterController> ();


		targetPos = new Vector3 ();
    }

	void Update() 
	{

		h = Input.GetAxisRaw ("Horizontal");
		v = Input.GetAxisRaw ("Vertical");


		walking = h != 0f || v != 0f;
		orbit = h != 0f;

		CalcMovement (h, v);

		ProcessEvasion ();

		anim.SetBool("move", walking);
		anim.SetFloat ("velx", h);
		anim.SetFloat ("vely", v);

		if(!controller.isGrounded){
			movement.y -= gravity;
		}

		controller.Move (movement * Time.deltaTime);
	}



    void CalcMovement (float h, float v) 
	{
		movement = new Vector3 ();

		if (v != 0 || h != 0) {
			movement = v * pivotForward() + (h/2f) * pivotRight();

		}

		HandleRotation ();
		movement *= speed;
    }

	void HandleRotation()
	{
		if (movement != Vector3.zero) {			
			if (!orbit && v > 0f) {
				newRotation = Quaternion.LookRotation (movement, Vector3.up);
				transform.rotation = newRotation;
			} else {
				newRotation = Quaternion.LookRotation (pivotForward (), Vector3.up);
				transform.rotation = newRotation;
			}
		}
	}

	void Evade() 
	{
		evadeDirection = -transform.forward.normalized;

		if (walking) {
			evadeDirection = movement.normalized;
		}

		if(!evading && Input.GetButtonDown("Fire2")) {
			evading = true;
			evadeTimer = evadeTime;
		}
	}

	void ProcessEvasion() 
	{
		if(evading) {
			evadeTimer = Mathf.Max(0f, evadeTimer - Time.deltaTime);
			controller.Move (evadeDirection * evadeDistance * Time.deltaTime);

			if(evadeTimer == 0) { 
				evading = false;
			}
		}
	}

	void LockOnEnemy() 
	{	
		if (target == null)
		{
			GameObject[] enemyList = GameObject.FindGameObjectsWithTag("LockOnTarget");
			if (enemyList.Length > 0)
			{
				int enemyID = -1; 
				float closestEnemyDistanceSqr = 400f;
				

				for (int i = 0; i < enemyList.Length; i++)
				{
					float enemyDeltaSqr = (transform.position - enemyList[i].transform.position).sqrMagnitude;
					if (enemyDeltaSqr < closestEnemyDistanceSqr)
					{
						closestEnemyDistanceSqr = enemyDeltaSqr;
						enemyID = i;
					}
				}

				if( enemyID > -1){
					target = enemyList[enemyID].transform;
				}
			}
		}
		
		else {
			this.target = null;
		}
	}

	void Attacking() 
	{
		attacking = true;
	}

	void AttackDone() 
	{
		attacking = false;
	}

	Vector3 pivotForward() 
	{
		Vector3 forwardVector = pivotTransform.transform.forward;
		forwardVector.y = 0;
		return forwardVector;
	}

	Vector3 pivotRight() 
	{
		Vector3 rightVector = pivotTransform.transform.right;
		rightVector.y = 0;
		return rightVector;
	}

	Vector3 targetPosition() 
	{
		return targetPos;
	}
}