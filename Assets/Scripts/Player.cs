using UnityEngine;

public class Player : MonoBehaviour
{
    public static Material[] Colors;
    private Animator _animator;
    private GameObject _arm;
    private GameObject _armHit;
    private Vector3 _armOrgpos;
    private arm_states _armState = arm_states.idle;
    public Material BlueMaterial;
    private bool _canJump = true;
    private bool _canUseArm = true;
    private float _cheatDelay;
    private float _damage = 5;
    private GameObject[] _enemysCount;
    public float ForceDown = 1;
    public bool GodMode;
    private int _godModeProgress;
    private GameObject[] _gosEnemy;
    private GameObject[] _gosObsHid;
    private GameObject[] _gosObsVis;
    public Material GreenMaterial;
    public float Health = 100;
    private bool _hp;
    public float JumpForceDown = 1;
    public float JumpForceUp = 1;
    public float LightResource = 100;
    private const int _lm = 1 << 9;
    private LineRenderer _lr;

    private bool _lrOn;
    public float MaxSpeed = 1;
    private readonly Material _nullMat = null;
    private Projector _pr;
    private Rigidbody _rb;
    public Material RedMaterial;
    private AColor _selectedColor;
    public float Speed = 1;
    private float _tmr;


    private void Start()
    {
        _pr = Camera.main.gameObject.AddComponent<Projector>();
        _pr.fieldOfView = 120;
        _rb = GetComponent<Rigidbody>();
        _lr = GetComponentInChildren<LineRenderer>();
        _animator = GetComponentInChildren<Animator>();
        Colors = new Material[4] {_nullMat, RedMaterial, GreenMaterial, BlueMaterial};
        _arm = GameObject.Find("Arm");
        _armOrgpos = _arm.transform.localPosition;
        _gosObsVis = GameObject.FindGameObjectsWithTag("Obstacles Visible");
        _gosObsHid = GameObject.FindGameObjectsWithTag("Obstacles Hidden");
        _gosEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        _enemysCount = GameObject.FindGameObjectsWithTag("Enemy");
        UpdateColors(AColor.None);
    }

    private void Update()
    {
        UpdateArm();
        AnimatorUpdate();
        UpdateCheats();

        if (_tmr > -0.1)
            _tmr -= Time.deltaTime;

        if (_tmr < 0 && !_canUseArm)
            _canUseArm = true;

        if (LightResource < 10f && !_lrOn)
            LightResource += 0.1f;

        if (GodMode)
        {
            Health = 50;
            LightResource = 50;
            _tmr = -1f;
        }

        if (_armState == arm_states.idle)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                UpdateColors(AColor.None);
                if (_selectedColor == AColor.Red)
                    _selectedColor = AColor.None;
                else
                    _selectedColor = AColor.Red;
                UpdateColors(_selectedColor);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                UpdateColors(AColor.None);
                if (_selectedColor == AColor.Green)
                    _selectedColor = AColor.None;
                else
                    _selectedColor = AColor.Green;
                UpdateColors(_selectedColor);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                UpdateColors(AColor.None);
                if (_selectedColor == AColor.Blue)
                    _selectedColor = AColor.None;
                else
                    _selectedColor = AColor.Blue;
                UpdateColors(_selectedColor);
            }

            if (LightResource <= 0.5f && _lrOn)
            {
                _selectedColor = AColor.None;
                LightResource = 0;
                UpdateColors(AColor.None);
            }

            if (_lrOn)
                LightResource -= Time.deltaTime * 10;

            if (Input.GetButtonDown("Jump") && _canJump && Physics.Raycast(transform.position, Vector3.down, 0.1f))
            {
                _rb.AddForce(Vector3.up * JumpForceUp);
                _canJump = false;
            }

            if (Input.GetButtonDown("Fire1"))
            {
                var rh = Physics.SphereCastAll(transform.position, 2, Vector3.right, 1, _lm);
                foreach (var ht in rh)
                {
                    var e = ht.collider.GetComponent<Enemy>();
                    if (e != null)
                        ht.collider.SendMessage("ApplyDamage", _damage);
                }
            }
        }
    }

    private void AnimatorUpdate()
    {
        var speed = Mathf.Abs(GetComponent<Rigidbody>().velocity.x);
        if (!Physics.Raycast(transform.position, Vector3.down, 0.1f))
            _animator.SetInteger("Status", 2);

        else if (speed > 5f)
            _animator.SetInteger("Status", 1);

        else
            _animator.SetInteger("Status", 0);
    }

    private void FixedUpdate()
    {
        if (!_canJump)
            _rb.AddForce(Vector3.down * JumpForceDown);

        if (_armState == arm_states.idle)
            if (Input.GetButton("Right"))
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
                if (_rb.velocity.x < MaxSpeed)
                    _rb.AddForce(Vector3.right * Speed);
            }

            else if (Input.GetButton("Left"))
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                if (_rb.velocity.x > -MaxSpeed)
                    _rb.AddForce(Vector3.left * Speed);
            }
            else if (_canJump && Physics.Raycast(transform.position, Vector3.down, 0.1f))
            {
                _rb.AddForce(Vector3.down * ForceDown);
            }
    }

    private void UpdateArm()
    {
        if (Input.GetButtonDown("Fire2") && _canUseArm && Physics.Raycast(transform.position, Vector3.down, 0.1f))
        {
            _tmr = 5;
            _canUseArm = false;
            _armState = arm_states.forward;
            _rb.isKinematic = true;
            _armHit = null;
        }

        if (_armState == arm_states.forward)
        {
            var a = _arm.transform.localPosition;
            var p = transform.position;
            p.y += 0.5f;
            _arm.transform.localPosition = new Vector3(a.x - 0.2f, a.y, a.z);
            RaycastHit ht;
            Debug.DrawLine(_arm.transform.position,
                _arm.transform.position + transform.InverseTransformDirection(Vector3.left));

            if (Physics.Raycast(transform.position, transform.InverseTransformDirection(Vector3.left), out ht,
                Mathf.Abs(a.x)))
            {
                _armState = arm_states.backward;

                if (ht.collider.tag == "Enemy")
                {
                    print("" + ht.collider.name);
                    _armHit = ht.collider.gameObject;
                    _armHit.SendMessage("Stun");
                }
            }

            if (a.x < -15)
                _armState = arm_states.backward;
            _lr.SetPosition(0, _arm.transform.localPosition);
        }

        if (_armState == arm_states.backward)
        {
            var a = _arm.transform.localPosition;
            _arm.transform.localPosition = new Vector3(a.x + 0.2f, a.y, a.z);
            if (_armHit != null)
            {
                var ah = _armHit.transform.position;
                _armHit.transform.position = new Vector3(_arm.transform.position.x, ah.y, ah.z);
            }

            if (a.x > _armOrgpos.x - 1.5f)
            {
                _arm.transform.localPosition = _armOrgpos;
                _armState = arm_states.idle;
                _rb.isKinematic = false;
            }
            _lr.SetPosition(0, _arm.transform.localPosition);
        }
    }

    private void UpdateColors(AColor color)
    {
        _pr.material = Colors[(int) color];

        if (color == AColor.Red)
        {
            print("color == AColor.Red");
            _lrOn = true;
            _damage = 10;
        }

        else if (color == AColor.Green)
        {
            print("color == AColor.Green");
            _lrOn = true;
            _hp = true;
        }

        else if (color == AColor.Blue)
        {
            print("color == AColor.Blue");
            _lrOn = true;
            _gosEnemy = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var go in _gosObsVis)
                go.SetActive(false);
            foreach (var go in _gosObsHid)
                go.SetActive(true);
            foreach (var go in _gosEnemy)
                go.SetActive(false);
        }

        else
        {
            try
            {
                print("None");
                _lrOn = false;
                _hp = false;
                _damage = 5;
                //gos_enemy = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var go in _gosObsVis)
                    go.SetActive(true);
                foreach (var go in _gosObsHid)
                    go.SetActive(false);
                foreach (var go in _gosEnemy)
                    go.SetActive(true);
            }
            catch
            {
            }
        }
    }

    public void ApplyDamage(float d)
    {
        if (!GodMode)
            Health -= d;

        var en = GetClosestObject("Enemy");

        if (en != null && _selectedColor != AColor.Red)
        {
            var v = new Vector3(en.transform.position.x, en.transform.position.y + 1f, en.transform.position.z);
            GetComponent<Rigidbody>().AddExplosionForce(100f, v, 5f);
        }
        if (Health <= 0)
            Application.LoadLevel(Application.loadedLevel);
    }

    private void OnCollisionEnter(Collision collision)
    {
        _canJump = true;
        foreach (var c in collision.contacts)
            if (c.otherCollider.tag == "LightResource")
            {
                if (!_hp)
                {
                    LightResource += 10;
                    if (LightResource > 100)
                        LightResource = 100;
                }
                else
                {
                    Health += 10;
                    if (Health > 100)
                        Health = 100;
                }
                Destroy(c.otherCollider.gameObject);
            }
    }

    private GameObject GetClosestObject(string tag)
    {
        var objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        var closestObject = GameObject.FindGameObjectWithTag(tag);

        foreach (var obj in objectsWithTag)
            if (Vector3.Distance(transform.position, obj.transform.position) <=
                Vector3.Distance(transform.position, closestObject.transform.position))
                closestObject = obj;
        return closestObject;
    }

    private void OnGUI()
    {
        if (_selectedColor == AColor.None)
            _enemysCount = GameObject.FindGameObjectsWithTag("Enemy");
        GUI.Box(new Rect(10, 10, 100, 60),
            Mathf.Floor(Health) + "\n" + Mathf.Floor(LightResource) + "\n" + _enemysCount.Length);
        if (_enemysCount.Length == 0)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "you won !");
            Time.timeScale = 0;
        }
    }

    private void UpdateCheats()
    {
        if (_cheatDelay > 0)
        {
            _cheatDelay -= Time.deltaTime;
            if (_cheatDelay <= 0)
            {
                _cheatDelay = 0f;
                _godModeProgress = 0;
            }
        }
        if (_godModeProgress == 0 && Input.GetKeyDown(KeyCode.E))
        {
            _godModeProgress++;
            _cheatDelay = 1f;
        }
        else if (_godModeProgress == 1 && Input.GetKeyDown(KeyCode.D))
        {
            _godModeProgress++;
            _cheatDelay = 1f;
        }
        else if (_godModeProgress == 2 && Input.GetKeyDown(KeyCode.G))
        {
            _godModeProgress++;
            _cheatDelay = 1f;
        }
        else if (_godModeProgress == 3 && Input.GetKeyDown(KeyCode.A))
        {
            _godModeProgress++;
            _cheatDelay = 1f;
        }
        else if (_godModeProgress == 4 && Input.GetKeyDown(KeyCode.R))
        {
            _godModeProgress = 0;
            GodMode = !GodMode;
            print("GodMode On!");
        }
    }

    private enum arm_states
    {
        idle,
        forward,
        backward
    }

    private enum AColor
    {
        None,
        Red,
        Green,
        Blue
    }
}