using UnityEngine;
using System.Collections;

public class SpikePit : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" || other.tag == "Player")
        {

            other.SendMessage("ApplyDamage", 999);

        }
    }

}
