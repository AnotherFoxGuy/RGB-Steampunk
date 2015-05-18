using UnityEngine;
using System.Collections;

public class SpikePit : MonoBehaviour 
{
	
	void OnTriggerStay(Collider other) 
	{
		if(other.tag == "Enemy" || other.tag == "Player")
		{
			if (other.attachedRigidbody)
			{
				other.attachedRigidbody.AddForce(Vector3.up * 10);  
				other.SendMessage("ApplyDamage", 1);
			}
		}
	}

}
