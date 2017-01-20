using UnityEngine;

public class Throwable : MonoBehaviour
{
    public enum ThrowableObject
    {
        Bold,
        SpiderSphere
    }

    private GameObject _player;

    public ThrowableObject CurrentObject;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        var dis = Mathf.Abs(transform.position.x - _player.transform.position.x);
        if (dis > 25)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CurrentObject == ThrowableObject.Bold)
        {
            if (collision.gameObject.CompareTag("Player"))
                _player.SendMessage("ApplyDamage", 1);
            Destroy(gameObject);
        }
        else
        {
            Instantiate(Resources.Load("SpiderBot"), transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}