using UnityEngine;
using System.Collections;

public class Bold : MonoBehaviour 
{
	void OnCollisionEnter(Collision collision) 
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		player.SendMessage ("ApplyDamage", 1);
		Destroy(this.gameObject);
	}
}
