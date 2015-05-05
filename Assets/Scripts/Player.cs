using UnityEngine;
using System.Collections;
using AGC.Tools;

public class Player : MonoBehaviour 
{
	public float Speed = 1;
	public float MaxSpeed = 1;
	public float JumpSpeed = 1;
	public Material Red;
	public Material Green;
	public Material Blue;
	public static Material[] Colors ;

	private Material NullMat;
	private int lm = 1 << 9;
	private Projector pr;
	private int sc = 0;
	private Rigidbody rb;


	void Start () 
	{
		pr = Camera.main.gameObject.AddComponent<Projector>();
		rb = this.GetComponent<Rigidbody>();
		Colors = new Material[4]{Red,Green,Blue,NullMat};
		pr.material = Colors[0];
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.E)) 
		{
			sc++;
			if(sc >= 4)
				sc = 0;
			pr.material = Colors[sc];
		}
		if (Input.GetKeyDown(KeyCode.Q)) 
		{
			sc--;
			if(sc <= 0)
				sc = 3;
			pr.material = Colors[sc];
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
				ht.collider.SendMessage ("ApplyDamage", 11);
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
}
