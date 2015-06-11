using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public bool GodMode = false;
    public float Health = 100;
    public float LightResource = 100;
    public float Speed = 1;
    public float MaxSpeed = 1;
    public float JumpForceUp = 1;
    public float JumpForceDown = 1;
    public float ForceDown = 1;
    public Material RedMaterial;
    public Material GreenMaterial;
    public Material BlueMaterial;
    public static Material[] Colors;

    private bool lr_on = false;
    private bool hp = false;
    private bool can_jump = true;
    private Material NullMat;
    private int lm = 1 << 9;
    private Projector pr;
    private GameObject arm;
    private GameObject arm_hit;
    private bool can_use_arm = true;
    private Vector3 arm_orgpos;
    private arm_states arm_state = arm_states.idle;
    enum arm_states { idle, forward, backward };
    private Rigidbody rb;
    private float damage = 5;
    private GameObject[] gos_obs_vis;
    private GameObject[] gos_obs_hid;
    private GameObject[] gos_enemy;
    private int GodModeProgress = 0;
    private float CheatDelay = 0f;
    private float tmr = 0;
    private AColor selected_color;
    private Animator animator;
    private GameObject[] enemys_count;
    enum AColor { None, Red, Green, Blue };


    void Start()
    {
        pr = Camera.main.gameObject.AddComponent<Projector>();
        pr.fieldOfView = 120;
        rb = this.GetComponent<Rigidbody>();
        animator = this.GetComponentInChildren<Animator>();
        Colors = new Material[4] { NullMat, RedMaterial, GreenMaterial, BlueMaterial };
        arm = GameObject.Find("Arm");
        arm_orgpos = arm.transform.localPosition;
        gos_obs_vis = GameObject.FindGameObjectsWithTag("Obstacles Visible");
        gos_obs_hid = GameObject.FindGameObjectsWithTag("Obstacles Hidden");
        gos_enemy = GameObject.FindGameObjectsWithTag("Enemy");
        enemys_count = GameObject.FindGameObjectsWithTag("Enemy");
        UpdateColors(AColor.None);
    }

    void Update()
    {
        UpdateArm();
        AnimatorUpdate();
        UpdateCheats();

        if (tmr > -0.1)
            tmr -= Time.deltaTime;

        if (tmr < 0 && !can_use_arm)
            can_use_arm = true;

        if (LightResource < 10f && !lr_on)
            LightResource += 0.1f;

        if (GodMode)
        {
            Health = 50;
            LightResource = 50;
            tmr = -1f;
        }

        if (arm_state == arm_states.idle)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                UpdateColors(AColor.None);
                if (selected_color == AColor.Red)
                    selected_color = AColor.None;
                else
                    selected_color = AColor.Red;
                UpdateColors(selected_color);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                UpdateColors(AColor.None);
                if (selected_color == AColor.Green)
                    selected_color = AColor.None;
                else
                    selected_color = AColor.Green;
                UpdateColors(selected_color);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                UpdateColors(AColor.None);
                if (selected_color == AColor.Blue)
                    selected_color = AColor.None;
                else
                    selected_color = AColor.Blue;
                UpdateColors(selected_color);
            }

            if (LightResource <= 0.5f && lr_on)
            {
                selected_color = AColor.None;
                LightResource = 0;
                UpdateColors(AColor.None);
            }

            if (lr_on)
            {
                LightResource -= Time.deltaTime * 10;
            }

            if (Input.GetButtonDown("Jump") && can_jump && Physics.Raycast(this.transform.position, Vector3.down, 0.1f))
            {
                rb.AddForce(Vector3.up * JumpForceUp);
                can_jump = false;
            }

            if (Input.GetButtonDown("Fire1"))
            {
                RaycastHit[] rh = Physics.SphereCastAll(this.transform.position, 2, Vector3.right, 1, lm);
                foreach (RaycastHit ht in rh)
                {
                    Enemy e = ht.collider.GetComponent<Enemy>();
                    if (e != null)
                    {
                        ht.collider.SendMessage("ApplyDamage", damage);
                    }
                }

            }
        }
    }
    void AnimatorUpdate()
    {
        float speed = Mathf.Abs(this.GetComponent<Rigidbody>().velocity.x);
        if (!Physics.Raycast(this.transform.position, Vector3.down, 0.1f))
            animator.SetInteger("Status", 2);

        else if (speed > 5f)
            animator.SetInteger("Status", 1);

        else
            animator.SetInteger("Status", 0);

    }
    void FixedUpdate()
    {
        if (!can_jump)
        {
            rb.AddForce(Vector3.down * JumpForceDown);
        }

        if (arm_state == arm_states.idle)
        {
            if (Input.GetButton("Right"))
            {
                this.transform.eulerAngles = new Vector3(0, 180, 0);
                if (rb.velocity.x < MaxSpeed)
                    rb.AddForce(Vector3.right * Speed);
            }

            else if (Input.GetButton("Left"))
            {
                this.transform.eulerAngles = new Vector3(0, 0, 0);
                if (rb.velocity.x > -MaxSpeed)
                    rb.AddForce(Vector3.left * Speed);
            }
            else if (can_jump && Physics.Raycast(this.transform.position, Vector3.down, 0.1f))
            {
                rb.AddForce(Vector3.down * ForceDown);
            }

        }
    }
    void UpdateArm()
    {
        if (Input.GetButtonDown("Fire2") && can_use_arm && Physics.Raycast(this.transform.position, Vector3.down, 0.1f))
        {
            tmr = 5;
            can_use_arm = false;
            arm_state = arm_states.forward;
            rb.isKinematic = true;
            arm_hit = null;
        }

        if (arm_state == arm_states.forward)
        {
            Vector3 a = arm.transform.localPosition;
            Vector3 p = this.transform.position;
            p.y += 0.5f;
            arm.transform.localPosition = new Vector3(a.x - 0.2f, a.y, a.z);
            RaycastHit ht;
            Debug.DrawLine(arm.transform.position, arm.transform.position + transform.InverseTransformDirection(Vector3.left));

            if (Physics.Raycast(this.transform.position, transform.InverseTransformDirection(Vector3.left), out ht, Mathf.Abs(a.x)))
            {
                arm_state = arm_states.backward;

                if (ht.collider.tag == "Enemy")
                {
                    print("" + ht.collider.name);
                    arm_hit = ht.collider.gameObject;
                    arm_hit.SendMessage("Stun");
                }
            }

            if (a.x < -15)
            {
                arm_state = arm_states.backward;
            }
        }

        if (arm_state == arm_states.backward)
        {
            Vector3 a = arm.transform.localPosition;
            arm.transform.localPosition = new Vector3(a.x + 0.2f, a.y, a.z);
            if (arm_hit != null)
            {
                Vector3 ah = arm_hit.transform.position;
                arm_hit.transform.position = new Vector3(arm.transform.position.x, ah.y, ah.z);
            }

            if (a.x > arm_orgpos.x - 1.5f)
            {
                arm.transform.localPosition = arm_orgpos;
                arm_state = arm_states.idle;
                rb.isKinematic = false;
            }
        }
    }
    void UpdateColors(AColor color)
    {
        pr.material = Colors[(int)color];

        if (color == AColor.Red)
        {
            print("color == AColor.Red");
            lr_on = true;
            damage = 10;
        }

        else if (color == AColor.Green)
        {
            print("color == AColor.Green");
            lr_on = true;
            hp = true;
        }

        else if (color == AColor.Blue)
        {
            print("color == AColor.Blue");
            lr_on = true;
            gos_enemy = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject go in gos_obs_vis)
                go.SetActive(false);
            foreach (GameObject go in gos_obs_hid)
                go.SetActive(true);
            foreach (GameObject go in gos_enemy)
                go.SetActive(false);
        }

        else
        {
            try
            {
                print("None");
                lr_on = false;
                hp = false;
                damage = 5;
                //gos_enemy = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject go in gos_obs_vis)
                    go.SetActive(true);
                foreach (GameObject go in gos_obs_hid)
                    go.SetActive(false);
                foreach (GameObject go in gos_enemy)
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

#if UNITY_EDITOR
        print("Health " + Health + " Damage " + d);
#endif//UNITY_EDITOR

        GameObject en = GetClosestObject("Enemy");

        if (en != null)
        {
            Vector3 v = new Vector3(en.transform.position.x, en.transform.position.y + 1f, en.transform.position.z);
            this.GetComponent<Rigidbody>().AddExplosionForce(100f, v, 5f);
        }
        if (Health <= 0)
            Application.LoadLevel(Application.loadedLevel);
    }
    void OnCollisionEnter(Collision collision)
    {
        can_jump = true;
        foreach (ContactPoint c in collision.contacts)
        {
            if (c.otherCollider.tag == "LightResource")
            {
                if (!hp)
                {
                    LightResource += 50;
                    if (LightResource > 100)
                        LightResource = 100;
                }
                else
                {
                    Health += 50;
                    if (Health > 100)
                        Health = 100;
                }
                Destroy(c.otherCollider.gameObject);
            }
        }
    }

    GameObject GetClosestObject(string tag)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        GameObject closestObject = GameObject.FindGameObjectWithTag(tag);

        foreach (GameObject obj in objectsWithTag)
        {
            //compares distances
            if (Vector3.Distance(transform.position, obj.transform.position) <= Vector3.Distance(transform.position, closestObject.transform.position))
            {
                closestObject = obj;
            }
        }
        return closestObject;
    }

    void OnGUI()
    {

        if (selected_color == AColor.None)
        {
            enemys_count = GameObject.FindGameObjectsWithTag("Enemy");
        }
        GUI.Box(new Rect(10, 10, 100, 60), Mathf.Floor(Health) + "\n" + Mathf.Floor(LightResource) + "\n" + enemys_count.Length);
        if (enemys_count.Length == 0)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "you won !");
            Time.timeScale = 0;
        }
    }

    void UpdateCheats()
    {
        if (CheatDelay > 0) { CheatDelay -= Time.deltaTime; if (CheatDelay <= 0) { CheatDelay = 0f; GodModeProgress = 0; } } if (GodModeProgress == 0 && Input.GetKeyDown(KeyCode.E))
        { GodModeProgress++; CheatDelay = 1f; }
        else if (GodModeProgress == 1 && Input.GetKeyDown(KeyCode.D)) { GodModeProgress++; CheatDelay = 1f; }
        else if (GodModeProgress == 2 && Input.GetKeyDown(KeyCode.G)) { GodModeProgress++; CheatDelay = 1f; }
        else if (GodModeProgress == 3 && Input.GetKeyDown(KeyCode.A)) { GodModeProgress++; CheatDelay = 1f; }
        else if (GodModeProgress == 4 && Input.GetKeyDown(KeyCode.R)) { GodModeProgress = 0; GodMode = !GodMode; print("GodMode On!"); }
    }
}
