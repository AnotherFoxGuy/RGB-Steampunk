using UnityEngine;
using System.Collections;

public class Bold : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        float dis = Mathf.Abs(this.transform.position.x - player.transform.position.x);
        if (dis > 25)
        {
            Destroy(this.gameObject);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            player.SendMessage("ApplyDamage", 1);
        Destroy(this.gameObject);
    }
}
