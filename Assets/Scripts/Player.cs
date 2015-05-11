using UnityEngine;
using System.Collections;
using AGC.Tools;

public class Player : MonoBehaviour 
{
	public float Health = 1;
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
	private int sc = 0;
	private AColor selected_color;
	private Rigidbody rb;
	private float damage = 5;
	private GameObject[] go_hp;
	private	GameObject[] gos_obs;

	enum AColor{ None, Red, Green, Blue};



	void Start () 
	{
		pr = Camera.main.gameObject.AddComponent<Projector>();
		pr.fieldOfView = 120;
		rb = this.GetComponent<Rigidbody>();
		Colors = new Material[4]{NullMat,RedMaterial,GreenMaterial,BlueMaterial};
		go_hp = GameObject.FindGameObjectsWithTag("HP");
		gos_obs = GameObject.FindGameObjectsWithTag("Obstacles");
		UpdateColors ();
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.E)) 
		{
			sc++;
			if(sc > 3)
				sc = 0;
			pr.material = Colors[sc];
			selected_color = (AColor)sc;
			UpdateColors ();
		}
		if (Input.GetKeyDown(KeyCode.Q)) 
		{
			sc--;
			if(sc < 0)
				sc = 3;
			pr.material = Colors[sc];
			selected_color = (AColor)sc;
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
			if(rb.velocity.x < MaxSpeed)
				rb.AddForce(transform.TransformDirection(Vector3.right * Speed));
		}
		if(Input.GetButton("Left"))
		{
			if(rb.velocity.x > -MaxSpeed)
				rb.AddForce(transform.TransformDirection(Vector3.left * Speed));
		}
	}
	void UpdateColors ()
	{
		damage = 5;

		foreach(GameObject go in go_hp)
			go.SetActive(false);
		foreach(GameObject go in gos_obs)
			go.SetActive(true);

		if(selected_color == AColor.Red)
		{
			print("selected_color == AColor.Red");
			damage = 10;
		}

		else if(selected_color == AColor.Green)
		{
			print("selected_color == AColor.Green");

			foreach(GameObject go in go_hp)
				go.SetActive(true);
		}

		else if(selected_color == AColor.Blue)
		{
			print("selected_color == AColor.Blue");

			foreach(GameObject go in gos_obs)
				go.SetActive(false);
		}

		else
		{
			print("None");
		}
	}
}
