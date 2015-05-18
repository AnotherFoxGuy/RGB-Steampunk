using UnityEngine;
using System.Collections;
using AGC.Tools;

public class Player : MonoBehaviour 
{
	public float Health = 100;
	public float LightResource = 100;
	public float Speed = 1;
	public float MaxSpeed = 1;
	public float JumpSpeed = 1;
	public Material RedMaterial;
	public Material GreenMaterial;
	public Material BlueMaterial;
	public static Material[] Colors;

	private bool lr_on = false;
	private bool hp = false;
	private Material NullMat;
	private int lm = 1 << 9;
	private Projector pr;
	private GameObject arm;
	private GameObject arm_hit;
	private Vector3 arm_orgpos;
	private arm_states arm_state = arm_states.idle;
	enum arm_states{ idle, forward, backward};
	private Rigidbody rb;
	private float damage = 5;
	private GameObject go_hp;
	private	GameObject[] gos_obs;
	private	GameObject[] gos_enemy;
	private int GodModeProgress = 0;
	private float CheatDelay = 0f;
	private bool GodMode = false;
	enum AColor{ None, Red, Green, Blue};


	void Start () 
	{
		pr = Camera.main.gameObject.AddComponent<Projector>();
		pr.fieldOfView = 120;
		rb = this.GetComponent<Rigidbody>();
		Colors = new Material[4]{NullMat,RedMaterial,GreenMaterial,BlueMaterial};
		arm = GameObject.Find("Arm");
		arm_orgpos = arm.transform.localPosition;
		go_hp =  (GameObject)Resources.Load("/Prefabs/yourPrefab");
		gos_obs = GameObject.FindGameObjectsWithTag("Obstacles");
		gos_enemy = GameObject.FindGameObjectsWithTag("Enemy");
		UpdateColors (AColor.None);
	}

	void Update ()
	{
		UpdateArm();
		UpdateCheats();
		if(GodMode)
		{
			Health = 100;
		}
		if(arm_state == arm_states.idle)
		{
			if (Input.GetKeyDown(KeyCode.Alpha1)) 
			{
				if(!lr_on)
					UpdateColors (AColor.Red);
				else
					UpdateColors (AColor.None);
			}
			if (Input.GetKeyDown(KeyCode.Alpha2)) 
			{
				if(!lr_on)
					UpdateColors (AColor.Green);
				else
					UpdateColors (AColor.None);
			}
			if (Input.GetKeyDown(KeyCode.Alpha3)) 
			{
				if(!lr_on)
					UpdateColors (AColor.Blue);
				else
					UpdateColors (AColor.None);
			}
			if (LightResource <= 0 && lr_on) 
			{
				UpdateColors (AColor.None);
				LightResource = 0;
			}
			if(lr_on)
			{
				LightResource -= Time.deltaTime * 10;
			}
			if(Input.GetButtonDown("Jump"))
			{
				if(Physics.Raycast(this.transform.position, Vector3.down, 1.5f))
					rb.AddForce(transform.TransformDirection(Vector3.up * JumpSpeed));
			}
			if(Input.GetButtonDown("Fire1"))
			{
				RaycastHit[] rh = Physics.SphereCastAll(this.transform.position,2,Vector3.right,1,lm);
				foreach(RaycastHit ht in rh)
				{
					Enemy e = ht.collider.GetComponent<Enemy>();
					if(e != null)
					{
						ht.collider.SendMessage ("ApplyDamage", damage);
					}
				}
					
			}
		}
	}
	void FixedUpdate ()
	{
		if(arm_state == arm_states.idle)
		{
			if(Input.GetButton("Right"))
			{
				this.transform.eulerAngles = new Vector3(0,180,0);
				if(rb.velocity.x < MaxSpeed)
					rb.AddForce(Vector3.right * Speed);
			}
			if(Input.GetButton("Left"))
			{
				this.transform.eulerAngles = new Vector3(0,0,0);
				if(rb.velocity.x > -MaxSpeed)
					rb.AddForce(Vector3.left * Speed);
			}
		}
	}
	void UpdateArm ()
	{
		if(Input.GetButtonDown("Fire2"))
		{
			arm_state = arm_states.forward;
			rb.isKinematic = true;
			arm_hit = null;
		}
		if(arm_state == arm_states.forward)
		{
			Vector3 a = arm.transform.localPosition;
			Vector3 p = this.transform.position;
			p.y+=0.5f;
			arm.transform.localPosition = new Vector3(a.x - 0.2f,a.y,a.z);
			RaycastHit ht;
			Debug.DrawLine(arm.transform.position,arm.transform.position + transform.InverseTransformDirection(Vector3.left));
			if(Physics.Raycast(this.transform.position, transform.InverseTransformDirection(Vector3.left),out ht,Mathf.Abs( a.x)))
			{
				AGCTools.log(""+ ht.collider.name);
				arm_state = arm_states.backward;
				arm_hit = ht.collider.gameObject;
				arm_hit.SendMessage("Stun");
			}
			if(a.x < -10)
			{
				arm_state = arm_states.backward;
			}
		}
		if(arm_state == arm_states.backward)
		{
			Vector3 a = arm.transform.localPosition;
			arm.transform.localPosition = new Vector3(a.x + 0.2f,a.y,a.z);
			if (arm_hit != null)
			{
				Vector3 ah = arm_hit.transform.position;
				arm_hit.transform.position = new Vector3(arm.transform.position.x,ah.y,ah.z);
			}
			if(a.x > arm_orgpos.x - 1.5f)
			{
				arm.transform.localPosition = arm_orgpos;
				arm_state = arm_states.idle;
				rb.isKinematic = false;
			}
		}
	}
	void UpdateColors (AColor selected_color)
	{
		pr.material = Colors[(int)selected_color];

		if(selected_color == AColor.Red)
		{
			AGCTools.log("selected_color == AColor.Red");
			lr_on = true;
			damage = 10;
		}

		else if(selected_color == AColor.Green)
		{
			AGCTools.log("selected_color == AColor.Green");
			lr_on = true;
			hp = true;
		}

		else if(selected_color == AColor.Blue)
		{
			AGCTools.log("selected_color == AColor.Blue");
			lr_on = true;
			gos_enemy = GameObject.FindGameObjectsWithTag("Enemy");
			foreach(GameObject go in gos_obs)
				go.SetActive(false);
			foreach(GameObject go in gos_enemy)
				go.SetActive(false);
		}

		else
		{
			try
			{
				AGCTools.log("None");
				lr_on = false;
				hp = false;
				damage = 5;
				//gos_enemy = GameObject.FindGameObjectsWithTag("Enemy");
				foreach(GameObject go in gos_obs)
					go.SetActive(true);
				foreach(GameObject go in gos_enemy)
					go.SetActive(true);
			}
			catch
			{
			}
			
		}
	}
	public void ApplyDamage (float d) 
	{
		Health -= d;
		#if UNITY_EDITOR
		//AGCTools.log("Health "+ Health + " Damage " + d);
		this.GetComponent<Rigidbody>().AddExplosionForce(500f,GetClosestObject("Enemy").transform.position,5f);
		#endif//UNITY_EDITOR
		if (Health <= 0)
			Application.LoadLevel(Application.loadedLevel);
	}
	void OnCollisionEnter(Collision collision) 
	{
		foreach (ContactPoint c in collision.contacts) 
		{
			if(c.otherCollider.tag == "LightResource")
			{
				if(!hp)
				{
					LightResource += 50;
					if(LightResource > 100)
						LightResource = 100;
				}
				else
				{
					Health += 50;
					if(Health > 100)
						Health = 100;
				}
				Destroy(c.otherCollider.gameObject);
			}
		}
	}

	GameObject GetClosestObject(string tag)
	{
		GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
		GameObject closestObject = GameObject.FindGameObjectWithTag(tag);
		foreach (GameObject obj in objectsWithTag)
		{
			//compares distances
			if(Vector3.Distance(transform.position, obj.transform.position) <= Vector3.Distance(transform.position, closestObject.transform.position))
			{
				closestObject = obj;
			}
		}
		return closestObject;
	}

	void OnGUI() 
	{
		GUI.Box(new Rect(10, 10, 100, 40), Mathf.Floor(Health)+"\n"+ Mathf.Floor(LightResource));
	}

	void UpdateCheats() {
		if (CheatDelay > 0) {CheatDelay -= Time.deltaTime;
		if (CheatDelay <= 0) {CheatDelay = 0f;GodModeProgress = 0;}}
		if (GodModeProgress == 0 && Input.GetKeyDown(KeyCode.E)) {GodModeProgress++;CheatDelay = 1f;} else if (GodModeProgress == 1 && Input.GetKeyDown(KeyCode.D)) {GodModeProgress++;CheatDelay = 1f;} else if (GodModeProgress == 2 && Input.GetKeyDown(KeyCode.G)) {GodModeProgress++;CheatDelay = 1f;} else if (GodModeProgress == 3 && Input.GetKeyDown(KeyCode.A)) {GodModeProgress++;CheatDelay = 1f;} else if (GodModeProgress == 4 && Input.GetKeyDown(KeyCode.R)) {GodModeProgress = 0;GodMode = !GodMode;print("GodMode On!");}
	}
}
