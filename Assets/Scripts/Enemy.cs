using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	public e_types EnemyType = e_types.SpiderBot;
	public GameObject InstantiateGameObject;
	public float MovementSpeed = 10;
	public float Health = 10f;
	public float Damage = 1f;
	
	private GameObject player;
	private bool active = false;
	private bool can_move = false;
	private float MoveTo = 1;
	private float dis;
	private int one = 1;
	private int spibot = 5;
	private int lm = 1 << 9;
	private float tmr = 0;
	public enum e_types{Witch,Engineer,SpiderBot};


	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player");
		lm = ~lm;
		MoveTo = MovementSpeed;
	}

	void Update () 
	{
		if(tmr > -0.1)
			tmr -= Time.deltaTime; 

		if(tmr < 0 && !can_move)
			can_move = true;

		if(can_move)
		{
			if(EnemyType == e_types.Engineer) UpdateEngineer();
			else if(EnemyType == e_types.Witch) UpdateWitch();
			else UpdateSpiderBot();
		}
	
	}
	void UpdateEngineer () 
	{
		dis = Vector3.Distance(this.transform.position, player.transform.position);

		if(dis < 10)
		{
			active = true;
		}

		if(active)
		{
			if(spibot > 0 && tmr < 0)
			{
				Instantiate(InstantiateGameObject,new Vector3(this.transform.position.x + one,this.transform.position.y ,this.transform.position.z),Quaternion.identity);
				spibot--;
				tmr = 1f;
			}

			else if (spibot <=0)
			{
				UpdateMove();
				if(dis < 2.3 && tmr < 0)
				{
					player.SendMessage("ApplyDamage", Damage);
					tmr = 1f;
				}
			}
		}
	}
	void UpdateWitch () 
	{
		dis = Vector3.Distance(this.transform.position, player.transform.position);

		if(dis < 10 && !active)
		{
			active = true;
			UpdateMove();
		}

		if(active)
		{
			if (this.transform.position.x > player.transform.position.x + 0.2) 
			{
				one = -1;
				this.transform.eulerAngles = new Vector3(0,180,0);
			} 

			else if (this.transform.position.x < player.transform.position.x - 0.2) 
			{
				one = 1;
				this.transform.eulerAngles = new Vector3(0,0,0);
			} 

			if(tmr < 0)
			{
				GameObject cl = Instantiate(InstantiateGameObject,new Vector3(this.transform.position.x + one,this.transform.position.y ,this.transform.position.z),Quaternion.identity) as GameObject;
				Rigidbody rb = cl.AddComponent<Rigidbody>();
				rb.velocity = new Vector3(one*10,0,0);
				rb.useGravity = false;
				tmr = 1f;
			}

			else if(dis > 10)
			{
				UpdateMove();
			}
		}
	}
	void UpdateSpiderBot () 
	{
		UpdateMove();
		dis = Vector3.Distance(this.transform.position, player.transform.position);

		if(dis < 2.3 && tmr < 0)
		{
			player.SendMessage("ApplyDamage", Damage);
			tmr = 1f;
		}
	}
	void UpdateMove () 
	{
		var translation = Time.deltaTime * MoveTo;
		transform.Translate(translation, 0, 0);
		Vector3 fall = new Vector3(this.transform.position.x + one,this.transform.position.y,this.transform.position.z);

		if (Physics.Raycast(this.transform.position, new Vector3(one, 0, 0), 2, lm) || !Physics.Raycast(fall, Vector3.down, 1, lm)) 
		{
			MoveTo = 0;
		}				

		else 
		{
			if (this.transform.position.x > player.transform.position.x + 0.2) 
			{
				one = -1;
				this.transform.eulerAngles = new Vector3(0,180,0);
				MoveTo = MovementSpeed;
			} 

			else if (this.transform.position.x < player.transform.position.x - 0.2) 
			{
				one = 1;
				this.transform.eulerAngles = new Vector3(0,0,0);
				MoveTo = MovementSpeed;
			} 

			else
			{
				MoveTo = 0;
			}

		}
	}
	public void ApplyDamage (float d) 
	{
		Health -= d;

		#if UNITY_EDITOR
		print("Health "+ Health + " Damage " + d);
		#endif//UNITY_EDITOR

		this.GetComponent<Rigidbody>().AddExplosionForce(750f,player.transform.position,5f);
		
		if (Health <= 0)
		{
			Destroy (this.gameObject);
			Instantiate(Resources.Load("LightResource"),this.transform.position,Quaternion.identity);
		}
	}
	public void Stun () 
	{
		can_move = false;
		tmr = 10;
	}
}
