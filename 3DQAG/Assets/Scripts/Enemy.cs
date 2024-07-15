using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D };
    public Type enemyType;

    public float maxHp;
    public float curHp;
    public Transform target;
    public bool isChase;
    public bool isAttack;
    public bool isDead;
    public BoxCollider meleeArea;
    public GameObject bullet;

    protected Rigidbody rb;
    protected BoxCollider BoxCol;
    protected MeshRenderer[] meshs;
    protected NavMeshAgent nav;
    protected Animator anim;




    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        BoxCol = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if(enemyType != Type.D)
        {
            Invoke("ChaseStart", 2);
        }
    }

    private void Update()
    {
        if (nav.enabled && enemyType != Type.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    void Targeting()
    {
        if (!isDead && enemyType != Type.D)
        {
            float targetRedius = 0;
            float targetRange = 0;

            switch (enemyType)
            {
                case Type.A:
                    targetRedius = 1.5f;
                    targetRange = 3f;
                    break;
                case Type.B:
                    targetRedius = 1f;
                    targetRange = 12f;
                    break;
                case Type.C:
                    targetRedius = 0.5f;
                    targetRange = 25f;
                    break;
            }

            RaycastHit[] rayHits =
                Physics.SphereCastAll(transform.position,
                                            targetRedius,
                                            transform.forward,
                                            targetRange,
                                            LayerMask.GetMask("Player"));
            if (rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;

            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rb.AddForce(transform.forward * 30, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rb.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1.5f);
                break;

            case Type.C:
                yield return new WaitForSeconds(0.5f);
                Vector3 vector3 = new Vector3(0, 1, 0);
                GameObject instantBullet = Instantiate(bullet, transform.position + vector3, transform.rotation);
                Rigidbody bulletRb = instantBullet.GetComponent<Rigidbody>();
                bulletRb.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
        }

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHp -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));

            Debug.Log("Melee: "+curHp);
        }else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHp -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));

            Debug.Log("Range: " + curHp);
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHp -= 100;
        Vector3 reactvec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactvec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        if(curHp > 0)
        {
            foreach(MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.white;
            }
        }
        else
        {
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.gray;
            }
            isDead = true;
            gameObject.layer = 14;
            gameObject.tag = "DeadEnemy";
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");

            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rb.freezeRotation = false;
                rb.AddForce(reactVec * 5, ForceMode.Impulse);
                rb.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rb.AddForce(reactVec * 5, ForceMode.Impulse);
            }

            if(enemyType != Type.D)
            {
                Destroy(gameObject, 1f);
            }
        }
    }
}
