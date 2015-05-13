using UnityEngine;
using System.Collections;
using AGC.Tools;

public class Player : MonoBehaviour 
{
	public float Health = 100;
	public float MR = 100;
	public float Speed = 1;
	public float MaxSpeed = 1;
	public float JumpSpeed = 1;
	public Material RedMaterial;
	public Material GreenMaterial;
	public Material BlueMaterial;
	public static Material[] Colors;

	private Material NullMat;
	private int lm = 1 << 9;
	private Projector pr;
	private AColor selected_color;
	private Rigidbody rb;
	private float damage = 5;
	private GameObject[] go_hp;
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
		go_hp = GameObject.FindGameObjectsWithTag("HP");
		gos_obs = GameObject.FindGameObjectsWithTag("Obstacles");
		gos_enemy = GameObject.FindGameObjectsWithTag("Enemy");
		UpdateColors ();
	}

	void Update ()
	{
		UpdateCheats();
		if(GodMode)
		{
			Health = 100;
		}
		if (Input.GetKeyDown(KeyCode.Alpha1)) 
		{
			pr.material = Colors[(int)AColor.Red];
			selected_color = AColor.Red;
			UpdateColors ();
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) 
		{
			pr.material = Colors[(int)AColor.Green];
			selected_color = AColor.Green;
			UpdateColors ();
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)) 
		{
			pr.material = Colors[(int)AColor.Blue];
			selected_color = AColor.Blue;
			UpdateColors ();
		}
		if (Input.GetKeyDown(KeyCode.Space)) 
		{
			pr.material = Colors[(int)AColor.None];
			selected_color = AColor.None;
			UpdateColors ();
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
				ht.collider.SendMessage ("ApplyDamage", damage);
		}
	}


	void FixedUpdate ()
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
	void UpdateColors ()
	{
		damage = 5;
		gos_enemy = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(GameObject go in go_hp)
			go.SetActive(false);
		foreach(GameObject go in gos_obs)
			go.SetActive(true);
		foreach(GameObject go in gos_enemy)
			go.SetActive(true);

		if(selected_color == AColor.Red)
		{
			AGCTools.log("selected_color == AColor.Red");
			damage = 10;
		}

		else if(selected_color == AColor.Green)
		{
			AGCTools.log("selected_color == AColor.Green");

			foreach(GameObject go in go_hp)
				go.SetActive(true);
		}

		else if(selected_color == AColor.Blue)
		{
			AGCTools.log("selected_color == AColor.Blue");

			foreach(GameObject go in gos_obs)
				go.SetActive(false);
			foreach(GameObject go in gos_enemy)
				go.SetActive(false);
		}

		else
		{
			AGCTools.log("None");
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
		GUI.Box(new Rect(10, 10, 100, 20), ""+Health);
	}
	void UpdateCheats() {
		if (CheatDelay > 0) {
			CheatDelay -= Time.deltaTime;
			if (CheatDelay <= 0) {
				CheatDelay = 0f;
				GodModeProgress = 0;
			}
		}
		if (GodModeProgress == 0 && Input.GetKeyDown(KeyCode.E)) {
			++GodModeProgress;
			CheatDelay = 1f;
		} else if (GodModeProgress == 1 && Input.GetKeyDown(KeyCode.D)) {
			++GodModeProgress;
			CheatDelay = 1f;
		} else if (GodModeProgress == 2 && Input.GetKeyDown(KeyCode.G)) {
			++GodModeProgress;
			CheatDelay = 1f;
		} else if (GodModeProgress == 3 && Input.GetKeyDown(KeyCode.A)) {
			++GodModeProgress;
			CheatDelay = 1f;
		} else if (GodModeProgress == 4 && Input.GetKeyDown(KeyCode.R)) {
			GodModeProgress = 0;
			GodMode = !GodMode;
			print("GodMode On!");
		}
	}
}
