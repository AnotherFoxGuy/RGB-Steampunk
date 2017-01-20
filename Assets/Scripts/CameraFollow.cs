using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject Cam;
    private Vector3 _camPos;
    //private Rigidbody rb;

    // Use this for initialization
    private void Start()
    {
        //rb = GetComponent<Rigidbody>();
        _camPos = Cam.transform.position;
        //print(""+cam.name);
    }

    // Update is called once per frame
    private void Update()
    {
        var dif = transform.position - _camPos;
        //dif /= 10;
        Cam.transform.position -= dif;
        Cam.transform.position = transform.position;
        //cam_pos = this.transform.position;
    }
}