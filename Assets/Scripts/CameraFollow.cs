using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
	public GameObject cam;
	private Vector3 cam_pos;
	//private Rigidbody rb;

	// Use this for initialization
	void Start () 
	{
		//rb = GetComponent<Rigidbody>();
		cam_pos = cam.transform.position;
		//print(""+cam.name);
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 dif = this.transform.position - cam_pos;
		//dif /= 10;
		cam.transform.position -= dif;
		cam.transform.position = this.transform.position;
		//cam_pos = this.transform.position;
	}
}
