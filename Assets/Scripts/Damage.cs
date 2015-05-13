using UnityEngine;
using System.Collections;
using AGC.Tools;

public class Damage : MonoBehaviour {

public float Health = 10f;



	public void ApplyDamage (float d)
	{
		Health -= d;
		#if UNITY_EDITOR
		AGCTools.log("Health "+ Health + " Damage " + d);
		#endif//UNITY_EDITOR
		if (Health <= 0)
			Destroy (this.gameObject);
	}

}
