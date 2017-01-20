using UnityEngine;

public class Damage : MonoBehaviour
{
    public float Health = 10f;


    public void ApplyDamage(float d)
    {
        Health -= d;
#if UNITY_EDITOR
        print("Health " + Health + " Damage " + d);
#endif //UNITY_EDITOR
        if (Health <= 0)
            Destroy(gameObject);
    }
}