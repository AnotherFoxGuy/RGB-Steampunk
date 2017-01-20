using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum ETypes
    {
        Witch,
        Engineer,
        EngineerUnlimited,
        SpiderBot
    }

    private bool _active;

    private Animator _animator;
    private bool _canMove;
    public float Damage = 1f;
    private float _dis;
    public ETypes EnemyType = ETypes.SpiderBot;
    public float Health = 10f;
    private GameObject _insgameobj;
    private int lm = 1 << 9;
    public float MovementSpeed = 10;
    private float _moveTo = 1;
    private int _one = 1;
    private GameObject _player;
    private int _spibot = 5;
    private float _tmr = 0.1f;


    private void Start()
    {
        if (EnemyType == ETypes.Witch) _insgameobj = Resources.Load("Bold") as GameObject;
        else _insgameobj = Resources.Load("SpiderSphere") as GameObject;
        _animator = GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player");
        lm = ~lm;
        _moveTo = MovementSpeed;
        if (transform.position.x > _player.transform.position.x + 0.2)
        {
            _one = -1;
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        else if (transform.position.x < _player.transform.position.x - 0.2)
        {
            _one = 1;
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    private void Update()
    {
        if (_tmr > -0.1)
            _tmr -= Time.deltaTime;

        if (_tmr < 0 && !_canMove)
            _canMove = true;

        if (_canMove)
            if (EnemyType == ETypes.Engineer) UpdateEngineer();
            else if (EnemyType == ETypes.EngineerUnlimited) UpdateEngineerUnlimited();
            else if (EnemyType == ETypes.Witch) UpdateWitch();
            else UpdateSpiderBot();
    }

    private void UpdateEngineer()
    {
        _dis = Vector3.Distance(transform.position, _player.transform.position);

        if (_dis < 10)
            _active = true;

        if (_active)
            if (_spibot > 0 && _tmr < 0)
            {
                ThrowSpider();
                _spibot--;
                _tmr = 1f;
            }

            else if (_spibot <= 0)
            {
                UpdateMove();
                if (_dis < 2.3 && _tmr < 0)
                {
                    _player.SendMessage("ApplyDamage", Damage);
                    _tmr = 1f;
                }
            }
            else
            {
                if (transform.position.x > _player.transform.position.x + 0.2)
                {
                    _one = -1;
                    transform.eulerAngles = new Vector3(0, 180, 0);
                }

                else if (transform.position.x < _player.transform.position.x - 0.2)
                {
                    _one = 1;
                    transform.eulerAngles = new Vector3(0, 0, 0);
                }
            }
    }

    private void UpdateEngineerUnlimited()
    {
        _dis = Vector3.Distance(transform.position, _player.transform.position);

        if (_dis < 10)
            _active = true;

        if (_active)
            if (_tmr < 0)
            {
                ThrowSpider();
                _tmr = 0.5f;
            }
            else
            {
                if (transform.position.x > _player.transform.position.x + 0.2)
                {
                    _one = -1;
                    transform.eulerAngles = new Vector3(0, 180, 0);
                }

                else if (transform.position.x < _player.transform.position.x - 0.2)
                {
                    _one = 1;
                    transform.eulerAngles = new Vector3(0, 0, 0);
                }
            }
    }

    private void ThrowSpider()
    {
        var pos = new Vector3(transform.position.x + _one, transform.position.y + 0.5f, transform.position.z);
        var spy = Instantiate(_insgameobj, pos, Quaternion.identity);
        spy.GetComponent<Rigidbody>().AddExplosionForce(40000f, transform.position, 50f);
    }

    private void UpdateWitch()
    {
        _dis = Vector3.Distance(transform.position, _player.transform.position);

        if (_dis < 10 && !_active)
        {
            _active = true;
            UpdateMove();
        }

        if (_active)
        {
            if (transform.position.x > _player.transform.position.x + 0.2)
            {
                _one = -1;
                transform.eulerAngles = new Vector3(0, 180, 0);
            }

            else if (transform.position.x < _player.transform.position.x - 0.2)
            {
                _one = 1;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }

            if (_tmr < 0)
            {
                _animator.SetInteger("State", 2);
                var cl = Instantiate(_insgameobj,
                    new Vector3(transform.position.x + _one, transform.position.y, transform.position.z),
                    Quaternion.identity);
                var rb = cl.AddComponent<Rigidbody>();
                rb.velocity = new Vector3(_one * 10, 0, 0);
                rb.useGravity = false;
                _tmr = 1f;
            }

            else if (_dis > 10)
            {
                UpdateMove();
            }
        }
    }

    private void UpdateSpiderBot()
    {
        UpdateMove();
        _dis = Vector3.Distance(transform.position, _player.transform.position);

        if (_dis < 2.3 && _tmr < 0)
        {
            _animator.SetInteger("State", Random.Range(2, 4));
            _player.SendMessage("ApplyDamage", Damage);
            _tmr = 1f;
        }
    }

    private void UpdateMove()
    {
        var translation = Time.deltaTime * _moveTo;
        transform.Translate(translation, 0, 0);
        var fall = new Vector3(transform.position.x + _one, transform.position.y, transform.position.z);

        if (Physics.Raycast(transform.position, new Vector3(_one, 0, 0), 1, lm) ||
            !Physics.Raycast(fall, Vector3.down, 1, lm))
        {
            if (_animator != null) _animator.SetInteger("State", 0);
            _moveTo = 0;
            if (transform.position.x > _player.transform.position.x + 0.2)
                _one = -1;
            else if (transform.position.x < _player.transform.position.x - 0.2)
                _one = 1;
        }

        else
        {
            if (transform.position.x > _player.transform.position.x + 0.2)
            {
                _one = -1;
                if (_animator != null) _animator.SetInteger("State", 1);
                transform.eulerAngles = new Vector3(0, 180, 0);
                _moveTo = MovementSpeed;
            }

            else if (transform.position.x < _player.transform.position.x - 0.2)
            {
                _one = 1;
                if (_animator != null) _animator.SetInteger("State", 1);
                transform.eulerAngles = new Vector3(0, 0, 0);
                _moveTo = MovementSpeed;
            }

            else
            {
                if (_animator != null) _animator.SetInteger("State", 0);
                _moveTo = 0;
            }
        }
    }

    public void ApplyDamage(float d)
    {
        Health -= d;

#if UNITY_EDITOR
        print("Health " + Health + " Damage " + d);
#endif //UNITY_EDITOR

        GetComponent<Rigidbody>().AddExplosionForce(1000f, _player.transform.position, 5f);
        if (Health <= 0)
        {
            Destroy(gameObject);
            Instantiate(Resources.Load("LightResource"), transform.position, Quaternion.identity);
        }
    }

    public void Stun()
    {
        _canMove = false;
        _tmr = 10;
    }
}