using UnityEngine;
using System.Collections;

public class Throwable : MonoBehaviour
{

    public thing throwable;
    private GameObject player;

    public enum thing { bold, SpiderSphere };

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
        if (throwable == thing.bold)
        {
            if (collision.gameObject.tag == "Player")
                player.SendMessage("ApplyDamage", 1);
            Destroy(this.gameObject);
        }
        else
        {
            Instantiate(Resources.Load("SpiderBot"), this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
