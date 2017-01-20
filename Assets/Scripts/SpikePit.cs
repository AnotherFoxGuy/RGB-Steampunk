using UnityEngine;

public class SpikePit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Player"))
            other.SendMessage("ApplyDamage", 999);
    }
}